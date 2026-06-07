namespace AIKernel.Control;

public interface IExecutionGraph
{
    string GraphId { get; }

    IReadOnlyList<IExecutionNode> Nodes { get; }
}
