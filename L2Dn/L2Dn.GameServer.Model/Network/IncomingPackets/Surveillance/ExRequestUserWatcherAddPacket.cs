using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Surveillance;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Surveillance;

public struct ExRequestUserWatcherAddPacket: IIncomingPacket<GameSession>
{
    private string _name;

    public void ReadContent(PacketBitReader reader)
    {
        _name = reader.ReadSizedString();
        //reader.ReadInt32(); // World Id
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        int targetId = CharInfoTable.getInstance().getIdByName(_name);
        if (targetId == -1)
        {
            player.sendPacket(SystemMessageId.THAT_CHARACTER_DOES_NOT_EXIST);
            return ValueTask.CompletedTask;
        }
		
        // You cannot add yourself to your own friend list.
        if (targetId == player.ObjectId)
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_ADD_YOURSELF_TO_YOUR_SURVEILLANCE_LIST);
            return ValueTask.CompletedTask;
        }
		
        // Target already in surveillance list.
        if (player.getSurveillanceList().Contains(targetId))
        {
            player.sendPacket(SystemMessageId.THE_CHARACTER_IS_ALREADY_IN_YOUR_SURVEILLANCE_LIST);
            return ValueTask.CompletedTask;
        }
		
        if (player.getSurveillanceList().size() >= 10)
        {
            player.sendPacket(SystemMessageId.MAXIMUM_NUMBER_OF_PEOPLE_ADDED_YOU_CANNOT_ADD_MORE);
            return ValueTask.CompletedTask;
        }
		
        try
        {
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            ctx.CharacterSurveillances.Add(new CharacterSurveillance()
            {
                CharacterId = player.ObjectId,
                TargetId = targetId
            });

            ctx.SaveChanges();
        }
        catch (Exception e)
        {
            PacketLogger.Instance.Warn("ExRequestUserWatcherAdd: Could not add surveillance objectid: " + e);
        }
		
        // Player added to your friend list.
        SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_VE_ADDED_C1_TO_YOUR_SURVEILLANCE_LIST);
        sm.Params.addString(_name);
        player.sendPacket(sm);
        player.getSurveillanceList().add(targetId);
        player.sendPacket(new ExUserWatcherTargetListPacket(player));
		
        Player target = World.getInstance().getPlayer(targetId);
        if ((target != null) && target.isVisibleFor(player))
        {
            player.sendPacket(new RelationChangedPacket());
        }
        
        return ValueTask.CompletedTask;
    }
}