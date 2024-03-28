using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.HuntingZones;

public readonly struct TimeRestrictFieldDieLimitTimePacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_TIME_RESTRICT_FIELD_DIE_LIMT_TIME);
        
        writer.WriteInt32(600); // RemainTime (zone left time) in seconds
    }
}