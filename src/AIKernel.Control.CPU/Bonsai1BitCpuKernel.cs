using AIKernel.Control.Core.Bonsai;
using System.Buffers.Binary;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace AIKernel.Control.CPU;

/// <summary>[EN] Documents this public package API member. [JA] Bonsai1BitCpuKernel を表します。</summary>
/// <include file="docs.en.xml" path="doc/members/member[@name='T:AIKernel.Control.CPU.Bonsai1BitCpuKernel']" />
/// <include file="docs.ja.xml" path="doc/members/member[@name='T:AIKernel.Control.CPU.Bonsai1BitCpuKernel']" />
public sealed class Bonsai1BitCpuKernel : IBonsaiInferenceKernel
{
    /// <summary>[EN] Documents this public package API member. [JA] Q1BlockElements 定数を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.CPU.Bonsai1BitCpuKernel.Q1BlockElements']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.CPU.Bonsai1BitCpuKernel.Q1BlockElements']" />
    public const int Q1BlockElements = 256;
    /// <summary>[EN] Documents this public package API member. [JA] Q1ScaleBytes 定数を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.CPU.Bonsai1BitCpuKernel.Q1ScaleBytes']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.CPU.Bonsai1BitCpuKernel.Q1ScaleBytes']" />
    public const int Q1ScaleBytes = 2;
    /// <summary>[EN] Documents this public package API member. [JA] Q1PayloadBytes 定数を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.CPU.Bonsai1BitCpuKernel.Q1PayloadBytes']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.CPU.Bonsai1BitCpuKernel.Q1PayloadBytes']" />
    public const int Q1PayloadBytes = Q1BlockElements / 8;
    /// <summary>[EN] Documents this public package API member. [JA] Q1BlockBytes 定数を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.CPU.Bonsai1BitCpuKernel.Q1BlockBytes']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.CPU.Bonsai1BitCpuKernel.Q1BlockBytes']" />
    public const int Q1BlockBytes = Q1ScaleBytes + Q1PayloadBytes;

    // Keep the published kernel identity and the instruction path on the same feature check.
    private static bool UseAvx2 => Avx2.IsSupported;

    /// <summary>[EN] Documents this public package API member. [JA] KernelId を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.CPU.Bonsai1BitCpuKernel.KernelId']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.CPU.Bonsai1BitCpuKernel.KernelId']" />
    public string KernelId => UseAvx2
        ? "bonsai.q1_0.cpu.avx2"
        : "bonsai.q1_0.cpu.scalar";

    /// <summary>[EN] Documents this public package API member. [JA] Q1BlockElementCount を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.CPU.Bonsai1BitCpuKernel.Q1BlockElementCount']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.CPU.Bonsai1BitCpuKernel.Q1BlockElementCount']" />
    public int Q1BlockElementCount => Q1BlockElements;

    /// <summary>[EN] Documents this public package API member. [JA] Q1BlockByteCount を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.CPU.Bonsai1BitCpuKernel.Q1BlockByteCount']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.CPU.Bonsai1BitCpuKernel.Q1BlockByteCount']" />
    public int Q1BlockByteCount => Q1BlockBytes;

    /// <summary>[EN] Documents this public package API member. [JA] Forward を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.CPU.Bonsai1BitCpuKernel.Forward']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.CPU.Bonsai1BitCpuKernel.Forward']" />
    public void Forward(
        BonsaiModelState state,
        ReadOnlySpan<int> inputTokenIds,
        Span<float> logits)
    {
        ArgumentNullException.ThrowIfNull(state);

        var hidden = Math.Max(Q1BlockElements, RoundUp(state.Config.HiddenSize, Q1BlockElements));
        var rowCount = Math.Min(logits.Length, state.Q1Weights.Length / Q1BlockBytes);

        PrepareActivation(inputTokenIds, state.ActivationBuffer.AsSpan(0, hidden));

        var rowBytes = checked((hidden / Q1BlockElements) * Q1BlockBytes);
        for (var row = 0; row < rowCount; row++)
        {
            logits[row] = DotRowQ1_0(
                state.Q1Weights.AsSpan(row * rowBytes, rowBytes),
                state.ActivationBuffer.AsSpan(0, hidden));
        }

        logits[rowCount..].Clear();
    }

    /// <summary>[EN] Documents this public package API member. [JA] DotRowQ1_0 を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.CPU.Bonsai1BitCpuKernel.DotRowQ1_0']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.CPU.Bonsai1BitCpuKernel.DotRowQ1_0']" />
    public static float DotRowQ1_0(
        ReadOnlySpan<byte> q1Blocks,
        ReadOnlySpan<float> input)
    {
        if (input.Length % Q1BlockElements != 0)
        {
            throw new ArgumentException("Input length must be a multiple of the Q1_0 block width.", nameof(input));
        }

        var requiredBytes = checked((input.Length / Q1BlockElements) * Q1BlockBytes);
        if (q1Blocks.Length < requiredBytes)
        {
            throw new ArgumentException("Q1_0 row is shorter than the input vector.", nameof(q1Blocks));
        }

        var sum = 0f;
        for (var block = 0; block < input.Length / Q1BlockElements; block++)
        {
            var blockOffset = block * Q1BlockBytes;
            var inputOffset = block * Q1BlockElements;
            var scale = (float)BitConverter.UInt16BitsToHalf(
                BinaryPrimitives.ReadUInt16LittleEndian(q1Blocks.Slice(blockOffset, sizeof(ushort))));

            sum += scale * DotBlockSigns(
                q1Blocks.Slice(blockOffset + Q1ScaleBytes, Q1PayloadBytes),
                input.Slice(inputOffset, Q1BlockElements));
        }

        return sum;
    }

    /// <summary>[EN] Documents this public package API member. [JA] DequantizeRowQ1_0 を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.CPU.Bonsai1BitCpuKernel.DequantizeRowQ1_0']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.CPU.Bonsai1BitCpuKernel.DequantizeRowQ1_0']" />
    public static void DequantizeRowQ1_0(
        ReadOnlySpan<byte> q1Blocks,
        Span<float> destination)
    {
        if (destination.Length % Q1BlockElements != 0)
        {
            throw new ArgumentException("Destination length must be a multiple of the Q1_0 block width.", nameof(destination));
        }

        var requiredBytes = checked((destination.Length / Q1BlockElements) * Q1BlockBytes);
        if (q1Blocks.Length < requiredBytes)
        {
            throw new ArgumentException("Q1_0 source is shorter than the destination row.", nameof(q1Blocks));
        }

        for (var block = 0; block < destination.Length / Q1BlockElements; block++)
        {
            var blockOffset = block * Q1BlockBytes;
            var outputOffset = block * Q1BlockElements;
            var scale = (float)BitConverter.UInt16BitsToHalf(
                BinaryPrimitives.ReadUInt16LittleEndian(q1Blocks.Slice(blockOffset, sizeof(ushort))));

            var packed = q1Blocks.Slice(blockOffset + Q1ScaleBytes, Q1PayloadBytes);
            var output = destination.Slice(outputOffset, Q1BlockElements);

            for (var i = 0; i < packed.Length; i++)
            {
                var bits = packed[i];
                var baseIndex = i * 8;
                output[baseIndex] = ((bits & 0x01) != 0) ? scale : -scale;
                output[baseIndex + 1] = ((bits & 0x02) != 0) ? scale : -scale;
                output[baseIndex + 2] = ((bits & 0x04) != 0) ? scale : -scale;
                output[baseIndex + 3] = ((bits & 0x08) != 0) ? scale : -scale;
                output[baseIndex + 4] = ((bits & 0x10) != 0) ? scale : -scale;
                output[baseIndex + 5] = ((bits & 0x20) != 0) ? scale : -scale;
                output[baseIndex + 6] = ((bits & 0x40) != 0) ? scale : -scale;
                output[baseIndex + 7] = ((bits & 0x80) != 0) ? scale : -scale;
            }
        }
    }

    private static float DotBlockSigns(
        ReadOnlySpan<byte> packedSigns,
        ReadOnlySpan<float> input)
    {
        Span<float> signedValues = stackalloc float[Q1BlockElements];
        if (UseAvx2)
        {
            for (var i = 0; i < packedSigns.Length; i++)
            {
                var bits = packedSigns[i];
                var offset = i * 8;
                var values = Vector256.Create(
                    input[offset],
                    input[offset + 1],
                    input[offset + 2],
                    input[offset + 3],
                    input[offset + 4],
                    input[offset + 5],
                    input[offset + 6],
                    input[offset + 7]);
                // Flip only the sign bit, just like scalar negation (including zero and NaN).
                var signMask = Vector256.Create(
                    (bits & 0x01) != 0 ? 0 : int.MinValue,
                    (bits & 0x02) != 0 ? 0 : int.MinValue,
                    (bits & 0x04) != 0 ? 0 : int.MinValue,
                    (bits & 0x08) != 0 ? 0 : int.MinValue,
                    (bits & 0x10) != 0 ? 0 : int.MinValue,
                    (bits & 0x20) != 0 ? 0 : int.MinValue,
                    (bits & 0x40) != 0 ? 0 : int.MinValue,
                    (bits & 0x80) != 0 ? 0 : int.MinValue).AsSingle();
                var signed = Avx.Xor(values, signMask);
                signed.CopyTo(signedValues.Slice(offset, 8));
            }
        }
        else
        {
            for (var i = 0; i < packedSigns.Length; i++)
            {
                var bits = packedSigns[i];
                var offset = i * 8;
                signedValues[offset] = ((bits & 0x01) != 0) ? input[offset] : -input[offset];
                signedValues[offset + 1] = ((bits & 0x02) != 0) ? input[offset + 1] : -input[offset + 1];
                signedValues[offset + 2] = ((bits & 0x04) != 0) ? input[offset + 2] : -input[offset + 2];
                signedValues[offset + 3] = ((bits & 0x08) != 0) ? input[offset + 3] : -input[offset + 3];
                signedValues[offset + 4] = ((bits & 0x10) != 0) ? input[offset + 4] : -input[offset + 4];
                signedValues[offset + 5] = ((bits & 0x20) != 0) ? input[offset + 5] : -input[offset + 5];
                signedValues[offset + 6] = ((bits & 0x40) != 0) ? input[offset + 6] : -input[offset + 6];
                signedValues[offset + 7] = ((bits & 0x80) != 0) ? input[offset + 7] : -input[offset + 7];
            }
        }

        // Share the exact accumulation code: lane reductions change rounding, and separate
        // addition expressions can propagate different NaN operands under different JIT tiers.
        var sum = 0f;
        foreach (var value in signedValues)
        {
            sum += value;
        }

        return sum;
    }

    private static void PrepareActivation(
        ReadOnlySpan<int> tokenIds,
        Span<float> activation)
    {
        activation.Clear();
        if (tokenIds.IsEmpty)
        {
            return;
        }

        var seed = unchecked((uint)tokenIds[^1] + 0x9E3779B9u);
        for (var i = 0; i < activation.Length; i++)
        {
            seed ^= seed << 13;
            seed ^= seed >> 17;
            seed ^= seed << 5;
            activation[i] = ((seed & 1u) == 0u) ? -1f : 1f;
        }
    }

    private static int RoundUp(
        int value,
        int multiple)
        => ((value + multiple - 1) / multiple) * multiple;
}
