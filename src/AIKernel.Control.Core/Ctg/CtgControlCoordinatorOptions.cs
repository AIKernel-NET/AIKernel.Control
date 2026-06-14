using AIKernel.Dtos.Governance;

namespace AIKernel.Control.Core.Ctg;

/// <summary>
/// EN: Configures the opt-in CTG control coordinator.
/// JA: opt-in の CTG Control coordinator を構成します。
/// </summary>
public sealed record CtgControlCoordinatorOptions
{
    /// <summary>
    /// EN: Gets the default operation identifier used when the request does not provide one.
    /// JA: 要求が識別子を提供しない場合に使用する既定の operation 識別子を取得します。
    /// </summary>
    public string OperationId { get; init; } = string.Empty;

    /// <summary>
    /// EN: Gets the default step identifier used for the policy boundary.
    /// JA: policy 境界で使用する既定の step 識別子を取得します。
    /// </summary>
    public string StepId { get; init; } = string.Empty;

    /// <summary>
    /// EN: Gets the provider vote outputs supplied by the opt-in host.
    /// JA: opt-in した host が供給する provider vote 出力を取得します。
    /// </summary>
    public IReadOnlyList<ProviderVoteOutput> ProviderOutputs { get; init; } = [];

    /// <summary>
    /// EN: Gets canonical references attached to generated governance requests.
    /// JA: 生成される governance request に付与する CanonReference を取得します。
    /// </summary>
    public IReadOnlyList<CanonReference> CanonReferences { get; init; } = [];

    /// <summary>
    /// EN: Gets the correlation identifier attached to governance DTOs.
    /// JA: governance DTO に付与する correlation 識別子を取得します。
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// EN: Gets the trace identifier attached to governance DTOs.
    /// JA: governance DTO に付与する trace 識別子を取得します。
    /// </summary>
    public string? TraceId { get; init; }

    /// <summary>
    /// EN: Gets the deterministic observed timestamp supplied by tests or hosts.
    /// JA: test または host が供給する決定論的な観測時刻を取得します。
    /// </summary>
    public DateTimeOffset? ObservedAt { get; init; }

    /// <summary>
    /// EN: Gets metadata copied into generated control governance DTOs.
    /// JA: 生成される Control governance DTO にコピーする metadata を取得します。
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; init; } =
        new Dictionary<string, string>(StringComparer.Ordinal);
}
