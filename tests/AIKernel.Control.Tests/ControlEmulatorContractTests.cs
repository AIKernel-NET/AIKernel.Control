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
}
