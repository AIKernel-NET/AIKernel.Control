using AIKernel.Abstractions.Control;
using AIKernel.Control.Core.Ctg;
using AIKernel.Control.Emulator;
using AIKernel.Dtos.Control;
using AIKernel.Dtos.Governance;
using AIKernel.Enums.Governance;
using Microsoft.Extensions.DependencyInjection;

namespace AIKernel.Control.Tests;

/// <summary>
/// [EN] Verifies CTG Control policy adapter behavior.
/// [JA] CTG Control policy adapter の動作を検証します。
/// </summary>
public sealed class CtgControlPolicyAdapterTests
{
    /// <summary>
    /// [EN] Verifies approved council votes allow emulator execution.
    /// [JA] approve された council vote が emulator execution を許可することを検証します。
    /// </summary>
    [Fact]
    public async Task CtgControlPolicyAdapter_AllowsApprovedVotes_CompletesEmulatorExecution()
    {
        using var provider = CreateProvider(
            ProviderVote(CouncilKind.Logos, CouncilVoteValue.Approve),
            ProviderVote(CouncilKind.Ethos, CouncilVoteValue.Approve),
            ProviderVote(CouncilKind.Pathos, CouncilVoteValue.Abstain));
        var policy = provider.GetRequiredService<IControlPolicy>();
        var engine = new ControlEmulatorEngine(policy: policy);
        var graph = Graph();
        var request = Request();

        var policyResult = await policy.EvaluateAsync(graph, request, TestContext.Current.CancellationToken);
        var executionResult = await engine.ExecuteAsync(graph, request, TestContext.Current.CancellationToken);

        Assert.True(policyResult.Allowed);
        Assert.Equal("ALLOW", policyResult.Code);
        Assert.Equal("Completed", executionResult.Status);
    }

    /// <summary>
    /// [EN] Verifies an unknown vote denies emulator execution.
    /// [JA] unknown vote が emulator execution を拒否することを検証します。
    /// </summary>
    [Fact]
    public async Task CtgControlPolicyAdapter_DeniesUnknownVote_SuppressesEmulatorExecution()
    {
        using var provider = CreateProvider(
            ProviderVote(CouncilKind.Logos, CouncilVoteValue.Approve),
            ProviderVote(CouncilKind.Ethos, CouncilVoteValue.Unknown),
            ProviderVote(CouncilKind.Pathos, CouncilVoteValue.Approve));
        var policy = provider.GetRequiredService<IControlPolicy>();
        var engine = new ControlEmulatorEngine(policy: policy);
        var graph = Graph();
        var request = Request();

        var policyResult = await policy.EvaluateAsync(graph, request, TestContext.Current.CancellationToken);
        var executionResult = await engine.ExecuteAsync(graph, request, TestContext.Current.CancellationToken);

        Assert.False(policyResult.Allowed);
        Assert.Equal("DENY", policyResult.Code);
        Assert.Equal("Denied", executionResult.Status);
    }

    /// <summary>
    /// [EN] Verifies rejected trajectory results map to abort policy decisions.
    /// [JA] rejected trajectory result が abort policy decision に対応することを検証します。
    /// </summary>
    [Fact]
    public void CtgPolicyDecisionMapper_MapsRejectedTrajectory_ToAbortPolicy()
    {
        var mapper = new CtgPolicyDecisionMapper();
        var trajectoryGate = new TrajectoryGateResult
        {
            Accepted = false,
            RejectReasons =
            [
                new RejectReasonInfo
                {
                    Kind = RejectReasonKind.StepDenied,
                    ReasonCode = "STEP_DENIED",
                    Message = "A step was denied."
                }
            ]
        };

        var evaluation = mapper.Map(trajectoryGate);

        Assert.False(evaluation.Allowed);
        Assert.Equal("ABORT", evaluation.Code);
        Assert.Equal("STEP_DENIED", evaluation.Reason);
    }

    /// <summary>
    /// [EN] Verifies provider metadata stays outside GateInput.
    /// [JA] provider metadata が GateInput の外側に保持されることを検証します。
    /// </summary>
    [Fact]
    public async Task CtgControlCoordinator_KeepsProviderMetadataOutsideGateInput()
    {
        using var provider = CreateProvider(
            ProviderVote(CouncilKind.Logos, CouncilVoteValue.Approve) with
            {
                Metadata = new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["provider.discrete_note"] = "trace-only",
                    ["provider.confidence"] = "0.9"
                }
            },
            ProviderVote(CouncilKind.Ethos, CouncilVoteValue.Approve),
            ProviderVote(CouncilKind.Pathos, CouncilVoteValue.Abstain));
        var coordinator = provider.GetRequiredService<ICtgControlCoordinator>();
        var options = provider.GetRequiredService<CtgControlCoordinatorOptions>();
        var context = new CtgControlExecutionContext
        {
            OperationId = "exec.ctg",
            StepId = "graph.ctg",
            Graph = Graph(),
            Request = Request(),
            ProviderOutputs = options.ProviderOutputs,
            ObservedAt = options.ObservedAt
        };

        var envelope = await coordinator.EvaluateAsync(context, TestContext.Current.CancellationToken);

        Assert.True(envelope.IsSuccess);
        Assert.Equal(CouncilVoteValue.Approve, envelope.Value!.GateInput.Logos);
        Assert.Equal(CouncilVoteValue.Approve, envelope.Value.GateInput.Ethos);
        Assert.Equal(CouncilVoteValue.Abstain, envelope.Value.GateInput.Pathos);
        Assert.Contains(
            envelope.Value.CouncilEvaluation.Decision.Votes,
            vote => vote.CouncilKind == CouncilKind.Logos &&
                    vote.Metadata.ContainsKey("provider.discrete_note") &&
                    !vote.Metadata.ContainsKey("provider.confidence"));
    }

    /// <summary>
    /// [EN] Verifies retry intent is attached without changing GateInput.
    /// [JA] retry intent が GateInput を変更せずに添付されることを検証します。
    /// </summary>
    [Fact]
    public async Task CtgControlCoordinator_RetryIntent_AttachesCarrierOutsideGateInput()
    {
        using var provider = CreateProvider(
            ProviderVote(CouncilKind.Logos, CouncilVoteValue.Approve),
            ProviderVote(CouncilKind.Ethos, CouncilVoteValue.Approve),
            ProviderVote(CouncilKind.Pathos, CouncilVoteValue.Abstain));
        var coordinator = provider.GetRequiredService<ICtgControlCoordinator>();
        var options = provider.GetRequiredService<CtgControlCoordinatorOptions>();
        var context = new CtgControlExecutionContext
        {
            OperationId = "exec.ctg",
            StepId = "graph.ctg",
            Graph = Graph(),
            Request = Request(),
            ProviderOutputs = options.ProviderOutputs,
            ObservedAt = options.ObservedAt,
            RetryIntent = new CtgRetryIntentCarrier
            {
                Requested = true,
                ReasonCode = "health-death",
                Priority = 100,
                Confidence = 0.91,
                SourceSensor = "health",
                Metadata = new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["conceptName"] = "Aisthesis"
                }
            }
        };

        var envelope = await coordinator.EvaluateAsync(context, TestContext.Current.CancellationToken);

        Assert.True(envelope.IsSuccess);
        Assert.NotNull(envelope.Value!.RetryIntent);
        Assert.Equal(CouncilVoteValue.Approve, envelope.Value.GateInput.Logos);
        Assert.Equal(CouncilVoteValue.Approve, envelope.Value.GateInput.Ethos);
        Assert.Equal(CouncilVoteValue.Abstain, envelope.Value.GateInput.Pathos);
        Assert.Equal("True", envelope.Value.Metadata["ctg.control.retry.requested"]);
        Assert.Equal("health-death", envelope.Value.Metadata["ctg.control.retry.reason_code"]);
        Assert.Equal("Aisthesis", envelope.Value.Metadata["ctg.control.retry.metadata.conceptName"]);
    }

    private static ServiceProvider CreateProvider(params ProviderVoteOutput[] outputs)
    {
        var services = new ServiceCollection();
        services.AddCtgControl(new CtgControlCoordinatorOptions
        {
            ProviderOutputs = outputs,
            OperationId = "exec.ctg",
            StepId = "graph.ctg",
            ObservedAt = new DateTimeOffset(2026, 6, 14, 0, 0, 0, TimeSpan.Zero)
        });

        return services.BuildServiceProvider();
    }

    private static ProviderVoteOutput ProviderVote(
        CouncilKind councilKind,
        CouncilVoteValue voteValue)
    {
        return new ProviderVoteOutput
        {
            ProviderId = $"provider.{councilKind.ToString().ToLowerInvariant()}",
            CouncilKind = councilKind,
            VoteValue = voteValue
        };
    }

    private static EmulatedExecutionGraph Graph()
    {
        return new EmulatedExecutionGraph(
            "graph.ctg",
            [new EmulatedExecutionNode("node-1", "op.mock", new Dictionary<string, string>())]);
    }

    private static ControlExecutionRequest Request()
    {
        return new ControlExecutionRequest(
            "exec.ctg",
            new Dictionary<string, string>(StringComparer.Ordinal));
    }
}
