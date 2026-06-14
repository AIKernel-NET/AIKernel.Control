using AIKernel.Abstractions.Control;
using AIKernel.Control.Core.Ctg;
using AIKernel.Control.Emulator;
using AIKernel.Dtos.Control;
using AIKernel.Dtos.Governance;
using AIKernel.Enums.Governance;
using Microsoft.Extensions.DependencyInjection;

namespace AIKernel.Control.Tests;

public sealed class CtgControlPolicyAdapterTests
{
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
