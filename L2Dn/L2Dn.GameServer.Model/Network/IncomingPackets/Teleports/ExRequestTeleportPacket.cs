using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Teleports;

public struct ExRequestTeleportPacket: IIncomingPacket<GameSession>
{
    private int _teleportId;

    public void ReadContent(PacketBitReader reader)
    {
        _teleportId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;
		
		TeleportListHolder teleport = TeleportListData.getInstance().getTeleport(_teleportId);
		if (teleport == null)
		{
			PacketLogger.Instance.Warn("No registered teleport location for id: " + _teleportId);
			return ValueTask.CompletedTask;
		}
		
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

		// Teleport in combat configuration.
		if (!Config.TELEPORT_WHILE_PLAYER_IN_COMBAT && (player.isInCombat() || player.isCastingNow()))
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_WHILE_IN_COMBAT);
			return ValueTask.CompletedTask;
		}
		
		// Karma related configurations.
		if ((!Config.ALT_GAME_KARMA_PLAYER_CAN_TELEPORT || !Config.ALT_GAME_KARMA_PLAYER_CAN_USE_GK) && (player.getReputation() < 0))
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_RIGHT_NOW);
			return ValueTask.CompletedTask;
		}
		
		// Cannot escape effect.
		if (player.cannotEscape())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_RIGHT_NOW);
			return ValueTask.CompletedTask;
		}
		
		Location3D location = teleport.getLocation();
		if (!Config.TELEPORT_WHILE_SIEGE_IN_PROGRESS)
		{
			Castle castle = CastleManager.getInstance().getCastle(location.X, location.Y, location.Z);
			if ((castle != null) && castle.getSiege().isInProgress())
			{
				player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_TO_A_VILLAGE_THAT_IS_IN_A_SIEGE);
				return ValueTask.CompletedTask;
			}
		}
		
		if (player.getLevel() > Config.MAX_FREE_TELEPORT_LEVEL)
		{
			int price = teleport.getPrice();
			if (price > 0)
			{
				// Check if player has fee.
				if (teleport.isSpecial())
				{
					if (player.getInventory().getInventoryItemCount(Inventory.LCOIN_ID, -1) < price)
					{
						player.sendPacket(SystemMessageId.THERE_ARE_NOT_ENOUGH_L_COINS);
						return ValueTask.CompletedTask;
					}
				}
				else if (player.getAdena() < price)
				{
					player.sendPacket(SystemMessageId.NOT_ENOUGH_ADENA);
					return ValueTask.CompletedTask;
				}

				// Reduce items.
				if (teleport.isSpecial())
				{
					player.destroyItemByItemId("Teleport", Inventory.LCOIN_ID, price, player, true);
				}
				else
				{
					player.reduceAdena("Teleport", price, player, true);
				}
			}
		}
		
		player.abortCast();
		player.stopMove(null);
		
		player.setTeleportLocation(new Location(location, 0));
		player.doCast(CommonSkill.TELEPORT.getSkill());

		return ValueTask.CompletedTask;
    }
}