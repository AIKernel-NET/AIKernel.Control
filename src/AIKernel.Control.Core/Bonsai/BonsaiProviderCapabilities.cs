using AIKernel.Abstractions.Models;
using AIKernel.Abstractions.Providers;
using AIKernel.Dtos.Core;
using AIKernel.Dtos.Routing;

namespace AIKernel.Control.Core.Bonsai;

/// <summary>EN: Documentation for public API. JA: BonsaiProviderCapabilities を表します。</summary>
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

    /// <summary>EN: Documentation for public API. JA: SupportedOperations を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportedOperations']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportedOperations']" />
    public IReadOnlyList<string> SupportedOperations => Operations;

    /// <summary>EN: Documentation for public API. JA: SupportedDataTypes を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportedDataTypes']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportedDataTypes']" />
    public IReadOnlyList<string> SupportedDataTypes => DataTypes;

    /// <summary>EN: Documentation for public API. JA: MaxConcurrentConnections を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.MaxConcurrentConnections']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.MaxConcurrentConnections']" />
    public int MaxConcurrentConnections => 1;

    /// <summary>EN: Documentation for public API. JA: RateLimit を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.RateLimit']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.RateLimit']" />
    public RateLimitInfo? RateLimit => null;

    /// <summary>EN: Documentation for public API. JA: Vector を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.new']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.new']" />
    public ModelCapacityVector Vector { get; } = new(
        structuralIntegrity: 0.92f,
        linguisticFluidity: 0.68f,
        reasoningDepth: 0.42f,
        fidelity: 0.88f,
        latencyPerformance: 0.95f);

    /// <summary>EN: Documentation for public API. JA: SupportsQueryAugmentation を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsQueryAugmentation']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsQueryAugmentation']" />
    public bool SupportsQueryAugmentation => false;

    /// <summary>EN: Documentation for public API. JA: SupportsQueryDecomposition を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsQueryDecomposition']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsQueryDecomposition']" />
    public bool SupportsQueryDecomposition => false;

    /// <summary>EN: Documentation for public API. JA: SupportsQueryRouting を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsQueryRouting']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsQueryRouting']" />
    public bool SupportsQueryRouting => false;

    /// <summary>EN: Documentation for public API. JA: MaxQueryParts を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.MaxQueryParts']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.MaxQueryParts']" />
    public int MaxQueryParts => 1;

    /// <summary>EN: Documentation for public API. JA: SupportedQueryProcessingOperations を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportedQueryProcessingOperations']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportedQueryProcessingOperations']" />
    public IReadOnlyList<string> SupportedQueryProcessingOperations => [];

    /// <summary>EN: Documentation for public API. JA: SupportsEmbedding を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsEmbedding']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsEmbedding']" />
    public bool SupportsEmbedding => false;

    /// <summary>EN: Documentation for public API. JA: EmbeddingDimensions を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.EmbeddingDimensions']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.EmbeddingDimensions']" />
    public int? EmbeddingDimensions => null;

    /// <summary>EN: Documentation for public API. JA: SupportedEmbeddingModels を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportedEmbeddingModels']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportedEmbeddingModels']" />
    public IReadOnlyList<string> SupportedEmbeddingModels => [];

    /// <summary>EN: Documentation for public API. JA: SupportsOperation を実行します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsOperation']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsOperation']" />
    public bool SupportsOperation(string operation)
        => Operations.Contains(operation, StringComparer.Ordinal);

    /// <summary>EN: Documentation for public API. JA: SupportsDataType を実行します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsDataType']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsDataType']" />
    public bool SupportsDataType(string dataType)
        => DataTypes.Contains(dataType, StringComparer.Ordinal);

    /// <summary>EN: Documentation for public API. JA: GetDynamicCapacities を実行します。</summary>
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

    /// <summary>EN: Documentation for public API. JA: GetCapabilityProfile を実行します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.GetCapabilityProfile']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.GetCapabilityProfile']" />
    public ICapabilityProfile? GetCapabilityProfile()
        => null;

    /// <summary>EN: Documentation for public API. JA: SupportsQuantization を実行します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsQuantization']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsQuantization']" />
    public bool SupportsQuantization(string quantizationLevel)
        => quantizationLevel.Equals("Q1_0", StringComparison.OrdinalIgnoreCase)
            || quantizationLevel.Equals("1BIT", StringComparison.OrdinalIgnoreCase);

    /// <summary>EN: Documentation for public API. JA: SupportsQueryProcessingOperation を実行します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsQueryProcessingOperation']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiProviderCapabilities.SupportsQueryProcessingOperation']" />
    public bool SupportsQueryProcessingOperation(string operation)
        => false;
}
