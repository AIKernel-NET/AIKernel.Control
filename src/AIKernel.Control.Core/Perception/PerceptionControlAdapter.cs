namespace AIKernel.Control.Core.Perception;

using AIKernel.Common.Results;
using AIKernel.Control.Core.Ctg;

/// <summary>
/// EN: Converts perception-derived signals into provider vote outputs without executing Gate logic.
/// JA: Gate logic を実行せず perception 由来 signal を provider vote output に変換します。
/// </summary>
public sealed class PerceptionControlAdapter
{
    /// <summary>
    /// EN: Adapts perception signals into CTG provider vote outputs.
    /// JA: perception signal を CTG provider vote output に変換します。
    /// </summary>
    /// <param name="request">EN: Perception control request. JA: perception control request です。</param>
    /// <returns>EN: Provider vote output result. JA: provider vote output result を返します。</returns>
    public Result<IReadOnlyList<ProviderVoteOutput>> Adapt(PerceptionControlRequest? request)
    {
        var result =
            from validRequest in ValidateRequest(request)
            select (IReadOnlyList<ProviderVoteOutput>)validRequest.Signals
                .OrderBy(signal => signal.ProviderId, StringComparer.Ordinal)
                .ThenBy(signal => signal.SignalId, StringComparer.Ordinal)
                .Select(signal => new ProviderVoteOutput
                {
                    ProviderId = string.IsNullOrWhiteSpace(signal.ProviderId)
                        ? "perception"
                        : signal.ProviderId,
                    CouncilKind = signal.CouncilKind,
                    VoteValue = signal.VoteValue,
                    CanonReferences = signal.CanonReferences,
                    Metadata = MergeMetadata(
                        validRequest.Metadata,
                        signal.Metadata,
                        new Dictionary<string, string>(StringComparer.Ordinal)
                        {
                            ["perception.signal_id"] = signal.SignalId
                        })
                })
                .ToArray();

        return result;
    }

    private static Result<PerceptionControlRequest> ValidateRequest(PerceptionControlRequest? request)
        => request is null
            ? Result<PerceptionControlRequest>.Fail(new ErrorContext(
                "Perception control request is required.",
                "PERCEPTION_CONTROL_REQUEST_REQUIRED",
                false))
            : Result<PerceptionControlRequest>.Ok(request);

    private static IReadOnlyDictionary<string, string> MergeMetadata(params IReadOnlyDictionary<string, string>[] maps)
    {
        var metadata = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var map in maps)
        {
            foreach (var item in map.OrderBy(item => item.Key, StringComparer.Ordinal))
            {
                metadata[item.Key] = item.Value;
            }
        }

        return metadata;
    }
}
