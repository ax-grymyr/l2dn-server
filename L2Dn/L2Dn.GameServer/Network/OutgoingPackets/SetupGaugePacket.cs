using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct SetupGaugePacket(int objectId, int color, int time, int maxTime)
    : IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x6B); // packet code

        writer.WriteInt32(objectId);
        writer.WriteInt32(color);
        writer.WriteInt32(time);
        writer.WriteInt32(maxTime);
    }
}