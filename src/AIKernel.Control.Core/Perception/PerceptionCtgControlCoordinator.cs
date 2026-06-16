namespace AIKernel.Control.Core.Perception;

using AIKernel.Common.Results;
using AIKernel.Control.Core.Ctg;

/// <summary>
/// EN: Orchestrates perception-derived votes through the existing CTG control coordinator.
/// EN: Documentation for public API. JA: perception 由来 vote を既存 CTG Control coordinator へ orchestration します。
/// </summary>
public sealed class PerceptionCtgControlCoordinator
{
    private readonly PerceptionControlAdapter _adapter;
    private readonly ICtgControlCoordinator _coordinator;

    /// <summary>
    /// EN: Initializes a perception CTG control coordinator.
    /// EN: Documentation for public API. JA: perception CTG Control coordinator を初期化します。
    /// </summary>
    /// <param name="adapter">EN: Perception control adapter. JA: perception control adapter です。</param>
    /// <param name="coordinator">EN: CTG control coordinator. JA: CTG Control coordinator です。</param>
    public PerceptionCtgControlCoordinator(
        PerceptionControlAdapter adapter,
        ICtgControlCoordinator coordinator)
    {
        _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        _coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator));
    }

    /// <summary>
    /// EN: Evaluates perception-derived control signals by delegating Gate evaluation to Core through CTG Control.
    /// EN: Documentation for public API. JA: CTG Control 経由で Gate 評価を Core に委譲し、perception 由来 control signal を評価します。
    /// </summary>
    /// <param name="request">EN: Perception control request. JA: perception control request です。</param>
    /// <param name="cancellationToken">EN: Cancellation token. JA: キャンセル通知を監視するトークンです。</param>
    /// <returns>EN: CTG control decision envelope result. JA: CTG Control decision envelope result を返します。</returns>
    public async ValueTask<Result<CtgControlDecisionEnvelope>> EvaluateAsync(
        PerceptionControlRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var providerOutputs = _adapter.Adapt(request);
        var contextResult =
            from outputs in providerOutputs
            select new CtgControlExecutionContext
            {
                OperationId = request.OperationId,
                StepId = request.StepId,
                Graph = request.Graph,
                Request = request.ExecutionRequest,
                ProviderOutputs = outputs,
                RetryIntent = request.RetryIntent,
                CanonReferences = request.CanonReferences,
                CorrelationId = request.CorrelationId,
                TraceId = request.TraceId,
                Metadata = request.Metadata
            };

        return await contextResult.Match(
            error => ValueTask.FromResult(Result<CtgControlDecisionEnvelope>.Fail(error)),
            context => _coordinator.EvaluateAsync(context, cancellationToken)).ConfigureAwait(false);
    }
}
