using AIKernel.Abstractions.Control;
using AIKernel.Dtos.Control;

namespace AIKernel.Control.Core.Ctg;

/// <summary>
/// EN: Provides a semantic opt-in policy name for CTG execution gate evaluation.
/// [EN] Documents this public package API member. [JA] CTG execution gate evaluation 用の意味論的な opt-in policy 名を提供します。
/// </summary>
public sealed class CtgExecutionGatePolicy : IControlPolicy
{
    private readonly CtgControlPolicyAdapter _adapter;

    /// <summary>
    /// EN: Initializes a policy wrapper over the CTG control policy adapter.
    /// [EN] Documents this public package API member. [JA] CTG Control policy adapter を包む policy wrapper を初期化します。
    /// </summary>
    /// <param name="adapter">EN: The CTG control policy adapter. JA: CTG Control policy adapter です。</param>
    public CtgExecutionGatePolicy(CtgControlPolicyAdapter adapter)
    {
        _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
    }

    /// <summary>
    /// EN: Delegates policy evaluation to the CTG control policy adapter.
    /// [EN] Documents this public package API member. [JA] policy evaluation を CTG Control policy adapter に委譲します。
    /// </summary>
    /// <param name="graph">EN: The execution graph. JA: execution graph です。</param>
    /// <param name="request">EN: The control execution request. JA: control execution request です。</param>
    /// <param name="cancellationToken">EN: The cancellation token. JA: キャンセル通知を監視するトークンです。</param>
    /// <returns>EN: The mapped control policy evaluation. JA: 写像された control policy evaluation を返します。</returns>
    public ValueTask<ControlPolicyEvaluation> EvaluateAsync(
        IExecutionGraph graph,
        ControlExecutionRequest request,
        CancellationToken cancellationToken = default)
        => _adapter.EvaluateAsync(graph, request, cancellationToken);
}
