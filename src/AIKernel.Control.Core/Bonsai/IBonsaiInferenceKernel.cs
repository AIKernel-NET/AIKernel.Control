namespace AIKernel.Control.Core.Bonsai;

/// <summary>[EN] Documents this public package API member. [JA] IBonsaiInferenceKernel contract を定義します。</summary>
/// <include file="docs.en.xml" path="doc/members/member[@name='T:AIKernel.Control.Core.Bonsai.IBonsaiInferenceKernel']" />
/// <include file="docs.ja.xml" path="doc/members/member[@name='T:AIKernel.Control.Core.Bonsai.IBonsaiInferenceKernel']" />
public interface IBonsaiInferenceKernel
{
    /// <include file="docs.en.xml" path="doc/members/member[@name='P:AIKernel.Control.Core.Bonsai.IBonsaiInferenceKernel.KernelId']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='P:AIKernel.Control.Core.Bonsai.IBonsaiInferenceKernel.KernelId']" />
    string KernelId { get; }

    /// <include file="docs.en.xml" path="doc/members/member[@name='P:AIKernel.Control.Core.Bonsai.IBonsaiInferenceKernel.Q1BlockElementCount']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='P:AIKernel.Control.Core.Bonsai.IBonsaiInferenceKernel.Q1BlockElementCount']" />
    int Q1BlockElementCount { get; }

    /// <include file="docs.en.xml" path="doc/members/member[@name='P:AIKernel.Control.Core.Bonsai.IBonsaiInferenceKernel.Q1BlockByteCount']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='P:AIKernel.Control.Core.Bonsai.IBonsaiInferenceKernel.Q1BlockByteCount']" />
    int Q1BlockByteCount { get; }

    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.IBonsaiInferenceKernel.Forward']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.IBonsaiInferenceKernel.Forward']" />
    void Forward(
        BonsaiModelState state,
        ReadOnlySpan<int> inputTokenIds,
        Span<float> logits);
}
