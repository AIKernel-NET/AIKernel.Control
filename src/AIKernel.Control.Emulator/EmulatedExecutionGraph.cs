using AIKernel.Abstractions.Control;

namespace AIKernel.Control.Emulator;

public sealed record EmulatedExecutionGraph(
    string GraphId,
    IReadOnlyList<IExecutionNode> Nodes) : IExecutionGraph;
