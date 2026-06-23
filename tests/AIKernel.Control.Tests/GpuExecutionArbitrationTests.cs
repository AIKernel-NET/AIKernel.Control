using AIKernel.Control.GPU;
using AIKernel.Dtos.Gpu;
using AIKernel.Enums;

namespace AIKernel.Control.Tests;

public sealed class GpuExecutionArbitrationTests
{
    [Fact]
    public void ResolveApprovesGpuWhenRequiredCapabilitiesAreAvailable()
    {
        var decision = GpuExecutionArbitrator.Resolve(new GpuExecutionArbitrationRequest
        {
            IntentId = "render",
            ObjectiveId = "doom-hud",
            ActionId = "compose",
            Backend = GpuBackend.WebGpu,
            RequiredCapabilities = GpuProviderCapabilities.SupportsCompute |
                GpuProviderCapabilities.SupportsHudComposite |
                GpuProviderCapabilities.SupportsZeroCopyRawTexture,
            AvailableCapabilities = GpuProviderCapabilities.SupportsCompute |
                GpuProviderCapabilities.SupportsHudComposite |
                GpuProviderCapabilities.SupportsZeroCopyRawTexture
        });

        Assert.True(decision.UseGpu);
        Assert.Equal(GpuBackend.WebGpu, decision.Backend);
        Assert.Equal(GpuExecutionArbitrationReasons.GpuExecutionApproved, decision.Reason);
        Assert.Equal(GpuOperationNames.ComputeDispatch, decision.PassId);
        Assert.Contains(GpuOperationNames.ComputeDispatch, decision.Tags);

        var metadata = decision.ToDiagnosticsMetadata();
        Assert.Equal(GpuOperationNames.ComputeDispatch, metadata[GpuDiagnosticsMetadataKeys.Rev3PassId]);
        Assert.Equal(
            GpuExecutionArbitrationExecutionModes.ControlGpuApproved,
            metadata[GpuDiagnosticsMetadataKeys.Rev3ExecutionMode]);
        Assert.Equal(GpuExecutionArbitrationReasons.None, metadata[GpuProviderMetadataKeys.Fallback]);
        Assert.Equal("render", metadata[GpuControlMetadataKeys.IntentId]);
        Assert.Equal("doom-hud", metadata[GpuControlMetadataKeys.ObjectiveId]);
        Assert.Equal("compose", metadata[GpuControlMetadataKeys.ActionId]);
        Assert.Equal(GpuExecutionArbitrationReasons.GpuExecutionApproved, metadata[GpuControlMetadataKeys.Reason]);
        Assert.Equal(GpuProviderCapabilities.None.ToString(), metadata[GpuControlMetadataKeys.MissingCapabilities]);
        Assert.Contains("SupportsHudComposite", metadata[GpuControlMetadataKeys.RequiredCapabilities], StringComparison.Ordinal);
        Assert.Contains("approved", metadata[GpuControlMetadataKeys.Tags], StringComparison.Ordinal);
        Assert.True(GpuCanonicalValidation.ValidateControlArbitrationMetadata(metadata).IsValid);
    }

    [Fact]
    public void ResolveFallsBackOneWayAfterGpuLost()
    {
        var decision = GpuExecutionArbitrator.Resolve(new GpuExecutionArbitrationRequest
        {
            IntentId = "render",
            ObjectiveId = "doom-hud",
            ActionId = "compose",
            Backend = GpuBackend.WebGpu,
            GpuLost = true,
            NativePassBridgeAvailable = true,
            RequiredCapabilities = GpuProviderCapabilities.SupportsCompute,
            AvailableCapabilities = GpuProviderCapabilities.SupportsCompute
        });

        Assert.False(decision.UseGpu);
        Assert.Equal(GpuBackend.CpuFallback, decision.Backend);
        Assert.Equal(GpuExecutionArbitrationReasons.GpuLostOneWayFallback, decision.Reason);

        var metadata = decision.ToDiagnosticsMetadata();
        Assert.Equal(
            GpuExecutionArbitrationExecutionModes.ControlCpuFallback,
            metadata[GpuDiagnosticsMetadataKeys.Rev3ExecutionMode]);
        Assert.Equal(GpuExecutionArbitrationReasons.GpuLostOneWayFallback, metadata[GpuProviderMetadataKeys.Fallback]);
        Assert.Equal("true", metadata[GpuControlMetadataKeys.NativePassBridgeAvailable]);
        Assert.Equal("false", metadata[GpuControlMetadataKeys.ZeroCopyRawTextureRequired]);
        Assert.True(GpuCanonicalValidation.ValidateControlArbitrationMetadata(metadata).IsValid);
    }

    [Fact]
    public void ResolveFallsBackWhenZeroCopyIsRequiredButUnavailable()
    {
        var decision = GpuExecutionArbitrator.Resolve(new GpuExecutionArbitrationRequest
        {
            IntentId = "sense",
            ObjectiveId = "raw-analysis",
            ActionId = "aisthesis",
            Backend = GpuBackend.WebGpu,
            RequireZeroCopyRawTexture = true,
            RequiredCapabilities = GpuProviderCapabilities.SupportsCompute,
            AvailableCapabilities = GpuProviderCapabilities.SupportsCompute
        });

        Assert.False(decision.UseGpu);
        Assert.Equal(GpuExecutionArbitrationReasons.ZeroCopyRequiredButUnavailable, decision.Reason);

        var metadata = decision.ToDiagnosticsMetadata();
        Assert.Equal("true", metadata[GpuControlMetadataKeys.ZeroCopyRawTextureRequired]);
        Assert.Equal(GpuFrameTargetKind.Unknown.ToString(), metadata[GpuControlMetadataKeys.InputTargetKind]);
        Assert.True(GpuCanonicalValidation.ValidateControlArbitrationMetadata(metadata).IsValid);
    }

    [Fact]
    public void ResolveProjectsMissingCapabilitiesForDiagnostics()
    {
        var decision = GpuExecutionArbitrator.Resolve(new GpuExecutionArbitrationRequest
        {
            IntentId = "sense",
            ObjectiveId = "spatial-reasoning",
            ActionId = "topos",
            PassId = GpuOperationNames.GpuSpatialReasoning,
            Backend = GpuBackend.WebGpu,
            RequiredCapabilities = GpuProviderCapabilities.SupportsCompute |
                GpuProviderCapabilities.SupportsSpatialReasoning,
            AvailableCapabilities = GpuProviderCapabilities.SupportsCompute
        });

        Assert.False(decision.UseGpu);
        Assert.Equal(GpuProviderCapabilities.SupportsSpatialReasoning, decision.MissingCapabilities);

        var metadata = decision.ToDiagnosticsMetadata();
        Assert.Equal(GpuOperationNames.GpuSpatialReasoning, metadata[GpuDiagnosticsMetadataKeys.Rev3PassId]);
        Assert.Contains("SupportsSpatialReasoning", metadata[GpuControlMetadataKeys.MissingCapabilities], StringComparison.Ordinal);
        Assert.Contains("missing-capabilities", metadata[GpuControlMetadataKeys.Reason], StringComparison.Ordinal);
        Assert.True(GpuCanonicalValidation.ValidateControlArbitrationMetadata(metadata).IsValid);
    }

    [Fact]
    public void ResolveRejectsHudCompositeInputForRawAisthesisAnalysis()
    {
        var decision = GpuExecutionArbitrator.Resolve(new GpuExecutionArbitrationRequest
        {
            IntentId = "sense",
            ObjectiveId = "raw-analysis",
            ActionId = "aisthesis",
            PassId = GpuOperationNames.GpuAisthesisRawFrame,
            Backend = GpuBackend.WebGpu,
            RequireZeroCopyRawTexture = true,
            InputTargetKind = GpuFrameTargetKind.HudCompositeOffscreen,
            RequiredCapabilities = GpuProviderCapabilities.SupportsCompute |
                GpuProviderCapabilities.SupportsAisthesis |
                GpuProviderCapabilities.SupportsZeroCopyRawTexture,
            AvailableCapabilities = GpuProviderCapabilities.SupportsCompute |
                GpuProviderCapabilities.SupportsAisthesis |
                GpuProviderCapabilities.SupportsZeroCopyRawTexture
        });

        Assert.False(decision.UseGpu);
        Assert.Equal(GpuExecutionArbitrationReasons.RawFramebufferRequiredForZeroCopyAnalysis, decision.Reason);
        Assert.Equal(GpuOperationNames.GpuAisthesisRawFrame, decision.PassId);
        Assert.Contains("raw-target-required", decision.Tags);
        Assert.Contains(GpuOperationNames.GpuAisthesisRawFrame, decision.Tags);

        var metadata = decision.ToDiagnosticsMetadata();
        Assert.Equal(GpuFrameTargetKind.HudCompositeOffscreen.ToString(), metadata[GpuControlMetadataKeys.InputTargetKind]);
        Assert.Equal("true", metadata[GpuControlMetadataKeys.ZeroCopyRawTextureRequired]);
        Assert.True(GpuCanonicalValidation.ValidateControlArbitrationMetadata(metadata).IsValid);
    }

    [Fact]
    public void ResolveFallsBackWhenNativePassBridgeIsRequiredButUnavailable()
    {
        var decision = GpuExecutionArbitrator.Resolve(new GpuExecutionArbitrationRequest
        {
            IntentId = "render",
            ObjectiveId = "doom-hud",
            ActionId = "compose",
            PassId = GpuOperationNames.GpuHudComposite,
            Backend = GpuBackend.WebGpu,
            RequireNativePassBridge = true,
            NativePassBridgeAvailable = false,
            RequiredCapabilities = GpuProviderCapabilities.SupportsCompute |
                GpuProviderCapabilities.SupportsHudComposite,
            AvailableCapabilities = GpuProviderCapabilities.SupportsCompute |
                GpuProviderCapabilities.SupportsHudComposite
        });

        Assert.False(decision.UseGpu);
        Assert.Equal(GpuExecutionArbitrationReasons.NativePassBridgeUnavailable, decision.Reason);
        Assert.Equal(GpuOperationNames.GpuHudComposite, decision.PassId);
        Assert.Contains("pass-bridge-miss", decision.Tags);
        Assert.Contains(GpuOperationNames.GpuHudComposite, decision.Tags);

        var metadata = decision.ToDiagnosticsMetadata();
        Assert.Equal("true", metadata[GpuControlMetadataKeys.NativePassBridgeRequired]);
        Assert.Equal("false", metadata[GpuControlMetadataKeys.NativePassBridgeAvailable]);
        Assert.True(GpuCanonicalValidation.ValidateControlArbitrationMetadata(metadata).IsValid);
    }
}
