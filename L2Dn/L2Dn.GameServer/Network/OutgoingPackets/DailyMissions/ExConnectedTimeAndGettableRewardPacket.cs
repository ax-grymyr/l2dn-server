using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.DailyMissions;

public readonly struct ExConnectedTimeAndGettableRewardPacket: IOutgoingPacket
{
    private readonly int _oneDayRewardAvailableCount;
	
    public ExConnectedTimeAndGettableRewardPacket(Player player)
    {
        _oneDayRewardAvailableCount = DailyMissionData.getInstance()
            .getDailyMissionData(player).Count(d => d.getStatus(player) == DailyMissionStatus.AVAILABLE);
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        if (!DailyMissionData.getInstance().isAvailable())
        {
            return;
        }
		
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