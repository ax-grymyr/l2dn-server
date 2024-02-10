using System.Buffers;

namespace L2Dn.IO;

public abstract class ProcessingStream(Stream stream, int bufferSize = 4096): WrapperStream(stream)
{
    private readonly byte[] _buffer = new byte[bufferSize];
    private ReadOnlyMemory<byte> _data;
    private bool _endOfStream;

    public override int Read(Span<byte> buffer)
    {
        ReadToBuffer();
        int length = Math.Min(buffer.Length, _data.Length);
        _data.Span[..length].CopyTo(buffer);
        _data = _data[length..];
        return length;
    }

    public override int Read(byte[] buffer, int offset, int count) => Read(buffer.AsSpan(offset, count));

    public override int ReadByte()
    {
        ReadToBuffer();
        int result = -1;
        if (_data.Length != 0)
        {
            result = _data.Span[0];
            _data = _data[1..];
        }

        return result;
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        await ReadToBufferAsync(cancellationToken);
        int length = Math.Min(buffer.Length, _data.Length);
        _data.Span[..length].CopyTo(buffer.Span);
        _data = _data[length..];
        return length;
    }

    public override async Task<int>
        ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        await ReadAsync(buffer.AsMemory(offset, count), cancellationToken);

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        ThisAsyncResult asyncResult = new();


        // TODO: not asynchronous
        ReadToBuffer();

        return base.BeginRead(buffer, offset, count, callback, state);
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        ReadOnlySpan<byte> temp = buffer;
        int dataLength = _data.Length;
        while (temp.Length != 0)
        {
            int length = Math.Min(temp.Length, _buffer.Length - dataLength);
            temp[..length].CopyTo(_buffer.AsSpan(dataLength));
            dataLength += length;

            if (dataLength == _buffer.Length)
            {
                WriteBuffer(dataLength);
                dataLength = 0;
            }

            temp = temp[length..];
        }

        _data = _buffer.AsMemory(0, dataLength);
    }

    public override void Write(byte[] buffer, int offset, int count) => Write(buffer.AsSpan(offset, count));

    public override void WriteByte(byte value)
    {
        int dataLength = _data.Length;
        _buffer[dataLength] = value;
        dataLength++;
        if (dataLength == _buffer.Length)
        {
            WriteBuffer(dataLength);
            dataLength = 0;
        }

        _data = _buffer.AsMemory(0, dataLength);
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer,
        CancellationToken cancellationToken = default)
    {
        ReadOnlyMemory<byte> temp = buffer;
        int dataLength = _data.Length;
        while (temp.Length != 0)
        {
            int length = Math.Min(temp.Length, _buffer.Length - dataLength);
            temp[..length].CopyTo(_buffer.AsMemory(dataLength));
            dataLength += length;

            if (dataLength == _buffer.Length)
            {
                await WriteBufferAsync(dataLength, cancellationToken);
                dataLength = 0;
            }

            temp = temp[length..];
        }

        _data = _buffer.AsMemory(0, dataLength);
    }

    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        await WriteAsync(buffer.AsMemory(offset, count), cancellationToken);

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback,
        object? state)
    {
        // TODO: buffer modified inline
        Span<byte> span = buffer.AsSpan(offset, count);
        ProcessOutputData(span, span);
        return base.BeginWrite(buffer, offset, count, callback, state);
    }

    public override void Flush()
    {
        if (CanWrite && _data.Length != 0)
        {
            WriteBuffer(_data.Length);
            _data = new ReadOnlyMemory<byte>();
        }

        base.Flush();
    }

    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
        if (CanWrite && _data.Length != 0)
        {
            await WriteBufferAsync(_data.Length, cancellationToken);
            _data = new ReadOnlyMemory<byte>();
        }

        await base.FlushAsync(cancellationToken);
    }

    protected virtual void ProcessInputData(Span<byte> output, ReadOnlySpan<byte> input)
    {
    }

    protected virtual void ProcessOutputData(Span<byte> output, ReadOnlySpan<byte> input)
    {
    }

    private void ReadToBuffer()
    {
        if (_data.Length == 0 && !_endOfStream)
        {
            byte[] buf = ArrayPool<byte>.Shared.Rent(_buffer.Length);
            try
            {
                int bytesRead = ReadAtLeast(buf.AsSpan(0, _buffer.Length), _buffer.Length, false);
                _endOfStream = bytesRead != _buffer.Length;
                ProcessInputData(_buffer, buf.AsSpan(0, bytesRead));
                _data = _buffer.AsMemory(0, bytesRead);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buf);
            }
        }
    }

    private async ValueTask ReadToBufferAsync(CancellationToken cancellationToken)
    {
        if (_data.Length == 0 && !_endOfStream)
        {
            byte[] buf = ArrayPool<byte>.Shared.Rent(_buffer.Length);
            try
            {
                int bytesRead = await ReadAtLeastAsync(buf.AsMemory(0, _buffer.Length), _buffer.Length, false,
                    cancellationToken);

                _endOfStream = bytesRead != _buffer.Length;
                ProcessInputData(_buffer, buf.AsSpan(0, bytesRead));
                _data = _buffer.AsMemory(0, bytesRead);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buf);
            }
        }
    }

    private void WriteBuffer(int length)
    {
        byte[] buf = ArrayPool<byte>.Shared.Rent(length);
        try
        {
            ProcessOutputData(buf.AsSpan(0, length), _buffer.AsSpan(0, length));
            base.Write(buf, 0, length);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buf);
        }
    }

    private async ValueTask WriteBufferAsync(int length, CancellationToken cancellationToken)
    {
        byte[] buf = ArrayPool<byte>.Shared.Rent(length);
        try
        {
            ProcessOutputData(buf.AsSpan(0, length), _buffer.AsSpan(0, length));
            await base.WriteAsync(buf, 0, length, cancellationToken);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buf);
        }
    }

    private sealed class ThisAsyncResult: IAsyncResult
    {
        public object? AsyncState => throw new NotImplementedException();

        public WaitHandle AsyncWaitHandle => throw new NotImplementedException();

        public bool CompletedSynchronously => throw new NotImplementedException();

        public bool IsCompleted => throw new NotImplementedException();
    }
}
