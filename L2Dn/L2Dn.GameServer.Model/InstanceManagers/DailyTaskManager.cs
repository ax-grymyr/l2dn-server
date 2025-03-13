using L2Dn.Extensions;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Stats;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Model.PrimeShop;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Model.Vips;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;
using Config = L2Dn.GameServer.Configuration.Config;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author UnAfraid
 */
public class DailyTaskManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(DailyTaskManager));
	private static readonly Set<int> RESET_SKILLS = new();
	public static readonly Set<int> RESET_ITEMS = new();

	static DailyTaskManager()
	{
		RESET_SKILLS.add(39199); // Hero's Wondrous Cubic
		RESET_ITEMS.add(49782); // Balthus Knights' Supply Box
	}

	protected DailyTaskManager()
	{
		// Schedule reset every day at 6:30.
		DateTime currentTime = DateTime.Now;
		DateTime calendar = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 6, 30, 0, DateTimeKind.Local);
		if (calendar < currentTime)
			calendar = calendar.AddDays(1);

		// Check if 24 hours have passed since the last daily reset.
        DateTime resetTime = GlobalVariablesManager.getInstance().Get<DateTime>(GlobalVariablesManager.DAILY_TASK_RESET);
		if (resetTime < calendar)
		{
			LOGGER.Info(GetType().Name +": Next schedule at " + calendar.ToString("dd/MM HH:mm") + ".");
		}
		else
		{
			LOGGER.Info(GetType().Name +": Daily task will run now.");
			onReset();
		}

		// Daily reset task.
		TimeSpan startDelay = calendar - currentTime;
		if (startDelay < TimeSpan.Zero)
			startDelay = TimeSpan.Zero;

		ThreadPool.scheduleAtFixedRate(onReset, startDelay, TimeSpan.FromDays(1));

		// Global save task.
		ThreadPool.scheduleAtFixedRate(onSave, 1800000, 1800000); // 1800000 = 30 minutes
	}

	private void onReset()
	{
		// Store last reset time.
		GlobalVariablesManager.getInstance().Set(GlobalVariablesManager.DAILY_TASK_RESET, DateTime.Now);

		// Wednesday weekly tasks.
		DateTime calendar = DateTime.Now;
		if (calendar.DayOfWeek == DayOfWeek.Wednesday)
		{
			clanLeaderApply();
			resetMonsterArenaWeekly();
			resetTimedHuntingZonesWeekly();
			resetVitalityWeekly();
			resetPrivateStoreHistory();
		}
		else // All days, except Wednesday.
		{
			resetVitalityDaily();
		}

		if (Config.ENABLE_HUNT_PASS && calendar.Day == Config.HUNT_PASS_PERIOD)
		{
			resetHuntPass();
		}

		if (calendar.Day == 1)
		{
			resetMontlyLimitShopData();
		}

		// Daily tasks.
		resetClanBonus();
		resetClanContributionList();
		resetClanDonationPoints();
		resetDailyHennaPattern();
		resetDailySkills();
		resetDailyItems();
		resetDailyPrimeShopData();
		resetDailyLimitShopData();
		resetWorldChatPoints();
		resetRecommends();
		resetTrainingCamp();
		resetTimedHuntingZones();
		resetMorgosMilitaryBase();
		resetDailyMissionRewards();
		resetAttendanceRewards();
		resetVip();
		resetResurrectionByPayment();
	}

	private void onSave()
	{
		GlobalVariablesManager.getInstance().storeMe();

		RevengeHistoryManager.getInstance().storeMe();

		if (Config.WORLD_EXCHANGE_LAZY_UPDATE)
		{
			WorldExchangeManager.getInstance().storeMe();
		}

		if (Olympiad.getInstance().inCompPeriod())
		{
			Olympiad.getInstance().saveOlympiadStatus();
			LOGGER.Info("Olympiad System: Data updated.");
		}
	}

	private void clanLeaderApply()
	{
		foreach (Clan clan in ClanTable.getInstance().getClans())
        {
            int? newLeaderId = clan.getNewLeaderId();
			if (newLeaderId != null)
			{
				ClanMember? member = clan.getClanMember(newLeaderId.Value);
				if (member == null)
				{
					continue;
				}

				clan.setNewLeader(member);
			}
		}
		LOGGER.Info("Clan leaders has been updated.");
	}

	private void resetClanContributionList()
	{
		foreach (Clan clan in ClanTable.getInstance().getClans())
		{
			clan.getVariables().DeleteWeeklyContribution();
		}
	}

	private void resetVitalityDaily()
	{
		if (!Config.Character.ENABLE_VITALITY)
		{
			return;
		}

		int vitality = PlayerStat.MAX_VITALITY_POINTS / 4;
		foreach (Player player in World.getInstance().getPlayers())
		{
			int VP = player.getVitalityPoints();
			player.setVitalityPoints(VP + vitality, false);
			foreach (SubClassHolder subclass in player.getSubClasses().Values)
			{
				int VPS = subclass.getVitalityPoints();
				subclass.setVitalityPoints(VPS + vitality);
			}
		}

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

			// TODO: possibly the expression was incorrect

			ctx.CharacterSubClasses.ExecuteUpdate(s => s.SetProperty(c => c.VitalityPoints,
				c => c.VitalityPoints == PlayerStat.MAX_VITALITY_POINTS
					? PlayerStat.MAX_VITALITY_POINTS
					: c.VitalityPoints + PlayerStat.MAX_VITALITY_POINTS / 4));

			ctx.Characters.ExecuteUpdate(s => s.SetProperty(c => c.VitalityPoints,
				c => c.VitalityPoints == PlayerStat.MAX_VITALITY_POINTS
					? PlayerStat.MAX_VITALITY_POINTS
					: c.VitalityPoints + PlayerStat.MAX_VITALITY_POINTS / 4));
		}
		catch (Exception e)
		{
			LOGGER.Error("Error while updating vitality" + e);
		}

		LOGGER.Info("Daily Vitality Added");
	}

	private void resetVitalityWeekly()
	{
		if (!Config.Character.ENABLE_VITALITY)
		{
			return;
		}

		foreach (Player player in World.getInstance().getPlayers())
		{
			player.setVitalityPoints(PlayerStat.MAX_VITALITY_POINTS, false);
			foreach (SubClassHolder subclass in player.getSubClasses().Values)
			{
				subclass.setVitalityPoints(PlayerStat.MAX_VITALITY_POINTS);
			}
		}

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

			ctx.CharacterSubClasses.ExecuteUpdate(s =>
				s.SetProperty(c => c.VitalityPoints, PlayerStat.MAX_VITALITY_POINTS));

			ctx.Characters.ExecuteUpdate(s =>
				s.SetProperty(c => c.VitalityPoints, PlayerStat.MAX_VITALITY_POINTS));
		}
		catch (Exception e)
		{
			LOGGER.Error("Error while updating vitality" + e);
		}

		LOGGER.Info("Vitality reset");
	}

	private void resetMonsterArenaWeekly()
	{
		foreach (Clan clan in ClanTable.getInstance().getClans())
		{
			GlobalVariablesManager.getInstance().Remove(GlobalVariablesManager.MONSTER_ARENA_VARIABLE + clan.getId());
		}
	}

	private void resetClanBonus()
	{
		ClanTable.getInstance().getClans().ForEach(x => x.resetClanBonus());
		LOGGER.Info("Daily clan bonus has been reset.");
	}

	private void resetDailySkills()
	{
		// Update data for offline players.
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (int skillId in RESET_SKILLS)
				ctx.CharacterSkillReuses.Where(s => s.SkillId == skillId).ExecuteDelete(); // TODO: delete all at once
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not reset daily skill reuse: " + e);
		}

		// Update data for online players.
		// Set<Player> updates = new();
		foreach (int skillId in RESET_SKILLS)
		{
			Skill? skill = SkillData.getInstance().getSkill(skillId, 1 /* No known need for more levels */);
			if (skill != null)
			{
				foreach (Player player in World.getInstance().getPlayers())
				{
					if (player.hasSkillReuse(skill.getReuseHashCode()))
					{
						player.removeTimeStamp(skill);
						// updates.add(player);
					}
				}
			}
		}
		// for (Player player : updates)
		// {
		// player.sendSkillList();
		// }

		LOGGER.Info("Daily skill reuse cleaned.");
	}

	private void resetDailyItems()
	{
		// Update data for offline players.
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (int itemId in RESET_ITEMS)
				ctx.CharacterItemReuses.Where(r => r.ItemId == itemId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not reset daily item reuse: " + e);
		}

		// Update data for online players.
		bool update;
		foreach (Player player in World.getInstance().getPlayers())
		{
			update = false;
			foreach (int itemId in RESET_ITEMS)
			{
				foreach (Item item in player.getInventory().getAllItemsByItemId(itemId))
				{
					player.getItemReuseTimeStamps().remove(item.ObjectId);
					update = true;
				}
			}
			if (update)
			{
				player.sendItemList();
			}
		}

		LOGGER.Info("Daily item reuse cleaned.");
	}

	private void resetClanDonationPoints()
	{
		// Update data for offline players.
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.CharacterVariables.Where(v => v.Name == PlayerVariables.CLAN_DONATION_POINTS).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not reset clan donation points: " + e);
		}

		// Update data for online players.
		foreach (Player player in World.getInstance().getPlayers())
		{
			player.getVariables().Remove(PlayerVariables.CLAN_DONATION_POINTS);
			player.getVariables().storeMe();
		}

		LOGGER.Info("Daily clan donation points have been reset.");
	}

	private void resetWorldChatPoints()
	{
		if (!Config.ENABLE_WORLD_CHAT)
		{
			return;
		}

		// Update data for offline players.
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.CharacterVariables.Where(v => v.Name == PlayerVariables.WORLD_CHAT_VARIABLE_NAME)
				.ExecuteUpdate(s => s.SetProperty(v => v.Value, "0"));
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not reset daily world chat points: " + e);
		}

		// Update data for online players.
		foreach (Player player in World.getInstance().getPlayers())
		{
			player.setWorldChatUsed(0);
			player.sendPacket(new ExWorldCharCntPacket(player));
			player.getVariables().storeMe();
		}

		LOGGER.Info("Daily world chat points has been reset.");
	}

	private void resetRecommends()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.CharacterRecoBonuses.ExecuteUpdate(s =>
				s.SetProperty(c => c.RecLeft, 0)
					.SetProperty(c => c.RecHave, c => c.RecHave >= 20 ? c.RecHave - 20 : 0));
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not reset Recommendations System: " + e);
		}

		foreach (Player player in World.getInstance().getPlayers())
		{
			player.setRecomLeft(0);
			player.setRecomHave(player.getRecomHave() - 20);
			player.sendPacket(new ExVoteSystemInfoPacket(player));
			player.broadcastUserInfo();
		}
	}

	private void resetTrainingCamp()
	{
		if (Config.TRAINING_CAMP_ENABLE)
		{
			// Update data for offline players.
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.AccountVariables.Where(v => v.Name == "TRAINING_CAMP_DURATION").ExecuteDelete();
			}
			catch (Exception e)
			{
				LOGGER.Error("Could not reset Training Camp: " + e);
			}

			// Update data for online players.
			foreach (Player player in World.getInstance().getPlayers())
			{
				player.resetTraingCampDuration();
				player.getAccountVariables().storeMe();
			}

			LOGGER.Info("Training Camp daily time has been reset.");
		}
	}

	private void resetVip()
	{
		// Delete all entries for received gifts
		AccountVariables.DeleteVariable(AccountVariables.VIP_ITEM_BOUGHT);

		// Checks the tier expiration for online players
		// offline players get handled on next time they log in.
		foreach (Player player in World.getInstance().getPlayers())
		{
			if (player.getVipTier() > 0)
			{
				VipManager.getInstance().checkVipTierExpiration(player);
			}

			// player.getAccountVariables().restoreMe(); // TODO: why it here?
		}
	}

	private void resetDailyMissionRewards()
	{
		DailyMissionData.getInstance().getDailyMissionData().ForEach(x => x.reset());
	}

	private void resetTimedHuntingZones()
	{
		foreach (TimedHuntingZoneHolder holder in TimedHuntingZoneData.getInstance().getAllHuntingZones())
		{
			if (holder.isWeekly())
			{
				continue;
			}

			// Update data for offline players.
			try
			{
				// TODO: separate table
				string name1 = PlayerVariables.HUNTING_ZONE_ENTRY + holder.getZoneId();
				string name2 = PlayerVariables.HUNTING_ZONE_TIME + holder.getZoneId();
				string name3 = PlayerVariables.HUNTING_ZONE_REMAIN_REFILL + holder.getZoneId();

				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.CharacterVariables.Where(v => v.Name == name1 || v.Name == name2 || v.Name == name3)
					.ExecuteDelete();
			}
			catch (Exception e)
			{
				LOGGER.Error("Could not reset Special Hunting Zones: " + e);
			}

			// Update data for online players.
			foreach (Player player in World.getInstance().getPlayers())
			{
				player.getVariables().Remove(PlayerVariables.HUNTING_ZONE_ENTRY + holder.getZoneId());
				player.getVariables().Remove(PlayerVariables.HUNTING_ZONE_TIME + holder.getZoneId());
				player.getVariables().Remove(PlayerVariables.HUNTING_ZONE_REMAIN_REFILL + holder.getZoneId());
				player.getVariables().storeMe();
			}
		}

		LOGGER.Info("Special Hunting Zones has been reset.");
	}

	private void resetTimedHuntingZonesWeekly()
	{
		foreach (TimedHuntingZoneHolder holder in TimedHuntingZoneData.getInstance().getAllHuntingZones())
		{
			if (!holder.isWeekly())
			{
				continue;
			}

			// Update data for offline players.
			try
			{
				// TODO: separate table
				string name1 = PlayerVariables.HUNTING_ZONE_ENTRY + holder.getZoneId();
				string name2 = PlayerVariables.HUNTING_ZONE_TIME + holder.getZoneId();
				string name3 = PlayerVariables.HUNTING_ZONE_REMAIN_REFILL + holder.getZoneId();

				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.CharacterVariables.Where(v => v.Name == name1 || v.Name == name2 || v.Name == name3)
					.ExecuteDelete();
			}
			catch (Exception e)
			{
				LOGGER.Error("Could not reset Weekly Special Hunting Zones: " + e);
			}

			// Update data for online players.
			foreach (Player player in World.getInstance().getPlayers())
			{
				player.getVariables().Remove(PlayerVariables.HUNTING_ZONE_ENTRY + holder.getZoneId());
				player.getVariables().Remove(PlayerVariables.HUNTING_ZONE_TIME + holder.getZoneId());
				player.getVariables().Remove(PlayerVariables.HUNTING_ZONE_REMAIN_REFILL + holder.getZoneId());
				player.getVariables().storeMe();
			}
		}

		LOGGER.Info("Weekly Special Hunting Zones has been reset.");
	}

	private void resetAttendanceRewards()
	{
		if (Config.Attendance.ATTENDANCE_REWARDS_SHARE_ACCOUNT)
		{
			// Update data for offline players.
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.AccountVariables.Where(v => v.Name == "ATTENDANCE_DATE").ExecuteDelete();
			}
			catch (Exception e)
			{
				LOGGER.Error(GetType().Name + ": Could not reset Attendance Rewards: " + e);
			}

			// Update data for online players.
			foreach (Player player in World.getInstance().getPlayers())
			{
				player.getAccountVariables().Remove("ATTENDANCE_DATE");
				player.getAccountVariables().storeMe();
			}

			LOGGER.Info("Account shared Attendance Rewards has been reset.");
		}
		else
		{
			// Update data for offline players.
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.CharacterVariables.Where(v => v.Name == "ATTENDANCE_DATE").ExecuteDelete();
			}
			catch (Exception e)
			{
				LOGGER.Error(GetType().Name + ": Could not reset Attendance Rewards: " + e);
			}

			// Update data for online players.
			foreach (Player player in World.getInstance().getPlayers())
			{
				player.getVariables().Remove(PlayerVariables.ATTENDANCE_DATE);
				player.getVariables().storeMe();
			}

			LOGGER.Info("Attendance Rewards has been reset.");
		}
	}

	private void resetDailyPrimeShopData()
	{
		foreach (PrimeShopGroup holder in PrimeShopData.getInstance().getPrimeItems().Values)
		{
			// Update data for offline players.
			try
			{
				string name = AccountVariables.PRIME_SHOP_PRODUCT_DAILY_COUNT + holder.getBrId();
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.AccountVariables.Where(v => v.Name == name).ExecuteDelete();
			}
			catch (Exception e)
			{
				LOGGER.Error(GetType().Name + ": Could not reset PrimeShopData: " + e);
			}

			// Update data for online players.
			foreach (Player player in World.getInstance().getPlayers())
			{
				player.getAccountVariables().Remove(AccountVariables.PRIME_SHOP_PRODUCT_DAILY_COUNT + holder.getBrId());
				player.getAccountVariables().storeMe();
			}
		}
		LOGGER.Info("PrimeShopData has been reset.");
	}

	private void resetDailyLimitShopData()
	{
		foreach (LimitShopProductHolder holder in LimitShopData.getInstance().getProducts())
		{
			// Update data for offline players.
			try
			{
				string name = AccountVariables.LCOIN_SHOP_PRODUCT_DAILY_COUNT + holder.getProductionId();
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.AccountVariables.Where(v => v.Name == name).ExecuteDelete();
			}
			catch (Exception e)
			{
				LOGGER.Error(GetType().Name + ": Could not reset LimitShopData: " + e);
			}

			// Update data for online players.
			foreach (Player player in World.getInstance().getPlayers())
			{
				player.getAccountVariables().Remove(AccountVariables.LCOIN_SHOP_PRODUCT_DAILY_COUNT + holder.getProductionId());
				player.getAccountVariables().storeMe();
			}
		}
		LOGGER.Info("LimitShopData has been reset.");
	}

	private void resetMontlyLimitShopData()
	{
		foreach (LimitShopProductHolder holder in LimitShopData.getInstance().getProducts())
		{
			// Update data for offline players.
			try
			{
				string name = AccountVariables.LCOIN_SHOP_PRODUCT_MONTLY_COUNT + holder.getProductionId();
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.AccountVariables.Where(v => v.Name == name).ExecuteDelete();
			}
			catch (Exception e)
			{
				LOGGER.Error(GetType().Name + ": Could not reset LimitShopData: " + e);
			}
			// Update data for online players.
			foreach (Player player in World.getInstance().getPlayers())
			{
				player.getAccountVariables().Remove(AccountVariables.LCOIN_SHOP_PRODUCT_MONTLY_COUNT + holder.getProductionId());
				player.getAccountVariables().storeMe();
			}
		}
		LOGGER.Info("LimitShopData has been reset.");
	}

	private void resetHuntPass()
	{
		// Update data for offline players.
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.HuntPasses.ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Could not delete entries from hunt pass: " + e);
		}

		// Update data for online players.
		foreach (Player player in World.getInstance().getPlayers())
		{
			player.getHuntPass().resetHuntPass();
		}

		LOGGER.Info("HuntPassData has been reset.");
	}

	private void resetResurrectionByPayment()
	{
		// Update data for offline players.
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.CharacterVariables.Where(v => v.Name == PlayerVariables.RESURRECT_BY_PAYMENT_COUNT).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Could not reset payment resurrection count for players: " + e);
		}

		// Update data for online players.
		foreach (Player player in World.getInstance().getPlayers())
		{
			player.getVariables().Remove(PlayerVariables.RESURRECT_BY_PAYMENT_COUNT);
			player.getVariables().storeMe();
		}

		LOGGER.Info("Daily payment resurrection count for player has been reset.");
	}

	public void resetPrivateStoreHistory()
	{
		try
		{
			PrivateStoreHistoryManager.getInstance().reset();
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Could not reset private store history! " + e);
		}

		LOGGER.Info("Private store history has been reset.");
	}

	private void resetDailyHennaPattern()
	{
		// Update data for offline players.
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.CharacterVariables.Where(v => v.Name == PlayerVariables.DYE_POTENTIAL_DAILY_COUNT).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Could not reset Daily Henna Count: " + e);
		}

		// Update data for online players.
		foreach (Player player in World.getInstance().getPlayers())
		{
			player.getVariables().Remove(PlayerVariables.DYE_POTENTIAL_DAILY_COUNT);
			player.getVariables().storeMe();
		}

		LOGGER.Info("Daily Henna Count has been reset.");
	}

	private void resetMorgosMilitaryBase()
	{
		// Update data for offline players.
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.CharacterVariables.Where(v => v.Name == "MORGOS_MILITARY_FREE").ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Could not reset MorgosMilitaryBase: " + e);
		}

		// Update data for online players.
		foreach (Player player in World.getInstance().getPlayers())
		{
			player.getAccountVariables().Remove("MORGOS_MILITARY_FREE");
			player.getAccountVariables().storeMe();
		}

		LOGGER.Info("MorgosMilitaryBase has been reset.");
	}

	public static DailyTaskManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly DailyTaskManager INSTANCE = new DailyTaskManager();
	}
}