using L2Dn.Events;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Items;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets.Subjugation;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Berezkin Nikolay
 */
public class PurgeRankingManager
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(PurgeRankingManager));
	private static readonly Map<int, Map<int, StatSet>> _ranking = new();

	public PurgeRankingManager()
	{
		updateRankingFromDB();

		DateTime time = DateTime.UtcNow;
		int nextDate = time.Minute;
		while (nextDate % 5 != 0)
		{
			nextDate = nextDate + 1;
		}

		TimeSpan delay = nextDate - time.Minute > 0
			? TimeSpan.FromMinutes(nextDate - time.Minute)
			: TimeSpan.FromMinutes(60 + nextDate - time.Minute);

		ThreadPool.scheduleAtFixedRate(updateRankingFromDB, delay, TimeSpan.FromSeconds(300));
	}

	private void updateRankingFromDB()
	{
		// Weekly rewards.
		DateTime now = DateTime.UtcNow;
		DateTime lastPurgeRewards = GlobalVariablesManager.getInstance().getDateTime(GlobalVariablesManager.PURGE_REWARD_TIME, DateTime.MinValue);
		if (now.DayOfWeek == DayOfWeek.Sunday && now - lastPurgeRewards > TimeSpan.FromSeconds(604800 /* 1 week */ - 600 /* task delay x2 */))
		{
			GlobalVariablesManager.getInstance().set(GlobalVariablesManager.PURGE_REWARD_TIME, now);
			for (int category = 1; category <= 9; category++)
			{
				if (getTop5(category) != null)
				{
					int counter = 0;
					foreach (var purgeData in getTop5(category))
					{
						int charId = CharInfoTable.getInstance().getIdByName(purgeData.Key);
						Message msg = new Message(charId, Config.SUBJUGATION_TOPIC_HEADER,
							Config.SUBJUGATION_TOPIC_BODY, MailType.PURGE_REWARD);
						Mail attachment = msg.createAttachments();
						int reward;
						switch (category)
						{
							case 1:
							{
								reward = 95460;
								break;
							}
							case 2:
							{
								reward = 95461;
								break;
							}
							case 3:
							{
								reward = 95462;
								break;
							}
							case 4:
							{
								reward = 95463;
								break;
							}
							case 5:
							{
								reward = 95464;
								break;
							}
							case 6:
							{
								reward = 95465;
								break;
							}
							case 7:
							{
								reward = 96724;
								break;
							}
							case 8:
							{
								reward = 97225;
								break;
							}
							case 9:
							{
								reward = 95466;
								break;
							}
							default:
							{
								throw new InvalidOperationException("Unexpected value: " + category);
							}
						}

						attachment.addItem("Purge reward", reward, 5 - counter, null, null);
						MailManager.getInstance().sendMessage(msg);

						// Notify to scripts.
						Player? player = World.getInstance().getPlayer(charId);
						Item? item = attachment.getItemByItemId(reward);
						if (player != null && item != null)
						{
							EventContainer itemEvents = item.getTemplate().Events;
							if (itemEvents.HasSubscribers<OnItemPurgeReward>())
							{
								itemEvents.NotifyAsync(new OnItemPurgeReward(player, item));
							}
						}

						try
						{
							using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
							ctx.CharacterPurges.Where(c => c.CharacterId == charId && c.Category == category)
								.ExecuteDelete();
						}
						catch (Exception e)
						{
							LOGGER.Error("Failed to delete character subjugation info " + charId, e);
						}

						Player? onlinePlayer = World.getInstance().getPlayer(charId);
						if (onlinePlayer != null)
						{
							onlinePlayer.getPurgePoints().Clear();
							onlinePlayer.sendPacket(
								new ExSubjugationSidebarPacket(null, new PurgePlayerHolder(0, 0, 0)));
						}

						counter++;
					}
				}
			}
		}

		// Clear ranking.
		_ranking.Clear();

		// Restore ranking.
		for (int category = 1; category <= 9; category++)
		{
			restoreByCategories(category);
		}
	}

	private void restoreByCategories(int category)
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

			int rank = 1;
			Map<int, StatSet> rankingInCategory = new();
			foreach (CharacterPurge record in ctx.CharacterPurges.Where(r => r.Category == category)
				         .OrderByDescending(r => r.Points + r.Keys * 1000000))
			{
				StatSet set = new StatSet();
				set.set("charId", record.CharacterId);
				set.set("points", record.Points + record.Keys * 1000000);
				rankingInCategory.put(rank, set);
				rank++;
			}

			_ranking.put(category, rankingInCategory);
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not restore subjugation ranking data" + e);
		}
	}

	public Map<string, int> getTop5(int category)
	{
		Map<string, int> top5 = new();
		for (int i = 1; i <= 5; i++)
		{
			try
			{
				StatSet? ss = _ranking.get(category)?.get(i);
				if (ss == null)
				{
					continue;
				}

				string? charName = CharInfoTable.getInstance().getNameById(ss.getInt("charId"));
                if (charName == null)
                    continue;
                
				int points = ss.getInt("points");
				top5.put(charName, points);
			}
			catch (Exception e)
			{
				LOGGER.Error(e);
			}
		}

		//return top5.sorted(Entry.<String, int> comparingByValue().reversed()).collect(Collectors.toMap(Entry::getKey, Entry::getValue, (e1, e2) => e2, LinkedHashMap::new));
		return top5; // TODO: figure out the sort order
	}

	public (int, int) getPlayerRating(int category, int charId)
    {
        Map<int, StatSet>? rankingCategory = _ranking.get(category);
		if (rankingCategory == null)
		{
			return (0, 0);
		}

        KeyValuePair<int, StatSet> kvp = rankingCategory.
            FirstOrDefault(it => it.Value.getInt("charId") == charId);

		if (kvp.Value is null)
			return (0, 0);

		return (kvp.Key, kvp.Value.getInt("points"));
	}

	public static PurgeRankingManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly PurgeRankingManager INSTANCE = new PurgeRankingManager();
	}
}