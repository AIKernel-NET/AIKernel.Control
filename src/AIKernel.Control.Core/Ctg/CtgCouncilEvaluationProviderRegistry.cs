using AIKernel.Common.Results;
using AIKernel.Enums.Governance;

namespace AIKernel.Control.Core.Ctg;

/// <summary>
/// EN: Stores provider vote outputs for CTG council evaluation orchestration.
/// EN: Documentation for public API. JA: CTG council evaluation orchestration 用の provider vote output を保持します。
/// </summary>
public sealed class CtgCouncilEvaluationProviderRegistry
{
    private readonly Dictionary<CouncilKind, List<ProviderVoteOutput>> _outputs = new();

    /// <summary>
    /// EN: Registers a provider vote output without evaluating gate semantics.
    /// EN: Documentation for public API. JA: gate semantics を評価せず provider vote output を登録します。
    /// </summary>
    /// <param name="output">EN: The provider vote output. JA: provider vote output です。</param>
    public void Register(ProviderVoteOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);

        if (!_outputs.TryGetValue(output.CouncilKind, out var outputs))
        {
            outputs = [];
            _outputs[output.CouncilKind] = outputs;
        }

        outputs.Add(output);
    }

    /// <summary>
    /// EN: Resolves registered provider vote outputs as a fail-closed result.
    /// EN: Documentation for public API. JA: 登録済み provider vote output を fail-closed result として解決します。
    /// </summary>
    /// <param name="cancellationToken">EN: The cancellation token. JA: キャンセル通知を監視するトークンです。</param>
    /// <returns>EN: The provider vote output result. JA: provider vote output result を返します。</returns>
    public ValueTask<Result<IReadOnlyList<ProviderVoteOutput>>> ResolveResultAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var outputs = new List<ProviderVoteOutput>();

        foreach (var councilKind in RequiredCouncils())
        {
            if (!_outputs.TryGetValue(councilKind, out var registered) || registered.Count == 0)
            {
                outputs.Add(CreateMissingVote(councilKind));
                continue;
            }

            if (registered.Count > 1)
            {
                return ValueTask.FromResult(Result<IReadOnlyList<ProviderVoteOutput>>.Fail(
                    new ErrorContext(
                        $"Multiple providers matched council {councilKind}.",
                        "CTG_PROVIDER_ROUTER_MULTIPLE_MATCH",
                        false)
                    {
                        Metadata = new Dictionary<string, string>(StringComparer.Ordinal)
                        {
                            ["ctg.council"] = councilKind.ToString(),
                            ["ctg.provider_count"] = registered.Count.ToString(System.Globalization.CultureInfo.InvariantCulture)
                        }
                    }));
            }

            outputs.Add(registered[0]);
        }

        return ValueTask.FromResult(Result<IReadOnlyList<ProviderVoteOutput>>.Ok(outputs));
    }

    /// <summary>
    /// EN: Resolves registered provider vote outputs in deterministic council order.
    /// EN: Documentation for public API. JA: 登録済み provider vote output を決定論的な council 順で解決します。
    /// </summary>
    /// <param name="cancellationToken">EN: The cancellation token. JA: キャンセル通知を監視するトークンです。</param>
    /// <returns>EN: The resolved provider vote outputs. JA: 解決された provider vote output を返します。</returns>
    public async ValueTask<IReadOnlyList<ProviderVoteOutput>> ResolveAsync(
        CancellationToken cancellationToken)
    {
        var result = await ResolveResultAsync(cancellationToken).ConfigureAwait(false);

        return result.Match(
            error => throw new InvalidOperationException(error.Message),
            outputs => outputs);
    }

    /// <summary>
    /// EN: Clears registered provider vote outputs.
    /// EN: Documentation for public API. JA: 登録済み provider vote output を消去します。
    /// </summary>
    public void Clear()
        => _outputs.Clear();

    private static CouncilKind[] RequiredCouncils()
        =>
        [
            CouncilKind.Logos,
            CouncilKind.Ethos,
            CouncilKind.Pathos
        ];

    private static ProviderVoteOutput CreateMissingVote(CouncilKind councilKind)
        => new()
        {
            ProviderId = $"ctg.provider.missing.{councilKind.ToString().ToLowerInvariant()}",
            CouncilKind = councilKind,
            VoteValue = CouncilVoteValue.Unknown,
            Metadata = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["ctg.provider_router.missing"] = councilKind.ToString()
            }
        };
}
