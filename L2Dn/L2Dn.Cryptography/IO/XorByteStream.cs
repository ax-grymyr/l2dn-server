namespace L2Dn.IO;

public sealed class XorByteStream: Stream
{
    private readonly Stream _baseStream;
    private readonly int _xorKey;

    public XorByteStream(Stream baseStream, byte xorKey)
    {
        _baseStream = baseStream;
        _xorKey = xorKey;
    }

    public override void Flush()
    {
        _baseStream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int result = _baseStream.Read(buffer, offset, count);
        Xor(buffer.AsSpan(offset, result), _xorKey);
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
        Xor(buffer.AsSpan(offset, count), _xorKey);
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

    private static void Xor(Span<byte> span, int key)
    {
        for (int i = 0; i < span.Length; i++)
            span[i] = (byte)(span[i] ^ key);
    }
}