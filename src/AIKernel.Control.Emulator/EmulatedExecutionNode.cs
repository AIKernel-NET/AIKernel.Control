using AIKernel.Abstractions.Control;

namespace AIKernel.Control.Emulator;

/// <summary>[EN] Documents this public package API member. [JA] EmulatedExecutionNode を表します。</summary>
/// <include file="docs.en.xml" path="doc/members/member[@name='T:AIKernel.Control.Emulator.EmulatedExecutionNode']" />
/// <include file="docs.ja.xml" path="doc/members/member[@name='T:AIKernel.Control.Emulator.EmulatedExecutionNode']" />
public sealed record EmulatedExecutionNode(
    string NodeId,
    string OperatorId,
    IReadOnlyDictionary<string, string> Metadata) : IExecutionNode
{
    /// <summary>[EN] Documents this public package API member. [JA] EmulatedExecutionNode を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Emulator.EmulatedExecutionNode.#ctor']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Emulator.EmulatedExecutionNode.#ctor']" />
    public EmulatedExecutionNode(
        string nodeId,
        string operatorId,
        IReadOnlyDictionary<string, string> metadata,
        string faultMessage)
        : this(nodeId, operatorId, metadata)
    {
        FaultMessage = faultMessage;
    }

    /// <summary>[EN] Documents this public package API member. [JA] FaultMessage を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='P:AIKernel.Control.Emulator.EmulatedExecutionNode.FaultMessage']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='P:AIKernel.Control.Emulator.EmulatedExecutionNode.FaultMessage']" />
    public string? FaultMessage { get; init; }
}
