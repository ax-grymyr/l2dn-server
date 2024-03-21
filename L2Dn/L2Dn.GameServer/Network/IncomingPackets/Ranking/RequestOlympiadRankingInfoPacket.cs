using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Ranking;
using L2Dn.Model;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Ranking;

public struct RequestOlympiadRankingInfoPacket: IIncomingPacket<GameSession>
{
    private RankingOlympiadCategory _tabId;
    private int _rankingType;
    private int _unk;
    private CharacterClass _classId;
    private int _serverId;

    public void ReadContent(PacketBitReader reader)
    {
        _tabId = (RankingOlympiadCategory)reader.ReadByte();
        _rankingType = reader.ReadByte();
        _unk = reader.ReadByte();
        _classId = (CharacterClass)reader.ReadInt32();
        _serverId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new ExOlympiadRankingInfoPacket(player, _tabId, _rankingType, _unk, _classId, _serverId));

        return ValueTask.CompletedTask;
    }
}