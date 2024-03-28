using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct AuthLoginFailedPacket(int success, AuthFailedReason reason): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        // 0x14 in C4
        writer.WriteByte(0x0A); // packet code

        writer.WriteInt32(success); // missing in C4
        writer.WriteInt32((int)reason);
    }
}
