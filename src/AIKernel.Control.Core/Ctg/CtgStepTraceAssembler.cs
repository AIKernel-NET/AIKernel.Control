using AIKernel.Dtos.Governance;

namespace AIKernel.Control.Core.Ctg;

/// <summary>
/// EN: Assembles a skeleton step governance trace at the Control policy boundary.
/// EN: Documentation for public API. JA: Control policy 境界で skeleton の step governance trace を組み立てます。
/// </summary>
public sealed class CtgStepTraceAssembler
{
    /// <summary>
    /// EN: Creates a step governance trace from council and decision gate DTOs.
    /// EN: Documentation for public API. JA: council DTO と decision gate DTO から step governance trace を作成します。
    /// </summary>
    /// <param name="context">EN: The CTG control execution context. JA: CTG Control execution context です。</param>
    /// <param name="councilEvaluation">EN: The council evaluation result. JA: council evaluation result です。</param>
    /// <param name="decisionGate">EN: The Core decision gate result. JA: Core decision gate result です。</param>
    /// <returns>EN: The assembled step governance trace. JA: 組み立てた step governance trace を返します。</returns>
    public StepGovernanceTrace Assemble(
        CtgControlExecutionContext context,
        CouncilEvaluationResult councilEvaluation,
        DecisionGateResult decisionGate)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(councilEvaluation);
        ArgumentNullException.ThrowIfNull(decisionGate);

        var metadata = CtgControlMetadata.Merge(
            context.Metadata,
            decisionGate.Metadata,
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["ctg.control.trace_stage"] = "apply_policy"
            });

        return new StepGovernanceTrace
        {
            TraceId = CtgControlMetadata.FirstNonEmpty(context.TraceId, decisionGate.TraceId, context.OperationId),
            StepId = CtgControlMetadata.StepId(context),
            CouncilEvaluation = councilEvaluation,
            DecisionGate = decisionGate,
            CanonReferences = decisionGate.CanonReferences,
            RejectReasons = decisionGate.RejectReasons,
            ObservedAt = decisionGate.ObservedAt == default
                ? CtgControlMetadata.ObservedAt(context)
                : decisionGate.ObservedAt,
            CorrelationId = CtgControlMetadata.FirstNonEmpty(context.CorrelationId, decisionGate.CorrelationId),
            Metadata = metadata
        };
    }
}
