using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Friends;

public struct RequestFriendListPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        SystemMessagePacket sm;

        // ======<Friend List>======
        player.sendPacket(SystemMessageId.FRIENDS_LIST);

        Player? friend;
        foreach (int id in player.getFriendList())
        {
            // int friendId = rset.getInt("friendId");
            string? friendName = CharInfoTable.getInstance().getNameById(id);
            if (friendName == null)
            {
                continue;
            }

            friend = World.getInstance().getPlayer(friendName);
            if (friend == null || !friend.isOnline())
            {
                // (Currently: Offline)
                sm = new SystemMessagePacket(SystemMessageId.S1_OFFLINE);
                sm.Params.addString(friendName);
            }
            else
            {
                // (Currently: Online)
                sm = new SystemMessagePacket(SystemMessageId.S1_CURRENTLY_ONLINE);
                sm.Params.addString(friendName);
            }

            player.sendPacket(sm);
        }

        // =========================
        player.sendPacket(SystemMessageId.EMPTY_3);
        return ValueTask.CompletedTask;
    }
}