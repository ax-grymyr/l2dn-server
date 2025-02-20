using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Friends;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Network.IncomingPackets.Friends;

public struct RequestFriendDelPacket: IIncomingPacket<GameSession>
{
    private string _name;

    public void ReadContent(PacketBitReader reader)
    {
        _name = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        SystemMessagePacket sm;
        int id = CharInfoTable.getInstance().getIdByName(_name);
        if (id == -1)
        {
            sm = new SystemMessagePacket(SystemMessageId.C1_IS_NOT_ON_YOUR_FRIEND_LIST);
            sm.Params.addString(_name);
            player.sendPacket(sm);
            return ValueTask.CompletedTask;
        }

        if (!player.getFriendList().Contains(id))
        {
            sm = new SystemMessagePacket(SystemMessageId.C1_IS_NOT_ON_YOUR_FRIEND_LIST);
            sm.Params.addString(_name);
            player.sendPacket(sm);
            return ValueTask.CompletedTask;
        }

        try
        {
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            int playerId = player.ObjectId;
            ctx.CharacterFriends.Where(r =>
                    r.CharacterId == playerId && r.FriendId == id || r.CharacterId == id && r.FriendId == playerId)
                .ExecuteDelete();

            // Player deleted from your friend list
            sm = new SystemMessagePacket(SystemMessageId.S1_HAS_BEEN_REMOVED_FROM_YOUR_FRIEND_LIST_2);
            sm.Params.addString(_name);
            player.sendPacket(sm);

            player.getFriendList().remove(id);
            player.sendPacket(new FriendRemovePacket(_name, 1));

            Player? target = World.getInstance().getPlayer(_name);
            if (target != null)
            {
                target.getFriendList().remove(player.ObjectId);
                target.sendPacket(new FriendRemovePacket(player.getName(), 1));
            }

            CharInfoTable.getInstance().removeFriendMemo(player.ObjectId, id);
        }
        catch (Exception e)
        {
            PacketLogger.Instance.Warn("Could not del friend objectid: " + e);
        }

        return ValueTask.CompletedTask;
    }
}