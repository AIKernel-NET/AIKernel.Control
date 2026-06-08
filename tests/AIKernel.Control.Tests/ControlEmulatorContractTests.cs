using AIKernel.Abstractions.Control;
using AIKernel.Control.Emulator;
using AIKernel.Dtos.Control;

namespace AIKernel.Control.Tests;

public sealed class ControlEmulatorContractTests
{
    [Fact]
    public async Task DeterministicSchedulerOrdersNodesByContractIdentity()
    {
        var scheduler = new DeterministicNodeScheduler();
        var graph = new EmulatedExecutionGraph(
            "graph.contract",
            [
                new EmulatedExecutionNode("node-b", "op-z", new Dictionary<string, string>()),
                new EmulatedExecutionNode("node-a", "op-z", new Dictionary<string, string>()),
                new EmulatedExecutionNode("node-a", "op-a", new Dictionary<string, string>())
            ]);

        var nodes = await scheduler.ScheduleAsync(graph, TestContext.Current.CancellationToken);

        Assert.Collection(
            nodes,
            node => Assert.Equal(("node-a", "op-a"), (node.NodeId, node.OperatorId)),
            node => Assert.Equal(("node-a", "op-z"), (node.NodeId, node.OperatorId)),
            node => Assert.Equal(("node-b", "op-z"), (node.NodeId, node.OperatorId)));
    }

    [Fact]
    public async Task EmulatorExecutesAgainstSharedControlContracts()
    {
        var observer = new RecordingControlStateObserver();
        var engine = new ControlEmulatorEngine(observer: observer);
        var graph = new EmulatedExecutionGraph(
            "graph.contract",
            [
                new EmulatedExecutionNode("node-2", "op.mock", new Dictionary<string, string>()),
                new EmulatedExecutionNode("node-1", "op.mock", new Dictionary<string, string>())
            ]);
        var request = new ControlExecutionRequest(
            "exec.contract",
            new Dictionary<string, string> { ["source"] = "test" });

        var result = await engine.ExecuteAsync(graph, request, TestContext.Current.CancellationToken);

        Assert.Equal("exec.contract", result.ExecutionId);
        Assert.Equal("Completed", result.Status);
        Assert.Equal("control-emulator", result.Metadata["engine_id"]);
        Assert.Equal("2", result.Metadata["scheduled_node_count"]);
        Assert.Collection(
            observer.Snapshots,
            snapshot => Assert.Equal("node-1", snapshot.NodeId),
            snapshot => Assert.Equal("node-2", snapshot.NodeId));
    }

    [Theory]
    [InlineData("ALLOW", true, "Completed")]
    [InlineData("DENY", false, "Denied")]
    [InlineData("ABORT", false, "Aborted")]
    public async Task EmulatorHonorsControlPolicyEvaluation(
        string policyCode,
        bool allowed,
        string expectedStatus)
    {
        var engine = new ControlEmulatorEngine(policy: new StaticControlPolicy(allowed, policyCode));
        var graph = new EmulatedExecutionGraph(
            "graph.policy",
            [new EmulatedExecutionNode("node-1", "op.mock", new Dictionary<string, string>())]);
        var request = new ControlExecutionRequest(
            "exec.policy",
            new Dictionary<string, string>());

        var result = await engine.ExecuteAsync(graph, request, TestContext.Current.CancellationToken);

        Assert.Equal(expectedStatus, result.Status);
        Assert.Equal("control-emulator", result.Metadata["engine_id"]);
        Assert.Equal("graph.policy", result.Metadata["graph_id"]);

        if (!allowed)
        {
            Assert.Equal(policyCode, result.Metadata["policy_code"]);
            Assert.Equal("test policy", result.Metadata["policy_reason"]);
        }
    }

    [Fact]
    public async Task EmulatorCompletesEmptyExecutionGraph()
    {
        var observer = new RecordingControlStateObserver();
        var engine = new ControlEmulatorEngine(observer: observer);
        var graph = new EmulatedExecutionGraph("graph.empty", []);
        var request = new ControlExecutionRequest(
            "exec.empty",
            new Dictionary<string, string>());

        var result = await engine.ExecuteAsync(graph, request, TestContext.Current.CancellationToken);

        Assert.Equal("Completed", result.Status);
        Assert.Equal("0", result.Metadata["scheduled_node_count"]);
        Assert.Empty(observer.Snapshots);
    }

    [Fact]
    public async Task EmulatorPropagatesExecutionNodeMetadataIntoResult()
    {
        var engine = new ControlEmulatorEngine();
        var graph = new EmulatedExecutionGraph(
            "graph.metadata",
            [
                new EmulatedExecutionNode(
                    "node-2",
                    "op.second",
                    new Dictionary<string, string> { ["custom"] = "second" }),
                new EmulatedExecutionNode(
                    "node-1",
                    "op.first",
                    new Dictionary<string, string> { ["custom"] = "first", ["slot"] = "G" })
            ]);
        var request = new ControlExecutionRequest(
            "exec.metadata",
            new Dictionary<string, string>());

        var result = await engine.ExecuteAsync(graph, request, TestContext.Current.CancellationToken);

        Assert.Equal("Completed", result.Status);
        Assert.Equal("node-2", result.Metadata["last_node_id"]);
        Assert.Equal("op.second", result.Metadata["last_operator_id"]);
        Assert.Equal("first", result.Metadata["node.node-1.custom"]);
        Assert.Equal("G", result.Metadata["node.node-1.slot"]);
        Assert.Equal("second", result.Metadata["node.node-2.custom"]);
    }

    [Fact]
    public async Task EmulatorReturnsCanceledWhenCancellationIsRequestedDuringExecution()
    {
        using var cts = new CancellationTokenSource();
        var observer = new CancelAfterFirstSnapshotObserver(cts);
        var engine = new ControlEmulatorEngine(observer: observer);
        var graph = new EmulatedExecutionGraph(
            "graph.cancel",
            [
                new EmulatedExecutionNode("node-1", "op.mock", new Dictionary<string, string>()),
                new EmulatedExecutionNode("node-2", "op.mock", new Dictionary<string, string>())
            ]);
        var request = new ControlExecutionRequest(
            "exec.cancel",
            new Dictionary<string, string>());

        var result = await engine.ExecuteAsync(graph, request, cts.Token);

        Assert.Equal("Canceled", result.Status);
        Assert.Equal("CONTROL_CANCELED", result.Metadata["error_code"]);
        Assert.Single(observer.Snapshots);
        Assert.Equal("node-1", observer.Snapshots[0].NodeId);
    }

    [Fact]
    public async Task EmulatorReturnsFaultedWhenEmulatedNodeThrows()
    {
        var observer = new RecordingControlStateObserver();
        var engine = new ControlEmulatorEngine(observer: observer);
        var graph = new EmulatedExecutionGraph(
            "graph.fault",
            [
                new EmulatedExecutionNode(
                    "node-1",
                    "op.fault",
                    new Dictionary<string, string> { ["custom"] = "before-fault" },
                    "node failed")
            ]);
        var request = new ControlExecutionRequest(
            "exec.fault",
            new Dictionary<string, string>());

        var result = await engine.ExecuteAsync(graph, request, TestContext.Current.CancellationToken);

        Assert.Equal("Faulted", result.Status);
        Assert.Equal("CONTROL_FAULTED", result.Metadata["error_code"]);
        Assert.Equal("node failed", result.Metadata["error_message"]);
        Assert.Single(observer.Snapshots);
        Assert.Equal("node-1", observer.Snapshots[0].NodeId);
    }

    private sealed class RecordingControlStateObserver : IControlStateObserver
    {
        private readonly List<ControlStateSnapshot> _snapshots = [];

        public IReadOnlyList<ControlStateSnapshot> Snapshots => _snapshots;

        public ValueTask ObserveAsync(
            ControlStateSnapshot snapshot,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _snapshots.Add(snapshot);
            return ValueTask.CompletedTask;
        }
    }

    private sealed class CancelAfterFirstSnapshotObserver(CancellationTokenSource cancellationTokenSource)
        : IControlStateObserver
    {
        private readonly List<ControlStateSnapshot> _snapshots = [];

        public IReadOnlyList<ControlStateSnapshot> Snapshots => _snapshots;

        public ValueTask ObserveAsync(
            ControlStateSnapshot snapshot,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _snapshots.Add(snapshot);
            cancellationTokenSource.Cancel();
            return ValueTask.CompletedTask;
        }
    }

    private sealed class StaticControlPolicy(bool allowed, string code) : IControlPolicy
    {
        public ValueTask<ControlPolicyEvaluation> EvaluateAsync(
            IExecutionGraph graph,
            ControlExecutionRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(graph);
            ArgumentNullException.ThrowIfNull(request);
            cancellationToken.ThrowIfCancellationRequested();

            return ValueTask.FromResult(new ControlPolicyEvaluation(
                allowed,
                code,
                "test policy"));
        }
    }
}
