using AIKernel.Control.Core.Bonsai;
using System.Buffers.Binary;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace AIKernel.Control.CPU;

public sealed class Bonsai1BitCpuKernel : IBonsaiInferenceKernel
{
    public const int Q1BlockElements = 256;
    public const int Q1ScaleBytes = 2;
    public const int Q1PayloadBytes = Q1BlockElements / 8;
    public const int Q1BlockBytes = Q1ScaleBytes + Q1PayloadBytes;

    public string KernelId => Avx2.IsSupported
        ? "bonsai.q1_0.cpu.avx2"
        : "bonsai.q1_0.cpu.scalar";

    public int Q1BlockElementCount => Q1BlockElements;

    public int Q1BlockByteCount => Q1BlockBytes;

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
        if (Avx.IsSupported)
        {
            var positive = Vector256<float>.Zero;
            var negative = Vector256<float>.Zero;

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
                var signs = Vector256.Create(
                    (bits & 0x01) != 0 ? 1f : -1f,
                    (bits & 0x02) != 0 ? 1f : -1f,
                    (bits & 0x04) != 0 ? 1f : -1f,
                    (bits & 0x08) != 0 ? 1f : -1f,
                    (bits & 0x10) != 0 ? 1f : -1f,
                    (bits & 0x20) != 0 ? 1f : -1f,
                    (bits & 0x40) != 0 ? 1f : -1f,
                    (bits & 0x80) != 0 ? 1f : -1f);
                var signed = Avx.Multiply(values, signs);
                positive = Avx.Add(positive, Avx.Max(signed, Vector256<float>.Zero));
                negative = Avx.Add(negative, Avx.Min(signed, Vector256<float>.Zero));
            }

            return HorizontalAdd(Avx.Add(positive, negative));
        }

        var sum = 0f;
        for (var i = 0; i < packedSigns.Length; i++)
        {
            var bits = packedSigns[i];
            var offset = i * 8;
            sum += ((bits & 0x01) != 0) ? input[offset] : -input[offset];
            sum += ((bits & 0x02) != 0) ? input[offset + 1] : -input[offset + 1];
            sum += ((bits & 0x04) != 0) ? input[offset + 2] : -input[offset + 2];
            sum += ((bits & 0x08) != 0) ? input[offset + 3] : -input[offset + 3];
            sum += ((bits & 0x10) != 0) ? input[offset + 4] : -input[offset + 4];
            sum += ((bits & 0x20) != 0) ? input[offset + 5] : -input[offset + 5];
            sum += ((bits & 0x40) != 0) ? input[offset + 6] : -input[offset + 6];
            sum += ((bits & 0x80) != 0) ? input[offset + 7] : -input[offset + 7];
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

    private static float HorizontalAdd(Vector256<float> vector)
    {
        return vector.GetElement(0)
            + vector.GetElement(1)
            + vector.GetElement(2)
            + vector.GetElement(3)
            + vector.GetElement(4)
            + vector.GetElement(5)
            + vector.GetElement(6)
            + vector.GetElement(7);
    }

    private static int RoundUp(
        int value,
        int multiple)
        => ((value + multiple - 1) / multiple) * multiple;
}
