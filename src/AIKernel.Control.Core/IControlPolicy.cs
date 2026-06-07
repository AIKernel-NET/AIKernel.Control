namespace AIKernel.Control;

public interface IControlPolicy
{
    ValueTask<ControlPolicyEvaluation> EvaluateAsync(
        IExecutionGraph graph,
        ControlExecutionRequest request,
        CancellationToken cancellationToken = default);
}
