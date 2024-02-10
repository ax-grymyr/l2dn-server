namespace L2Dn.IO;

public class XorArrayStream: ProcessingStream
{
    private readonly ReadOnlyMemory<byte> _xorKey;
    private int _keyPos;

    protected XorArrayStream(Stream stream, ReadOnlyMemory<byte> xorKey, int startKeyPos = 0): base(stream)
    {
        if (xorKey.Length == 0)
            throw new ArgumentException("Key cannot be empty", nameof(xorKey));

        if (startKeyPos >= xorKey.Length || startKeyPos < 0)
            throw new ArgumentOutOfRangeException(nameof(startKeyPos));

        _xorKey = xorKey;
        _keyPos = startKeyPos;
    }

    protected override void ProcessInputData(Span<byte> output, ReadOnlySpan<byte> input) => Xor(output, input);
    protected override void ProcessOutputData(Span<byte> output, ReadOnlySpan<byte> input) => Xor(output, input);

    private void Xor(Span<byte> output, ReadOnlySpan<byte> input)
    {
        int pos = _keyPos;
        ReadOnlySpan<byte> key = _xorKey.Span;
        try
        {
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = (byte)(input[i] ^ key[pos]);
                pos++;
                if (pos == key.Length)
                    pos = 0;
            }
        }
        finally
        {
            _keyPos = pos;
        }
    }
}
