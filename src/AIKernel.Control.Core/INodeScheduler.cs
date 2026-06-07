namespace AIKernel.Control;

public interface INodeScheduler
{
    ValueTask<IReadOnlyList<IExecutionNode>> ScheduleAsync(
        IExecutionGraph graph,
        CancellationToken cancellationToken = default);
}
