using AIKernel.Dtos.Gpu;
using AIKernel.Enums;

namespace AIKernel.Control.GPU;

/// <summary>
/// [EN] Request for Control-plane GPU execution arbitration.
/// [JA] Control-plane GPU execution arbitration 用 request です。
/// </summary>
public sealed record GpuExecutionArbitrationRequest
{
    /// <summary>[EN] Intent identifier. [JA] intent 識別子です。</summary>
    public required string IntentId { get; init; }

    /// <summary>[EN] Objective identifier. [JA] objective 識別子です。</summary>
    public required string ObjectiveId { get; init; }

    /// <summary>[EN] Action identifier. [JA] action 識別子です。</summary>
    public required string ActionId { get; init; }

    /// <summary>[EN] Backend selected by the provider. [JA] Provider が選択した backend です。</summary>
    public GpuBackend Backend { get; init; } = GpuBackend.Unknown;

    /// <summary>[EN] Capabilities required by the action. [JA] action が必要とする capability です。</summary>
    public GpuProviderCapabilities RequiredCapabilities { get; init; } = GpuProviderCapabilities.SupportsCompute;

    /// <summary>[EN] Capabilities currently available. [JA] 現在利用できる capability です。</summary>
    public GpuProviderCapabilities AvailableCapabilities { get; init; }

    /// <summary>[EN] Canonical pass identifier used for diagnostics. [JA] diagnostics 用の canonical pass 識別子です。</summary>
    public string PassId { get; init; } = GpuOperationNames.ComputeDispatch;

    /// <summary>[EN] Input frame target kind consumed by this control action. [JA] この control action が消費する input frame target kind です。</summary>
    public GpuFrameTargetKind InputTargetKind { get; init; } = GpuFrameTargetKind.Unknown;

    /// <summary>[EN] True when the action requires a native/browser rev3 pass bridge. [JA] action が native/browser rev3 pass bridge を必要とする場合 true です。</summary>
    public bool RequireNativePassBridge { get; init; }

    /// <summary>[EN] True when the provider has a native/browser rev3 pass bridge. [JA] provider が native/browser rev3 pass bridge を持つ場合 true です。</summary>
    public bool NativePassBridgeAvailable { get; init; }

    /// <summary>[EN] True when user or policy forces CPU. [JA] user または policy が CPU を強制する場合 true です。</summary>
    public bool ForceCpu { get; init; }

    /// <summary>[EN] True when the GPU was lost and fallback must be one-way. [JA] GPU lost が発生し fallback を one-way にする必要がある場合 true です。</summary>
    public bool GpuLost { get; init; }

    /// <summary>[EN] Requested readback policy. [JA] 要求された readback policy です。</summary>
    public GpuReadbackPolicy ReadbackPolicy { get; init; } = GpuReadbackPolicy.None;

    /// <summary>[EN] True when zero-copy raw texture is required. [JA] zero-copy raw texture が必須の場合 true です。</summary>
    public bool RequireZeroCopyRawTexture { get; init; }
}

/// <summary>
/// [EN] Decision produced by Control-plane GPU arbitration.
/// [JA] Control-plane GPU arbitration が生成する decision です。
/// </summary>
public sealed record GpuExecutionArbitrationDecision
{
    /// <summary>[EN] Intent identifier. [JA] intent 識別子です。</summary>
    public required string IntentId { get; init; }

    /// <summary>[EN] Objective identifier. [JA] objective 識別子です。</summary>
    public required string ObjectiveId { get; init; }

    /// <summary>[EN] Action identifier. [JA] action 識別子です。</summary>
    public required string ActionId { get; init; }

    /// <summary>[EN] True when the action should use GPU execution. [JA] action が GPU execution を使う場合 true です。</summary>
    public bool UseGpu { get; init; }

    /// <summary>[EN] Canonical pass identifier used for diagnostics. [JA] diagnostics 用の canonical pass 識別子です。</summary>
    public string PassId { get; init; } = GpuOperationNames.ComputeDispatch;

    /// <summary>[EN] Effective backend. [JA] effective backend です。</summary>
    public GpuBackend Backend { get; init; } = GpuBackend.CpuFallback;

    /// <summary>[EN] Human-readable decision reason. [JA] 人間可読な decision reason です。</summary>
    public required string Reason { get; init; }

    /// <summary>[EN] Missing capabilities, if any. [JA] 不足している capability です。</summary>
    public GpuProviderCapabilities MissingCapabilities { get; init; }

    /// <summary>[EN] Capabilities required by the action. [JA] action が必要とする capability です。</summary>
    public GpuProviderCapabilities RequiredCapabilities { get; init; }

    /// <summary>[EN] Capabilities available to the action. [JA] action が利用できる capability です。</summary>
    public GpuProviderCapabilities AvailableCapabilities { get; init; }

    /// <summary>[EN] Input frame target kind consumed by this action. [JA] この action が消費する input frame target kind です。</summary>
    public GpuFrameTargetKind InputTargetKind { get; init; } = GpuFrameTargetKind.Unknown;

    /// <summary>[EN] Effective readback policy. [JA] effective readback policy です。</summary>
    public GpuReadbackPolicy ReadbackPolicy { get; init; } = GpuReadbackPolicy.None;

    /// <summary>[EN] True when zero-copy raw texture was required. [JA] zero-copy raw texture が必須だった場合 true です。</summary>
    public bool ZeroCopyRawTextureRequired { get; init; }

    /// <summary>[EN] True when native/browser pass bridge was required. [JA] native/browser pass bridge が必須だった場合 true です。</summary>
    public bool NativePassBridgeRequired { get; init; }

    /// <summary>[EN] True when native/browser pass bridge was available. [JA] native/browser pass bridge が利用可能だった場合 true です。</summary>
    public bool NativePassBridgeAvailable { get; init; }

    /// <summary>[EN] True when CPU fallback is active. [JA] CPU fallback が有効な場合 true です。</summary>
    public bool UsingCpuFallback => !UseGpu;

    /// <summary>[EN] Compact diagnostics tags for HUD/logging. [JA] HUD/logging 用の compact diagnostics tag です。</summary>
    public IReadOnlyList<string> Tags { get; init; } = [];

    /// <summary>
    /// [EN] Projects this decision into canonical rev3 diagnostics metadata.
    /// [JA] この decision を canonical rev3 diagnostics metadata に投影します。
    /// </summary>
    public IReadOnlyDictionary<string, string> ToDiagnosticsMetadata()
        => new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            [GpuControlMetadataKeys.IntentId] = IntentId,
            [GpuControlMetadataKeys.ObjectiveId] = ObjectiveId,
            [GpuControlMetadataKeys.ActionId] = ActionId,
            [GpuControlMetadataKeys.Reason] = Reason,
            [GpuControlMetadataKeys.RequiredCapabilities] = RequiredCapabilities.ToString(),
            [GpuControlMetadataKeys.AvailableCapabilities] = AvailableCapabilities.ToString(),
            [GpuControlMetadataKeys.MissingCapabilities] = MissingCapabilities.ToString(),
            [GpuControlMetadataKeys.InputTargetKind] = InputTargetKind.ToString(),
            [GpuControlMetadataKeys.ReadbackPolicy] = ReadbackPolicy.ToString(),
            [GpuControlMetadataKeys.ZeroCopyRawTextureRequired] = ZeroCopyRawTextureRequired.ToString().ToLowerInvariant(),
            [GpuControlMetadataKeys.NativePassBridgeRequired] = NativePassBridgeRequired.ToString().ToLowerInvariant(),
            [GpuControlMetadataKeys.NativePassBridgeAvailable] = NativePassBridgeAvailable.ToString().ToLowerInvariant(),
            [GpuControlMetadataKeys.Tags] = string.Join(",", Tags),
            [GpuDiagnosticsMetadataKeys.Rev3PassId] = PassId,
            [GpuDiagnosticsMetadataKeys.Rev3ExecutionMode] = UseGpu
                ? GpuRev3ExecutionModes.ControlGpuApproved
                : GpuRev3ExecutionModes.ControlCpuFallback,
            [GpuProviderMetadataKeys.Backend] = Backend.ToString(),
            [GpuProviderMetadataKeys.Fallback] = UsingCpuFallback ? Reason : GpuExecutionArbitrationReasons.None
        };
}

/// <summary>
/// [EN] Stable reason strings emitted by Control-plane GPU arbitration.
/// [JA] Control-plane GPU arbitration が出力する stable reason 文字列です。
/// </summary>
public static class GpuExecutionArbitrationReasons
{
    /// <summary>[EN] No fallback is active. [JA] fallback が有効ではありません。</summary>
    public const string None = "none";

    /// <summary>[EN] GPU execution is approved. [JA] GPU execution が承認されました。</summary>
    public const string GpuExecutionApproved = "gpu-execution-approved";

    /// <summary>[EN] CPU execution was forced by user or policy. [JA] user または policy により CPU execution が強制されました。</summary>
    public const string CpuForced = "cpu-forced";

    /// <summary>[EN] GPU was lost and one-way CPU fallback is active. [JA] GPU lost により one-way CPU fallback が有効です。</summary>
    public const string GpuLostOneWayFallback = "gpu-lost-one-way-fallback";

    /// <summary>[EN] Required zero-copy raw texture is unavailable. [JA] 必須の zero-copy raw texture が利用できません。</summary>
    public const string ZeroCopyRequiredButUnavailable = "zero-copy-required-but-unavailable";

    /// <summary>[EN] Raw framebuffer is required for zero-copy analysis. [JA] zero-copy analysis には raw framebuffer が必要です。</summary>
    public const string RawFramebufferRequiredForZeroCopyAnalysis = "raw-framebuffer-required-for-zero-copy-analysis";

    /// <summary>[EN] Required native/browser pass bridge is unavailable. [JA] 必須の native/browser pass bridge が利用できません。</summary>
    public const string NativePassBridgeUnavailable = "native-pass-bridge-unavailable";

    /// <summary>[EN] Requested readback policy requires fallback. [JA] 要求された readback policy が fallback を必要とします。</summary>
    public const string ReadbackRequiresFallback = "readback-requires-fallback";
}

/// <summary>
/// [EN] Stable execution-mode strings emitted by Control-plane GPU arbitration metadata.
/// [JA] Control-plane GPU arbitration metadata が出力する stable execution-mode 文字列です。
/// </summary>
public static class GpuExecutionArbitrationExecutionModes
{
    /// <summary>[EN] Control approved GPU execution. [JA] Control が GPU execution を承認しました。</summary>
    public const string ControlGpuApproved = GpuRev3ExecutionModes.ControlGpuApproved;

    /// <summary>[EN] Control selected CPU fallback. [JA] Control が CPU fallback を選択しました。</summary>
    public const string ControlCpuFallback = GpuRev3ExecutionModes.ControlCpuFallback;
}

/// <summary>
/// [EN] Deterministic GPU execution arbitration used by Intent/Objective/Action layers.
/// [JA] Intent/Objective/Action layer が使用する deterministic GPU execution arbitration です。
/// </summary>
public static class GpuExecutionArbitrator
{
    /// <summary>
    /// [EN] Resolves whether a control action may use GPU execution.
    /// [JA] control action が GPU execution を使用してよいかを解決します。
    /// </summary>
    public static GpuExecutionArbitrationDecision Resolve(GpuExecutionArbitrationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.ForceCpu)
        {
            return Cpu(GpuExecutionArbitrationReasons.CpuForced, request, ["cpu", "forced"]);
        }

        if (request.GpuLost)
        {
            return Cpu(GpuExecutionArbitrationReasons.GpuLostOneWayFallback, request, ["cpu", "gpu-lost"]);
        }

        var missing = request.RequiredCapabilities & ~request.AvailableCapabilities;
        if (missing != GpuProviderCapabilities.None)
        {
            return Cpu($"missing-capabilities:{missing}", request, ["cpu", "capability-miss"]);
        }

        if (request.RequireZeroCopyRawTexture &&
            !request.AvailableCapabilities.HasFlag(GpuProviderCapabilities.SupportsZeroCopyRawTexture))
        {
            return Cpu(GpuExecutionArbitrationReasons.ZeroCopyRequiredButUnavailable, request, ["cpu", "zcp-miss"]);
        }

        if (request.RequireZeroCopyRawTexture &&
            request.InputTargetKind == GpuFrameTargetKind.HudCompositeOffscreen)
        {
            return Cpu(GpuExecutionArbitrationReasons.RawFramebufferRequiredForZeroCopyAnalysis, request, ["cpu", "raw-target-required", request.PassId]);
        }

        if (request.RequireNativePassBridge && !request.NativePassBridgeAvailable)
        {
            return Cpu(GpuExecutionArbitrationReasons.NativePassBridgeUnavailable, request, ["cpu", "pass-bridge-miss", request.PassId]);
        }

        if (request.ReadbackPolicy == GpuReadbackPolicy.RequiredFallback)
        {
            return Cpu(GpuExecutionArbitrationReasons.ReadbackRequiresFallback, request, ["cpu", "readback"]);
        }

        return new GpuExecutionArbitrationDecision
        {
            IntentId = request.IntentId,
            ObjectiveId = request.ObjectiveId,
            ActionId = request.ActionId,
            UseGpu = true,
            PassId = request.PassId,
            Backend = request.Backend == GpuBackend.Unknown ? GpuBackend.WebGpu : request.Backend,
            Reason = GpuExecutionArbitrationReasons.GpuExecutionApproved,
            MissingCapabilities = GpuProviderCapabilities.None,
            RequiredCapabilities = request.RequiredCapabilities,
            AvailableCapabilities = request.AvailableCapabilities,
            InputTargetKind = request.InputTargetKind,
            ReadbackPolicy = request.ReadbackPolicy,
            ZeroCopyRawTextureRequired = request.RequireZeroCopyRawTexture,
            NativePassBridgeRequired = request.RequireNativePassBridge,
            NativePassBridgeAvailable = request.NativePassBridgeAvailable,
            Tags = ["gpu", "approved", request.IntentId, request.ObjectiveId, request.ActionId, request.PassId]
        };
    }

    private static GpuExecutionArbitrationDecision Cpu(
        string reason,
        GpuExecutionArbitrationRequest request,
        IReadOnlyList<string> tags)
        => new()
        {
            IntentId = request.IntentId,
            ObjectiveId = request.ObjectiveId,
            ActionId = request.ActionId,
            UseGpu = false,
            PassId = request.PassId,
            Backend = GpuBackend.CpuFallback,
            Reason = reason,
            MissingCapabilities = request.RequiredCapabilities & ~request.AvailableCapabilities,
            RequiredCapabilities = request.RequiredCapabilities,
            AvailableCapabilities = request.AvailableCapabilities,
            InputTargetKind = request.InputTargetKind,
            ReadbackPolicy = request.ReadbackPolicy,
            ZeroCopyRawTextureRequired = request.RequireZeroCopyRawTexture,
            NativePassBridgeRequired = request.RequireNativePassBridge,
            NativePassBridgeAvailable = request.NativePassBridgeAvailable,
            Tags = tags
        };
}
