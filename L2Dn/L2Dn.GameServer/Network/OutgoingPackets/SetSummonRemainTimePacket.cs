using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct SetSummonRemainTimePacket: IOutgoingPacket
{
    private readonly int _maxTime;
    private readonly int _remainingTime;
	
    public SetSummonRemainTimePacket(int maxTime, int remainingTime)
    {
        _remainingTime = remainingTime;
        _maxTime = maxTime;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SET_SUMMON_REMAIN_TIME);
        
        writer.WriteInt32(_maxTime);
        writer.WriteInt32(_remainingTime);
    }
}