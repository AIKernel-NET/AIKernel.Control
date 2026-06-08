using AIKernel.Abstractions.Models;
using AIKernel.Abstractions.Providers;
using AIKernel.Dtos.Core;
using AIKernel.Dtos.Routing;

namespace AIKernel.Control.Core.Bonsai;

/// <include file="docs.en.xml" path="doc/members/member[@name='T:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities']" />
/// <include file="docs.ja.xml" path="doc/members/member[@name='T:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities']" />
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

    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportedOperations']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportedOperations']" />
    public IReadOnlyList<string> SupportedOperations => Operations;

    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportedDataTypes']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportedDataTypes']" />
    public IReadOnlyList<string> SupportedDataTypes => DataTypes;

    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.MaxConcurrentConnections']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.MaxConcurrentConnections']" />
    public int MaxConcurrentConnections => 1;

    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.RateLimit']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.RateLimit']" />
    public RateLimitInfo? RateLimit => null;

    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.new']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.new']" />
    public ModelCapacityVector Vector { get; } = new(
        structuralIntegrity: 0.92f,
        linguisticFluidity: 0.68f,
        reasoningDepth: 0.42f,
        fidelity: 0.88f,
        latencyPerformance: 0.95f);

    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsQueryAugmentation']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsQueryAugmentation']" />
    public bool SupportsQueryAugmentation => false;

    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsQueryDecomposition']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsQueryDecomposition']" />
    public bool SupportsQueryDecomposition => false;

    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsQueryRouting']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsQueryRouting']" />
    public bool SupportsQueryRouting => false;

    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.MaxQueryParts']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.MaxQueryParts']" />
    public int MaxQueryParts => 1;

    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportedQueryProcessingOperations']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportedQueryProcessingOperations']" />
    public IReadOnlyList<string> SupportedQueryProcessingOperations => [];

    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsEmbedding']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsEmbedding']" />
    public bool SupportsEmbedding => false;

    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.EmbeddingDimensions']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.EmbeddingDimensions']" />
    public int? EmbeddingDimensions => null;

    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportedEmbeddingModels']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportedEmbeddingModels']" />
    public IReadOnlyList<string> SupportedEmbeddingModels => [];

    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsOperation']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsOperation']" />
    public bool SupportsOperation(string operation)
        => Operations.Contains(operation, StringComparer.Ordinal);

    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsDataType']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsDataType']" />
    public bool SupportsDataType(string dataType)
        => DataTypes.Contains(dataType, StringComparer.Ordinal);

    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.GetDynamicCapacities']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.GetDynamicCapacities']" />
    public IDictionary<string, float>? GetDynamicCapacities(IExecutionConstraints constraints)
    {
        ArgumentNullException.ThrowIfNull(constraints);

        return new Dictionary<string, float>(StringComparer.Ordinal)
        {
            ["latency"] = constraints.ComputeDeviceType.Equals("CPU", StringComparison.OrdinalIgnoreCase) ? 0.95f : 0.7f,
            ["quantization.q1_0"] = 1.0f
        };
    }

    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.GetCapabilityProfile']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.GetCapabilityProfile']" />
    public ICapabilityProfile? GetCapabilityProfile()
        => null;

    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsQuantization']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsQuantization']" />
    public bool SupportsQuantization(string quantizationLevel)
        => quantizationLevel.Equals("Q1_0", StringComparison.OrdinalIgnoreCase)
            || quantizationLevel.Equals("1BIT", StringComparison.OrdinalIgnoreCase);

    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsQueryProcessingOperation']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsQueryProcessingOperation']" />
    public bool SupportsQueryProcessingOperation(string operation)
        => false;
}
