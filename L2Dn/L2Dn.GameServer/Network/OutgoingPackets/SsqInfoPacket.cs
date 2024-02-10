using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct SsqInfoPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x73); // packet code (0xF8 in C4)
        writer.WriteInt16(256); // who won in Seven Signs: 258 - Dawn, 257 - Dusk, 256 - None
    }
}
