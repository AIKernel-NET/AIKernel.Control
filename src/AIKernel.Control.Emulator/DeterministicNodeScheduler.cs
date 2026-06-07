using AIKernel.Abstractions.Control;

namespace AIKernel.Control.Emulator;

public sealed class DeterministicNodeScheduler : INodeScheduler
{
    public ValueTask<IReadOnlyList<IExecutionNode>> ScheduleAsync(
        IExecutionGraph graph,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(graph);
        cancellationToken.ThrowIfCancellationRequested();

        var ordered = graph.Nodes
            .OrderBy(node => node.NodeId, StringComparer.Ordinal)
            .ThenBy(node => node.OperatorId, StringComparer.Ordinal)
            .ToArray();

        return ValueTask.FromResult<IReadOnlyList<IExecutionNode>>(ordered);
    }
}
