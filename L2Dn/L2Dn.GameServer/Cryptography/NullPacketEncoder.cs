using L2Dn.Cryptography;

namespace L2Dn.GameServer.Cryptography;

internal sealed class NullPacketEncoder: IPacketEncoder
{
    public int GetRequiredLength(int length) => length;
    public int Encode(Span<byte> buffer, int packetLength) => packetLength;
    public bool Decode(Span<byte> packet) => true;
}
