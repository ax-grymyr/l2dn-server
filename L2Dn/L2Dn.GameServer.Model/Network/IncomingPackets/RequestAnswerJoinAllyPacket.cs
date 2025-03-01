using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestAnswerJoinAllyPacket: IIncomingPacket<GameSession>
{
    private int _response;

    public void ReadContent(PacketBitReader reader)
    {
        _response = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Clan? playerClan = player.getClan();
        if (playerClan == null)
            return ValueTask.CompletedTask;

        Player? requestor = player.getRequest().getPartner();
        if (requestor == null)
            return ValueTask.CompletedTask;

        Clan? requestorClan = requestor.getClan();
        if (requestorClan == null)
            return ValueTask.CompletedTask;

        if (_response == 0)
        {
            player.sendPacket(SystemMessageId.NO_RESPONSE_YOUR_ENTRANCE_TO_THE_ALLIANCE_HAS_BEEN_CANCELLED);
            requestor.sendPacket(SystemMessageId.NO_RESPONSE_THE_INVITATION_TO_JOIN_THE_ALLIANCE_IS_CANCELLED);
        }
        else
        {
            if (requestor.getRequest().getRequestPacket() is not RequestJoinAllyPacket)
                return ValueTask.CompletedTask; // hax

            // we must double-check this cause of hack
            if (requestorClan.checkAllyJoinCondition(requestor, player))
            {
                // TODO: Need correct message id
                requestor.sendPacket(SystemMessageId.THAT_PERSON_HAS_BEEN_SUCCESSFULLY_ADDED_TO_YOUR_FRIEND_LIST);
                player.sendPacket(SystemMessageId.YOU_HAVE_ACCEPTED_THE_ALLIANCE);

                playerClan.setAllyId(requestorClan.getAllyId());
                playerClan.setAllyName(requestorClan.getAllyName());
                playerClan.setAllyPenaltyExpiryTime(null, 0);
                playerClan.changeAllyCrest(requestorClan.getAllyCrestId(), true);
                playerClan.updateClanInDB();
            }
        }

        player.getRequest().onRequestResponse();

        return ValueTask.CompletedTask;
    }
}