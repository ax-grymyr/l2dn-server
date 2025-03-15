namespace L2Dn.IO;

public class XorArrayStream: Stream
{
    private readonly Stream _baseStream;
    private readonly ReadOnlyMemory<byte> _xorKey;
    private int _keyPos;

    public XorArrayStream(Stream baseStream, ReadOnlyMemory<byte> xorKey, int startKeyPos = 0)
    {
        if (xorKey.Length == 0)
            throw new ArgumentException("Key cannot be empty", nameof(xorKey));

        if (startKeyPos >= xorKey.Length || startKeyPos < 0)
            throw new ArgumentOutOfRangeException(nameof(startKeyPos));

        _baseStream = baseStream;
        _xorKey = xorKey;
        _keyPos = startKeyPos;
    }

    public override void Flush()
    {
        _baseStream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int result = _baseStream.Read(buffer, offset, count);
        Xor(buffer.AsSpan(offset, result));
        return result;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        Xor(buffer.AsSpan(offset, count));
        _baseStream.Write(buffer, offset, count);
    }

    public override bool CanRead => _baseStream.CanRead;
    public override bool CanSeek => false;
    public override bool CanWrite => _baseStream.CanWrite;
    public override long Length => _baseStream.Length;

    public override long Position
    {
        get => _baseStream.Position;
        set => throw new NotSupportedException();
    }

    private void Xor(Span<byte> span)
    {
        int pos = _keyPos;
        ReadOnlySpan<byte> key = _xorKey.Span;
        for (int i = 0; i < span.Length; i++)
        {
            span[i] = (byte)(span[i] ^ key[pos]);
            pos++;
            if (pos == key.Length)
                pos = 0;
        }

        _keyPos = pos;
    }
}