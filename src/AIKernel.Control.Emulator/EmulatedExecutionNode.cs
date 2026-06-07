using AIKernel.Abstractions.Control;

namespace AIKernel.Control.Emulator;

public sealed record EmulatedExecutionNode(
    string NodeId,
    string OperatorId,
    IReadOnlyDictionary<string, string> Metadata) : IExecutionNode;
