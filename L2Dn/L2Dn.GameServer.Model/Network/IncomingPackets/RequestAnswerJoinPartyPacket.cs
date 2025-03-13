using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Matching;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestAnswerJoinPartyPacket: IIncomingPacket<GameSession>
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

        PartyRequest? request = player.getRequest<PartyRequest>();
        if (request == null || request.isProcessing() || !player.removeRequest<PartyRequest>())
            return ValueTask.CompletedTask;

        request.setProcessing(true);

        Player requestor = request.getActiveChar();
        if (requestor == null)
            return ValueTask.CompletedTask;

        Party party = request.getParty();
        Party? requestorParty = requestor.getParty();
        if (requestorParty != null && requestorParty != party)
            return ValueTask.CompletedTask;

        requestor.sendPacket(new JoinPartyPacket(_response, requestor));
        if (_response == 1)
        {
            if (party.getMemberCount() >= Config.Character.ALT_PARTY_MAX_MEMBERS)
            {
                SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_PARTY_IS_FULL);
                player.sendPacket(sm);
                requestor.sendPacket(sm);
                return ValueTask.CompletedTask;
            }

            // Assign the party to the leader upon accept of his partner
            if (requestorParty == null)
            {
                requestor.setParty(party);
            }

            player.joinParty(party);

            MatchingRoom? requestorRoom = requestor.getMatchingRoom();
            if (requestorRoom != null)
            {
                requestorRoom.addMember(player);
            }
        }
        else if (_response == -1)
        {
            SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_IS_SET_TO_REFUSE_PARTY_REQUESTS_AND_CANNOT_RECEIVE_A_PARTY_REQUEST);
            sm.Params.addPcName(player);
            requestor.sendPacket(sm);

            if (party.getMemberCount() == 1)
            {
                party.removePartyMember(requestor, PartyMessageType.NONE);
            }
        }
        else if (party.getMemberCount() == 1)
        {
            party.removePartyMember(requestor, PartyMessageType.NONE);
        }

        party.setPendingInvitation(false);
        request.setProcessing(false);
        return ValueTask.CompletedTask;
    }
}