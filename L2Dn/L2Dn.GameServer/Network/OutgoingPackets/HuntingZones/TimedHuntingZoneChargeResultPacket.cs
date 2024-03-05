using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.HuntingZones;

public readonly struct TimedHuntingZoneChargeResultPacket: IOutgoingPacket
{
    private readonly int _zoneId;
    private readonly int _remainTime;
    private readonly int _refillTime;
    private readonly int _chargeTime;
	
    public TimedHuntingZoneChargeResultPacket(int zoneId, int remainTime, int refillTime, int chargeTime)
    {
        _zoneId = zoneId;
        _remainTime = remainTime;
        _refillTime = refillTime;
        _chargeTime = chargeTime;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_TIME_RESTRICT_FIELD_USER_CHARGE_RESULT);
        
        writer.WriteInt32(_zoneId);
        writer.WriteInt32(_remainTime);
        writer.WriteInt32(_refillTime);
        writer.WriteInt32(_chargeTime);
    }
}