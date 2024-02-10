namespace L2Dn.IO;

internal sealed class LameCryptStream: XorArrayStream
{
    private static readonly byte[] _cryptKey =
        "Range check error while converting variant of type (%s) into type (%s)"u8.ToArray();

    internal LameCryptStream(Stream stream): base(stream, _cryptKey, EncryptedStream.HeaderSize)
    {
    }
}
