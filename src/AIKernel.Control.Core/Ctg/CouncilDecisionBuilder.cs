using AIKernel.Common.Results;
using AIKernel.Dtos.Diagnostics;
using AIKernel.Dtos.Governance;
using AIKernel.Enums.Diagnostics;
using AIKernel.Enums.Governance;

namespace AIKernel.Control.Core.Ctg;

/// <summary>
/// EN: Builds a council decision from normalized council votes.
/// EN: Documentation for public API. JA: 正規化済み council vote から council decision を構築します。
/// </summary>
public sealed class CouncilDecisionBuilder
{
    private static readonly CouncilKind[] RequiredCouncils =
    [
        CouncilKind.Logos,
        CouncilKind.Ethos,
        CouncilKind.Pathos
    ];

    /// <summary>
    /// EN: Builds a council evaluation result without applying Gate logic.
    /// EN: Documentation for public API. JA: Gate logic を適用せず council evaluation result を構築します。
    /// </summary>
    /// <param name="context">EN: The CTG control execution context. JA: CTG Control execution context です。</param>
    /// <param name="votes">EN: The normalized council votes. JA: 正規化済み council vote です。</param>
    /// <returns>EN: The council evaluation result. JA: council evaluation result を返します。</returns>
    public Result<CouncilEvaluationResult> Build(
        CtgControlExecutionContext? context,
        IReadOnlyList<CouncilVote>? votes)
    {
        var result =
            from validContext in ValidateContext(context)
            from validVotes in ValidateVotes(votes)
            select CreateEvaluation(validContext, validVotes);

        return result;
    }

    private static Result<CtgControlExecutionContext> ValidateContext(CtgControlExecutionContext? context)
        => context is null
            ? Result<CtgControlExecutionContext>.Fail(new ErrorContext(
                "CTG control execution context is required.",
                "CTG_CONTROL_CONTEXT_REQUIRED",
                false))
            : Result<CtgControlExecutionContext>.Ok(context);

    private static Result<IReadOnlyList<CouncilVote>> ValidateVotes(IReadOnlyList<CouncilVote>? votes)
        => votes is null
            ? Result<IReadOnlyList<CouncilVote>>.Fail(new ErrorContext(
                "Council votes are required.",
                "CTG_COUNCIL_VOTES_REQUIRED",
                false))
            : Result<IReadOnlyList<CouncilVote>>.Ok(votes);

    private static CouncilEvaluationResult CreateEvaluation(
        CtgControlExecutionContext context,
        IReadOnlyList<CouncilVote> votes)
    {
        var observedAt = CtgControlMetadata.ObservedAt(context);
        var selectedVotes = SelectDeterministicVotes(context, votes, observedAt);
        var duplicateCouncils = FindDuplicateCouncils(votes);
        var diagnostics = CreateDiagnostics(context, duplicateCouncils, observedAt);
        var metadata = CreateMetadata(context, duplicateCouncils);
        var decision = new CouncilDecision
        {
            DecisionId = $"{CtgControlMetadata.OperationId(context)}.{CtgControlMetadata.StepId(context)}.council",
            DecisionKind = CouncilDecisionKind.Inconclusive,
            Votes = selectedVotes,
            CanonReferences = context.CanonReferences,
            ObservedAt = observedAt,
            CorrelationId = context.CorrelationId,
            TraceId = context.TraceId,
            Metadata = metadata
        };

        return new CouncilEvaluationResult
        {
            OperationId = CtgControlMetadata.OperationId(context),
            Succeeded = true,
            Decision = decision,
            CanonReferences = context.CanonReferences,
            Diagnostics = diagnostics,
            ObservedAt = observedAt,
            CorrelationId = context.CorrelationId,
            TraceId = context.TraceId,
            Metadata = metadata
        };
    }

    private static IReadOnlyList<CouncilVote> SelectDeterministicVotes(
        CtgControlExecutionContext context,
        IReadOnlyList<CouncilVote> votes,
        DateTimeOffset observedAt)
    {
        var selected = votes
            .Where(vote => RequiredCouncils.Contains(vote.CouncilKind))
            .GroupBy(vote => vote.CouncilKind)
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderBy(vote => vote.VoteId, StringComparer.Ordinal)
                    .First());

        return RequiredCouncils
            .Select(kind => selected.TryGetValue(kind, out var vote)
                ? vote
                : CreateUnknownVote(context, kind, observedAt))
            .ToArray();
    }

    private static CouncilVote CreateUnknownVote(
        CtgControlExecutionContext context,
        CouncilKind councilKind,
        DateTimeOffset observedAt)
    {
        return new CouncilVote
        {
            VoteId = $"{CtgControlMetadata.OperationId(context)}.{councilKind.ToString().ToLowerInvariant()}.missing",
            CouncilKind = councilKind,
            VoteValue = CouncilVoteValue.Unknown,
            Reason = "Council vote was not supplied by the provider orchestration boundary.",
            CanonReferences = context.CanonReferences,
            ObservedAt = observedAt,
            Metadata = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["ctg.control.missing_vote"] = councilKind.ToString()
            }
        };
    }

    private static IReadOnlyList<CouncilKind> FindDuplicateCouncils(IReadOnlyList<CouncilVote> votes)
        => votes
            .Where(vote => RequiredCouncils.Contains(vote.CouncilKind))
            .GroupBy(vote => vote.CouncilKind)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .OrderBy(kind => kind)
            .ToArray();

    private static IReadOnlyList<DiagnosticEntry> CreateDiagnostics(
        CtgControlExecutionContext context,
        IReadOnlyList<CouncilKind> duplicateCouncils,
        DateTimeOffset observedAt)
    {
        return duplicateCouncils.Count == 0
            ? []
            :
            [
                new DiagnosticEntry
                {
                    DiagnosticId = "ctg.control.duplicate_council_vote",
                    Code = "CTG_CONTROL_DUPLICATE_COUNCIL_VOTE",
                    Message = "Multiple provider vote outputs were supplied for at least one council; the deterministic first vote was selected.",
                    Severity = DiagnosticSeverity.Warning,
                    Scope = DiagnosticScope.Governance,
                    ObservedAt = observedAt,
                    CorrelationId = context.CorrelationId,
                    TraceId = context.TraceId,
                    Metadata = new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                        ["ctg.control.duplicate_councils"] = string.Join(",", duplicateCouncils)
                    }
                }
            ];
    }

    private static IReadOnlyDictionary<string, string> CreateMetadata(
        CtgControlExecutionContext context,
        IReadOnlyList<CouncilKind> duplicateCouncils)
    {
        var metadata = new Dictionary<string, string>(
            CtgControlMetadata.Merge(context.Metadata),
            StringComparer.Ordinal);

        metadata["ctg.control.operation_id"] = CtgControlMetadata.OperationId(context);
        metadata["ctg.control.step_id"] = CtgControlMetadata.StepId(context);

        if (duplicateCouncils.Count > 0)
        {
            metadata["ctg.control.duplicate_councils"] = string.Join(",", duplicateCouncils);
        }

        return metadata;
    }
}
