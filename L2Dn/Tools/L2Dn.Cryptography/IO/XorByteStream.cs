namespace L2Dn.IO;

public sealed class XorByteStream: Stream
{
    private readonly Stream _baseStream;
    private readonly long _offset;
    private readonly int _xorKey;

    public XorByteStream(Stream baseStream, byte xorKey)
    {
        _baseStream = baseStream;
        _xorKey = xorKey;
        if (baseStream.CanSeek)
            _offset = baseStream.Position;
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
        if (origin == SeekOrigin.Current)
            return _baseStream.Seek(offset, origin) - _offset;
        
        return _baseStream.Seek(offset + _offset, origin) - _offset;
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
    public override bool CanSeek => _baseStream.CanSeek;
    public override bool CanWrite => _baseStream.CanWrite;
    public override long Length => _baseStream.Length - _offset;

    public override long Position
    {
        get => _baseStream.Position - _offset;
        set => throw new NotSupportedException();
    }

    private static void Xor(Span<byte> span, int key)
    {
        for (int i = 0; i < span.Length; i++)
            span[i] = (byte)(span[i] ^ key);
    }
}