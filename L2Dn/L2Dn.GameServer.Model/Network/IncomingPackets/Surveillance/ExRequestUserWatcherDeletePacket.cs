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
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Network.IncomingPackets.Surveillance;

public struct ExRequestUserWatcherDeletePacket: IIncomingPacket<GameSession>
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

        SystemMessagePacket sm;
        if (!player.getSurveillanceList().Contains(targetId))
        {
            sm = new SystemMessagePacket(SystemMessageId.C1_IS_NOT_ON_YOUR_FRIEND_LIST);
            sm.Params.addString(_name);
            player.sendPacket(sm);
            return ValueTask.CompletedTask;
        }
		
        try
        {
            int characterId = player.ObjectId;
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            ctx.CharacterSurveillances.Where(r => r.CharacterId == characterId && r.TargetId == targetId)
                .ExecuteDelete();
        }
        catch (Exception e)
        {
            PacketLogger.Instance.Warn("ExRequestUserWatcherDelete: Could not add surveillance objectid: " + e);
        }
		
        sm = new SystemMessagePacket(SystemMessageId.YOU_VE_REMOVED_C1_FROM_YOUR_SURVEILLANCE_LIST);
        sm.Params.addString(_name);
        player.sendPacket(sm);
		
        player.getSurveillanceList().remove(targetId);
        player.sendPacket(new ExUserWatcherTargetListPacket(player));
		
        Player target = World.getInstance().getPlayer(targetId);
        if ((target != null) && target.isVisibleFor(player))
        {
            player.sendPacket(new RelationChangedPacket());
        }
        
        return ValueTask.CompletedTask;
    }
}