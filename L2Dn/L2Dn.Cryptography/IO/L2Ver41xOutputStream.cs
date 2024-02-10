using System.IO.Compression;
using Org.BouncyCastle.Crypto.Parameters;

namespace L2Dn.IO;

internal sealed class L2Ver41xOutputStream: WrapperStream
{
    internal L2Ver41xOutputStream(Stream stream, RsaKeyParameters parameters): base(
        new DeflateStream(new RsaEncryptStream(stream, parameters), CompressionLevel.SmallestSize))
    {
    }
}
