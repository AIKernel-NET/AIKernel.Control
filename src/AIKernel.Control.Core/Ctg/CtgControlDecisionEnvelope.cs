using AIKernel.Dtos.Governance;

namespace AIKernel.Control.Core.Ctg;

/// <summary>
/// EN: Carries the CTG control decision and intermediate contract DTOs.
/// EN: Documentation for public API. JA: CTG Control decision と中間 contract DTO を保持します。
/// </summary>
public sealed record CtgControlDecisionEnvelope
{
    /// <summary>
    /// EN: Gets the execution context used to produce this envelope.
    /// EN: Documentation for public API. JA: この envelope を生成した execution context を取得します。
    /// </summary>
    public CtgControlExecutionContext Context { get; init; } = new();

    /// <summary>
    /// EN: Gets the normalized council evaluation result.
    /// EN: Documentation for public API. JA: 正規化された council evaluation result を取得します。
    /// </summary>
    public CouncilEvaluationResult CouncilEvaluation { get; init; } = new();

    /// <summary>
    /// EN: Gets the vote-only gate input passed to the Core evaluator.
    /// EN: Documentation for public API. JA: Core evaluator に渡した vote-only の gate input を取得します。
    /// </summary>
    public GateInput GateInput { get; init; } = new();

    /// <summary>
    /// EN: Gets the decision gate result returned by Core.
    /// EN: Documentation for public API. JA: Core が返した decision gate result を取得します。
    /// </summary>
    public DecisionGateResult DecisionGate { get; init; } = new();

    /// <summary>
    /// EN: Gets the skeleton step governance trace attached at the policy boundary.
    /// EN: Documentation for public API. JA: policy 境界に付与された skeleton の step governance trace を取得します。
    /// </summary>
    public StepGovernanceTrace StepTrace { get; init; } = new();

    /// <summary>
    /// EN: Gets an optional retry intent carrier kept outside GateInput.
    /// EN: Documentation for public API. JA: GateInput の外側に保持する任意の retry intent carrier を取得します。
    /// </summary>
    public CtgRetryIntentCarrier? RetryIntent { get; init; }

    /// <summary>
    /// EN: Gets an optional trajectory gate result when a later stage supplies one.
    /// EN: Documentation for public API. JA: 後続 stage が供給する場合の optional な trajectory gate result を取得します。
    /// </summary>
    public TrajectoryGateResult? TrajectoryGate { get; init; }

    /// <summary>
    /// EN: Gets envelope metadata.
    /// EN: Documentation for public API. JA: envelope metadata を取得します。
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; init; } =
        new Dictionary<string, string>(StringComparer.Ordinal);
}
