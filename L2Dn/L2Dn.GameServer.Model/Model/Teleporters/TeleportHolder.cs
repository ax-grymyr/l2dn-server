using System.Collections.Immutable;
using System.Text;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Npcs;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.DataPack;
using NLog;

namespace L2Dn.GameServer.Model.Teleporters;

public class TeleportHolder
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(TeleportHolder));

	private readonly string _name;
	private readonly TeleportType _type;
	private readonly ImmutableArray<TeleportLocation> _locations;

	/**
	 * Constructor
	 * @param name name of teleport list
	 * @param type type of teleport list
	 */
	public TeleportHolder(string name, TeleportType type, ImmutableArray<TeleportLocation> locations)
	{
		_name = name;
		_type = type;
		_locations = locations;
	}

	/**
	 * Gets list identification (name).
	 * @return list name
	 */
	public string getName()
	{
		return _name;
	}

	/**
	 * Check if teleport list is for noblesse or not.
	 * @return {@code true} if is for noblesse otherwise {@code false}
	 */
	public bool isNoblesse()
	{
		return (_type == TeleportType.NOBLES_ADENA) || (_type == TeleportType.NOBLES_TOKEN);
	}

	/**
	 * Gets type of teleport list.
	 * @return type of list
	 */
	public TeleportType getType()
	{
		return _type;
	}

	/**
	 * Gets teleport location with specific index.
	 * @param locationId index of location (begins with {@code 0})
	 * @return instance of {@link TeleportLocation} if found otherwise {@code null}
	 */
	public TeleportLocation getLocation(int locationId)
	{
		return _locations[locationId];
	}

	/**
	 * Gets all teleport locations registered in current holder.
	 * @return collection of {@link TeleportLocation}
	 */
	public ImmutableArray<TeleportLocation> getLocations()
	{
		return _locations;
	}

	/**
	 * Build HTML message from teleport list and send it to player.
	 * @param player receiver of HTML message
	 * @param npc teleporter
	 */
	public void showTeleportList(Player player, Npc npc)
	{
		showTeleportList(player, npc, "npc_" + npc.getObjectId() + "_teleport");
	}

	/**
	 * Build HTML message from teleport list and send it to player.
	 * @param player receiver of HTML message
	 * @param npc teleporter
	 * @param bypass bypass used while building message
	 */
	public void showTeleportList(Player player, Npc npc, string bypass)
	{
		if (isNoblesse() && !player.isNoble())
		{
			LOGGER.Warn(player + " requested noblesse teleport without being noble!");
			return;
		}

		// Load variables
		int questZoneId = isNormalTeleport() ? player.getQuestZoneId() : -1;

		// Build html
		StringBuilder sb = new StringBuilder();
		StringBuilder sbF = new StringBuilder();
		foreach (TeleportLocation loc in _locations)
		{
			string finalName = loc.getName();
			string confirmDesc = loc.getName();
			if (loc.getNpcStringId() != null)
			{
				NpcStringId stringId = loc.getNpcStringId().Value;
				finalName = "<fstring>" + stringId + "</fstring>";
				confirmDesc = "F;" + stringId;
			}

			if (shouldPayFee(player, loc))
			{
				long fee = calculateFee(player, loc);
				if (fee != 0)
				{
					finalName += " - " + fee + " " + getItemName(loc.getFeeId(), true);
				}
			}

			bool isQuestTeleport = (questZoneId >= 0) && (loc.getQuestZoneId() == questZoneId);
			if (isQuestTeleport)
			{
				sbF.Append("<button align=left icon=\"quest\" action=\"bypass -h " + bypass + " " + _name + " " +
				           loc.getId() + "\" msg=\"811;" + confirmDesc + "\">" + finalName + "</button>");
			}
			else
			{
				sb.Append("<button align=left icon=\"teleport\" action=\"bypass -h " + bypass + " " + _name + " " +
				          loc.getId() + "\" msg=\"811;" + confirmDesc + "\">" + finalName + "</button>");
			}
		}

		sbF.Append(sb.ToString());

		// Send html message
		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/teleporter/teleports.htm", player);
		htmlContent.Replace("%locations%", sbF.ToString());
		NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(npc.getObjectId(), 0, htmlContent);
		player.sendPacket(msg);
	}

	/**
	 * Teleports player to location
	 * @param player player being teleported
	 * @param npc teleporter
	 * @param locId destination
	 */
	public void doTeleport(Player player, Npc npc, int locId)
	{
		if (isNoblesse() && !player.isNoble())
		{
			LOGGER.Warn(player + " requested noblesse teleport without being noble!");
			return;
		}

		TeleportLocation loc = getLocation(locId);
		if (loc == null)
		{
			LOGGER.Warn(player + " requested unknown teleport location " + locId + " within list " + _name + "!");
			return;
		}

		// Check if castle is in siege
		if (!Config.TELEPORT_WHILE_SIEGE_IN_PROGRESS)
		{
			foreach (int castleId in loc.getCastleId())
			{
				if (CastleManager.getInstance().getCastleById(castleId).getSiege().isInProgress())
				{
					player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_TO_A_VILLAGE_THAT_IS_IN_A_SIEGE);
					return;
				}
			}
		}

		// Validate conditions for NORMAL teleport
		if (isNormalTeleport())
		{
			if (!Config.TELEPORT_WHILE_SIEGE_IN_PROGRESS && npc.getCastle().getSiege().isInProgress())
			{
				HtmlContent htmlContent = HtmlContent.LoadFromFile("html/teleporter/castleteleporter-busy.htm", player);
				NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(npc.getObjectId(), 0, htmlContent);
				player.sendPacket(msg);
				return;
			}
			else if (!Config.ALT_GAME_KARMA_PLAYER_CAN_USE_GK && (player.getReputation() < 0))
			{
				player.sendMessage("Go away, you're not welcome here.");
				return;
			}
			else if (player.isCombatFlagEquipped())
			{
				player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_WHILE_IN_POSSESSION_OF_A_WARD);
				return;
			}
		}

		// Notify listeners
		if (npc.Events.HasSubscribers<OnNpcTeleportRequest>())
		{
			OnNpcTeleportRequest onNpcTeleportRequest = new OnNpcTeleportRequest(player, npc, loc);
			if (npc.Events.Notify(onNpcTeleportRequest) && onNpcTeleportRequest.Terminate)
			{
				return;
			}
		}

		// Check rest of conditions
		if (shouldPayFee(player, loc) &&
		    !player.destroyItemByItemId("Teleport", loc.getFeeId(), calculateFee(player, loc), npc, true))
		{
			if (loc.getFeeId() == Inventory.ADENA_ID)
			{
				player.sendPacket(SystemMessageId.NOT_ENOUGH_ADENA);
			}
			else
			{
				player.sendMessage("You do not have enough " + getItemName(loc.getFeeId(), false));
			}
		}
		else if (!player.isAlikeDead())
		{
			player.teleToLocation(loc);
		}
	}

	/**
	 * Check if player have to play fee or not.
	 * @param player player which request teleport
	 * @param loc location where player should be teleported
	 * @return {@code true} when all requirements are met otherwise {@code false}
	 */
	private bool shouldPayFee(Player player, TeleportLocation loc)
	{
		return !isNormalTeleport() ||
		       (((player.getLevel() > Config.MAX_FREE_TELEPORT_LEVEL) || player.isSubClassActive()) &&
		        ((loc.getFeeId() != 0) && (loc.getFeeCount() > 0)));
	}

	/**
	 * Calculate fee amount for requested teleport.<br>
	 * For characters below level 77 teleport service is free.<br>
	 * From 8.00 pm to 00.00 from Monday till Tuesday for all characters there's a 50% discount on teleportation services
	 * @param player player which request teleport
	 * @param loc location where player should be teleported
	 * @return fee amount
	 */
	private long calculateFee(Player player, TeleportLocation loc)
	{
		if (isNormalTeleport())
		{
			if (!player.isSubClassActive() && (player.getLevel() <= Config.MAX_FREE_TELEPORT_LEVEL))
			{
				return 0;
			}

			DateTime cal = DateTime.UtcNow;
			int hour = cal.Hour;
			DayOfWeek dayOfWeek = cal.DayOfWeek;
			if ((hour >= 20) && ((dayOfWeek >= DayOfWeek.Monday) && (dayOfWeek <= DayOfWeek.Tuesday)))
			{
				return loc.getFeeCount() / 2;
			}
		}

		return loc.getFeeCount();
	}

	private bool isNormalTeleport()
	{
		return (_type == TeleportType.NORMAL) || (_type == TeleportType.HUNTING);
	}

	/**
	 * Gets name of specified item.
	 * @param itemId template id of item
	 * @param fstring prefer using client strings
	 * @return item name
	 */
	private string getItemName(int itemId, bool fstring)
	{
		if (fstring)
		{
			if (itemId == Inventory.ADENA_ID)
			{
				return "<fstring>1000308</fstring>";
			}
			else if (itemId == Inventory.ANCIENT_ADENA_ID)
			{
				return "<fstring>1000309</fstring>";
			}
		}

		ItemTemplate item = ItemData.getInstance().getTemplate(itemId);
		if (item != null)
		{
			return item.getName();
		}

		SpecialItemType specialItem = (SpecialItemType)itemId;
		if (specialItem != null)
		{
			switch (specialItem)
			{
				case SpecialItemType.PC_CAFE_POINTS:
				{
					return "Player Commendation Points";
				}
				case SpecialItemType.CLAN_REPUTATION:
				{
					return "Clan Reputation Points";
				}
				case SpecialItemType.FAME:
				{
					return "Fame";
				}
				case SpecialItemType.FIELD_CYCLE_POINTS:
				{
					return "Field Cycle Points";
				}
				case SpecialItemType.RAIDBOSS_POINTS:
				{
					return "Raid Points";
				}
				case SpecialItemType.HONOR_COINS:
				{
					return "Honor Points";
				}
			}
		}

		return "Unknown item: " + itemId;
	}
}