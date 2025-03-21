using System.Text;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Npcs;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Geometry;
using NLog;

namespace L2Dn.GameServer.Model.Teleporters;

public static class TeleportHolderExtensions
{
    private static readonly Logger LOGGER = LogManager.GetLogger(nameof(TeleportHolder));

    /**
	 * Build HTML message from teleport list and send it to player.
	 * @param player receiver of HTML message
	 * @param npc teleporter
	 */
    public static void showTeleportList(this TeleportHolder teleportHolder, Player player, Npc npc)
    {
        teleportHolder.showTeleportList(player, npc, "npc_" + npc.ObjectId + "_teleport");
    }

    /**
	 * Build HTML message from teleport list and send it to player.
	 * @param player receiver of HTML message
	 * @param npc teleporter
	 * @param bypass bypass used while building message
	 */
    public static void showTeleportList(this TeleportHolder teleportHolder, Player player, Npc npc, string bypass)
    {
        if (teleportHolder.IsNoblesse && !player.isNoble())
        {
            LOGGER.Warn(player + " requested noblesse teleport without being noble!");
            return;
        }

        // Load variables
        int questZoneId = teleportHolder.IsNormalTeleport ? player.getQuestZoneId() : -1;

        // Build html
        StringBuilder sb = new StringBuilder();
        StringBuilder sbF = new StringBuilder();
        foreach (TeleportLocation loc in teleportHolder.Locations)
        {
            string finalName = loc.Name;
            string confirmDesc = loc.Name;
            NpcStringId? npcString = loc.NpcStringId;
            if (npcString != null)
            {
                NpcStringId stringId = npcString.Value;
                finalName = "<fstring>" + stringId + "</fstring>";
                confirmDesc = "F;" + stringId;
            }

            if (teleportHolder.shouldPayFee(player, loc))
            {
                long fee = teleportHolder.calculateFee(player, loc);
                if (fee != 0)
                {
                    finalName += " - " + fee + " " + getItemName(loc.FeeId, true);
                }
            }

            bool isQuestTeleport = questZoneId >= 0 && loc.QuestZoneId == questZoneId;
            if (isQuestTeleport)
            {
                sbF.Append("<button align=left icon=\"quest\" action=\"bypass -h " + bypass + " " + teleportHolder.Name + " " +
                    loc.Id + "\" msg=\"811;" + confirmDesc + "\">" + finalName + "</button>");
            }
            else
            {
                sb.Append("<button align=left icon=\"teleport\" action=\"bypass -h " + bypass + " " + teleportHolder.Name + " " +
                    loc.Id + "\" msg=\"811;" + confirmDesc + "\">" + finalName + "</button>");
            }
        }

        sbF.Append(sb.ToString());

        // Send html message
        HtmlContent htmlContent = HtmlContent.LoadFromFile("html/teleporter/teleports.htm", player);
        htmlContent.Replace("%locations%", sbF.ToString());
        NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(npc.ObjectId, 0, htmlContent);
        player.sendPacket(msg);
    }

    /**
	 * Teleports player to location
	 * @param player player being teleported
	 * @param npc teleporter
	 * @param locId destination
	 */
    public static void doTeleport(this TeleportHolder teleportHolder, Player player, Npc npc, int locId)
    {
        if (teleportHolder.IsNoblesse && !player.isNoble())
        {
            LOGGER.Warn(player + " requested noblesse teleport without being noble!");
            return;
        }

        TeleportLocation? loc = null;
        if (locId >= 0 && locId < teleportHolder.Locations.Length)
            loc = teleportHolder.Locations[locId];
        
        if (loc == null)
        {
            LOGGER.Warn(player + " requested unknown teleport location " + locId + " within list " + teleportHolder.Name + "!");
            return;
        }

        // Check if castle is in siege
        if (!Config.Character.TELEPORT_WHILE_SIEGE_IN_PROGRESS)
        {
            foreach (int castleId in loc.CastleIds)
            {
                Castle? castle = CastleManager.getInstance().getCastleById(castleId);
                if (castle != null && castle.getSiege().isInProgress())
                {
                    player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_TO_A_VILLAGE_THAT_IS_IN_A_SIEGE);
                    return;
                }
            }
        }

        // Validate conditions for NORMAL teleport
        if (teleportHolder.IsNormalTeleport)
        {
            Castle? castle = npc.getCastle();
            if (!Config.Character.TELEPORT_WHILE_SIEGE_IN_PROGRESS && castle != null && castle.getSiege().isInProgress())
            {
                HtmlContent htmlContent = HtmlContent.LoadFromFile("html/teleporter/castleteleporter-busy.htm", player);
                NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(npc.ObjectId, 0, htmlContent);
                player.sendPacket(msg);
                return;
            }

            if (!Config.Character.ALT_GAME_KARMA_PLAYER_CAN_USE_GK && player.getReputation() < 0)
            {
                player.sendMessage("Go away, you're not welcome here.");
                return;
            }

            if (player.isCombatFlagEquipped())
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
        if (teleportHolder.shouldPayFee(player, loc) &&
            !player.destroyItemByItemId("Teleport", loc.FeeId, teleportHolder.calculateFee(player, loc), npc, true))
        {
            if (loc.FeeId == Inventory.AdenaId)
            {
                player.sendPacket(SystemMessageId.NOT_ENOUGH_ADENA);
            }
            else
            {
                player.sendMessage("You do not have enough " + getItemName(loc.FeeId, false));
            }
        }
        else if (!player.isAlikeDead())
        {
            player.teleToLocation(new Location(loc.Location, 0));
        }
    }

    /**
	 * Check if player have to play fee or not.
	 * @param player player which request teleport
	 * @param loc location where player should be teleported
	 * @return {@code true} when all requirements are met otherwise {@code false}
	 */
    private static bool shouldPayFee(this TeleportHolder teleportHolder, Player player, TeleportLocation loc)
    {
        return !teleportHolder.IsNormalTeleport ||
            ((player.getLevel() > Config.Character.MAX_FREE_TELEPORT_LEVEL || player.isSubClassActive()) &&
                loc.FeeId != 0 && loc.FeeCount > 0);
    }

    /**
	 * Calculate fee amount for requested teleport.<br>
	 * For characters below level 77 teleport service is free.<br>
	 * From 8.00 pm to 00.00 from Monday till Tuesday for all characters there's a 50% discount on teleportation services
	 * @param player player which request teleport
	 * @param loc location where player should be teleported
	 * @return fee amount
	 */
    private static long calculateFee(this TeleportHolder teleportHolder, Player player, TeleportLocation loc)
    {
        if (teleportHolder.IsNormalTeleport)
        {
            if (!player.isSubClassActive() && player.getLevel() <= Config.Character.MAX_FREE_TELEPORT_LEVEL)
            {
                return 0;
            }

            DateTime cal = DateTime.UtcNow;
            int hour = cal.Hour;
            DayOfWeek dayOfWeek = cal.DayOfWeek;
            if (hour >= 20 && dayOfWeek >= DayOfWeek.Monday && dayOfWeek <= DayOfWeek.Tuesday)
            {
                return loc.FeeCount / 2;
            }
        }

        return loc.FeeCount;
    }

    /**
     * Gets name of specified item.
     * @param itemId template id of item
     * @param fstring prefer using client strings
     * @return item name
     */
    private static string getItemName(int itemId, bool fstring)
    {
        if (fstring)
        {
            if (itemId == Inventory.AdenaId)
            {
                return "<fstring>1000308</fstring>";
            }

            if (itemId == Inventory.ANCIENT_ADENA_ID)
            {
                return "<fstring>1000309</fstring>";
            }
        }

        ItemTemplate? item = ItemData.getInstance().getTemplate(itemId);
        if (item != null)
        {
            return item.getName();
        }

        SpecialItemType specialItem = (SpecialItemType)itemId;
        if (specialItem != 0)
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