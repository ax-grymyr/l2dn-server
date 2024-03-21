using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Ranking;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Ranking;

public struct RequestPetRankingListPacket: IIncomingPacket<GameSession>
{
    private int _season;
    private RankingCategory _tabId;
    private int _type;
    private int _petItemObjectId;

    public void ReadContent(PacketBitReader reader)
    {
        _season = reader.ReadByte();
        _tabId = (RankingCategory)reader.ReadByte();
        _type = reader.ReadByte();
        _petItemObjectId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new ExPetRankingListPacket(player, _season, _tabId, _type, _petItemObjectId));
        
        return ValueTask.CompletedTask;
    }
}