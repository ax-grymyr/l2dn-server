namespace L2Dn.IO;

public sealed class LameCryptStream: XorArrayStream
{
    public LameCryptStream(Stream stream)
        : base(stream, EncryptionKeys.LameCryptKey, EncryptedStream.HeaderSize)
    {
    }
}