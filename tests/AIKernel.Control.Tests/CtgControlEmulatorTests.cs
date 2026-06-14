using AIKernel.Control.Core.Ctg;
using AIKernel.Control.Emulator;
using AIKernel.Core.Governance;
using AIKernel.Dtos.Governance;
using AIKernel.Enums.Governance;

namespace AIKernel.Control.Tests;

public sealed class CtgControlEmulatorTests
{
    [Fact]
    public async Task CtgTruthTableScenario_AllCombinations_MatchCoreDecisionGate()
    {
        var emulator = new CtgControlEmulator();
        var coreGate = new CtgDecisionGateEvaluator();
        var scenarios = new CtgTruthTableScenario().CreateScenarios();

        Assert.Equal(27, scenarios.Count);

        foreach (var scenario in scenarios)
        {
            var result = await emulator.RunScenarioAsync(
                scenario,
                TestContext.Current.CancellationToken);
            var coreResult = coreGate.Evaluate(
                new DecisionGateRequest
                {
                    OperationId = scenario.Request.ExecutionId,
                    StepId = scenario.Graph.GraphId,
                    GateInput = result.DecisionEnvelope!.GateInput
                });

            Assert.Equal(coreResult.Accepted, result.DecisionEnvelope.DecisionGate.Accepted);
            Assert.Equal(
                coreResult.Accepted ? "Completed" : "Denied",
                result.ExecutionResult.Status);
        }
    }

    [Fact]
    public async Task EthosRejectScenario_WhenRun_DeniesBeforeExecution()
    {
        var emulator = new CtgControlEmulator();
        var scenario = new EthosRejectScenario().Create();

        var result = await emulator.RunScenarioAsync(
            scenario,
            TestContext.Current.CancellationToken);

        Assert.Equal("Denied", result.ExecutionResult.Status);
        Assert.Empty(result.Snapshots);
        Assert.False(result.DecisionEnvelope!.DecisionGate.Accepted);
    }

    [Fact]
    public async Task EmptyTrajectoryHaltScenario_WhenRun_AbortsBeforeExecution()
    {
        var emulator = new CtgControlEmulator();
        var scenario = new EmptyTrajectoryHaltScenario().Create();

        var result = await emulator.RunScenarioAsync(
            scenario,
            TestContext.Current.CancellationToken);

        Assert.Equal("Aborted", result.ExecutionResult.Status);
        Assert.NotNull(result.TrajectoryGate);
        Assert.False(result.TrajectoryGate!.Accepted);
        Assert.Empty(result.Snapshots);
    }

    [Fact]
    public async Task AnyDenyTrajectoryHaltScenario_WhenRun_AbortsBeforeExecution()
    {
        var emulator = new CtgControlEmulator();
        var scenario = new AnyDenyTrajectoryHaltScenario().Create();

        var result = await emulator.RunScenarioAsync(
            scenario,
            TestContext.Current.CancellationToken);

        Assert.Equal("Aborted", result.ExecutionResult.Status);
        Assert.NotNull(result.TrajectoryGate);
        Assert.False(result.TrajectoryGate!.Accepted);
    }

    [Fact]
    public async Task CtgReplayScenario_WhenRunTwice_ProducesStableReplayMetadata()
    {
        var emulator = new CtgControlEmulator();
        var scenario = new CtgReplayScenario().Create();

        var first = await emulator.RunScenarioAsync(
            scenario,
            TestContext.Current.CancellationToken);
        var second = await emulator.RunScenarioAsync(
            scenario,
            TestContext.Current.CancellationToken);

        Assert.Equal(first.ExecutionResult.Status, second.ExecutionResult.Status);
        Assert.Equal(first.ReplayMetadata["ctg.scenario_id"], second.ReplayMetadata["ctg.scenario_id"]);
        Assert.Equal(first.ReplayMetadata["ctg.decision.accepted"], second.ReplayMetadata["ctg.decision.accepted"]);
    }

    [Fact]
    public async Task CtgCouncilEvaluationProviderRegistry_ResolveResultAsync_ReturnsDeterministicCouncilOrder()
    {
        var registry = new CtgCouncilEvaluationProviderRegistry();
        registry.Register(Vote("z.pathos", CouncilKind.Pathos, CouncilVoteValue.Abstain));
        registry.Register(Vote("a.logos", CouncilKind.Logos, CouncilVoteValue.Approve));
        registry.Register(Vote("z.ethos", CouncilKind.Ethos, CouncilVoteValue.Approve));

        var result = await registry.ResolveResultAsync(TestContext.Current.CancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Collection(
            result.Value!,
            output => Assert.Equal(("a.logos", CouncilKind.Logos), (output.ProviderId, output.CouncilKind)),
            output => Assert.Equal(("z.ethos", CouncilKind.Ethos), (output.ProviderId, output.CouncilKind)),
            output => Assert.Equal(("z.pathos", CouncilKind.Pathos), (output.ProviderId, output.CouncilKind)));
    }

    [Fact]
    public async Task CtgCouncilEvaluationProviderRegistry_ResolveResultAsync_MissingProviderReturnsUnknownVote()
    {
        var registry = new CtgCouncilEvaluationProviderRegistry();
        registry.Register(Vote("logos", CouncilKind.Logos, CouncilVoteValue.Approve));
        registry.Register(Vote("ethos", CouncilKind.Ethos, CouncilVoteValue.Approve));

        var result = await registry.ResolveResultAsync(TestContext.Current.CancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Contains(
            result.Value!,
            output => output.CouncilKind == CouncilKind.Pathos &&
                      output.VoteValue == CouncilVoteValue.Unknown);
    }

    [Fact]
    public async Task CtgCouncilEvaluationProviderRegistry_ResolveResultAsync_MultipleProvidersReturnsDeterministicError()
    {
        var registry = new CtgCouncilEvaluationProviderRegistry();
        registry.Register(Vote("logos-a", CouncilKind.Logos, CouncilVoteValue.Approve));
        registry.Register(Vote("logos-b", CouncilKind.Logos, CouncilVoteValue.Approve));

        var result = await registry.ResolveResultAsync(TestContext.Current.CancellationToken);

        Assert.True(result.IsFailure);
        Assert.Equal("CTG_PROVIDER_ROUTER_MULTIPLE_MATCH", result.Error!.Code);
    }

    private static ProviderVoteOutput Vote(
        string providerId,
        CouncilKind councilKind,
        CouncilVoteValue voteValue)
    {
        return new ProviderVoteOutput
        {
            ProviderId = providerId,
            CouncilKind = councilKind,
            VoteValue = voteValue
        };
    }
}
