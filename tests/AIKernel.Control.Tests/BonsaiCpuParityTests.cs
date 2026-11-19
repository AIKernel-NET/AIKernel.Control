using AIKernel.Control.CPU;
using System.Buffers.Binary;
using System.Runtime.Intrinsics.X86;

namespace AIKernel.Control.Tests;

public sealed class BonsaiCpuParityTests(ITestOutputHelper output)
{
    [Fact]
    public void KernelIdMatchesRuntimeInstructionSupport()
    {
        output.WriteLine($"AVX={Avx.IsSupported}; AVX2={Avx2.IsSupported}; KernelId={new Bonsai1BitCpuKernel().KernelId}");
        Assert.Equal(Avx2.IsSupported ? "bonsai.q1_0.cpu.avx2" : "bonsai.q1_0.cpu.scalar",
            new Bonsai1BitCpuKernel().KernelId);
        // The validation matrix sets this to verify that testhost inherited the ISA override.
        var expected = Environment.GetEnvironmentVariable("AIKERNEL_CT5_EXPECT_ISA");
        if (expected == "avx2") Assert.True(Avx2.IsSupported);
        if (expected == "avx-only") { Assert.True(Avx.IsSupported); Assert.False(Avx2.IsSupported); }
        if (expected == "scalar") { Assert.False(Avx.IsSupported); Assert.False(Avx2.IsSupported); }
    }

    [Fact]
    public void CancellationKeepsScalarAdditionOrder()
    {
        var row = Row(1, 0xff, 1);
        var input = new float[256];
        input[0] = 16777216f;
        input[8] = 1f;
        input[16] = -16777216f;
        input[24] = 1f;
        AssertBits(1f, Bonsai1BitCpuKernel.DotRowQ1_0(row, input));
    }

    [Fact]
    public void EveryPackedSignPatternMatchesScalarReference()
    {
        var input = Enumerable.Range(0, 256).Select(i => (i - 128) * 0.03125f).ToArray();
        for (var bits = 0; bits <= byte.MaxValue; bits++)
        {
            var row = Row(1, (byte)bits, 0.5f);
            AssertBits(Reference(row, input), Bonsai1BitCpuKernel.DotRowQ1_0(row, input));
        }
    }

    [Fact]
    public void SeededMultiBlockRowsMatchScalarReferenceBitForBit()
    {
        uint seed = 0x435435;
        uint Next() { seed ^= seed << 13; seed ^= seed >> 17; seed ^= seed << 5; return seed; }
        for (var sample = 0; sample < 512; sample++)
        {
            var blocks = 1 + sample % 8;
            var row = Row(blocks, 0, 1);
            var input = new float[blocks * 256];
            for (var block = 0; block < blocks; block++)
            {
                var scale = (Half)((int)(Next() % 33) - 16);
                BinaryPrimitives.WriteUInt16LittleEndian(row.AsSpan(block * 34), BitConverter.HalfToUInt16Bits(scale));
                for (var b = 2; b < 34; b++) row[block * 34 + b] = (byte)Next();
            }
            for (var i = 0; i < input.Length; i++)
            {
                var bits = (Next() & 0x807fffff) | ((107 + Next() % 41) << 23);
                input[i] = BitConverter.Int32BitsToSingle(unchecked((int)bits));
            }
            AssertBits(Reference(row, input), Bonsai1BitCpuKernel.DotRowQ1_0(row, input));
        }
    }

    [Theory]
    [InlineData(0x00000000)] // Positive zero
    [InlineData(unchecked((int)0x80000000))] // Negative zero
    [InlineData(0x00000001)] // Smallest subnormal
    [InlineData(0x007fffff)] // Largest subnormal
    [InlineData(0x7f7fffff)] // Largest finite value
    [InlineData(0x7f800000)] // Positive infinity
    [InlineData(unchecked((int)0xff800000))] // Negative infinity
    [InlineData(0x7fc01234)] // Quiet NaN with a payload
    [InlineData(unchecked((int)0xffc01234))]
    [InlineData(0x7f801234)] // Signaling NaN with a payload
    [InlineData(unchecked((int)0xff801234))]
    public void ExceptionalValuesMatchScalarReferenceBits(int valueBits)
    {
        foreach (var signs in new byte[] { 0, 0xff, 0x55, 0xaa })
        {
            var row = Row(1, signs, 1);
            var input = new float[256];
            input[0] = BitConverter.Int32BitsToSingle(valueBits);
            input[8] = input[0];
            input[16] = -input[0];
            AssertBits(Reference(row, input), Bonsai1BitCpuKernel.DotRowQ1_0(row, input));
        }
    }

    [Theory]
    [InlineData(0x0000)] // Zero
    [InlineData(0x8000)] // Negative zero
    [InlineData(0x0001)] // Smallest half subnormal
    [InlineData(0x7bff)] // Largest finite half
    [InlineData(0xc000)] // Negative scale
    [InlineData(0x7c00)] // Infinity
    [InlineData(0x7e12)] // NaN
    public void MultipleBlockScalesMatchScalarReferenceBits(int scaleBits)
    {
        var row = Row(3, 0x55, 1);
        BinaryPrimitives.WriteUInt16LittleEndian(row.AsSpan(34), (ushort)scaleBits);
        var input = Enumerable.Range(0, 768).Select(i => (i % 31 - 15) * 0.125f).ToArray();
        AssertBits(Reference(row, input), Bonsai1BitCpuKernel.DotRowQ1_0(row, input));
    }

    [Fact]
    public void EmptyRowHasPositiveZeroResult()
        => AssertBits(0f, Bonsai1BitCpuKernel.DotRowQ1_0([], []));

    private static byte[] Row(int blocks, byte signs, float scale)
    {
        var row = new byte[blocks * 34];
        for (var block = 0; block < blocks; block++)
        {
            BinaryPrimitives.WriteUInt16LittleEndian(row.AsSpan(block * 34), BitConverter.HalfToUInt16Bits((Half)scale));
            row.AsSpan(block * 34 + 2, 32).Fill(signs);
        }
        return row;
    }

    // Q1_0 reference: decode one sign at a time, fold in input order, then apply each block's scale.
    private static float Reference(byte[] row, float[] input)
    {
        var result = 0f;
        for (var block = 0; block < input.Length / 256; block++)
        {
            var total = 0f;
            for (var i = 0; i < 256; i++)
            {
                var positive = (row[block * 34 + 2 + i / 8] & (1 << (i % 8))) != 0;
                total += positive ? input[block * 256 + i] : -input[block * 256 + i];
            }
            var scale = (float)BitConverter.UInt16BitsToHalf(BinaryPrimitives.ReadUInt16LittleEndian(row.AsSpan(block * 34)));
            result += scale * total;
        }
        return result;
    }

    private static void AssertBits(float expected, float actual)
        => Assert.Equal(BitConverter.SingleToInt32Bits(expected), BitConverter.SingleToInt32Bits(actual));
}
