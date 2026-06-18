using AIKernel.Common.Results;
using AIKernel.Dtos.Governance;
using AIKernel.Enums.Governance;

namespace AIKernel.Control.Core.Ctg;

/// <summary>
/// EN: Converts a council decision into the vote-only gate input consumed by Core.
/// [EN] Documents this public package API member. [JA] council decision を Core が消費する vote-only gate input に変換します。
/// </summary>
public sealed class CouncilDecisionToGateInputAdapter
{
    /// <summary>
    /// EN: Adapts a council decision into GateInput without reading diagnostics or continuous carriers.
    /// [EN] Documents this public package API member. [JA] diagnostics や continuous carrier を読まずに council decision を GateInput に変換します。
    /// </summary>
    /// <param name="decision">EN: The council decision. JA: council decision です。</param>
    /// <returns>EN: The gate input result. JA: gate input result を返します。</returns>
    public Result<GateInput> Adapt(CouncilDecision? decision)
    {
        var result =
            from validDecision in ValidateDecision(decision)
            select new GateInput
            {
                Logos = VoteFor(validDecision, CouncilKind.Logos),
                Ethos = VoteFor(validDecision, CouncilKind.Ethos),
                Pathos = VoteFor(validDecision, CouncilKind.Pathos)
            };

        return result;
    }

    private static Result<CouncilDecision> ValidateDecision(CouncilDecision? decision)
        => decision is null
            ? Result<CouncilDecision>.Fail(new ErrorContext(
                "Council decision is required.",
                "CTG_COUNCIL_DECISION_REQUIRED",
                false))
            : Result<CouncilDecision>.Ok(decision);

    private static CouncilVoteValue VoteFor(CouncilDecision decision, CouncilKind councilKind)
        => decision.Votes
            .Where(vote => vote.CouncilKind == councilKind)
            .OrderBy(vote => vote.VoteId, StringComparer.Ordinal)
            .Select(vote => vote.VoteValue)
            .DefaultIfEmpty(CouncilVoteValue.Unknown)
            .First();
}
