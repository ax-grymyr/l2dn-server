using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Subjugation;

public readonly struct ExSubjugationRankingPacket: IOutgoingPacket
{
    private readonly Map<string, int> _ranking;
    private readonly int _category;
    private readonly int _objectId;
	
    public ExSubjugationRankingPacket(int category, int objectId)
    {
        _ranking = PurgeRankingManager.getInstance().getTop5(category);
        _category = category;
        _objectId = objectId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SUBJUGATION_RANKING);
        
        writer.WriteInt32(_ranking.Count);
        int counter = 1;
        foreach (var data in _ranking)
        {
            writer.WriteSizedString(data.Key);
            writer.WriteInt32(data.Value);
            writer.WriteInt32(counter++);
        }
 
        writer.WriteInt32(_category);
        writer.WriteInt32(PurgeRankingManager.getInstance().getPlayerRating(_category, _objectId).Item2);
        writer.WriteInt32(PurgeRankingManager.getInstance().getPlayerRating(_category, _objectId).Item1);
    }
}