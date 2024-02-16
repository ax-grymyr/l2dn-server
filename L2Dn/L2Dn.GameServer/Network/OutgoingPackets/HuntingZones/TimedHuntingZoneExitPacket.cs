using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.HuntingZones;

public readonly struct TimedHuntingZoneExitPacket: IOutgoingPacket
{
    private readonly int _zoneId;
	
    public TimedHuntingZoneExitPacket(int zoneId)
    {
        _zoneId = zoneId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_TIME_RESTRICT_FIELD_USER_EXIT);
        writer.WriteInt32(_zoneId);
    }
}