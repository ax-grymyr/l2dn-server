using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Sayune;

public struct RequestFlyMoveStartPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (player.hasSummon())
        {
            player.sendPacket(SystemMessageId.YOU_MAY_NOT_USE_SAYUNE_WHILE_A_SERVITOR_IS_AROUND);
            return ValueTask.CompletedTask;
        }

        if (player.getReputation() < 0)
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_USE_SAYUNE_WHILE_IN_A_CHAOTIC_STATE);
            return ValueTask.CompletedTask;
        }

        if (player.hasRequests())
        {
            player.sendPacket(SystemMessageId.SAYUNE_CANNOT_BE_USED_WHILE_TAKING_OTHER_ACTIONS);
            return ValueTask.CompletedTask;
        }

        SayuneZone? zone = ZoneManager.getInstance().getZone<SayuneZone>(player.Location.Location3D);
        if (zone == null || zone.getMapId() == -1)
        {
            player.sendMessage("That zone is not supported yet!");
            PacketLogger.Instance.Warn(GetType().Name + ": " + player + " Requested sayune on zone with no map id set");
            return ValueTask.CompletedTask;
        }

        SayuneEntry? map = SayuneData.getInstance().getMap(zone.getMapId());
        if (map == null)
        {
            player.sendMessage("This zone is not handled yet!!");
            PacketLogger.Instance.Warn(GetType().Name + ": " + player + " Requested sayune on unhandled map zone " + zone.getName());
            return ValueTask.CompletedTask;
        }

        SayuneRequest request = new(player, map.getId());
        if (player.addRequest(request))
        {
            request.move(player, 0);
        }

        return ValueTask.CompletedTask;
    }
}