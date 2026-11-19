using AIKernel.Control.Core.Ctg;
using AIKernel.Control.Emulator;
using AIKernel.Dtos.Governance;
using AIKernel.Enums.Governance;

namespace AIKernel.Control.Tests;

public sealed class ProviderVoteNormalizationTests
{
    public static IEnumerable<object[]> VoteValues()
    {
        (CouncilVoteValue Input, CouncilVoteValue Expected)[] votes =
        [
            (CouncilVoteValue.Approve, CouncilVoteValue.Approve),
            (CouncilVoteValue.Abstain, CouncilVoteValue.Abstain),
            (CouncilVoteValue.Reject, CouncilVoteValue.Reject),
            (CouncilVoteValue.Unknown, CouncilVoteValue.Unknown),
            ((CouncilVoteValue)(-1), CouncilVoteValue.Unknown),
            ((CouncilVoteValue)4, CouncilVoteValue.Unknown),
            ((CouncilVoteValue)int.MinValue, CouncilVoteValue.Unknown)
        ];

        foreach (var council in new[] { CouncilKind.Logos, CouncilKind.Ethos, CouncilKind.Pathos })
        foreach (var (input, expected) in votes)
        {
            yield return [council, input, expected];
        }
    }

    [Theory]
    [MemberData(nameof(VoteValues))]
    public async Task AdaptAsync_NormalizesVoteValueAndPreservesVoteIdentity(
        CouncilKind council, CouncilVoteValue input, CouncilVoteValue expected)
    {
        var output = new ProviderVoteOutput
        {
            ProviderId = "provider.ct1", CouncilKind = council, VoteValue = input,
            Metadata = new Dictionary<string, string>(StringComparer.Ordinal) { ["source"] = "ct1" }
        };
        var context = new CtgControlExecutionContext
        {
            OperationId = "ct1", ObservedAt = DateTimeOffset.UnixEpoch
        };

        var result = await new ProviderVoteAdapter().AdaptAsync(
            output, context, TestContext.Current.CancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value!.VoteValue);
        Assert.Equal(council, result.Value.CouncilKind);
        Assert.Equal($"ct1.provider.ct1.{council.ToString().ToLowerInvariant()}", result.Value.VoteId);
        Assert.Equal(DateTimeOffset.UnixEpoch, result.Value.ObservedAt);
        Assert.Equal("ct1", result.Value.Metadata["source"]);
        Assert.Equal(input, output.VoteValue);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    [InlineData(int.MinValue)]
    public async Task UndefinedEthos_WithTwoApprovals_DeniesBeforeExecution(int vote)
    {
        var scenario = new EthosRejectScenario().Create();
        scenario = scenario with
        {
            ProviderOutputs = scenario.ProviderOutputs
                .Select(output => output.CouncilKind == CouncilKind.Ethos
                    ? output with { VoteValue = (CouncilVoteValue)vote }
                    : output)
                .ToArray()
        };

        var result = await new CtgControlEmulator().RunScenarioAsync(
            scenario, TestContext.Current.CancellationToken);

        var envelope = Assert.IsType<CtgControlDecisionEnvelope>(result.DecisionEnvelope);
        Assert.Equal(CouncilVoteValue.Unknown, envelope.GateInput.Ethos);
        Assert.Equal(CouncilVoteValue.Approve, envelope.GateInput.Logos);
        Assert.Equal(CouncilVoteValue.Approve, envelope.GateInput.Pathos);
        Assert.Equal(GateDecisionKind.Deny, envelope.DecisionGate.DecisionKind);
        Assert.False(envelope.DecisionGate.Accepted);
        Assert.Equal("ctg.unknown_vote_value", Assert.Single(envelope.DecisionGate.Diagnostics).Code);
        Assert.Equal("Denied", result.ExecutionResult.Status);
        Assert.Empty(result.Snapshots);
    }
}
