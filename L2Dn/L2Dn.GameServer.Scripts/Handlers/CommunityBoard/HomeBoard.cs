using L2Dn.Extensions;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.BuyList;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;
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
	private static readonly string NAVIGATION_PATH = "html/CommunityBoard/Custom/navigation.html";

	private static readonly string[] COMMANDS =
    [
        "_bbshome",
		"_bbstop",
    ];

	private static readonly string?[] CUSTOM_COMMANDS = // TODO: keep only non-null values
    [
        Config.PremiumSystem.PREMIUM_SYSTEM_ENABLED && Config.CommunityBoard.COMMUNITY_PREMIUM_SYSTEM_ENABLED ? "_bbspremium" : null,
		Config.CommunityBoard.COMMUNITYBOARD_ENABLE_MULTISELLS ? "_bbsexcmultisell" : null,
		Config.CommunityBoard.COMMUNITYBOARD_ENABLE_MULTISELLS ? "_bbsmultisell" : null,
		Config.CommunityBoard.COMMUNITYBOARD_ENABLE_MULTISELLS ? "_bbssell" : null,
		Config.CommunityBoard.COMMUNITYBOARD_ENABLE_TELEPORTS ? "_bbsteleport" : null,
		Config.CommunityBoard.COMMUNITYBOARD_ENABLE_BUFFS ? "_bbsbuff" : null,
		Config.CommunityBoard.COMMUNITYBOARD_ENABLE_HEAL ? "_bbsheal" : null,
		Config.CommunityBoard.COMMUNITYBOARD_ENABLE_DELEVEL ? "_bbsdelevel" : null,
    ];

	private static readonly Func<string, Player, bool> COMBAT_CHECK = (command, player) =>
	{
		bool commandCheck = false;
		foreach (string? c in CUSTOM_COMMANDS)
		{
			if (c != null && command.StartsWith(c))
			{
				commandCheck = true;
				break;
			}
		}
		return commandCheck && (player.isCastingNow() || player.isInCombat() || player.isInDuel() || player.isInOlympiadMode() || player.isInsideZone(ZoneId.SIEGE) || player.isInsideZone(ZoneId.PVP) || player.getPvpFlag() > 0 || player.isAlikeDead() || player.isOnEvent());
	};

	private static readonly Predicate<Player> KARMA_CHECK = player => Config.CommunityBoard.COMMUNITYBOARD_KARMA_DISABLED && player.getReputation() < 0;

	public string[] getCommunityBoardCommands()
	{
		List<string?> commands = [];
		commands.AddRange(COMMANDS);
		commands.AddRange(CUSTOM_COMMANDS);
        return commands.Where(x => x != null).ToArray()!;
    }

	public bool parseCommunityBoardCommand(string command, Player player)
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

		string? returnHtml = null;
		string navigation = HtmCache.getInstance().getHtm(NAVIGATION_PATH, player.getLang());
		if (command.equals("_bbshome") || command.equals("_bbstop"))
		{
			string customPath = Config.CommunityBoard.CUSTOM_CB_ENABLED ? "Custom/" : "";
			CommunityBoardHandler.getInstance().addBypass(player, "Home", command);
			returnHtml = HtmCache.getInstance().getHtm("html/CommunityBoard/" + customPath + "home.html", player.getLang());
			if (!Config.CommunityBoard.CUSTOM_CB_ENABLED)
			{
				returnHtml = returnHtml.Replace("%fav_count%", getFavoriteCount(player).ToString());
				returnHtml = returnHtml.Replace("%region_count%", getRegionCount(player).ToString());
				returnHtml = returnHtml.Replace("%clan_count%", ClanTable.getInstance().getClanCount().ToString());
			}
		}
		else if (command.startsWith("_bbstop;"))
		{
			string customPath = Config.CommunityBoard.CUSTOM_CB_ENABLED ? "Custom/" : "";
			string path = command.Replace("_bbstop;", "");
			if (path.Length > 0 && path.endsWith(".html"))
			{
				returnHtml = HtmCache.getInstance().getHtm("html/CommunityBoard/" + customPath + path, player.getLang());
			}
		}
		else if (command.startsWith("_bbsmultisell"))
		{
			string fullBypass = command.Replace("_bbsmultisell;", "");
			string[] buypassOptions = fullBypass.Split(",");
			int multisellId = int.Parse(buypassOptions[0]);
			string page = buypassOptions[1];
			returnHtml = HtmCache.getInstance().getHtm("html/CommunityBoard/Custom/" + page + ".html", player.getLang());
			MultisellData.getInstance().separateAndSend(multisellId, player, null, false);
		}
		else if (command.startsWith("_bbsexcmultisell"))
		{
			string fullBypass = command.Replace("_bbsexcmultisell;", "");
			string[] buypassOptions = fullBypass.Split(",");
			int multisellId = int.Parse(buypassOptions[0]);
			string page = buypassOptions[1];
			returnHtml = HtmCache.getInstance().getHtm( "html/CommunityBoard/Custom/" + page + ".html", player.getLang());
			MultisellData.getInstance().separateAndSend(multisellId, player, null, true);
		}
		else if (command.startsWith("_bbssell"))
		{
			string page = command.Replace("_bbssell;", "");
			returnHtml = HtmCache.getInstance().getHtm("html/CommunityBoard/Custom/" + page + ".html", player.getLang());
            ProductList? buyList = BuyListData.getInstance().getBuyList(423);
            if (buyList == null)
            {
                _logger.Error("BuyList 423 not found!");
                return false;
            }

			player.sendPacket(new ExBuySellListPacket(buyList, player, 0));
			player.sendPacket(new ExBuySellListPacket(player, false));
		}
		else if (command.startsWith("_bbsteleport"))
		{
			string teleBuypass = command.Replace("_bbsteleport;", "");
			if (player.getInventory().getInventoryItemCount(Config.CommunityBoard.COMMUNITYBOARD_CURRENCY, -1) < Config.CommunityBoard.COMMUNITYBOARD_TELEPORT_PRICE)
			{
				player.sendMessage("Not enough currency!");
			}
			else if (Config.CommunityBoard.COMMUNITY_AVAILABLE_TELEPORTS.TryGetValue(teleBuypass, out Location value))
			{
				player.disableAllSkills();
				player.sendPacket(new ShowBoardPacket(false, string.Empty));
				player.destroyItemByItemId("CB_Teleport", Config.CommunityBoard.COMMUNITYBOARD_CURRENCY, Config.CommunityBoard.COMMUNITYBOARD_TELEPORT_PRICE, player, true);
				player.setInstanceById(0);
				player.teleToLocation(value, 0);
				ThreadPool.schedule(player.enableAllSkills, 3000);
			}
		}
		else if (command.startsWith("_bbsbuff"))
		{
			string fullBypass = command.Replace("_bbsbuff;", "");
			string[] buypassOptions = fullBypass.Split(";");
			int buffCount = buypassOptions.Length - 1;
			string page = buypassOptions[buffCount];
			if (player.getInventory().getInventoryItemCount(Config.CommunityBoard.COMMUNITYBOARD_CURRENCY, -1) < Config.CommunityBoard.COMMUNITYBOARD_BUFF_PRICE * buffCount)
			{
				player.sendMessage("Not enough currency!");
			}
			else
			{
				player.destroyItemByItemId("CB_Buff", Config.CommunityBoard.COMMUNITYBOARD_CURRENCY, Config.CommunityBoard.COMMUNITYBOARD_BUFF_PRICE * buffCount, player, true);
				Pet? pet = player.getPet();
				List<Creature> targets = new(4);
				targets.Add(player);
				if (pet != null)
				{
					targets.Add(pet);
				}

				player.getServitors().Values.ForEach(x => targets.Add(x));

				for (int i = 0; i < buffCount; i++)
				{
					Skill? skill = SkillData.Instance.GetSkill(int.Parse(buypassOptions[i].Split(",")[0]), int.Parse(buypassOptions[i].Split(",")[1]));
					if (skill == null || !Config.CommunityBoard.COMMUNITY_AVAILABLE_BUFFS.Contains(skill.Id))
					{
						continue;
					}
					foreach (Creature target in targets)
					{
						if (skill.IsSharedWithSummon || target.isPlayer())
						{
							skill.ApplyEffects(player, target);
							if (Config.CommunityBoard.COMMUNITYBOARD_CAST_ANIMATIONS)
							{
								player.sendPacket(new MagicSkillUsePacket(player, target, skill.Id, skill.Level, skill.HitTime, skill.ReuseDelay));
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
			string page = command.Replace("_bbsheal;", "");
			if (player.getInventory().getInventoryItemCount(Config.CommunityBoard.COMMUNITYBOARD_CURRENCY, -1) < Config.CommunityBoard.COMMUNITYBOARD_HEAL_PRICE)
			{
				player.sendMessage("Not enough currency!");
			}
			else
			{
				player.destroyItemByItemId("CB_Heal", Config.CommunityBoard.COMMUNITYBOARD_CURRENCY, Config.CommunityBoard.COMMUNITYBOARD_HEAL_PRICE, player, true);
				player.setCurrentHp(player.getMaxHp());
				player.setCurrentMp(player.getMaxMp());
				player.setCurrentCp(player.getMaxCp());
                Pet? pet = player.getPet();
				if (player.hasPet() && pet != null)
				{
                    pet.setCurrentHp(pet.getMaxHp());
                    pet.setCurrentMp(pet.getMaxMp());
                    pet.setCurrentCp(pet.getMaxCp());
				}
				foreach (Summon summon in player.getServitors().Values)
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
			if (player.getInventory().getInventoryItemCount(Config.CommunityBoard.COMMUNITYBOARD_CURRENCY, -1) < Config.CommunityBoard.COMMUNITYBOARD_DELEVEL_PRICE)
			{
				player.sendMessage("Not enough currency!");
			}
			else if (player.getLevel() == 1)
			{
				player.sendMessage("You are at minimum level!");
			}
			else
			{
				player.destroyItemByItemId("CB_Delevel", Config.CommunityBoard.COMMUNITYBOARD_CURRENCY, Config.CommunityBoard.COMMUNITYBOARD_DELEVEL_PRICE, player, true);
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
			string fullBypass = command.Replace("_bbspremium;", "");
			string[] buypassOptions = fullBypass.Split(",");
			int premiumDays = int.Parse(buypassOptions[0]);
			if (premiumDays < 1 || premiumDays > 30 || player.getInventory().getInventoryItemCount(Config.CommunityBoard.COMMUNITY_PREMIUM_COIN_ID, -1) < Config.CommunityBoard.COMMUNITY_PREMIUM_PRICE_PER_DAY * premiumDays)
			{
				player.sendMessage("Not enough currency!");
			}
			else
			{
				player.destroyItemByItemId("CB_Premium", Config.CommunityBoard.COMMUNITY_PREMIUM_COIN_ID, Config.CommunityBoard.COMMUNITY_PREMIUM_PRICE_PER_DAY * premiumDays, player, true);
				PremiumManager.getInstance().addPremiumTime(player.getAccountId(), TimeSpan.FromDays(premiumDays));
				player.sendMessage("Your account will now have premium status until " + PremiumManager.getInstance().getPremiumExpiration(player.getAccountId())?.ToString("dd.MM.yyyy HH:mm") + ".");
				if (Config.PremiumSystem.PC_CAFE_RETAIL_LIKE)
				{
					PcCafePointsManager.getInstance().run(player);
				}
				returnHtml = HtmCache.getInstance().getHtm("html/CommunityBoard/Custom/premium/thankyou.html", player.getLang());
			}
		}

		if (returnHtml != null)
		{
			if (Config.CommunityBoard.CUSTOM_CB_ENABLED)
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
            int playerId = player.ObjectId;
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            count = ctx.BbsFavorites.Count(r => r.PlayerId == playerId);
		}
		catch (Exception e)
		{
			_logger.Warn(nameof(FavoriteBoard) + ": Coudn't load favorites count for " + player + ": " + e);
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