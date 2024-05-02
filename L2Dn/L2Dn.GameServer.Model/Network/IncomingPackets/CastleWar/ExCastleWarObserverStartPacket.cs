using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.CastleWar;

public struct ExCastleWarObserverStartPacket: IIncomingPacket<GameSession>
{
    private int _castleId;

    public void ReadContent(PacketBitReader reader)
    {
        _castleId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (player.hasSummon())
        {
            player.sendPacket(SystemMessageId.YOU_MAY_NOT_OBSERVE_A_SIEGE_WITH_A_SERVITOR_SUMMONED);
            return ValueTask.CompletedTask;
        }
		
        if (player.isOnEvent())
        {
            player.sendMessage("Cannot use while on an event.");
            return ValueTask.CompletedTask;
        }
		
        Castle castle = CastleManager.getInstance().getCastleById(_castleId);
        if (castle == null)
            return ValueTask.CompletedTask;
		
        if (!castle.getSiege().isInProgress())
            return ValueTask.CompletedTask;
		
        Player? random = castle.getSiege().getPlayersInZone().FirstOrDefault();
        if (random == null)
            return ValueTask.CompletedTask;
		
        player.enterObserverMode(random.getLocation().ToLocationHeading());
        
        return ValueTask.CompletedTask;
    }
}