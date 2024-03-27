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
                
                player.getSummonedNpcs().forEach(npc => npc.teleToLocation(player, true));
            }

            world.onInstanceChange(player, false);
        }
		
        //LOGGER_ACCOUNTING.info("Logged out, " + client);
		
        if (!OfflineTradeUtil.enteredOfflineMode(player))
        {
            Disconnection.of(session, player).defaultSequence(LeaveWorldPacket.STATIC_PACKET);
        }
        
        return ValueTask.CompletedTask;
    }
}