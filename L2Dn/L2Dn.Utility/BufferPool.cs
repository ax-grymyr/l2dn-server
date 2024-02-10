using System.Collections.Concurrent;

namespace L2Dn;

public sealed class BufferPool
{
    private const int _1024 = 1 << 10;  
    private const int _4096 = 1 << 12;  
    private const int _16384 = 1 << 14;  
    private const int _65536 = 1 << 16;  
    private readonly Bucket _bucket1024 = new(_1024);
    private readonly Bucket _bucket4096 = new(_4096);
    private readonly Bucket _bucket16384 = new(_16384);
    private readonly Bucket _bucket65536 = new(_65536);

    public byte[] Rent(int length) =>
        length switch
        {
            < 0 => throw new ArgumentOutOfRangeException(nameof(length)),
            0 => Array.Empty<byte>(),
            <= _4096 => (length <= _1024 ? _bucket1024 : _bucket4096).Rent(),
            <= _16384 => _bucket16384.Rent(),
            <= _65536 => _bucket65536.Rent(),
            _ => new byte[length]
        };

    public void Return(byte[] buffer)
    {
        switch (buffer.Length)
        {
            case _65536: 
                _bucket65536.Return(buffer);
                break;
            case _16384:
                _bucket16384.Return(buffer);
                break;
            case _4096: 
                _bucket4096.Return(buffer);
                break;
            case _1024:
                _bucket1024.Return(buffer);
                break;
        }
    }

    private sealed class Bucket
    {
        private readonly ConcurrentQueue<byte[]> _queue = new();
        private readonly int _bufferLength;
        public Bucket(int bufferLength) => _bufferLength = bufferLength;
        public byte[] Rent() => _queue.TryDequeue(out byte[]? result) ? result : new byte[_bufferLength];
        public void Return(byte[] buffer) => _queue.Enqueue(buffer);
    }
}
