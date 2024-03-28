using System.IO.Compression;
using L2Dn.Conversion;
using Org.BouncyCastle.Crypto.Parameters;

namespace L2Dn.IO;

public static class L2Ver41X
{
    public static Stream CreateInputStream(Stream baseStream, RsaKeyParameters parameters)
    {
        RsaDecryptStream decryptStream = new(baseStream, parameters);
        Span<byte> span = stackalloc byte[4];
        if (decryptStream.Read(span) != 4)
        {
            decryptStream.Dispose();
            throw new InvalidOperationException("Invalid file");
        }

        int size = BigEndianBitConverter.ToInt32(span);

        ZLibStream zLibStream = new ZLibStream(decryptStream, CompressionMode.Decompress);
        return zLibStream;
    }
}