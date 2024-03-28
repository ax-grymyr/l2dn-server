using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Matching;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestOustFromPartyRoomPacket: IIncomingPacket<GameSession>
{
    private int _objectId;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Player member = World.getInstance().getPlayer(_objectId);
        if (member == null)
            return ValueTask.CompletedTask;

        MatchingRoom room = player.getMatchingRoom();
        if (room == null || room.getRoomType() != MatchingRoomType.PARTY || room.getLeader() != player ||
            player == member)
        {
            return ValueTask.CompletedTask;
        }

        Party playerParty = player.getParty();
        Party memberParty = player.getParty();
        if (playerParty != null && memberParty != null &&
            playerParty.getLeaderObjectId() == memberParty.getLeaderObjectId())
        {
            player.sendPacket(SystemMessageId.FAILED_TO_DISMISS_THE_PARTY_MEMBER_2);
        }
        else
        {
            room.deleteMember(member, true);
        }

        return ValueTask.CompletedTask;
    }
}