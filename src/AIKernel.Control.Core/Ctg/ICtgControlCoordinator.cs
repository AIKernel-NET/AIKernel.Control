using AIKernel.Common.Results;

namespace AIKernel.Control.Core.Ctg;

/// <summary>
/// EN: Coordinates the opt-in CTG control governance flow.
/// JA: opt-in の CTG Control governance flow を調整します。
/// </summary>
public interface ICtgControlCoordinator
{
    /// <summary>
    /// EN: Evaluates a CTG control execution context through the Core decision gate.
    /// JA: CTG Control execution context を Core decision gate 経由で評価します。
    /// </summary>
    /// <param name="context">EN: The CTG control execution context. JA: CTG Control execution context です。</param>
    /// <param name="cancellationToken">EN: The cancellation token. JA: キャンセル通知を監視するトークンです。</param>
    /// <returns>EN: The CTG control decision envelope result. JA: CTG Control decision envelope result を返します。</returns>
    ValueTask<Result<CtgControlDecisionEnvelope>> EvaluateAsync(
        CtgControlExecutionContext context,
        CancellationToken cancellationToken);
}
