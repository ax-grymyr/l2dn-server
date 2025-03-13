using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public readonly struct LogoutPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
        {
            connection.Close();
            return ValueTask.CompletedTask;
        }

        // Unregister from olympiad.
        if (OlympiadManager.getInstance().isRegistered(player))
        {
            OlympiadManager.getInstance().unRegisterNoble(player);
        }

        // Set restore location for next enter world // TODO: also save heading
        Location3D? location = null;
        Instance? world = player.getInstanceWorld();
        if (world != null)
        {
            if (Config.RESTORE_PLAYER_INSTANCE)
            {
                player.getVariables().Set(PlayerVariables.INSTANCE_RESTORE, world.getId());
            }
            else
            {
                location = world.getExitLocation(player) ?? MapRegionManager.getInstance()
                    .getTeleToLocation(player, TeleportWhereType.TOWN).Location3D;
            }

            player.setInstance(null);
        }
        else if (player.isInTimedHuntingZone())
        {
            location = MapRegionManager.getInstance().getTeleToLocation(player, TeleportWhereType.TOWN).Location3D;
        }

        if (location != null)
        {
            player.getVariables().Set(PlayerVariables.RESTORE_LOCATION, location.Value.X + ";" + location.Value.Y + ";" + location.Value.Z);
        }

        //LOGGER_ACCOUNTING.info("Logged out, " + client);

        if (!OfflineTradeUtil.enteredOfflineMode(player))
        {
            Disconnection.of(session, player).defaultSequence(LeaveWorldPacket.STATIC_PACKET);
        }

        return ValueTask.CompletedTask;
    }
}