using AIKernel.Common.Results;

namespace AIKernel.Control.Core.Ctg;

/// <summary>
/// EN: Orchestrates CTG council provider vote resolution without Gate logic.
/// JA: Gate logic を持たずに CTG council provider vote resolution を orchestration します。
/// </summary>
public sealed class CtgCouncilEvaluationOrchestrator
{
    private readonly CtgCouncilEvaluationProviderRegistry _providerRegistry;

    /// <summary>
    /// EN: Initializes a CTG council evaluation orchestrator.
    /// JA: CTG council evaluation orchestrator を初期化します。
    /// </summary>
    /// <param name="providerRegistry">EN: The provider vote registry. JA: provider vote registry です。</param>
    public CtgCouncilEvaluationOrchestrator(
        CtgCouncilEvaluationProviderRegistry providerRegistry)
    {
        _providerRegistry = providerRegistry ?? throw new ArgumentNullException(nameof(providerRegistry));
    }

    /// <summary>
    /// EN: Resolves provider vote outputs for the three councils.
    /// JA: 3 つの council 用 provider vote output を解決します。
    /// </summary>
    /// <param name="cancellationToken">EN: The cancellation token. JA: キャンセル通知を監視するトークンです。</param>
    /// <returns>EN: The provider vote output result. JA: provider vote output result を返します。</returns>
    public ValueTask<Result<IReadOnlyList<ProviderVoteOutput>>> ResolveProviderVotesAsync(
        CancellationToken cancellationToken)
        => _providerRegistry.ResolveResultAsync(cancellationToken);
}
