using AIKernel.Abstractions.Models;
using AIKernel.Abstractions.Providers;
using AIKernel.Dtos.Core;
using AIKernel.Dtos.Routing;

namespace AIKernel.Control.Core.Bonsai;

public sealed class BonsaiProviderCapabilities : IProviderCapabilities
{
    private static readonly string[] Operations =
    [
        "chat.local",
        "text.tokenize"
    ];

    private static readonly string[] DataTypes =
    [
        "text/plain",
        "tokens/int32",
        "logits/fp32"
    ];

    public IReadOnlyList<string> SupportedOperations => Operations;

    public IReadOnlyList<string> SupportedDataTypes => DataTypes;

    public int MaxConcurrentConnections => 1;

    public RateLimitInfo? RateLimit => null;

    public ModelCapacityVector Vector { get; } = new(
        structuralIntegrity: 0.92f,
        linguisticFluidity: 0.68f,
        reasoningDepth: 0.42f,
        fidelity: 0.88f,
        latencyPerformance: 0.95f);

    public bool SupportsQueryAugmentation => false;

    public bool SupportsQueryDecomposition => false;

    public bool SupportsQueryRouting => false;

    public int MaxQueryParts => 1;

    public IReadOnlyList<string> SupportedQueryProcessingOperations => [];

    public bool SupportsEmbedding => false;

    public int? EmbeddingDimensions => null;

    public IReadOnlyList<string> SupportedEmbeddingModels => [];

    public bool SupportsOperation(string operation)
        => Operations.Contains(operation, StringComparer.Ordinal);

    public bool SupportsDataType(string dataType)
        => DataTypes.Contains(dataType, StringComparer.Ordinal);

    public IDictionary<string, float>? GetDynamicCapacities(IExecutionConstraints constraints)
    {
        ArgumentNullException.ThrowIfNull(constraints);

        return new Dictionary<string, float>(StringComparer.Ordinal)
        {
            ["latency"] = constraints.ComputeDeviceType.Equals("CPU", StringComparison.OrdinalIgnoreCase) ? 0.95f : 0.7f,
            ["quantization.q1_0"] = 1.0f
        };
    }

    public ICapabilityProfile? GetCapabilityProfile()
        => null;

    public bool SupportsQuantization(string quantizationLevel)
        => quantizationLevel.Equals("Q1_0", StringComparison.OrdinalIgnoreCase)
            || quantizationLevel.Equals("1BIT", StringComparison.OrdinalIgnoreCase);

    public bool SupportsQueryProcessingOperation(string operation)
        => false;
}
