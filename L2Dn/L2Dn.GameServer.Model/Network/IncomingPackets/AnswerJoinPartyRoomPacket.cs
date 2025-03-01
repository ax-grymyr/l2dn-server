using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Matching;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct AnswerJoinPartyRoomPacket: IIncomingPacket<GameSession>
{
    private bool _answer;

    public void ReadContent(PacketBitReader reader)
    {
        _answer = reader.ReadInt32() == 1;
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Player? partner = player.getActiveRequester();
        if (partner == null)
        {
            player.sendPacket(SystemMessageId.THAT_PLAYER_IS_NOT_ONLINE);
            player.setActiveRequester(null);
            return ValueTask.CompletedTask;
        }

        if (_answer && !partner.isRequestExpired())
        {
            MatchingRoom? room = partner.getMatchingRoom();
            if (room == null)
                return ValueTask.CompletedTask;

            room.addMember(player);
        }
        else
        {
            partner.sendPacket(SystemMessageId.THE_RECIPIENT_OF_YOUR_INVITATION_DID_NOT_ACCEPT_THE_PARTY_MATCHING_INVITATION);
        }

        // reset transaction timers
        player.setActiveRequester(null);
        partner.onTransactionResponse();

        return ValueTask.CompletedTask;
    }
}