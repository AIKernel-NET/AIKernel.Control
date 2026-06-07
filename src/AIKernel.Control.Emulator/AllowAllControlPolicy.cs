using AIKernel.Abstractions.Control;
using AIKernel.Dtos.Control;

namespace AIKernel.Control.Emulator;

public sealed class AllowAllControlPolicy : IControlPolicy
{
    public ValueTask<ControlPolicyEvaluation> EvaluateAsync(
        IExecutionGraph graph,
        ControlExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(graph);
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        return ValueTask.FromResult(new ControlPolicyEvaluation(
            true,
            "ALLOW",
            "ControlEmulator default policy."));
    }
}
