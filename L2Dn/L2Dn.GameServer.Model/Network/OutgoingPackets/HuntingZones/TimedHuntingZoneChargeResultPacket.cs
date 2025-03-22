using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.HuntingZones;

public readonly struct TimedHuntingZoneChargeResultPacket(
    int zoneId, TimeSpan remainTime, TimeSpan refillTime, TimeSpan chargeTime)
    : IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_TIME_RESTRICT_FIELD_USER_CHARGE_RESULT);

        writer.WriteInt32(zoneId);
        writer.WriteInt32((int)remainTime.TotalSeconds); // in seconds
        writer.WriteInt32((int)refillTime.TotalSeconds); // in seconds
        writer.WriteInt32((int)chargeTime.TotalSeconds); // in seconds
    }
}