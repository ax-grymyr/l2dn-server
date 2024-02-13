using System.Security.Cryptography;
using L2Dn.Conversion;

namespace L2Dn.Cryptography
{
    public static class RandomGenerator
    {
        private static readonly RandomNumberGenerator _random = RandomNumberGenerator.Create();

        [ThreadStatic] 
        private static byte[]? _buffer; 

        public static int GetInt32()
        {
            _random.GetBytes(_buffer ??= new byte[4]);
            return NativeEndianBitConverter.ToInt32(_buffer);
        }

        public static int GetInt32(int maxValue)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxValue);

            _random.GetBytes(_buffer ??= new byte[4]);
            int result = NativeEndianBitConverter.ToInt32(_buffer) % maxValue;
            return result < 0 ? result + maxValue : result;
        }

        public static uint GetUInt32()
        {
            _random.GetBytes(_buffer ??= new byte[4]);
            return NativeEndianBitConverter.ToUInt32(_buffer);
        }

        public static void GetNonZeroBytes(Span<byte> buffer)
        {
            _random.GetNonZeroBytes(buffer);
        }
    }
}