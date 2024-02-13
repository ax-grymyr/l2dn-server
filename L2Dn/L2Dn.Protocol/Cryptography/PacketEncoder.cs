namespace L2Dn.Cryptography;

public abstract class PacketEncoder
{
    public abstract int GetRequiredLength(int length);
    public abstract int Encode(Span<byte> buffer, int packetLength);
    public abstract bool Decode(Span<byte> packet);
}