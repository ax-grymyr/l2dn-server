using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Balok;

public readonly struct BalrogWarShowRankingPacket: IOutgoingPacket
{
    private readonly Map<int, int> _rankingData;
	
    public BalrogWarShowRankingPacket()
    {
        _rankingData = BattleWithBalokManager.getInstance().getTopPlayers(150);
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BALROGWAR_SHOW_RANKING);
        
        writer.WriteInt32(_rankingData.Count);
        int rank = 0;
        foreach (var entry in _rankingData)
        {
            rank++;
            writer.WriteInt32(rank); // Rank
            writer.WriteSizedString(CharInfoTable.getInstance().getNameById(entry.Key)); // Name
            writer.WriteInt32(entry.Value); // Score
        }
    }
}