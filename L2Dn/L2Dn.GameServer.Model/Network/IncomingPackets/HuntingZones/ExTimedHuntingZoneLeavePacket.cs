using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.HuntingZones;
using L2Dn.Network;
using L2Dn.Packets;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Network.IncomingPackets.HuntingZones;

public struct ExTimedHuntingZoneLeavePacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        if (player.isInCombat())
        {
            connection.Send(new SystemMessagePacket(SystemMessageId.YOU_CANNOT_TELEPORT_IN_BATTLE));
            return ValueTask.CompletedTask;
        }
		
        TimedHuntingZoneHolder huntingZone = player.getTimedHuntingZone();
        if (huntingZone == null)
            return ValueTask.CompletedTask;
		
        Location exitLocation = huntingZone.getExitLocation();
        if (exitLocation != null)
        {
            player.teleToLocation(exitLocation.ToLocationHeading(), null);
        }
        else
        {
            Instance world = player.getInstanceWorld();
            if (world != null)
            {
                world.ejectPlayer(player);
            }
            else
            {
                player.teleToLocation(TeleportWhereType.TOWN);
            }
        }
		
        ThreadPool.schedule(() => connection.Send(new TimedHuntingZoneExitPacket(huntingZone.getZoneId())), 3000);
        return ValueTask.CompletedTask;
    }
}