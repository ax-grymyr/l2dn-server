using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct AnswerPartyLootModificationPacket: IIncomingPacket<GameSession>
{
    public bool _answer;

    public void ReadContent(PacketBitReader reader)
    {
        _answer = reader.ReadInt32() != 0;
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Party? party = player.getParty();
        if (party != null)
        {
            party.answerLootChangeRequest(player, _answer);
        }
        
        return ValueTask.CompletedTask;
    }
}