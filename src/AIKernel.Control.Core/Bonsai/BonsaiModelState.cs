namespace AIKernel.Control.Core.Bonsai;

public sealed class BonsaiModelState
{
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

    public BonsaiModelConfig Config { get; }

    public BonsaiTokenizer Tokenizer { get; }

    public byte[] Q1Weights { get; }

    public float[] ActivationBuffer { get; }

    public float[] LogitsBuffer { get; }

    public bool IsInitialized { get; private set; }

    public void MarkInitialized()
    {
        IsInitialized = true;
    }
}
