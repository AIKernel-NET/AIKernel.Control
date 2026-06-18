using AIKernel.Common.Results;
using AIKernel.Dtos.Governance;
using AIKernel.Enums.Governance;

namespace AIKernel.Control.Core.Ctg;

/// <summary>
/// EN: Normalizes provider vote output into a council vote contract DTO.
/// [EN] Documents this public package API member. [JA] provider vote output を council vote contract DTO に正規化します。
/// </summary>
public sealed class ProviderVoteAdapter
{
    private static readonly string[] ContinuousMetadataMarkers =
    [
        "confidence",
        "risk",
        "risk_score",
        "score"
    ];

    /// <summary>
    /// EN: Adapts a provider vote output without performing semantic evaluation.
    /// [EN] Documents this public package API member. [JA] semantic evaluation を行わず provider vote output を変換します。
    /// </summary>
    /// <param name="output">EN: The provider vote output. JA: provider vote output です。</param>
    /// <param name="context">EN: The CTG control execution context. JA: CTG Control execution context です。</param>
    /// <param name="cancellationToken">EN: The cancellation token. JA: キャンセル通知を監視するトークンです。</param>
    /// <returns>EN: The normalized council vote result. JA: 正規化された council vote result を返します。</returns>
    public ValueTask<Result<CouncilVote>> AdaptAsync(
        ProviderVoteOutput? output,
        CtgControlExecutionContext? context,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            from validOutput in ValidateOutput(output)
            from validContext in ValidateContext(context)
            select CreateVote(validOutput, validContext);

        return ValueTask.FromResult(result);
    }

    private static Result<ProviderVoteOutput> ValidateOutput(ProviderVoteOutput? output)
        => output is null
            ? Result<ProviderVoteOutput>.Fail(new ErrorContext(
                "Provider vote output is required.",
                "CTG_PROVIDER_VOTE_OUTPUT_REQUIRED",
                false))
            : Result<ProviderVoteOutput>.Ok(output);

    private static Result<CtgControlExecutionContext> ValidateContext(CtgControlExecutionContext? context)
        => context is null
            ? Result<CtgControlExecutionContext>.Fail(new ErrorContext(
                "CTG control execution context is required.",
                "CTG_CONTROL_CONTEXT_REQUIRED",
                false))
            : Result<CtgControlExecutionContext>.Ok(context);

    private static CouncilVote CreateVote(
        ProviderVoteOutput output,
        CtgControlExecutionContext context)
    {
        var observedAt = CtgControlMetadata.ObservedAt(context);
        var metadata = CtgControlMetadata.Merge(
            context.Metadata,
            SanitizeProviderMetadata(output.Metadata),
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["ctg.control.provider_id"] =
                    CtgControlMetadata.FirstNonEmpty(output.ProviderId, "provider")
            });

        return new CouncilVote
        {
            VoteId = CreateVoteId(context, output),
            CouncilKind = output.CouncilKind,
            VoteValue = output.VoteValue,
            CanonReferences = output.CanonReferences,
            ObservedAt = observedAt,
            Metadata = metadata
        };
    }

    private static string CreateVoteId(
        CtgControlExecutionContext context,
        ProviderVoteOutput output)
    {
        var operationId = CtgControlMetadata.OperationId(context);
        var providerId = CtgControlMetadata.FirstNonEmpty(output.ProviderId, "provider");
        var council = output.CouncilKind == CouncilKind.Unknown
            ? "unknown"
            : output.CouncilKind.ToString().ToLowerInvariant();

        return string.Create(
            System.Globalization.CultureInfo.InvariantCulture,
            $"{operationId}.{providerId}.{council}");
    }

    private static IReadOnlyDictionary<string, string> SanitizeProviderMetadata(
        IReadOnlyDictionary<string, string> metadata)
    {
        return metadata
            .Where(item => ContinuousMetadataMarkers.All(marker =>
                !item.Key.Contains(marker, StringComparison.OrdinalIgnoreCase)))
            .ToDictionary(item => item.Key, item => item.Value, StringComparer.Ordinal);
    }
}
