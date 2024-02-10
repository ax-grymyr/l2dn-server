namespace L2Dn.IO;

internal sealed class L2Ver120Stream: ProcessingStream
{
    private int _index = 0xE6;

    internal L2Ver120Stream(Stream stream): base(stream)
    {
    }

    protected override void ProcessInputData(Span<byte> output, ReadOnlySpan<byte> input) =>
        ProcessData(output, input);

    protected override void ProcessOutputData(Span<byte> output, ReadOnlySpan<byte> input) =>
        ProcessData(output, input);

    private void ProcessData(Span<byte> output, ReadOnlySpan<byte> input)
    {
        for (int i = 0; i < input.Length; i++)
            output[i] = (byte)(input[i] ^ GetXorKey(_index++));
    }

    private static int GetXorKey(int ix)
    {
        int d1 = ix & 0xf;
        int d2 = (ix >> 4) & 0xf;
        int d3 = (ix >> 8) & 0xf;
        int d4 = (ix >> 12) & 0xf;
        return ((d2 ^ d4) << 4) | (d1 ^ d3);
    }
}
