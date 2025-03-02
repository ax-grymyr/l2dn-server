using L2Dn.Extensions;
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

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestRestartPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (session.Characters is null)
        {
            // Characters must be loaded in AuthLoginPacket
            connection.Close();
            return ValueTask.CompletedTask;
        }

        if (!player.canLogout())
        {
            player.sendPacket(new RestartResponsePacket(false));
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        // Unregister from olympiad.
        if (OlympiadManager.getInstance().isRegistered(player))
        {
            OlympiadManager.getInstance().unRegisterNoble(player);
        }

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
                location = world.getExitLocation(player);
                if (location == null)
                {
                    location = MapRegionManager.getInstance().getTeleToLocation(player, TeleportWhereType.TOWN).Location3D;
                }
            }

            player.setInstance(null);
        }
        else if (player.isInTimedHuntingZone())
        {
            location = MapRegionManager.getInstance().getTeleToLocation(player, TeleportWhereType.TOWN).Location3D;
        }

        if (location != null)
        {
            player.getVariables().Set(PlayerVariables.RESTORE_LOCATION, location.Value);
        }

        //LOGGER_ACCOUNTING.info("Logged out, " + client);

        if (!OfflineTradeUtil.enteredOfflineMode(player))
        {
            Disconnection.of(session, player).storeMe().deleteMe();
        }

        // Return the client to the authenticated status.
        session.State = GameSessionState.CharacterScreen;
        connection.Send(new RestartResponsePacket(true));

        // Send character list
        session.Characters.UpdateActiveCharacter(player);
        CharacterListPacket characterListPacket = new(session.PlayKey1, session.AccountName, session.Characters);
        connection.Send(ref characterListPacket);
        return ValueTask.CompletedTask;
    }
}