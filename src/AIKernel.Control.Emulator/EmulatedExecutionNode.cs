using AIKernel.Abstractions.Control;

namespace AIKernel.Control.Emulator;

public sealed record EmulatedExecutionNode(
    string NodeId,
    string OperatorId,
    IReadOnlyDictionary<string, string> Metadata) : IExecutionNode
{
    public EmulatedExecutionNode(
        string nodeId,
        string operatorId,
        IReadOnlyDictionary<string, string> metadata,
        string faultMessage)
        : this(nodeId, operatorId, metadata)
    {
        FaultMessage = faultMessage;
    }

    public string? FaultMessage { get; init; }
}
