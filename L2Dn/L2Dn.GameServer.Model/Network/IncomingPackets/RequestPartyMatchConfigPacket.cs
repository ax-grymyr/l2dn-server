using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Matching;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPartyMatchConfigPacket: IIncomingPacket<GameSession>
{
    private int _page;
    private int _location;
    private PartyMatchingRoomLevelType _type;

    public void ReadContent(PacketBitReader reader)
    {
        _page = reader.ReadInt32();
        _location = reader.ReadInt32();
        _type = (PartyMatchingRoomLevelType)reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Party? party = player.getParty();
        CommandChannel? cc = party?.getCommandChannel();
        if (party != null && cc != null && cc.getLeader() == player)
        {
            if (player.getMatchingRoom() == null)
            {
                player.setMatchingRoom(new CommandChannelMatchingRoom(player.getName(),
                    party.getDistributionType(), 1, player.getLevel(), 50, player));
            }
        }
        else if (cc != null && cc.getLeader() != player)
        {
            player.sendPacket(SystemMessageId.THE_COMMAND_CHANNEL_AFFILIATED_PARTY_S_PARTY_MEMBER_CANNOT_USE_THE_MATCHING_SCREEN);
        }
        else if (party != null && party.getLeader() != player)
        {
            player.sendPacket(SystemMessageId.THE_LIST_OF_PARTY_ROOMS_CAN_ONLY_BE_VIEWED_BY_A_PERSON_WHO_IS_NOT_PART_OF_A_PARTY);
        }
        else
        {
            MatchingRoomManager.getInstance().addToWaitingList(player);
            player.sendPacket(new ListPartyWaitingPacket(_type, _location, _page, player.getLevel()));
        }

        return ValueTask.CompletedTask;
    }
}