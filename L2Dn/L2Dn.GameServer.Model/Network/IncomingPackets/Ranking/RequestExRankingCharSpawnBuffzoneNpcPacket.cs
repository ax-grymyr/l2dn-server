using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.Ranking;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Ranking;

public struct RequestExRankingCharSpawnBuffzoneNpcPacket: IIncomingPacket<GameSession>
{
    private const int COST = 20000000;

    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (GlobalVariablesManager.getInstance()
                .Get(GlobalVariablesManager.RANKING_POWER_COOLDOWN, DateTime.MinValue) > DateTime.UtcNow)
        {
            player.sendPacket(SystemMessageId.LEADER_POWER_COOLDOWN);
            return ValueTask.CompletedTask;
        }

        if (!player.destroyItemByItemId("Adena", 57, COST, player, true))
        {
            player.sendPacket(SystemMessageId.NOT_ENOUGH_MONEY_TO_USE_THE_FUNCTION);
            return ValueTask.CompletedTask;
        }

        if (!player.isInsideZone(ZoneId.PEACE) || player.isInStoreMode() ||
            World.getInstance().getVisibleObjectsInRange<Creature>(player, 50).Count != 0)
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_USE_LEADER_POWER_HERE);
            return ValueTask.CompletedTask;
        }

        RankingPowerManager.getInstance().activatePower(player);
        player.sendPacket(new ExRankingBuffZoneNpcPositionPacket());
        player.sendPacket(new ExRankingBuffZoneNpcInfoPacket());

        return ValueTask.CompletedTask;
    }
}