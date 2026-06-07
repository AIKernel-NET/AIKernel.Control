namespace AIKernel.Control;

public interface IExecutionNode
{
    string NodeId { get; }

    string OperatorId { get; }

    IReadOnlyDictionary<string, string> Metadata { get; }
}
