namespace AIKernel.Control.Tests;

using AIKernel.Control.Core.Perception;
using AIKernel.Control.Core.Ctg;
using AIKernel.Enums.Governance;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// EN: Tests perception-to-CTG Control adapters.
/// JA: perception-to-CTG Control adapter をテストします。
/// </summary>
public sealed class PerceptionControlTests
{
    /// <summary>
    /// EN: Verifies perception signals are normalized into provider vote outputs without Gate decisions.
    /// JA: Gate decision なしで perception signal が provider vote output に正規化されることを検証します。
    /// </summary>
    [Fact]
    public void PerceptionControlAdapter_PerceptionSignals_ReturnsProviderVoteOutputs()
    {
        var adapter = new PerceptionControlAdapter();

        var result = adapter.Adapt(new PerceptionControlRequest
        {
            Signals =
            [
                new PerceptionControlSignal
                {
                    SignalId = "s1",
                    ProviderId = "perception-provider",
                    CouncilKind = CouncilKind.Logos,
                    VoteValue = CouncilVoteValue.Approve
                }
            ]
        });

        Assert.False(result.IsFailure);
        Assert.Single(result.Value!);
        Assert.Equal(CouncilKind.Logos, result.Value![0].CouncilKind);
        Assert.Equal(CouncilVoteValue.Approve, result.Value[0].VoteValue);
    }

    /// <summary>
    /// EN: Verifies perception CTG registration includes coordinator services.
    /// JA: perception CTG registration が coordinator service を含むことを検証します。
    /// </summary>
    [Fact]
    public void AddPerceptionCtgControl_DefaultServices_RegistersPerceptionCoordinator()
    {
        var services = new ServiceCollection();

        services.AddPerceptionCtgControl();

        using var provider = services.BuildServiceProvider();
        Assert.NotNull(provider.GetRequiredService<PerceptionCtgControlCoordinator>());
        Assert.NotNull(provider.GetRequiredService<PerceptionPipelineSelector>());
    }

    /// <summary>
    /// EN: Verifies perception CTG coordinator delegates Allow decisions to Core through Control.
    /// JA: perception CTG coordinator が Allow decision を Control 経由で Core に委譲することを検証します。
    /// </summary>
    [Fact]
    public async Task PerceptionCtgControlCoordinator_ApprovedSignals_ReturnsExecuteSelection()
    {
        var services = new ServiceCollection();
        services.AddPerceptionCtgControl();
        using var provider = services.BuildServiceProvider();
        var coordinator = provider.GetRequiredService<PerceptionCtgControlCoordinator>();
        var selector = provider.GetRequiredService<PerceptionPipelineSelector>();

        var result = await coordinator.EvaluateAsync(
            new PerceptionControlRequest
            {
                OperationId = "op",
                StepId = "step",
                Signals =
                [
                    Signal(CouncilKind.Logos, CouncilVoteValue.Approve),
                    Signal(CouncilKind.Ethos, CouncilVoteValue.Approve),
                    Signal(CouncilKind.Pathos, CouncilVoteValue.Abstain)
                ]
            },
            TestContext.Current.CancellationToken);

        Assert.False(result.IsFailure);
        Assert.Equal("execute", selector.Select(result.Value!).Mode);
    }

    /// <summary>
    /// EN: Verifies priority retry intent routes the dynamic pipeline without entering GateInput.
    /// JA: 優先 retry intent が GateInput に入らず dynamic pipeline を routing することを検証します。
    /// </summary>
    [Fact]
    public async Task PerceptionPipelineSelector_RetryIntent_ReturnsRetrySelection()
    {
        var services = new ServiceCollection();
        services.AddPerceptionCtgControl();
        using var provider = services.BuildServiceProvider();
        var coordinator = provider.GetRequiredService<PerceptionCtgControlCoordinator>();
        var selector = provider.GetRequiredService<PerceptionPipelineSelector>();

        var result = await coordinator.EvaluateAsync(
            new PerceptionControlRequest
            {
                OperationId = "op",
                StepId = "step",
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
                },
                Signals =
                [
                    Signal(CouncilKind.Logos, CouncilVoteValue.Approve),
                    Signal(CouncilKind.Ethos, CouncilVoteValue.Approve),
                    Signal(CouncilKind.Pathos, CouncilVoteValue.Abstain)
                ]
            },
            TestContext.Current.CancellationToken);

        Assert.False(result.IsFailure);
        var selection = selector.Select(result.Value!);
        Assert.Equal("retry", selection.Mode);
        Assert.Equal("health-death", selection.RetryIntent!.ReasonCode);
        Assert.Equal("sensor-retry-intent", selection.Metadata["source"]);
        Assert.Equal(CouncilVoteValue.Approve, selection.DecisionEnvelope.GateInput.Logos);
        Assert.Equal(CouncilVoteValue.Approve, selection.DecisionEnvelope.GateInput.Ethos);
        Assert.Equal(CouncilVoteValue.Abstain, selection.DecisionEnvelope.GateInput.Pathos);
    }

    private static PerceptionControlSignal Signal(CouncilKind council, CouncilVoteValue vote)
        => new()
        {
            SignalId = council.ToString(),
            ProviderId = $"perception.{council}",
            CouncilKind = council,
            VoteValue = vote
        };
}
