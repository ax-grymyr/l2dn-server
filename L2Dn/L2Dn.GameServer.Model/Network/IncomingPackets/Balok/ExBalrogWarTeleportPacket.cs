using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Balok;

public struct ExBalrogWarTeleportPacket: IIncomingPacket<GameSession>
{
    private static readonly Location3D BALOK_LOCATION = new(-18414, 180442, -3862);
    private const int TELEPORT_COST = 50000;

    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        // Dead characters cannot use teleports.
        if (player.isDead())
        {
            player.sendPacket(SystemMessageId.DEAD_CHARACTERS_CANNOT_USE_TELEPORTS);
            return ValueTask.CompletedTask;
        }

        // Players should not be able to teleport if in a special location.
        if ((player.getMovieHolder() != null) || player.isFishing() || player.isInInstance() || player.isOnEvent() ||
            player.isInOlympiadMode() || player.inObserverMode() || player.isInTraingCamp() ||
            player.isInsideZone(ZoneId.TIMED_HUNTING))
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_RIGHT_NOW);
            return ValueTask.CompletedTask;
        }

        // Cannot teleport in combat.
        if ((player.isInCombat() || player.isCastingNow()))
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_WHILE_IN_COMBAT);
            return ValueTask.CompletedTask;
        }

        // Cannot escape effect.
        if (player.isAffected(EffectFlag.CANNOT_ESCAPE))
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_RIGHT_NOW);
            return ValueTask.CompletedTask;
        }

        // Take teleport fee.
        if (!player.destroyItemByItemId("Battle with Balok Teleport", 57, TELEPORT_COST, player, true))
        {
            player.sendPacket(SystemMessageId.NOT_ENOUGH_MONEY_TO_USE_THE_FUNCTION);
            return ValueTask.CompletedTask;
        }

        // Stop moving.
        player.abortCast();
        player.stopMove(null);

        // Teleport to Balok location.
        player.setTeleportLocation(new Location(BALOK_LOCATION, 0));
        player.doCast(CommonSkill.TELEPORT.getSkill());

        return ValueTask.CompletedTask;
    }
}