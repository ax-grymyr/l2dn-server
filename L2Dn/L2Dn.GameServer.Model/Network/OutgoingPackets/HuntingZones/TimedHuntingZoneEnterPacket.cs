using L2Dn.Extensions;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.HuntingZones;

public readonly struct TimedHuntingZoneEnterPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly int _zoneId;
	
    public TimedHuntingZoneEnterPacket(Player player, int zoneId)
    {
        _player = player;
        _zoneId = zoneId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_TIME_RESTRICT_FIELD_USER_ENTER);
        
        writer.WriteByte(1); // bEnterSuccess
        writer.WriteInt32(_zoneId);
        writer.WriteInt32(DateTime.UtcNow.getEpochSecond()); // nEnterTimeStamp
        writer.WriteInt32((_player.getTimedHuntingZoneRemainingTime(_zoneId) / 1000) + 59); // nRemainTime (zone left time)
    }
}