using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
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
		
        Instance world = player.getInstanceWorld();
        if (world != null)
        {
            if (Config.RESTORE_PLAYER_INSTANCE)
            {
                player.getVariables().set("INSTANCE_RESTORE", world.getId());
            }
            else
            {
                Location location = world.getExitLocation(player);
                if (location != null)
                {
                    player.teleToLocation(location);
                }
                else
                {
                    player.teleToLocation(TeleportWhereType.TOWN);
                }
                player.getSummonedNpcs().ForEach(npc => npc.teleToLocation(player, true));
            }
            
            world.onInstanceChange(player, false);
        }
		
        if (!OfflineTradeUtil.enteredOfflineMode(player))
        {
            Disconnection.of(session, player).storeMe().deleteMe();
        }
		
        // Return the client to the authenticated status.
        session.State = GameSessionState.CharacterScreen;
        connection.Send(new RestartResponsePacket(true));
		
        // Send character list
        CharacterListPacket characterListPacket = new(session.PlayKey1, session.AccountName, session.Characters);
        connection.Send(ref characterListPacket);
        return ValueTask.CompletedTask;
    }
}