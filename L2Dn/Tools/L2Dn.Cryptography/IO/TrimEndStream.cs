namespace L2Dn.IO;

public sealed class TrimEndStream: Stream
{
    private readonly Stream _baseStream;
    private readonly byte[] _buffer; // must be full
    private bool _endOfStream;

    public TrimEndStream(Stream baseStream, int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        if (!baseStream.CanRead)
            throw new ArgumentException();
        
        _baseStream = baseStream;
        _buffer = new byte[count];
        _baseStream.ReadExactly(_buffer, 0, _buffer.Length);
    }
    
    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        if (count == 0 || _endOfStream)
            return 0;

        int readBytes; 
        int bufferOffset;
        if (count < _buffer.Length)
        {
            readBytes = count;
            bufferOffset = _buffer.Length - count;
            Array.Copy(_buffer, 0, buffer, offset, count);
            Array.Copy(_buffer, count, _buffer, 0, bufferOffset);
        }
        else
        {
            readBytes = _buffer.Length;
            bufferOffset = 0;
            Array.Copy(_buffer, 0, buffer, offset, readBytes);

            while (count > readBytes)
            {
                int len = _baseStream.Read(buffer, offset + readBytes, count - readBytes);
                if (len == 0)
                {
                    _endOfStream = true;
                    break;
                }
                
                readBytes += len;
            }
        }

        // fill buffer
        while (bufferOffset != _buffer.Length)
        {
            int len = _baseStream.Read(_buffer, bufferOffset, _buffer.Length - bufferOffset);
            if (len == 0)
            {
                // end of stream
                _endOfStream = true;
                break;
            }

            bufferOffset += len;
        }

        if (_endOfStream && bufferOffset < _buffer.Length)
            readBytes -= _buffer.Length - bufferOffset;

        return readBytes;
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
        throw new NotSupportedException();
    }

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => _baseStream.Length - _buffer.Length;

    public override long Position
    {
        get => _baseStream.Position;
        set => throw new NotSupportedException();
    }
}