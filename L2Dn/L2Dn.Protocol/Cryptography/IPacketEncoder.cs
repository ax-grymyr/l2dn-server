namespace L2Dn.Cryptography
{
    public interface IPacketEncoder
    {
        int GetRequiredLength(int length);
        int Encode(Span<byte> buffer, int packetLength);
        bool Decode(Span<byte> packet);
    }
}
