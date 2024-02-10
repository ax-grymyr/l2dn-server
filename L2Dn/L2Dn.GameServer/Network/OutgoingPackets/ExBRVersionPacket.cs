using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct ExBRVersionPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0xFE); // packet code
        writer.WriteUInt16(0x308); // packet ex code

        writer.WriteByte(1); // enable world exchange
    }
}
