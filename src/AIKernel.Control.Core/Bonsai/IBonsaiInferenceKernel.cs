namespace AIKernel.Control.Core.Bonsai;

public interface IBonsaiInferenceKernel
{
    string KernelId { get; }

    int Q1BlockElementCount { get; }

    int Q1BlockByteCount { get; }

    void Forward(
        BonsaiModelState state,
        ReadOnlySpan<int> inputTokenIds,
        Span<float> logits);
}
