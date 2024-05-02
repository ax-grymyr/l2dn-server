using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Teleports;

public struct ExRequestSharedLocationTeleportPacket: IIncomingPacket<GameSession>
{
    private int _id;

    public void ReadContent(PacketBitReader reader)
    {
        _id = (reader.ReadInt32() - 1) / 256;
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        SharedTeleportHolder teleport = SharedTeleportManager.getInstance().getTeleport(_id);
        if (teleport == null || teleport.getCount() == 0)
        {
            player.sendPacket(SystemMessageId.TELEPORTATION_LIMIT_FOR_THE_COORDINATES_RECEIVED_IS_REACHED);
            return ValueTask.CompletedTask;
        }

        if (player.getName().equals(teleport.getName()))
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_TO_YOURSELF);
            return ValueTask.CompletedTask;
        }

        if (player.getInventory().getInventoryItemCount(Inventory.LCOIN_ID, -1) < Config.TELEPORT_SHARE_LOCATION_COST)
        {
            player.sendPacket(SystemMessageId.THERE_ARE_NOT_ENOUGH_L_COINS);
            return ValueTask.CompletedTask;
        }

        if (player.getMovieHolder() != null || player.isFishing() || player.isInInstance() || player.isOnEvent() ||
            player.isInOlympiadMode() || player.inObserverMode() || player.isInTraingCamp() ||
            player.isInTimedHuntingZone() || player.isInsideZone(ZoneId.SIEGE))
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_RIGHT_NOW);
            return ValueTask.CompletedTask;
        }

        if (player.destroyItemByItemId("Shared Location", Inventory.LCOIN_ID, Config.TELEPORT_SHARE_LOCATION_COST,
                player, true))
        {
            teleport.decrementCount();
            player.abortCast();
            player.stopMove(null);
            player.teleToLocation(new LocationHeading(teleport.getLocation(), 0));
        }

        return ValueTask.CompletedTask;
    }
}