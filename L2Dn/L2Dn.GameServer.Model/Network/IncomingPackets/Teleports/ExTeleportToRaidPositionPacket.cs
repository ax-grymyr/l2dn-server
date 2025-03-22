using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.Teleports;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets.Teleports;

public struct ExTeleportToRaidPositionPacket: IIncomingPacket<GameSession>
{
    private int _raidId;

    public void ReadContent(PacketBitReader reader)
    {
        _raidId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

 		TeleportListHolder? teleport = RaidTeleportListData.getInstance().getTeleport(_raidId);
		if (teleport == null)
		{
			PacketLogger.Instance.Warn("No registered teleport location for raid id: " + _raidId);
			return ValueTask.CompletedTask;
		}

		// Dead characters cannot use teleports.
		if (player.isDead())
		{
			player.sendPacket(SystemMessageId.DEAD_CHARACTERS_CANNOT_USE_TELEPORTS);
			return ValueTask.CompletedTask;
		}

		NpcTemplate? template = NpcData.getInstance().getTemplate(_raidId);
        if (template == null)
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_BECAUSE_THE_TARGET_IS_DEAD); // TODO: verify this message
            return ValueTask.CompletedTask;
        }

		if (template.isType("GrandBoss") && GrandBossManager.getInstance().getStatus(_raidId) != 0)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_RIGHT_NOW);
			return ValueTask.CompletedTask;
		}

		if (template.isType("RaidBoss") && DbSpawnManager.getInstance().getStatus(_raidId) != RaidBossStatus.ALIVE)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_RIGHT_NOW);
			return ValueTask.CompletedTask;
		}

		// Players should not be able to teleport if in combat, or in a special location.
		if (player.isCastingNow() || player.isInCombat() || player.isImmobilized() || player.isInInstance() || player.isOnEvent() || player.isInOlympiadMode() || player.inObserverMode() || player.isInTraingCamp() || player.isInsideZone(ZoneId.TIMED_HUNTING))
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_RIGHT_NOW);
			return ValueTask.CompletedTask;
		}

		// Karma related configurations.
		if ((!Config.Character.ALT_GAME_KARMA_PLAYER_CAN_TELEPORT || !Config.Character.ALT_GAME_KARMA_PLAYER_CAN_USE_GK) && player.getReputation() < 0)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_RIGHT_NOW);
			return ValueTask.CompletedTask;
		}

		// Cannot escape effect.
		if (player.isAffected(EffectFlags.CANNOT_ESCAPE))
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_RIGHT_NOW);
			return ValueTask.CompletedTask;
		}

		Location3D location = teleport.Location;
		if (!Config.Character.TELEPORT_WHILE_SIEGE_IN_PROGRESS)
		{
			Castle? castle = CastleManager.getInstance().getCastle(location);
			if (castle != null && castle.getSiege().isInProgress())
			{
				player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_TO_A_VILLAGE_THAT_IS_IN_A_SIEGE);
				return ValueTask.CompletedTask;
			}
		}

		int price;
		if (DateTime.UtcNow - player.getVariables().Get("LastFreeRaidTeleportTime", DateTime.MinValue) > TimeSpan.FromDays(1))
		{
			player.getVariables().Set("LastFreeRaidTeleportTime", DateTime.UtcNow);
			price = 0;
		}
		else
		{
			price = teleport.Price;
		}

		if (price > 0)
		{
			if (player.getInventory().getInventoryItemCount(Inventory.LCOIN_ID, -1) < price)
			{
				player.sendPacket(SystemMessageId.THERE_ARE_NOT_ENOUGH_L_COINS);
				return ValueTask.CompletedTask;
			}

			player.destroyItemByItemId("TeleportToRaid", Inventory.LCOIN_ID, price, player, true);
		}

		player.abortCast();
		player.stopMove(null);

		player.setTeleportLocation(new Location(location, 0));
		player.doCast(CommonSkill.TELEPORT.getSkill());
		player.sendPacket(new ExRaidTeleportInfoPacket(player));

		return ValueTask.CompletedTask;
    }
}