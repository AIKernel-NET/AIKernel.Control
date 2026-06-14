using AIKernel.Abstractions.Control;
using AIKernel.Dtos.Control;
using AIKernel.Dtos.Governance;

namespace AIKernel.Control.Core.Ctg;

/// <summary>
/// EN: Carries execution state for a CTG control policy evaluation.
/// JA: CTG Control policy 評価の実行状態を保持します。
/// </summary>
public sealed record CtgControlExecutionContext
{
    /// <summary>
    /// EN: Gets the governance operation identifier.
    /// JA: governance operation 識別子を取得します。
    /// </summary>
    public string OperationId { get; init; } = string.Empty;

    /// <summary>
    /// EN: Gets the policy step identifier.
    /// JA: policy step 識別子を取得します。
    /// </summary>
    public string StepId { get; init; } = string.Empty;

    /// <summary>
    /// EN: Gets the execution graph being evaluated.
    /// JA: 評価対象の execution graph を取得します。
    /// </summary>
    public IExecutionGraph? Graph { get; init; }

    /// <summary>
    /// EN: Gets the control execution request being evaluated.
    /// JA: 評価対象の control execution request を取得します。
    /// </summary>
    public ControlExecutionRequest? Request { get; init; }

    /// <summary>
    /// EN: Gets provider vote outputs to normalize into council votes.
    /// JA: council vote に正規化する provider vote 出力を取得します。
    /// </summary>
    public IReadOnlyList<ProviderVoteOutput> ProviderOutputs { get; init; } = [];

    /// <summary>
    /// EN: Gets canonical references attached to generated governance DTOs.
    /// JA: 生成される governance DTO に付与する CanonReference を取得します。
    /// </summary>
    public IReadOnlyList<CanonReference> CanonReferences { get; init; } = [];

    /// <summary>
    /// EN: Gets the correlation identifier.
    /// JA: correlation 識別子を取得します。
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// EN: Gets the trace identifier.
    /// JA: trace 識別子を取得します。
    /// </summary>
    public string? TraceId { get; init; }

    /// <summary>
    /// EN: Gets the observed timestamp used by generated DTOs.
    /// JA: 生成される DTO で使用する観測時刻を取得します。
    /// </summary>
    public DateTimeOffset? ObservedAt { get; init; }

    /// <summary>
    /// EN: Gets control metadata copied into generated governance DTOs.
    /// JA: 生成される governance DTO にコピーする Control metadata を取得します。
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; init; } =
        new Dictionary<string, string>(StringComparer.Ordinal);
}
