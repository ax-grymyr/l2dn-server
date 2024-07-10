using L2Dn.Extensions;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using NLog;
using Pet = L2Dn.GameServer.Model.Actor.Instances.Pet;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Scripts.Handlers.CommunityBoard;

/**
 * Home board.
 * @author Zoey76, Mobius
 */
public class HomeBoard: IParseBoardHandler
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(HomeBoard));
	private static readonly String NAVIGATION_PATH = "html/CommunityBoard/Custom/navigation.html";
	
	private static readonly String[] COMMANDS =
	{
		"_bbshome",
		"_bbstop",
	};
	
	private static readonly String[] CUSTOM_COMMANDS =
	{
		Config.PREMIUM_SYSTEM_ENABLED && Config.COMMUNITY_PREMIUM_SYSTEM_ENABLED ? "_bbspremium" : null,
		Config.COMMUNITYBOARD_ENABLE_MULTISELLS ? "_bbsexcmultisell" : null,
		Config.COMMUNITYBOARD_ENABLE_MULTISELLS ? "_bbsmultisell" : null,
		Config.COMMUNITYBOARD_ENABLE_MULTISELLS ? "_bbssell" : null,
		Config.COMMUNITYBOARD_ENABLE_TELEPORTS ? "_bbsteleport" : null,
		Config.COMMUNITYBOARD_ENABLE_BUFFS ? "_bbsbuff" : null,
		Config.COMMUNITYBOARD_ENABLE_HEAL ? "_bbsheal" : null,
		Config.COMMUNITYBOARD_ENABLE_DELEVEL ? "_bbsdelevel" : null
	};
	
	private static readonly Func<String, Player, bool> COMBAT_CHECK = (command, player) =>
	{
		bool commandCheck = false;
		foreach (String c in CUSTOM_COMMANDS)
		{
			if ((c != null) && command.StartsWith(c))
			{
				commandCheck = true;
				break;
			}
		}
		return commandCheck && (player.isCastingNow() || player.isInCombat() || player.isInDuel() || player.isInOlympiadMode() || player.isInsideZone(ZoneId.SIEGE) || player.isInsideZone(ZoneId.PVP) || (player.getPvpFlag() > 0) || player.isAlikeDead() || player.isOnEvent());
	};
	
	private static readonly Predicate<Player> KARMA_CHECK = player => Config.COMMUNITYBOARD_KARMA_DISABLED && (player.getReputation() < 0);
	
	public String[] getCommunityBoardCommands()
	{
		List<String> commands = new();
		commands.AddRange(COMMANDS);
		commands.AddRange(CUSTOM_COMMANDS);
        return commands.Where(x => x != null).ToArray();
    }
	
	public bool parseCommunityBoardCommand(String command, Player player)
	{
		// Old custom conditions check move to here
		if (COMBAT_CHECK(command, player))
		{
			player.sendMessage("You can't use the Community Board right now.");
			return false;
		}
		
		if (KARMA_CHECK(player))
		{
			player.sendMessage("Players with Karma cannot use the Community Board.");
			return false;
		}
		
		String returnHtml = null;
		String navigation = HtmCache.getInstance().getHtm(NAVIGATION_PATH, player.getLang());
		if (command.equals("_bbshome") || command.equals("_bbstop"))
		{
			String customPath = Config.CUSTOM_CB_ENABLED ? "Custom/" : "";
			CommunityBoardHandler.getInstance().addBypass(player, "Home", command);
			returnHtml = HtmCache.getInstance().getHtm("html/CommunityBoard/" + customPath + "home.html", player.getLang());
			if (!Config.CUSTOM_CB_ENABLED)
			{
				returnHtml = returnHtml.Replace("%fav_count%", getFavoriteCount(player).ToString());
				returnHtml = returnHtml.Replace("%region_count%", getRegionCount(player).ToString());
				returnHtml = returnHtml.Replace("%clan_count%", ClanTable.getInstance().getClanCount().ToString());
			}
		}
		else if (command.startsWith("_bbstop;"))
		{
			String customPath = Config.CUSTOM_CB_ENABLED ? "Custom/" : "";
			String path = command.Replace("_bbstop;", "");
			if ((path.Length > 0) && path.endsWith(".html"))
			{
				returnHtml = HtmCache.getInstance().getHtm("html/CommunityBoard/" + customPath + path, player.getLang());
			}
		}
		else if (command.startsWith("_bbsmultisell"))
		{
			String fullBypass = command.Replace("_bbsmultisell;", "");
			String[] buypassOptions = fullBypass.Split(",");
			int multisellId = int.Parse(buypassOptions[0]);
			String page = buypassOptions[1];
			returnHtml = HtmCache.getInstance().getHtm("html/CommunityBoard/Custom/" + page + ".html", player.getLang());
			MultisellData.getInstance().separateAndSend(multisellId, player, null, false);
		}
		else if (command.startsWith("_bbsexcmultisell"))
		{
			String fullBypass = command.Replace("_bbsexcmultisell;", "");
			String[] buypassOptions = fullBypass.Split(",");
			int multisellId = int.Parse(buypassOptions[0]);
			String page = buypassOptions[1];
			returnHtml = HtmCache.getInstance().getHtm( "html/CommunityBoard/Custom/" + page + ".html", player.getLang());
			MultisellData.getInstance().separateAndSend(multisellId, player, null, true);
		}
		else if (command.startsWith("_bbssell"))
		{
			String page = command.Replace("_bbssell;", "");
			returnHtml = HtmCache.getInstance().getHtm("html/CommunityBoard/Custom/" + page + ".html", player.getLang());
			player.sendPacket(new ExBuySellListPacket(BuyListData.getInstance().getBuyList(423), player, 0));
			player.sendPacket(new ExBuySellListPacket(player, false));
		}
		else if (command.startsWith("_bbsteleport"))
		{
			String teleBuypass = command.Replace("_bbsteleport;", "");
			if (player.getInventory().getInventoryItemCount(Config.COMMUNITYBOARD_CURRENCY, -1) < Config.COMMUNITYBOARD_TELEPORT_PRICE)
			{
				player.sendMessage("Not enough currency!");
			}
			else if (Config.COMMUNITY_AVAILABLE_TELEPORTS.TryGetValue(teleBuypass, out Location value))
			{
				player.disableAllSkills();
				player.sendPacket(new ShowBoardPacket(false, string.Empty));
				player.destroyItemByItemId("CB_Teleport", Config.COMMUNITYBOARD_CURRENCY, Config.COMMUNITYBOARD_TELEPORT_PRICE, player, true);
				player.setInstanceById(0);
				player.teleToLocation(value, 0);
				ThreadPool.schedule(player.enableAllSkills, 3000);
			}
		}
		else if (command.startsWith("_bbsbuff"))
		{
			String fullBypass = command.Replace("_bbsbuff;", "");
			String[] buypassOptions = fullBypass.Split(";");
			int buffCount = buypassOptions.Length - 1;
			String page = buypassOptions[buffCount];
			if (player.getInventory().getInventoryItemCount(Config.COMMUNITYBOARD_CURRENCY, -1) < (Config.COMMUNITYBOARD_BUFF_PRICE * buffCount))
			{
				player.sendMessage("Not enough currency!");
			}
			else
			{
				player.destroyItemByItemId("CB_Buff", Config.COMMUNITYBOARD_CURRENCY, Config.COMMUNITYBOARD_BUFF_PRICE * buffCount, player, true);
				Pet pet = player.getPet();
				List<Creature> targets = new(4);
				targets.Add(player);
				if (pet != null)
				{
					targets.Add(pet);
				}
				
				player.getServitors().Values.ForEach(x => targets.Add(x));
				
				for (int i = 0; i < buffCount; i++)
				{
					Skill skill = SkillData.getInstance().getSkill(int.Parse(buypassOptions[i].Split(",")[0]), int.Parse(buypassOptions[i].Split(",")[1]));
					if (!Config.COMMUNITY_AVAILABLE_BUFFS.Contains(skill.getId()))
					{
						continue;
					}
					foreach (Creature target in targets)
					{
						if (skill.isSharedWithSummon() || target.isPlayer())
						{
							skill.applyEffects(player, target);
							if (Config.COMMUNITYBOARD_CAST_ANIMATIONS)
							{
								player.sendPacket(new MagicSkillUsePacket(player, target, skill.getId(), skill.getLevel(), skill.getHitTime(), skill.getReuseDelay()));
								// not recommend broadcast
								// player.broadcastPacket(new MagicSkillUse(player, target, skill.getId(), skill.getLevel(), skill.getHitTime(), skill.getReuseDelay()));
							}
						}
					}
				}
			}
			
			returnHtml = HtmCache.getInstance().getHtm("html/CommunityBoard/Custom/" + page + ".html", player.getLang());
		}
		else if (command.startsWith("_bbsheal"))
		{
			String page = command.Replace("_bbsheal;", "");
			if (player.getInventory().getInventoryItemCount(Config.COMMUNITYBOARD_CURRENCY, -1) < (Config.COMMUNITYBOARD_HEAL_PRICE))
			{
				player.sendMessage("Not enough currency!");
			}
			else
			{
				player.destroyItemByItemId("CB_Heal", Config.COMMUNITYBOARD_CURRENCY, Config.COMMUNITYBOARD_HEAL_PRICE, player, true);
				player.setCurrentHp(player.getMaxHp());
				player.setCurrentMp(player.getMaxMp());
				player.setCurrentCp(player.getMaxCp());
				if (player.hasPet())
				{
					player.getPet().setCurrentHp(player.getPet().getMaxHp());
					player.getPet().setCurrentMp(player.getPet().getMaxMp());
					player.getPet().setCurrentCp(player.getPet().getMaxCp());
				}
				foreach (Summon summon in player.getServitors().values())
				{
					summon.setCurrentHp(summon.getMaxHp());
					summon.setCurrentMp(summon.getMaxMp());
					summon.setCurrentCp(summon.getMaxCp());
				}
				player.sendMessage("You used heal!");
			}
			
			returnHtml = HtmCache.getInstance().getHtm("html/CommunityBoard/Custom/" + page + ".html", player.getLang());
		}
		else if (command.equals("_bbsdelevel"))
		{
			if (player.getInventory().getInventoryItemCount(Config.COMMUNITYBOARD_CURRENCY, -1) < Config.COMMUNITYBOARD_DELEVEL_PRICE)
			{
				player.sendMessage("Not enough currency!");
			}
			else if (player.getLevel() == 1)
			{
				player.sendMessage("You are at minimum level!");
			}
			else
			{
				player.destroyItemByItemId("CB_Delevel", Config.COMMUNITYBOARD_CURRENCY, Config.COMMUNITYBOARD_DELEVEL_PRICE, player, true);
				int newLevel = player.getLevel() - 1;
				player.setExp(ExperienceData.getInstance().getExpForLevel(newLevel));
				player.getStat().setLevel(newLevel);
				player.setCurrentHpMp(player.getMaxHp(), player.getMaxMp());
				player.setCurrentCp(player.getMaxCp());
				player.broadcastUserInfo();
				player.checkPlayerSkills(); // Adjust skills according to new level.
				returnHtml = HtmCache.getInstance().getHtm("html/CommunityBoard/Custom/delevel/complete.html", player.getLang());
			}
		}
		else if (command.startsWith("_bbspremium"))
		{
			String fullBypass = command.Replace("_bbspremium;", "");
			String[] buypassOptions = fullBypass.Split(",");
			int premiumDays = int.Parse(buypassOptions[0]);
			if ((premiumDays < 1) || (premiumDays > 30) || (player.getInventory().getInventoryItemCount(Config.COMMUNITY_PREMIUM_COIN_ID, -1) < (Config.COMMUNITY_PREMIUM_PRICE_PER_DAY * premiumDays)))
			{
				player.sendMessage("Not enough currency!");
			}
			else
			{
				player.destroyItemByItemId("CB_Premium", Config.COMMUNITY_PREMIUM_COIN_ID, Config.COMMUNITY_PREMIUM_PRICE_PER_DAY * premiumDays, player, true);
				PremiumManager.getInstance().addPremiumTime(player.getAccountId(), TimeSpan.FromDays(premiumDays));
				player.sendMessage("Your account will now have premium status until " + PremiumManager.getInstance().getPremiumExpiration(player.getAccountId())?.ToString("dd.MM.yyyy HH:mm") + ".");
				if (Config.PC_CAFE_RETAIL_LIKE)
				{
					PcCafePointsManager.getInstance().run(player);
				}
				returnHtml = HtmCache.getInstance().getHtm("html/CommunityBoard/Custom/premium/thankyou.html", player.getLang());
			}
		}
		
		if (returnHtml != null)
		{
			if (Config.CUSTOM_CB_ENABLED)
			{
				returnHtml = returnHtml.Replace("%navigation%", navigation);
			}
			CommunityBoardHandler.separateAndSend(returnHtml, player);
		}
		return false;
	}
	
	/**
	 * Gets the Favorite links for the given player.
	 * @param player the player
	 * @return the favorite links count
	 */
	private static int getFavoriteCount(Player player)
	{
		int count = 0;
		try
        {
            int playerId = player.getObjectId();
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            count = ctx.BbsFavorites.Count(r => r.PlayerId == playerId);
		}
		catch (Exception e)
		{
			_logger.Warn(nameof(FavoriteBoard) + ": Coudn't load favorites count for " + player);
		}
        
		return count;
	}
	
	/**
	 * Gets the registered regions count for the given player.
	 * @param player the player
	 * @return the registered regions count
	 */
	private static int getRegionCount(Player player)
	{
		return 0; // TODO: Implement.
	}
}
