using System.Numerics;
using L2Dn.Conversion;
using Org.BouncyCastle.Crypto.Parameters;
using BigInteger = Org.BouncyCastle.Math.BigInteger;

namespace L2Dn.IO;

public sealed class RsaDecryptStream: Stream
{
    private readonly Stream _baseStream;
    private readonly RsaKeyParameters _parameters;
    private readonly byte[] _buffer;
    private readonly int _keySize;
    private int _offset;
    private int _length;

    public RsaDecryptStream(Stream baseStream, RsaKeyParameters parameters)
    {
        if (!baseStream.CanRead)
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
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (_length == 0)
        {
            _buffer[0] = 0;
            int len = _baseStream.Read(_buffer, 1, _keySize);
            if (len == 0)
            {
                // end of stream
                return 0;
            }

            if (len < _keySize)
                _baseStream.ReadExactly(_buffer, 1 + len, _keySize - len);

            BigInteger input = new BigInteger(_buffer);
            byte[] result = input.ModPow(_parameters.Exponent, _parameters.Modulus).ToByteArray();

            if (result.Length > _keySize + 1)
                throw new InvalidOperationException();
            
            if (result.Length == _keySize + 1)
            {
                if (result[0] != 0)
                    throw new InvalidOperationException();

                Array.Copy(result, 1, _buffer, 0, _keySize);
            }
            else if (result.Length == _keySize)
            {
                Array.Copy(result, 0, _buffer, 0, _keySize);
            }
            else
            {
                int diff = _keySize - result.Length;
                Array.Clear(_buffer, 0, diff);
                Array.Copy(result, 0, _buffer, diff, result.Length);
            }

            int size = BigEndianBitConverter.ToInt32(_buffer);
            int pad = -size & 0x3;

            if (size + pad > _keySize)
                throw new InvalidOperationException();
            
            _offset = _keySize - size - pad;
            _length = size;
        }

        int length = Math.Min(count, _length);
        Array.Copy(_buffer, _offset, buffer, offset, length);
        _offset += length;
        _length -= length;
        return length;
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
    public override long Length => _baseStream.Length;

    public override long Position
    {
        get => _baseStream.Position;
        set => throw new NotSupportedException();
    }
}