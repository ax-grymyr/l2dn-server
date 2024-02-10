namespace L2Dn.Cryptography;

public interface ICryptoEngine
{
    int Encode(Span<byte> destination, ReadOnlySpan<byte> source);
    int Decode(Span<byte> destination, ReadOnlySpan<byte> source);
}
