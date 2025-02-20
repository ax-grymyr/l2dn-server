using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestJoinAllyPacket: IIncomingPacket<GameSession>
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

        Player? target = World.getInstance().getPlayer(_objectId);
        if (target == null)
        {
            player.sendPacket(SystemMessageId.THE_TARGET_CANNOT_BE_INVITED);
            return ValueTask.CompletedTask;
        }

        Clan? clan = player.getClan();
        if (clan == null)
        {
            player.sendPacket(SystemMessageId.YOU_ARE_NOT_A_CLAN_MEMBER_2);
            return ValueTask.CompletedTask;
        }

        if (!clan.checkAllyJoinCondition(player, target))
            return ValueTask.CompletedTask;

        if (!player.getRequest().setRequest(target, this))
            return ValueTask.CompletedTask;

        string allyName = clan.getAllyName() ?? string.Empty;

        SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_LEADER_S2_HAS_REQUESTED_AN_ALLIANCE);
        sm.Params.addString(allyName);
        sm.Params.addString(player.getName());
        target.sendPacket(sm);
        target.sendPacket(new AskJoinAllyPacket(player.ObjectId, allyName));

        return ValueTask.CompletedTask;
    }
}