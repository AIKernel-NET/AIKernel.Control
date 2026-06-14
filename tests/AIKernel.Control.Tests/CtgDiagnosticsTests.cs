using AIKernel.Control.Diagnostics;
using AIKernel.Core.Governance;
using AIKernel.Dtos.Governance;
using AIKernel.Enums.Governance;

namespace AIKernel.Control.Tests;

public sealed class CtgDiagnosticsTests
{
    [Fact]
    public void CtgReplayMetadataWriter_WriteStep_UsesUpperSnakeRejectReasonKind()
    {
        var writer = new CtgReplayMetadataWriter();
        var trace = new StepGovernanceTrace
        {
            TraceId = "trace.1",
            StepId = "step.1",
            DecisionGate = new DecisionGateResult
            {
                Accepted = false
            },
            RejectReasons =
            [
                new RejectReasonInfo
                {
                    Kind = RejectReasonKind.StepDenied,
                    ReasonCode = "STEP_DENIED",
                    Message = "A step was denied."
                }
            ]
        };

        var metadata = writer.WriteStep(trace);

        Assert.Equal("STEP_DENIED", metadata["ctg.reject_reason.0.kind"]);
        Assert.Equal("STEP_DENIED", metadata["ctg.reject_reason.0.code"]);
    }

    [Fact]
    public void CtgGateTelemetryValidator_CoreGateMetadata_IsDiscreteOnly()
    {
        var validator = new CtgGateTelemetryValidator();
        var result = new CtgDecisionGateEvaluator().Evaluate(
            new DecisionGateRequest
            {
                OperationId = "op",
                StepId = "step",
                GateInput = new GateInput
                {
                    Logos = CouncilVoteValue.Approve,
                    Ethos = CouncilVoteValue.Approve,
                    Pathos = CouncilVoteValue.Abstain
                }
            });

        Assert.True(validator.IsGateTelemetryDiscreteOnly(result));
        Assert.Empty(validator.FindContinuousCarrierKeys(result));
    }

    [Fact]
    public void CtgGateTelemetryValidator_ContinuousCarrierMetadata_FindsViolation()
    {
        var validator = new CtgGateTelemetryValidator();
        var result = new DecisionGateResult
        {
            Metadata = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["provider.confidence"] = "0.9"
            }
        };

        var keys = validator.FindContinuousCarrierKeys(result);

        Assert.False(validator.IsGateTelemetryDiscreteOnly(result));
        Assert.Equal(["provider.confidence"], keys);
    }

    [Fact]
    public async Task CtgGovernanceTraceEmitter_EmitStepAsync_WritesControlStateSnapshot()
    {
        var observer = new CtgControlStateObserver();
        var emitter = new CtgGovernanceTraceEmitter();
        var trace = new StepGovernanceTrace
        {
            TraceId = "trace.emit",
            StepId = "step.emit",
            DecisionGate = new DecisionGateResult
            {
                Accepted = true
            }
        };

        await emitter.EmitStepAsync(
            observer,
            "exec.emit",
            "graph.emit",
            trace,
            TestContext.Current.CancellationToken);

        var snapshot = Assert.Single(observer.Snapshots);
        Assert.Equal("exec.emit", snapshot.ExecutionId);
        Assert.Equal("step.emit", snapshot.NodeId);
        Assert.Equal("True", snapshot.Metadata["ctg.decision.accepted"]);
    }

    [Fact]
    public void CtgTraceRenderer_RenderStep_IncludesDecisionAndReason()
    {
        var renderer = new CtgTraceRenderer();
        var trace = new StepGovernanceTrace
        {
            StepId = "step.render",
            DecisionGate = new DecisionGateResult
            {
                Accepted = false
            },
            RejectReasons =
            [
                new RejectReasonInfo
                {
                    Kind = RejectReasonKind.FailClosed,
                    ReasonCode = "FAIL_CLOSED",
                    Message = "Fail closed."
                }
            ]
        };

        var text = renderer.RenderStep(trace);

        Assert.Contains("step=step.render", text, StringComparison.Ordinal);
        Assert.Contains("FAIL_CLOSED", text, StringComparison.Ordinal);
    }
}
