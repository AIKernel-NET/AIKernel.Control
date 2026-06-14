using AIKernel.Abstractions.Governance;
using AIKernel.Common.Results;
using AIKernel.Dtos.Governance;

namespace AIKernel.Control.Core.Ctg;

/// <summary>
/// EN: Orchestrates provider vote normalization, council decision building, and Core decision gate evaluation.
/// JA: provider vote 正規化、council decision 構築、Core decision gate 評価を orchestration します。
/// </summary>
public sealed class CtgControlCoordinator : ICtgControlCoordinator
{
    private readonly ProviderVoteAdapter _providerVoteAdapter;
    private readonly CouncilDecisionBuilder _councilDecisionBuilder;
    private readonly CouncilDecisionToGateInputAdapter _gateInputAdapter;
    private readonly CtgStepTraceAssembler _stepTraceAssembler;
    private readonly IDecisionGate _decisionGate;

    /// <summary>
    /// EN: Initializes a CTG control coordinator.
    /// JA: CTG Control coordinator を初期化します。
    /// </summary>
    /// <param name="providerVoteAdapter">EN: The provider vote adapter. JA: provider vote adapter です。</param>
    /// <param name="councilDecisionBuilder">EN: The council decision builder. JA: council decision builder です。</param>
    /// <param name="gateInputAdapter">EN: The gate input adapter. JA: gate input adapter です。</param>
    /// <param name="stepTraceAssembler">EN: The step trace assembler. JA: step trace assembler です。</param>
    /// <param name="decisionGate">EN: The Core decision gate evaluator. JA: Core decision gate evaluator です。</param>
    public CtgControlCoordinator(
        ProviderVoteAdapter providerVoteAdapter,
        CouncilDecisionBuilder councilDecisionBuilder,
        CouncilDecisionToGateInputAdapter gateInputAdapter,
        CtgStepTraceAssembler stepTraceAssembler,
        IDecisionGate decisionGate)
    {
        _providerVoteAdapter = providerVoteAdapter ?? throw new ArgumentNullException(nameof(providerVoteAdapter));
        _councilDecisionBuilder = councilDecisionBuilder ?? throw new ArgumentNullException(nameof(councilDecisionBuilder));
        _gateInputAdapter = gateInputAdapter ?? throw new ArgumentNullException(nameof(gateInputAdapter));
        _stepTraceAssembler = stepTraceAssembler ?? throw new ArgumentNullException(nameof(stepTraceAssembler));
        _decisionGate = decisionGate ?? throw new ArgumentNullException(nameof(decisionGate));
    }

    /// <summary>
    /// EN: Evaluates a CTG control context by normalizing votes and calling the Core decision gate.
    /// JA: vote を正規化して Core decision gate を呼び出し、CTG Control context を評価します。
    /// </summary>
    /// <param name="context">EN: The CTG control execution context. JA: CTG Control execution context です。</param>
    /// <param name="cancellationToken">EN: The cancellation token. JA: キャンセル通知を監視するトークンです。</param>
    /// <returns>EN: The CTG control decision envelope result. JA: CTG Control decision envelope result を返します。</returns>
    public async ValueTask<Result<CtgControlDecisionEnvelope>> EvaluateAsync(
        CtgControlExecutionContext context,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var votesResult = await AdaptVotesAsync(context, cancellationToken).ConfigureAwait(false);

        var prepared =
            from votes in votesResult
            from councilEvaluation in _councilDecisionBuilder.Build(context, votes)
            from gateInput in _gateInputAdapter.Adapt(councilEvaluation.Decision)
            select new CtgControlPipelineState(councilEvaluation, gateInput);

        return await prepared.Match(
            error => ValueTask.FromResult(Result<CtgControlDecisionEnvelope>.Fail(error)),
            state => EvaluateCoreDecisionGateAsync(context, state, cancellationToken)).ConfigureAwait(false);
    }

    private async ValueTask<Result<IReadOnlyList<CouncilVote>>> AdaptVotesAsync(
        CtgControlExecutionContext context,
        CancellationToken cancellationToken)
    {
        var votes = new List<CouncilVote>(context.ProviderOutputs.Count);

        foreach (var output in context.ProviderOutputs)
        {
            var adapted = await _providerVoteAdapter
                .AdaptAsync(output, context, cancellationToken)
                .ConfigureAwait(false);

            if (adapted.IsFailure)
            {
                return Result<IReadOnlyList<CouncilVote>>.Fail(adapted.Error!);
            }

            votes.Add(adapted.Value!);
        }

        return Result<IReadOnlyList<CouncilVote>>.Ok(votes);
    }

    private async ValueTask<Result<CtgControlDecisionEnvelope>> EvaluateCoreDecisionGateAsync(
        CtgControlExecutionContext context,
        CtgControlPipelineState state,
        CancellationToken cancellationToken)
    {
        var requestResult =
            from gateInput in Result<GateInput>.Ok(state.GateInput)
            select new DecisionGateRequest
            {
                OperationId = CtgControlMetadata.OperationId(context),
                StepId = CtgControlMetadata.StepId(context),
                GateInput = gateInput,
                CanonReferences = context.CanonReferences,
                CorrelationId = context.CorrelationId,
                TraceId = context.TraceId,
                Metadata = CtgControlMetadata.Merge(context.Metadata)
            };

        return await requestResult.Match(
            error => ValueTask.FromResult(Result<CtgControlDecisionEnvelope>.Fail(error)),
            request => EvaluateCoreDecisionGateAsync(context, state, request, cancellationToken)).ConfigureAwait(false);
    }

    private async ValueTask<Result<CtgControlDecisionEnvelope>> EvaluateCoreDecisionGateAsync(
        CtgControlExecutionContext context,
        CtgControlPipelineState state,
        DecisionGateRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var decisionGate = await _decisionGate
                .EvaluateAsync(request, cancellationToken)
                .ConfigureAwait(false);
            var envelope =
                from gate in Result<DecisionGateResult>.Ok(decisionGate)
                select BuildEnvelope(context, state.CouncilEvaluation, state.GateInput, gate);

            return envelope;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return Result<CtgControlDecisionEnvelope>.Fail(ErrorContext.FromException(ex));
        }
    }

    private CtgControlDecisionEnvelope BuildEnvelope(
        CtgControlExecutionContext context,
        CouncilEvaluationResult councilEvaluation,
        GateInput gateInput,
        DecisionGateResult decisionGate)
    {
        var stepTrace = _stepTraceAssembler.Assemble(context, councilEvaluation, decisionGate);
        var metadata = CtgControlMetadata.Merge(
            context.Metadata,
            decisionGate.Metadata,
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["ctg.control.policy_stage"] = "apply_policy"
            });

        return new CtgControlDecisionEnvelope
        {
            Context = context,
            CouncilEvaluation = councilEvaluation,
            GateInput = gateInput,
            DecisionGate = decisionGate,
            StepTrace = stepTrace,
            Metadata = metadata
        };
    }

    private sealed record CtgControlPipelineState(
        CouncilEvaluationResult CouncilEvaluation,
        GateInput GateInput);
}
