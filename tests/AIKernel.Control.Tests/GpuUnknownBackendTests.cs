using AIKernel.Control.GPU;
using AIKernel.Dtos.Gpu;
using AIKernel.Enums;

namespace AIKernel.Control.Tests;

public sealed class GpuUnknownBackendTests
{
    public static IEnumerable<object[]> UnknownBackends()
    {
        foreach (var value in new[] { 0, -1, 8, int.MinValue, int.MaxValue })
        foreach (var required in new[] { GpuProviderCapabilities.None, GpuProviderCapabilities.SupportsCompute })
            yield return [value, required];
    }

    [Theory]
    [MemberData(nameof(UnknownBackends))]
    public void UnknownAndUndefinedBackendsSelectCpuWithConsistentDiagnostics(int backend, GpuProviderCapabilities required)
    {
        var request = ReadyRequest() with { Backend = (GpuBackend)backend, RequiredCapabilities = required };
        AssertUnknownFallback(request, GpuExecutionArbitrator.Resolve(request));
    }

    [Fact]
    public void OmittedBackendSelectsCpuEvenWhenAllRequirementsAreSatisfied()
    {
        var request = ReadyRequest();
        Assert.Equal(GpuBackend.Unknown, request.Backend);
        AssertUnknownFallback(request, GpuExecutionArbitrator.Resolve(request));
    }

    [Theory]
    [InlineData(GpuBackend.WebGpu)]
    [InlineData(GpuBackend.Dawn)]
    [InlineData(GpuBackend.Cuda)]
    [InlineData(GpuBackend.Vulkan)]
    [InlineData(GpuBackend.Direct3D12)]
    [InlineData(GpuBackend.Metal)]
    public void KnownGpuBackendsKeepTheirIdentityAndApproval(GpuBackend backend)
    {
        var decision = GpuExecutionArbitrator.Resolve(ReadyRequest() with { Backend = backend });
        Assert.True(decision.UseGpu);
        Assert.False(decision.UsingCpuFallback);
        Assert.Equal(backend, decision.Backend);
        Assert.Equal(GpuExecutionArbitrationReasons.GpuExecutionApproved, decision.Reason);
        var metadata = decision.ToDiagnosticsMetadata();
        Assert.Equal(backend.ToString(), metadata[GpuProviderMetadataKeys.Backend]);
        Assert.Equal(GpuExecutionModes.ControlGpuApproved, metadata[GpuDiagnosticsMetadataKeys.CanonicalExecutionMode]);
        Assert.Equal(GpuExecutionArbitrationReasons.None, metadata[GpuProviderMetadataKeys.Fallback]);
        Assert.True(GpuCanonicalValidation.ValidateControlArbitrationMetadata(metadata).IsValid);
    }

    public static IEnumerable<object[]> ExistingFallbacks()
    {
        foreach (var backend in new[] { GpuBackend.Unknown, (GpuBackend)(-1) })
        {
            yield return [backend, "forced", GpuExecutionArbitrationReasons.CpuForced];
            yield return [backend, "lost", GpuExecutionArbitrationReasons.GpuLostOneWayFallback];
            yield return [backend, "missing", "missing-capabilities:SupportsCompute"];
            yield return [backend, "zero-copy", GpuExecutionArbitrationReasons.ZeroCopyRequiredButUnavailable];
            yield return [backend, "raw-target", GpuExecutionArbitrationReasons.RawFramebufferRequiredForZeroCopyAnalysis];
            yield return [backend, "bridge", GpuExecutionArbitrationReasons.NativePassBridgeUnavailable];
            yield return [backend, "readback", GpuExecutionArbitrationReasons.ReadbackRequiresFallback];
        }
    }

    [Theory]
    [MemberData(nameof(ExistingFallbacks))]
    public void ExistingFallbackDecisionsAndReasonPriorityRemainUnchanged(GpuBackend backend, string condition, string reason)
    {
        var request = ReadyRequest() with { Backend = backend };
        request = condition switch
        {
            "forced" => request with { ForceCpu = true, GpuLost = true },
            "lost" => request with { GpuLost = true, AvailableCapabilities = GpuProviderCapabilities.None },
            "missing" => request with { AvailableCapabilities = GpuProviderCapabilities.None },
            "zero-copy" => request with { RequireZeroCopyRawTexture = true, AvailableCapabilities = GpuProviderCapabilities.SupportsCompute },
            "raw-target" => request with { RequireZeroCopyRawTexture = true, InputTargetKind = GpuFrameTargetKind.HudCompositeOffscreen },
            "bridge" => request with { NativePassBridgeAvailable = false },
            "readback" => request with { ReadbackPolicy = GpuReadbackPolicy.RequiredFallback },
            _ => throw new ArgumentOutOfRangeException(nameof(condition))
        };
        var decision = GpuExecutionArbitrator.Resolve(request);
        Assert.False(decision.UseGpu);
        Assert.True(decision.UsingCpuFallback);
        Assert.Equal(GpuBackend.CpuFallback, decision.Backend);
        Assert.Equal(reason, decision.Reason);
        Assert.Equal(request.RequiredCapabilities & ~request.AvailableCapabilities, decision.MissingCapabilities);
        var metadata = decision.ToDiagnosticsMetadata();
        Assert.Equal(reason, metadata[GpuProviderMetadataKeys.Fallback]);
        Assert.Equal(GpuExecutionModes.ControlCpuFallback, metadata[GpuDiagnosticsMetadataKeys.CanonicalExecutionMode]);
        Assert.True(GpuCanonicalValidation.ValidateControlArbitrationMetadata(metadata).IsValid);
    }

    private static GpuExecutionArbitrationRequest ReadyRequest() => new()
    {
        IntentId = "ct4-intent", ObjectiveId = "ct4-objective", ActionId = "ct4-action",
        PassId = GpuOperationNames.ComputeDispatch,
        RequiredCapabilities = GpuProviderCapabilities.SupportsCompute,
        AvailableCapabilities = GpuProviderCapabilities.SupportsCompute | GpuProviderCapabilities.SupportsZeroCopyRawTexture,
        InputTargetKind = GpuFrameTargetKind.RawFramebuffer,
        RequireNativePassBridge = true,
        NativePassBridgeAvailable = true
    };

    private static void AssertUnknownFallback(GpuExecutionArbitrationRequest request, GpuExecutionArbitrationDecision decision)
    {
        Assert.False(decision.UseGpu);
        Assert.True(decision.UsingCpuFallback);
        Assert.Equal(GpuBackend.CpuFallback, decision.Backend);
        Assert.Equal("unknown-backend", decision.Reason);
        Assert.Equal(new[] { "cpu", "unknown-backend" }, decision.Tags);
        Assert.Equal(request.IntentId, decision.IntentId);
        Assert.Equal(request.ObjectiveId, decision.ObjectiveId);
        Assert.Equal(request.ActionId, decision.ActionId);
        Assert.Equal(request.PassId, decision.PassId);
        Assert.Equal(request.RequiredCapabilities, decision.RequiredCapabilities);
        Assert.Equal(request.AvailableCapabilities, decision.AvailableCapabilities);
        Assert.Equal(GpuProviderCapabilities.None, decision.MissingCapabilities);
        Assert.Equal(request.InputTargetKind, decision.InputTargetKind);
        Assert.Equal(request.ReadbackPolicy, decision.ReadbackPolicy);
        Assert.Equal(request.RequireZeroCopyRawTexture, decision.ZeroCopyRawTextureRequired);
        Assert.Equal(request.RequireNativePassBridge, decision.NativePassBridgeRequired);
        Assert.Equal(request.NativePassBridgeAvailable, decision.NativePassBridgeAvailable);
        var metadata = decision.ToDiagnosticsMetadata();
        Assert.Equal(GpuBackend.CpuFallback.ToString(), metadata[GpuProviderMetadataKeys.Backend]);
        Assert.Equal("unknown-backend", metadata[GpuProviderMetadataKeys.Fallback]);
        Assert.Equal("unknown-backend", metadata[GpuControlMetadataKeys.Reason]);
        Assert.Equal(GpuExecutionModes.ControlCpuFallback, metadata[GpuDiagnosticsMetadataKeys.CanonicalExecutionMode]);
        Assert.True(GpuCanonicalValidation.ValidateControlArbitrationMetadata(metadata).IsValid);
    }
}
