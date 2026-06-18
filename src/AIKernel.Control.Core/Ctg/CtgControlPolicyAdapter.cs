using AIKernel.Abstractions.Control;
using AIKernel.Common.Results;
using AIKernel.Dtos.Control;

namespace AIKernel.Control.Core.Ctg;

/// <summary>
/// EN: Connects the opt-in CTG coordinator to the existing Apply Policy phase.
/// [EN] Documents this public package API member. [JA] opt-in の CTG coordinator を既存の Apply Policy phase に接続します。
/// </summary>
public sealed class CtgControlPolicyAdapter : IControlPolicy
{
    private readonly ICtgControlCoordinator _coordinator;
    private readonly CtgPolicyDecisionMapper _policyDecisionMapper;
    private readonly CtgControlCoordinatorOptions _options;

    /// <summary>
    /// EN: Initializes a CTG control policy adapter.
    /// [EN] Documents this public package API member. [JA] CTG Control policy adapter を初期化します。
    /// </summary>
    /// <param name="coordinator">EN: The CTG control coordinator. JA: CTG Control coordinator です。</param>
    /// <param name="policyDecisionMapper">EN: The policy decision mapper. JA: policy decision mapper です。</param>
    /// <param name="options">EN: The coordinator options. JA: coordinator option です。</param>
    public CtgControlPolicyAdapter(
        ICtgControlCoordinator coordinator,
        CtgPolicyDecisionMapper policyDecisionMapper,
        CtgControlCoordinatorOptions options)
    {
        _coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator));
        _policyDecisionMapper = policyDecisionMapper ?? throw new ArgumentNullException(nameof(policyDecisionMapper));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// EN: Evaluates the existing Control policy boundary through the opt-in CTG coordinator.
    /// [EN] Documents this public package API member. [JA] opt-in の CTG coordinator を通じて既存の Control policy 境界を評価します。
    /// </summary>
    /// <param name="graph">EN: The execution graph. JA: execution graph です。</param>
    /// <param name="request">EN: The control execution request. JA: control execution request です。</param>
    /// <param name="cancellationToken">EN: The cancellation token. JA: キャンセル通知を監視するトークンです。</param>
    /// <returns>EN: The mapped control policy evaluation. JA: 写像された control policy evaluation を返します。</returns>
    public async ValueTask<ControlPolicyEvaluation> EvaluateAsync(
        IExecutionGraph graph,
        ControlExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(graph);
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var contextResult =
            from validGraph in Result<IExecutionGraph>.Ok(graph)
            from validRequest in Result<ControlExecutionRequest>.Ok(request)
            select CtgControlMetadata.CreateContext(validGraph, validRequest, _options);

        var envelopeResult = await contextResult.Match(
            error => ValueTask.FromResult(Result<CtgControlDecisionEnvelope>.Fail(error)),
            context => _coordinator.EvaluateAsync(context, cancellationToken)).ConfigureAwait(false);

        var mapped =
            from envelope in envelopeResult
            select _policyDecisionMapper.Map(envelope);

        return mapped.Match(
            error => new ControlPolicyEvaluation(false, "DENY", error.Message),
            evaluation => evaluation);
    }
}
