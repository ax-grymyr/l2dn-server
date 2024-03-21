using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Ranking;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Ranking;

public struct RequestPetRankingMyInfoPacket: IIncomingPacket<GameSession>
{
    private int _petId;

    public void ReadContent(PacketBitReader reader)
    {
        _petId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new ExPetRankingMyInfoPacket(player, _petId));

        return ValueTask.CompletedTask;
    }
}