using System.Numerics;
using Org.BouncyCastle.Crypto.Parameters;
using BigInteger = Org.BouncyCastle.Math.BigInteger;

namespace L2Dn.IO;

public sealed class RsaEncryptStream: Stream
{
    private readonly Stream _baseStream;
    private readonly RsaKeyParameters _parameters;
    private readonly byte[] _buffer;
    private readonly int _keySize;
    private int _offset = 1;

    public RsaEncryptStream(Stream baseStream, RsaKeyParameters parameters)
    {
        if (!baseStream.CanWrite)
            throw new ArgumentException();
        
        _baseStream = baseStream;
        _parameters = parameters;

        uint length = (uint)parameters.Modulus.BitLength / 8;
        length = BitOperations.RoundUpToPowerOf2(length);
        _keySize = (int)length;
        _buffer = new byte[(int)length + 1];
    }
    
    public override void Flush()
    {
        if (_offset != 1)
            throw new InvalidOperationException();
        
        _baseStream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
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
        int length = Math.Min(count, _keySize - _offset);
        Array.Copy(buffer, offset, _buffer, _offset, length);
        _offset += length;
        
        if (_offset == _keySize + 1)
        {
            _buffer[0] = 0;
            BigInteger input = new BigInteger(_buffer);
            byte[] result = input.ModPow(_parameters.Exponent, _parameters.Modulus).ToByteArray();

            if (result.Length > _keySize + 1)
                throw new InvalidOperationException();
            
            if (result.Length == _keySize + 1)
            {
                if (result[0] != 0)
                    throw new InvalidOperationException();

                _baseStream.Write(result, 1, _keySize);
            }
            else if (result.Length == _keySize)
            {
                _baseStream.Write(result, 0, _keySize);
            }
            else
            {
                int diff = _keySize - result.Length;
                Array.Clear(_buffer, 0, diff);
                _baseStream.Write(_buffer, 0, diff);
                _baseStream.Write(result, 0, result.Length);
            }

            _offset = 1;
        }
    }

    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => _baseStream.Length;

    public override long Position
    {
        get => _baseStream.Position;
        set => throw new NotSupportedException();
    }
}