using AIKernel.Abstractions.Control;

namespace AIKernel.Control.Emulator;

/// <include file="docs.en.xml" path="doc/members/member[@name='T:AIKernel.Control.Emulator.DeterministicNodeScheduler']" />
/// <include file="docs.ja.xml" path="doc/members/member[@name='T:AIKernel.Control.Emulator.DeterministicNodeScheduler']" />
public sealed class DeterministicNodeScheduler : INodeScheduler
{
    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Emulator.DeterministicNodeScheduler.ScheduleAsync']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Emulator.DeterministicNodeScheduler.ScheduleAsync']" />
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
