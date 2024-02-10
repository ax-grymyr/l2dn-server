using System.IO.Compression;
using L2Dn.Conversion;
using Org.BouncyCastle.Crypto.Parameters;

namespace L2Dn.IO;

internal sealed class L2Ver41xInputStream: WrapperStream
{
    internal L2Ver41xInputStream(Stream stream, RsaKeyParameters parameters): base(
        new DeflateStream(CreateDecryptStream(stream, parameters), CompressionMode.Decompress))
    {
    }

    private static RsaDecryptStream CreateDecryptStream(Stream stream, RsaKeyParameters parameters)
    {
        RsaDecryptStream decryptStream = new(stream, parameters);
        Span<byte> span = stackalloc byte[4];
        if (decryptStream.Read(span) != 4)
        {
            decryptStream.Dispose();
            throw new InvalidOperationException("Invalid file");
        }

        int size = BigEndianBitConverter.ToInt32(span);
        return decryptStream;
    }
}
