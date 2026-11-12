using AIKernel.Abstractions.Control;
using AIKernel.Abstractions.Governance;
using AIKernel.Control.Core.Ctg;
using AIKernel.Control.Emulator;
using AIKernel.Dtos.Control;
using AIKernel.Dtos.Governance;

namespace AIKernel.Control.Tests;

public sealed class ControlCancellationTests
{
    [Theory]
    [InlineData("operation")]
    [InlineData("task")]
    [InlineData("derived")]
    [InlineData("linked")]
    public async Task CoordinatorPreservesCancellationException(string kind)
    {
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        linked.Cancel();
        var expected = Cancellation(kind, linked.Token);
        var gate = new FailingGate(expected);
        var pending = Coordinator(gate).EvaluateAsync(Context(), TestContext.Current.CancellationToken).AsTask();

        var error = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => pending);

        Assert.Same(expected, error);
        Assert.Equal(expected.CancellationToken, error.CancellationToken);
        Assert.True(pending.IsCanceled);
        Assert.Equal(1, gate.Calls);
    }

    [Theory]
    [InlineData("operation")]
    [InlineData("task")]
    [InlineData("derived")]
    [InlineData("linked")]
    public async Task EmulatorPreservesCanceledResult(string kind)
    {
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        linked.Cancel();
        var scheduler = new FailingScheduler(Cancellation(kind, linked.Token));
        var context = Context();

        var result = await new ControlEmulatorEngine(scheduler).ExecuteAsync(
            Assert.IsAssignableFrom<IExecutionGraph>(context.Graph),
            Assert.IsType<ControlExecutionRequest>(context.Request), TestContext.Current.CancellationToken);

        Assert.Equal("Canceled", result.Status);
        Assert.Equal("CONTROL_CANCELED", result.Metadata["error_code"]);
        Assert.Equal("Control execution was canceled.", result.Metadata["error_message"]);
        Assert.False(result.Metadata.ContainsKey("last_node_id"));
        Assert.Equal(1, scheduler.Calls);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CoordinatorPreservesOrdinaryFailure(bool timeout)
    {
        Exception expected = timeout ? new TimeoutException("test") : new IOException("test");
        var gate = new FailingGate(expected);

        var result = await Coordinator(gate).EvaluateAsync(Context(), TestContext.Current.CancellationToken);

        Assert.True(result.IsFailure);
        Assert.Equal("UNHANDLED_EXCEPTION", result.Error!.Code);
        Assert.NotNull(result.Error.Metadata);
        Assert.Equal(expected.GetType().FullName, result.Error.Metadata["exception_type"]);
        Assert.Equal(1, gate.Calls);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task EmulatorPreservesOrdinaryFailure(bool timeout)
    {
        Exception expected = timeout ? new TimeoutException("test") : new IOException("test");
        var scheduler = new FailingScheduler(expected);
        var context = Context();

        var result = await new ControlEmulatorEngine(scheduler).ExecuteAsync(
            Assert.IsAssignableFrom<IExecutionGraph>(context.Graph),
            Assert.IsType<ControlExecutionRequest>(context.Request), TestContext.Current.CancellationToken);

        Assert.Equal("Faulted", result.Status);
        Assert.Equal("CONTROL_FAULTED", result.Metadata["error_code"]);
        Assert.Equal("test", result.Metadata["error_message"]);
        Assert.Equal(1, scheduler.Calls);
    }

    private static CtgControlCoordinator Coordinator(IDecisionGate gate) => new(
        new ProviderVoteAdapter(), new CouncilDecisionBuilder(),
        new CouncilDecisionToGateInputAdapter(), new CtgStepTraceAssembler(), gate);

    private static CtgControlExecutionContext Context()
    {
        var scenario = new CtgTruthTableScenario().CreateScenarios()[0];
        return new CtgControlExecutionContext
        {
            OperationId = scenario.Request.ExecutionId,
            StepId = scenario.Graph.GraphId,
            Graph = scenario.Graph,
            Request = scenario.Request,
            ProviderOutputs = scenario.ProviderOutputs,
            Metadata = scenario.Metadata
        };
    }

    private static OperationCanceledException Cancellation(string kind, CancellationToken linkedToken)
    {
        var token = kind == "linked" ? linkedToken : CancellationToken.None;
        return kind switch
        {
            "task" or "linked" => new TaskCanceledException("test", null, token),
            "derived" => new DerivedCancellation(token),
            _ => new OperationCanceledException("test", token)
        };
    }

    private sealed class DerivedCancellation(CancellationToken token) : OperationCanceledException("test", token);

    private sealed class FailingGate(Exception error) : IDecisionGate
    {
        public int Calls { get; private set; }

        public ValueTask<DecisionGateResult> EvaluateAsync(DecisionGateRequest request, CancellationToken cancellationToken)
        {
            Calls++;
            return ValueTask.FromException<DecisionGateResult>(error);
        }
    }

    private sealed class FailingScheduler(Exception error) : INodeScheduler
    {
        public int Calls { get; private set; }

        public ValueTask<IReadOnlyList<IExecutionNode>> ScheduleAsync(IExecutionGraph graph, CancellationToken cancellationToken = default)
        {
            Calls++;
            return ValueTask.FromException<IReadOnlyList<IExecutionNode>>(error);
        }
    }
}
