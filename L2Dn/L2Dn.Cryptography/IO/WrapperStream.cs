namespace L2Dn.IO;

public abstract class WrapperStream: Stream
{
    private readonly Stream _baseStream;
    private readonly bool _seekable;

    internal WrapperStream(Stream stream, bool seekable = false)
    {
        _baseStream = stream;
        _seekable = seekable;
    }

    public Stream BaseStream => _baseStream;

    public override void Close() => _baseStream.Close();

    protected override void Dispose(bool disposing) => _baseStream.Dispose();

    public override ValueTask DisposeAsync() => _baseStream.DisposeAsync();

    public override void Flush() => _baseStream.Flush();

    public override Task FlushAsync(CancellationToken cancellationToken) =>
        _baseStream.FlushAsync(cancellationToken);

    public override int Read(Span<byte> buffer) => _baseStream.Read(buffer);
    public override int Read(byte[] buffer, int offset, int count) => _baseStream.Read(buffer, offset, count);
    public override int ReadByte() => _baseStream.ReadByte();

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
        _baseStream.ReadAsync(buffer, cancellationToken);

    public override Task<int>
        ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        _baseStream.ReadAsync(buffer, offset, count, cancellationToken);

    public override void Write(ReadOnlySpan<byte> buffer) => _baseStream.Write(buffer);
    public override void Write(byte[] buffer, int offset, int count) => _baseStream.Write(buffer, offset, count);
    public override void WriteByte(byte value) => _baseStream.WriteByte(value);

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        _baseStream.WriteAsync(buffer, offset, count, cancellationToken);

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer,
        CancellationToken cancellationToken = default) => _baseStream.WriteAsync(buffer, cancellationToken);

    public override long Seek(long offset, SeekOrigin origin)
    {
        CheckSeekable();
        return _baseStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        CheckSeekable();
        _baseStream.SetLength(value);
    }

    public override bool CanRead => _baseStream.CanRead;
    public override bool CanSeek => _seekable && _baseStream.CanSeek;
    public override bool CanWrite => _baseStream.CanWrite;
    public override bool CanTimeout => _baseStream.CanTimeout;
    public override long Length => _baseStream.Length;

    public override long Position
    {
        get => _baseStream.Position;
        set
        {
            CheckSeekable();
            _baseStream.Position = value;
        }
    }

    public override int ReadTimeout
    {
        get => _baseStream.ReadTimeout;
        set => _baseStream.ReadTimeout = value;
    }

    public override int WriteTimeout
    {
        get => _baseStream.WriteTimeout;
        set => _baseStream.WriteTimeout = value;
    }

    private void CheckSeekable()
    {
        if (!_seekable)
            throw new NotSupportedException();
    }
}