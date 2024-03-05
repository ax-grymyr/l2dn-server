using L2Dn.GameServer.InstanceManagers;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Ranking;

public readonly struct ExRankingBuffZoneNpcPositionPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RANKING_CHAR_BUFFZONE_NPC_POSITION);
        
        if (GlobalVariablesManager.getInstance().getDateTime(GlobalVariablesManager.RANKING_POWER_COOLDOWN, DateTime.MinValue) > DateTime.UtcNow)
        {
            List<int> location = GlobalVariablesManager.getInstance().getIntegerList(GlobalVariablesManager.RANKING_POWER_LOCATION);
            writer.WriteByte(1);
            writer.WriteInt32(location[0]);
            writer.WriteInt32(location[1]);
            writer.WriteInt32(location[2]);
        }
        else
        {
            writer.WriteByte(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
        }
    }
}