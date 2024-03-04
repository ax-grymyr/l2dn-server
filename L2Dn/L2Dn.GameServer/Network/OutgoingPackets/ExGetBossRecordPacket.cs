using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExGetBossRecordPacket: IOutgoingPacket
{
    private readonly Map<int, int> _bossRecordInfo;
    private readonly int _ranking;
    private readonly int _totalPoints;
	
    public ExGetBossRecordPacket(int ranking, int totalScore, Map<int, int> list)
    {
        _ranking = ranking;
        _totalPoints = totalScore;
        _bossRecordInfo = list;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_GET_BOSS_RECORD);
        
        writer.WriteInt32(_ranking);
        writer.WriteInt32(_totalPoints);
        if (_bossRecordInfo == null)
        {
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
        }
        else
        {
            writer.WriteInt32(_bossRecordInfo.size()); // list size
            foreach (var entry in _bossRecordInfo)
            {
                writer.WriteInt32(entry.Key);
                writer.WriteInt32(entry.Value);
                writer.WriteInt32(0); // ??
            }
        }
    }
}