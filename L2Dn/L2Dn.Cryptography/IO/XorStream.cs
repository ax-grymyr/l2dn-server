namespace L2Dn.IO;

public class XorStream(Stream stream, byte xorKey): ProcessingStream(stream)
{
    protected override void ProcessInputData(Span<byte> output, ReadOnlySpan<byte> input) => Xor(output, input);
    protected override void ProcessOutputData(Span<byte> output, ReadOnlySpan<byte> input) => Xor(output, input);

    private void Xor(Span<byte> output, ReadOnlySpan<byte> input)
    {
        int key = xorKey;
        for (int i = 0; i < input.Length; i++)
            output[i] = (byte)(input[i] ^ key);
    }
}
