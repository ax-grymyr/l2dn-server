using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.HuntingZones;

public readonly struct TimeRestrictFieldUserAlarmPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly int _zoneId;

    public TimeRestrictFieldUserAlarmPacket(Player player, int zoneId)
    {
        _player = player;
        _zoneId = zoneId;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_TIME_RESTRICT_FIELD_USER_ALARM);

        writer.WriteInt32(_zoneId);
        writer.WriteInt32((int)_player.getTimedHuntingZoneRemainingTime(_zoneId).TotalSeconds + 59); // RemainTime (zone left time)
    }
}