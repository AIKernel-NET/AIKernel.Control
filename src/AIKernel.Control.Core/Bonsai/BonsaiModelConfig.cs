using System.Text.Json;

namespace AIKernel.Control.Core.Bonsai;

public sealed record BonsaiModelConfig(
    int LayerCount,
    int HiddenSize,
    int HeadCount,
    int VocabularySize,
    int ContextLength)
{
    public static BonsaiModelConfig Parse(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        return new BonsaiModelConfig(
            ReadRequiredInt32(root, "num_hidden_layers", "n_layer", "n_layers", "layer_count"),
            ReadRequiredInt32(root, "hidden_size", "n_embd", "embedding_length"),
            ReadRequiredInt32(root, "num_attention_heads", "n_head", "head_count"),
            ReadRequiredInt32(root, "vocab_size", "vocabulary_size", "n_vocab"),
            ReadRequiredInt32(root, "max_position_embeddings", "context_length", "n_ctx"));
    }

    public long EstimateActivationFloatCount()
    {
        checked
        {
            return (long)HiddenSize * Math.Max(1, ContextLength);
        }
    }

    private static int ReadRequiredInt32(
        JsonElement root,
        params string[] names)
    {
        foreach (var name in names)
        {
            if (root.TryGetProperty(name, out var value)
                && value.ValueKind == JsonValueKind.Number
                && value.TryGetInt32(out var number)
                && number > 0)
            {
                return number;
            }
        }

        throw new InvalidDataException(
            $"Bonsai config is missing a positive integer property. Expected one of: {string.Join(", ", names)}.");
    }
}
