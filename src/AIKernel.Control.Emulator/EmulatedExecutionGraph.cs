using AIKernel.Abstractions.Control;

namespace AIKernel.Control.Emulator;

/// <include file="docs.en.xml" path="doc/members/member[@name='T:AIKernel.Control.Emulator.EmulatedExecutionGraph']" />
/// <include file="docs.ja.xml" path="doc/members/member[@name='T:AIKernel.Control.Emulator.EmulatedExecutionGraph']" />
public sealed record EmulatedExecutionGraph(
    string GraphId,
    IReadOnlyList<IExecutionNode> Nodes) : IExecutionGraph;
