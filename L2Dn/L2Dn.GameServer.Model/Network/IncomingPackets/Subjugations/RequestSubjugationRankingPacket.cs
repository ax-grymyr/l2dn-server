using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Subjugation;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Subjugations;

public struct RequestSubjugationRankingPacket: IIncomingPacket<GameSession>
{
    private int _rankingCategory;

    public void ReadContent(PacketBitReader reader)
    {
        _rankingCategory = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new ExSubjugationRankingPacket(_rankingCategory, player.getObjectId()));
        
        return ValueTask.CompletedTask;
    }
}