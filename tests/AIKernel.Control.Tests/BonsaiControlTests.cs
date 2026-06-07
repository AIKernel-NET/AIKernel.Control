using AIKernel.Control.Core.Bonsai;
using AIKernel.Control.CPU;
using AIKernel.Control.Emulator;
using AIKernel.Dtos.Vfs;
using AIKernel.Enums;
using AIKernel.Vfs;
using System.Buffers.Binary;
using System.Text;

namespace AIKernel.Control.Tests;

public sealed class BonsaiControlTests
{
    [Fact]
    public void Q1KernelDequantizesPackedSigns()
    {
        Span<byte> row = stackalloc byte[Bonsai1BitCpuKernel.Q1BlockBytes];
        BinaryPrimitives.WriteUInt16LittleEndian(row[..2], BitConverter.HalfToUInt16Bits((Half)2.0f));
        row[2] = 0b1010_0101;

        Span<float> output = stackalloc float[Bonsai1BitCpuKernel.Q1BlockElements];

        Bonsai1BitCpuKernel.DequantizeRowQ1_0(row, output);

        Assert.Equal(2f, output[0]);
        Assert.Equal(-2f, output[1]);
        Assert.Equal(2f, output[2]);
        Assert.Equal(-2f, output[3]);
        Assert.Equal(-2f, output[4]);
        Assert.Equal(2f, output[5]);
        Assert.Equal(-2f, output[6]);
        Assert.Equal(2f, output[7]);
    }

    [Fact]
    public void Q1KernelComputesConditionalAddSubtractDotProduct()
    {
        Span<byte> row = stackalloc byte[Bonsai1BitCpuKernel.Q1BlockBytes];
        BinaryPrimitives.WriteUInt16LittleEndian(row[..2], BitConverter.HalfToUInt16Bits((Half)1.0f));
        row[2] = 0b0000_1111;
        Span<float> input = stackalloc float[Bonsai1BitCpuKernel.Q1BlockElements];
        input.Fill(1f);

        var dot = Bonsai1BitCpuKernel.DotRowQ1_0(row, input);

        Assert.Equal(-248f, dot);
    }

    [Fact]
    public async Task BuiltInProviderInitializesFromVfsAndExecutesNode()
    {
        var observer = new RecordingObserver();
        var provider = new BonsaiBuiltInProvider(
            new Bonsai1BitCpuKernel(),
            observer);
        var vfs = new BonsaiTestVfsProvider(
            new Dictionary<string, byte[]>(StringComparer.Ordinal)
            {
                ["/sys/roms/bonsai-1.7b/config.json"] = Encoding.UTF8.GetBytes(
                    """
                    {
                      "num_hidden_layers": 2,
                      "hidden_size": 256,
                      "num_attention_heads": 4,
                      "vocab_size": 8,
                      "context_length": 4
                    }
                    """),
                ["/sys/roms/bonsai-1.7b/tokenizer.json"] = Encoding.UTF8.GetBytes(
                    """
                    {
                      "model": {
                        "vocab": {
                          "<unk>": 0,
                          "hello": 1,
                          "world": 2
                        }
                      }
                    }
                    """)
            });

        await provider.InitializeAsync(vfs);
        var result = await provider.ExecuteNodeAsync(
            new EmulatedExecutionNode(
                "node.chat",
                "bonsai",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["operation"] = BonsaiBuiltInProvider.OperationChatLocal,
                    ["prompt"] = "hello"
                }),
            TestContext.Current.CancellationToken);

        Assert.True(provider.IsInitialized);
        Assert.Equal("Completed", result.Status);
        Assert.Equal("aikernel.control.bonsai-1.7b", result.Metadata["provider_id"]);
        Assert.Equal(BonsaiBuiltInProvider.OperationChatLocal, result.Metadata["operation"]);
        Assert.Contains(observer.Snapshots, snapshot => snapshot.Metadata["phase"] == "ModelDownload");
        Assert.Contains(observer.Snapshots, snapshot => snapshot.Metadata["phase"] == "Initializing");
        Assert.Contains(observer.Snapshots, snapshot => snapshot.Metadata["phase"] == "Generating");
    }

    private sealed class RecordingObserver : AIKernel.Abstractions.Control.IControlStateObserver
    {
        private readonly List<AIKernel.Dtos.Control.ControlStateSnapshot> _snapshots = [];

        public IReadOnlyList<AIKernel.Dtos.Control.ControlStateSnapshot> Snapshots => _snapshots;

        public ValueTask ObserveAsync(
            AIKernel.Dtos.Control.ControlStateSnapshot snapshot,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _snapshots.Add(snapshot);
            return ValueTask.CompletedTask;
        }
    }

    private sealed class BonsaiTestVfsProvider(
        IReadOnlyDictionary<string, byte[]> files) : IVfsProvider
    {
        public string ProviderId => "bonsai-test-vfs";

        public string Name => "Bonsai Test VFS";

        public Task<IVfsSession> OpenSessionAsync(IVfsCredentials credentials)
        {
            ArgumentNullException.ThrowIfNull(credentials);
            return Task.FromResult<IVfsSession>(new BonsaiTestVfsSession(files));
        }

        public Task<bool> IsAvailableAsync()
            => Task.FromResult(true);

        public Task<VfsProviderHealth> GetHealthAsync()
            => Task.FromResult(new VfsProviderHealth
            {
                IsHealthy = true,
                Message = "OK",
                CheckedAtUtc = DateTimeOffset.UnixEpoch
            });
    }

    private sealed class BonsaiTestVfsSession(
        IReadOnlyDictionary<string, byte[]> files) : IVfsSession
    {
        public string SessionId => "bonsai-test-session";

        public Task<IVfsFile> ReadFileAsync(string path)
        {
            if (!files.TryGetValue(path, out var content))
            {
                throw new FileNotFoundException(path, path);
            }

            return Task.FromResult<IVfsFile>(new BonsaiTestVfsFile(path, content));
        }

        public Task<IVfsDirectory> GetDirectoryAsync(string path)
            => throw new NotSupportedException();

        public Task WriteFileAsync(string path, byte[] content)
            => throw new NotSupportedException();

        public Task DeleteAsync(string path)
            => throw new NotSupportedException();

        public Task<IVfsQueryResult> QueryAsync(IVfsQuery query)
            => throw new NotSupportedException();

        public Task<bool> ExistsAsync(string path)
            => Task.FromResult(files.ContainsKey(path));

        public ValueTask DisposeAsync()
            => ValueTask.CompletedTask;
    }

    private sealed class BonsaiTestVfsFile(
        string path,
        byte[] content) : IVfsFile
    {
        public string Name => System.IO.Path.GetFileName(path);

        public string Path => path;

        public long Size => content.LongLength;

        public DateTime CreatedAt => DateTime.UnixEpoch;

        public DateTime ModifiedAt => DateTime.UnixEpoch;

        public IReadOnlyDictionary<string, string>? GetMetadata()
            => null;

        public Task<byte[]> ReadAsync()
            => Task.FromResult(content);

        public Task<string> ReadAsTextAsync()
            => Task.FromResult(Encoding.UTF8.GetString(content));
    }
}
