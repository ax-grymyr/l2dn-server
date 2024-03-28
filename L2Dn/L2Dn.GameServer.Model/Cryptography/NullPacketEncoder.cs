using L2Dn.Cryptography;

namespace L2Dn.GameServer.Cryptography;

internal sealed class NullPacketEncoder: PacketEncoder
{
    public override int GetRequiredLength(int length) => length;
    public override int Encode(Span<byte> buffer, int packetLength) => packetLength;
    public override bool Decode(Span<byte> packet) => true;
}