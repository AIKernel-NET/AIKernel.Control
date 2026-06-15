namespace AIKernel.Control.Core.Perception;

using AIKernel.Abstractions.Control;
using AIKernel.Control.Core.Ctg;
using AIKernel.Dtos.Control;
using AIKernel.Dtos.Governance;
using AIKernel.Enums.Governance;

/// <summary>
/// EN: Carries a perception-derived council vote candidate before CTG normalization.
/// JA: CTG 正規化前の perception 由来 council vote 候補を保持します。
/// </summary>
public sealed record PerceptionControlSignal
{
    /// <summary>EN: Gets the signal identifier. JA: signal 識別子を取得します。</summary>
    public string SignalId { get; init; } = string.Empty;

    /// <summary>EN: Gets the provider identifier that supplied the signal. JA: signal を供給した provider 識別子を取得します。</summary>
    public string ProviderId { get; init; } = string.Empty;

    /// <summary>EN: Gets the target council kind. JA: 対象 council kind を取得します。</summary>
    public CouncilKind CouncilKind { get; init; } = CouncilKind.Unknown;

    /// <summary>EN: Gets the proposed discrete vote value. JA: 提案された discrete vote value を取得します。</summary>
    public CouncilVoteValue VoteValue { get; init; } = CouncilVoteValue.Unknown;

    /// <summary>EN: Gets canonical references attached to the perception signal. JA: perception signal に付与された CanonReference を取得します。</summary>
    public IReadOnlyList<CanonReference> CanonReferences { get; init; } = [];

    /// <summary>EN: Gets signal metadata retained outside GateInput. JA: GateInput の外側に保持する signal metadata を取得します。</summary>
    public IReadOnlyDictionary<string, string> Metadata { get; init; } =
        new Dictionary<string, string>(StringComparer.Ordinal);
}

/// <summary>
/// EN: Carries perception-to-control orchestration input.
/// JA: perception-to-control orchestration input を保持します。
/// </summary>
public sealed record PerceptionControlRequest
{
    /// <summary>EN: Gets the operation identifier. JA: operation 識別子を取得します。</summary>
    public string OperationId { get; init; } = string.Empty;

    /// <summary>EN: Gets the policy step identifier. JA: policy step 識別子を取得します。</summary>
    public string StepId { get; init; } = string.Empty;

    /// <summary>EN: Gets the execution graph under control. JA: Control 対象の execution graph を取得します。</summary>
    public IExecutionGraph? Graph { get; init; }

    /// <summary>EN: Gets the control execution request. JA: control execution request を取得します。</summary>
    public ControlExecutionRequest? ExecutionRequest { get; init; }

    /// <summary>EN: Gets perception-derived signals. JA: perception 由来 signal を取得します。</summary>
    public IReadOnlyList<PerceptionControlSignal> Signals { get; init; } = [];

    /// <summary>EN: Gets canonical references attached to generated governance DTOs. JA: 生成される governance DTO に付与する CanonReference を取得します。</summary>
    public IReadOnlyList<CanonReference> CanonReferences { get; init; } = [];

    /// <summary>EN: Gets the correlation identifier. JA: correlation 識別子を取得します。</summary>
    public string? CorrelationId { get; init; }

    /// <summary>EN: Gets the trace identifier. JA: trace 識別子を取得します。</summary>
    public string? TraceId { get; init; }

    /// <summary>EN: Gets request metadata. JA: request metadata を取得します。</summary>
    public IReadOnlyDictionary<string, string> Metadata { get; init; } =
        new Dictionary<string, string>(StringComparer.Ordinal);
}

/// <summary>
/// EN: Carries dynamic pipeline selection after Core CTG evaluation.
/// JA: Core CTG 評価後の dynamic pipeline selection を保持します。
/// </summary>
public sealed record PerceptionPipelineSelection
{
    /// <summary>EN: Gets the selected pipeline mode. JA: 選択された pipeline mode を取得します。</summary>
    public string Mode { get; init; } = "observe";

    /// <summary>EN: Gets the CTG control decision envelope that drove the selection. JA: selection の根拠となった CTG Control decision envelope を取得します。</summary>
    public CtgControlDecisionEnvelope DecisionEnvelope { get; init; } = new();

    /// <summary>EN: Gets selection metadata. JA: selection metadata を取得します。</summary>
    public IReadOnlyDictionary<string, string> Metadata { get; init; } =
        new Dictionary<string, string>(StringComparer.Ordinal);
}
