using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.DailyMissions;

public readonly struct ExConnectedTimeAndGettableRewardPacket: IOutgoingPacket
{
    private readonly int _oneDayRewardAvailableCount;
	
    public ExConnectedTimeAndGettableRewardPacket(Player player)
    {
        _oneDayRewardAvailableCount = player.getDailyMissions().getAvailableCount();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CONNECTED_TIME_AND_GETTABLE_REWARD);
        
        writer.WriteInt32(0);
        writer.WriteInt32(_oneDayRewardAvailableCount);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
    }
}