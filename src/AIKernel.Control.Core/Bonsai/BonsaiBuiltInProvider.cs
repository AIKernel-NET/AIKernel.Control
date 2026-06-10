using AIKernel.Abstractions.Control;
using AIKernel.Abstractions.Providers;
using AIKernel.Dtos.Control;
using AIKernel.Dtos.Core;
using AIKernel.Vfs;
using System.Buffers.Binary;
using System.Globalization;
using System.Text;

namespace AIKernel.Control.Core.Bonsai;

/// <include file="docs.en.xml" path="doc/members/member[@name='T:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider']" />
/// <include file="docs.ja.xml" path="doc/members/member[@name='T:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider']" />
public sealed class BonsaiBuiltInProvider : IProvider
{
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.DefaultModelRoot']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.DefaultModelRoot']" />
    public const string DefaultModelRoot = "/sys/roms/bonsai-1.7b";
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.OperationChatLocal']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.OperationChatLocal']" />
    public const string OperationChatLocal = "chat.local";
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.OperationTokenize']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.OperationTokenize']" />
    public const string OperationTokenize = "text.tokenize";

    private readonly IBonsaiInferenceKernel _kernel;
    private readonly IControlStateObserver? _observer;
    private readonly IProviderCapabilities _capabilities = new BonsaiProviderCapabilities();
    private BonsaiModelState? _state;

    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.#ctor']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.#ctor']" />
    public BonsaiBuiltInProvider(
        IBonsaiInferenceKernel kernel,
        IControlStateObserver? observer = null,
        string modelRoot = DefaultModelRoot)
    {
        _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        _observer = observer;
        ModelRoot = string.IsNullOrWhiteSpace(modelRoot)
            ? DefaultModelRoot
            : modelRoot.TrimEnd('/');
    }

    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.ProviderId']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.ProviderId']" />
    public string ProviderId => "aikernel.control.bonsai-1.7b";

    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.Name']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.Name']" />
    public string Name => "Bonsai-1.7B Built-in Provider";

    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.Version']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.Version']" />
    public string Version => "0.1.1";

    /// <include file="docs.en.xml" path="doc/members/member[@name='P:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.ModelRoot']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='P:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.ModelRoot']" />
    public string ModelRoot { get; }

    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.IsInitialized']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.IsInitialized']" />
    public bool IsInitialized => _state?.IsInitialized == true;

    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.InitializeAsync']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.InitializeAsync']" />
    public async Task InitializeAsync(IVfsProvider vfs)
    {
        ArgumentNullException.ThrowIfNull(vfs);

        await ObserveAsync("bonsai.init", "ModelDownload", "Loading Bonsai ROM assets from VFS.").ConfigureAwait(false);

        await using var session = await vfs
            .OpenSessionAsync(new BonsaiVfsCredentials())
            .ConfigureAwait(false);

        var configJson = await ReadTextAsync(session, $"{ModelRoot}/config.json").ConfigureAwait(false);
        var tokenizerJson = await ReadTextAsync(session, $"{ModelRoot}/tokenizer.json").ConfigureAwait(false);

        await ObserveAsync("bonsai.init", "Initializing", "Parsing topology and binding deterministic buffers.").ConfigureAwait(false);

        var config = BonsaiModelConfig.Parse(configJson);
        var tokenizer = BonsaiTokenizer.Parse(tokenizerJson);
        var weightBytes = await TryReadBytesAsync(session, $"{ModelRoot}/model.q1_0.bin").ConfigureAwait(false)
            ?? CreateDeterministicWeights(config, _kernel.Q1BlockElementCount, _kernel.Q1BlockByteCount);

        var activationBuffer = new float[checked((int)Math.Min(int.MaxValue, config.EstimateActivationFloatCount()))];
        var logitsBuffer = new float[Math.Max(1, Math.Min(config.VocabularySize, tokenizer.VocabularySize))];

        _state = new BonsaiModelState(
            config,
            tokenizer,
            weightBytes,
            activationBuffer,
            logitsBuffer);
        _state.MarkInitialized();

        await ObserveAsync("bonsai.init", "Ready", "Bonsai model buffers are bound.").ConfigureAwait(false);
    }

    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.InitializeAsync']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.InitializeAsync']" />
    public Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return Task.CompletedTask;
        }

        throw new InvalidOperationException(
            "BonsaiBuiltInProvider requires InitializeAsync(IVfsProvider) so model ROM assets are loaded through VFS.");
    }

    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.ShutdownAsync']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.ShutdownAsync']" />
    public Task ShutdownAsync()
    {
        _state = null;
        return Task.CompletedTask;
    }

    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.IsAvailableAsync']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.IsAvailableAsync']" />
    public Task<bool> IsAvailableAsync()
        => Task.FromResult(IsInitialized);

    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.GetHealthAsync']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.GetHealthAsync']" />
    public Task<ProviderHealthStatus> GetHealthAsync()
        => Task.FromResult(new ProviderHealthStatus(
            IsInitialized,
            IsInitialized ? null : "Bonsai model is not initialized.",
            DateTime.UnixEpoch,
            0));

    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.GetCapabilities']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.GetCapabilities']" />
    public IProviderCapabilities GetCapabilities()
        => _capabilities;

    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.ExecuteNodeAsync']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.ExecuteNodeAsync']" />
    public async ValueTask<ControlExecutionResult> ExecuteNodeAsync(
        IExecutionNode node,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(node);
        cancellationToken.ThrowIfCancellationRequested();

        var state = _state
            ?? throw new InvalidOperationException("Bonsai model is not initialized.");

        await ObserveAsync(node.NodeId, "Generating", "Executing Bonsai node.").ConfigureAwait(false);

        var operation = GetMetadata(node, "operation", OperationChatLocal);
        if (operation.Equals(OperationTokenize, StringComparison.Ordinal))
        {
            var text = GetMetadata(node, "text", string.Empty);
            var token = state.Tokenizer.TokenizeFirst(text.AsSpan());

            return new ControlExecutionResult(
                node.NodeId,
                "Completed",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["provider_id"] = ProviderId,
                    ["operation"] = operation,
                    ["token_id"] = token.ToString(CultureInfo.InvariantCulture)
                });
        }

        if (!operation.Equals(OperationChatLocal, StringComparison.Ordinal))
        {
            return new ControlExecutionResult(
                node.NodeId,
                "Unsupported",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["provider_id"] = ProviderId,
                    ["operation"] = operation
                });
        }

        Span<int> tokenIds = stackalloc int[1];
        tokenIds[0] = state.Tokenizer.TokenizeFirst(GetMetadata(node, "prompt", string.Empty).AsSpan());

        _kernel.Forward(state, tokenIds, state.LogitsBuffer);
        var tokenId = ArgMax(state.LogitsBuffer.AsSpan());

        return new ControlExecutionResult(
            node.NodeId,
            "Completed",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["provider_id"] = ProviderId,
                ["operation"] = operation,
                ["kernel_id"] = _kernel.KernelId,
                ["token_id"] = tokenId.ToString(CultureInfo.InvariantCulture),
                ["text"] = state.Tokenizer.Decode(tokenId)
            });
    }

    private async ValueTask ObserveAsync(
        string nodeId,
        string phase,
        string message)
    {
        if (_observer is null)
        {
            return;
        }

        await _observer.ObserveAsync(
            new ControlStateSnapshot(
                "bonsai-1.7b",
                "bonsai.builtin",
                nodeId,
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["phase"] = phase,
                    ["message"] = message,
                    ["provider_id"] = ProviderId
                })).ConfigureAwait(false);
    }

    private static async Task<string> ReadTextAsync(
        IVfsSession session,
        string path)
    {
        var file = await session.ReadFileAsync(path).ConfigureAwait(false);
        return await file.ReadAsTextAsync().ConfigureAwait(false);
    }

    private static async Task<byte[]?> TryReadBytesAsync(
        IVfsSession session,
        string path)
    {
        try
        {
            var file = await session.ReadFileAsync(path).ConfigureAwait(false);
            return await file.ReadAsync().ConfigureAwait(false);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    private static byte[] CreateDeterministicWeights(
        BonsaiModelConfig config,
        int blockElementCount,
        int blockByteCount)
    {
        var rowCount = Math.Max(1, Math.Min(config.VocabularySize, 4096));
        var columnCount = Math.Max(blockElementCount, RoundUp(config.HiddenSize, blockElementCount));
        var blocksPerRow = columnCount / blockElementCount;
        var weights = new byte[checked(rowCount * blocksPerRow * blockByteCount)];

        for (var row = 0; row < rowCount; row++)
        {
            for (var block = 0; block < blocksPerRow; block++)
            {
                var offset = ((row * blocksPerRow) + block) * blockByteCount;
                BinaryPrimitives.WriteUInt16LittleEndian(weights.AsSpan(offset, sizeof(ushort)), HalfToUInt16((Half)1.0f));

                for (var i = sizeof(ushort); i < blockByteCount; i++)
                {
                    weights[offset + i] = unchecked((byte)((row * 131 + block * 17 + i * 29) & 0xFF));
                }
            }
        }

        return weights;
    }

    private static int ArgMax(ReadOnlySpan<float> values)
    {
        var index = 0;
        var best = values.IsEmpty ? 0f : values[0];
        for (var i = 1; i < values.Length; i++)
        {
            if (values[i] > best)
            {
                best = values[i];
                index = i;
            }
        }

        return index;
    }

    private static string GetMetadata(
        IExecutionNode node,
        string key,
        string fallback)
        => node.Metadata.TryGetValue(key, out var value)
            ? value
            : fallback;

    private static int RoundUp(
        int value,
        int multiple)
        => ((value + multiple - 1) / multiple) * multiple;

    private static ushort HalfToUInt16(Half value)
        => BitConverter.HalfToUInt16Bits(value);

    private sealed class BonsaiVfsCredentials : IVfsCredentials
    {
        /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.Username']" />
        /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.Username']" />
        public string? Username => null;

        /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.ApiKey']" />
        /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.ApiKey']" />
        public string? ApiKey => null;

        /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.Token']" />
        /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.Token']" />
        public string? Token => null;

        /// <include file="docs.en.xml" path="doc/members/member[@name='P:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.object']" />
        /// <include file="docs.ja.xml" path="doc/members/member[@name='P:AIKernel.Control.Core.Bonsai.BonsaiBuiltInProvider.object']" />
        public IReadOnlyDictionary<string, object>? Parameters { get; } =
            new Dictionary<string, object>(StringComparer.Ordinal)
            {
                ["purpose"] = "bonsai-rom-read"
            };
    }
}
