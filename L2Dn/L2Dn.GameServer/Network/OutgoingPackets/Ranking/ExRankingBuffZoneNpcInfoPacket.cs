using L2Dn.GameServer.InstanceManagers;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Ranking;

public readonly struct ExRankingBuffZoneNpcInfoPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RANKING_CHAR_BUFFZONE_NPC_INFO);
        
        DateTime cooldown = GlobalVariablesManager.getInstance().getDateTime(GlobalVariablesManager.RANKING_POWER_COOLDOWN, DateTime.MinValue);
        DateTime currentTime = DateTime.UtcNow;
        if (cooldown > currentTime)
        {
            TimeSpan reuseTime = cooldown - currentTime;
            writer.WriteInt32((int)reuseTime.TotalSeconds);
        }
        else
        {
            writer.WriteInt32(0);
        }
    }
}