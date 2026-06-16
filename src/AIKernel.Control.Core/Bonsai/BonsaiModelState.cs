namespace AIKernel.Control.Core.Bonsai;

/// <summary>[EN] Documents this public package API member. [JA] BonsaiModelState を表します。</summary>
/// <include file="docs.en.xml" path="doc/members/member[@name='T:AIKernel.Control.Core.Bonsai.BonsaiModelState']" />
/// <include file="docs.ja.xml" path="doc/members/member[@name='T:AIKernel.Control.Core.Bonsai.BonsaiModelState']" />
public sealed class BonsaiModelState
{
    /// <summary>[EN] Documents this public package API member. [JA] BonsaiModelState を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiModelState.#ctor']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiModelState.#ctor']" />
    public BonsaiModelState(
        BonsaiModelConfig config,
        BonsaiTokenizer tokenizer,
        byte[] q1Weights,
        float[] activationBuffer,
        float[] logitsBuffer)
    {
        Config = config ?? throw new ArgumentNullException(nameof(config));
        Tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
        Q1Weights = q1Weights ?? throw new ArgumentNullException(nameof(q1Weights));
        ActivationBuffer = activationBuffer ?? throw new ArgumentNullException(nameof(activationBuffer));
        LogitsBuffer = logitsBuffer ?? throw new ArgumentNullException(nameof(logitsBuffer));
    }

    /// <summary>[EN] Documents this public package API member. [JA] Config を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='P:AIKernel.Control.Core.Bonsai.BonsaiModelState.Config']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='P:AIKernel.Control.Core.Bonsai.BonsaiModelState.Config']" />
    public BonsaiModelConfig Config { get; }

    /// <summary>[EN] Documents this public package API member. [JA] Tokenizer を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='P:AIKernel.Control.Core.Bonsai.BonsaiModelState.Tokenizer']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='P:AIKernel.Control.Core.Bonsai.BonsaiModelState.Tokenizer']" />
    public BonsaiTokenizer Tokenizer { get; }

    /// <summary>[EN] Documents this public package API member. [JA] Q1Weights を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='P:AIKernel.Control.Core.Bonsai.BonsaiModelState.Q1Weights']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='P:AIKernel.Control.Core.Bonsai.BonsaiModelState.Q1Weights']" />
    public byte[] Q1Weights { get; }

    /// <summary>[EN] Documents this public package API member. [JA] ActivationBuffer を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='P:AIKernel.Control.Core.Bonsai.BonsaiModelState.ActivationBuffer']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='P:AIKernel.Control.Core.Bonsai.BonsaiModelState.ActivationBuffer']" />
    public float[] ActivationBuffer { get; }

    /// <summary>[EN] Documents this public package API member. [JA] LogitsBuffer を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='P:AIKernel.Control.Core.Bonsai.BonsaiModelState.LogitsBuffer']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='P:AIKernel.Control.Core.Bonsai.BonsaiModelState.LogitsBuffer']" />
    public float[] LogitsBuffer { get; }

    /// <summary>[EN] Documents this public package API member. [JA] IsInitialized を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='P:AIKernel.Control.Core.Bonsai.BonsaiModelState.IsInitialized']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='P:AIKernel.Control.Core.Bonsai.BonsaiModelState.IsInitialized']" />
    public bool IsInitialized { get; private set; }

    /// <summary>[EN] Documents this public package API member. [JA] MarkInitialized を実行します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiModelState.MarkInitialized']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiModelState.MarkInitialized']" />
    public void MarkInitialized()
    {
        IsInitialized = true;
    }
}
