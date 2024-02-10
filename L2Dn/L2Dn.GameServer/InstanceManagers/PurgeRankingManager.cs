using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Items;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Berezkin Nikolay
 */
public class PurgeRankingManager
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(PurgeRankingManager));
	
	private static readonly Map<int, Map<int, StatSet>> _ranking = new();
	private const string RESTORE_SUBJUGATION = "SELECT *, `points` + `keys` * 1000000 as `total` FROM `character_purge` WHERE `category`=? ORDER BY `total` DESC";
	private const string DELETE_SUBJUGATION = "DELETE FROM character_purge WHERE charId=? and category=?";
	
	public PurgeRankingManager()
	{
		updateRankingFromDB();
		
		int nextDate = Calendar.getInstance().get(Calendar.MINUTE);
		while ((nextDate % 5) != 0)
		{
			nextDate = nextDate + 1;
		}
		
		ThreadPool.scheduleAtFixedRate(this::updateRankingFromDB, (nextDate - Calendar.getInstance().get(Calendar.MINUTE)) > 0 ? (long) (nextDate - Calendar.getInstance().get(Calendar.MINUTE)) * 60 * 1000 : (long) ((nextDate + 60) - Calendar.getInstance().get(Calendar.MINUTE)) * 60 * 1000, 300000);
	}
	
	private void updateRankingFromDB()
	{
		// Weekly rewards.
		long lastPurgeRewards = GlobalVariablesManager.getInstance().getLong(GlobalVariablesManager.PURGE_REWARD_TIME, 0);
		if ((Calendar.getInstance().get(Calendar.DAY_OF_WEEK) == Calendar.SUNDAY) && ((System.currentTimeMillis() - lastPurgeRewards) > (604800000 /* 1 week */ - 600000 /* task delay x2 */)))
		{
			GlobalVariablesManager.getInstance().set(GlobalVariablesManager.PURGE_REWARD_TIME, System.currentTimeMillis());
			for (int category = 1; category <= 9; category++)
			{
				if (getTop5(category) != null)
				{
					int counter = 0;
					foreach (var purgeData in getTop5(category))
					{
						int charId = CharInfoTable.getInstance().getIdByName(purgeData.getKey());
						Message msg = new Message(charId, Config.SUBJUGATION_TOPIC_HEADER, Config.SUBJUGATION_TOPIC_BODY, MailType.PURGE_REWARD);
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
								throw new IllegalStateException("Unexpected value: " + category);
							}
						}
						attachment.addItem("Purge reward", reward, 5 - counter, null, null);
						MailManager.getInstance().sendMessage(msg);
						
						// Notify to scripts.
						Player player = World.getInstance().getPlayer(charId);
						Item item = attachment.getItemByItemId(reward);
						if (player != null)
						{
							if (EventDispatcher.getInstance().hasListener(EventType.ON_ITEM_PURGE_REWARD))
							{
								EventDispatcher.getInstance().notifyEventAsync(new OnItemPurgeReward(player, item));
							}
						}
						
						try
						{
							Connection con = DatabaseFactory.getConnection();

							{
								PreparedStatement st = con.prepareStatement(DELETE_SUBJUGATION);
								st.setInt(1, charId);
								st.setInt(2, category);
								st.execute();
							}
						}
						catch (Exception e)
						{
							LOGGER.Error("Failed to delete character subjugation info " + charId, e);
						}
						
						Player onlinePlayer = World.getInstance().getPlayer(charId);
						if (onlinePlayer != null)
						{
							onlinePlayer.getPurgePoints().clear();
							onlinePlayer.sendPacket(new ExSubjugationSidebar(null, new PurgePlayerHolder(0, 0, 0)));
						}
						
						counter++;
					}
				}
			}
		}
		
		// Clear ranking.
		_ranking.clear();
		
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement statement = con.prepareStatement(RESTORE_SUBJUGATION);
			statement.setInt(1, category);

			{
				ResultSet rset = statement.executeQuery();
				int rank = 1;
				Map<int, StatSet> rankingInCategory = new();
				while (rset.next())
				{
					StatSet set = new StatSet();
					set.set("charId", rset.getInt("charId"));
					set.set("points", rset.getInt("total"));
					rankingInCategory.put(rank, set);
					rank++;
				}
				_ranking.put(category, rankingInCategory);
			}
			// LOGGER.Info(GetType().Name +": Loaded " + _ranking.get(category).size() + " records for category " + category + ".");
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not restore subjugation ranking data" + e);
		}
	}
	
	public Map<String, int> getTop5(int category)
	{
		Map<String, int> top5 = new();
		for (int i = 1; i <= 5; i++)
		{
			try
			{
				if (_ranking.get(category) == null)
				{
					continue;
				}
				
				StatSet ss = _ranking.get(category).get(i);
				if (ss == null)
				{
					continue;
				}
				
				String charName = CharInfoTable.getInstance().getNameById(ss.getInt("charId"));
				int points = ss.getInt("points");
				top5.put(charName, points);
			}
			catch (IndexOutOfBoundsException ignored)
			{
			}
		}
		return top5.entrySet().stream().sorted(Entry.<String, int> comparingByValue().reversed()).collect(Collectors.toMap(Entry::getKey, Entry::getValue, (e1, e2) => e2, LinkedHashMap::new));
	}
	
	public SimpleEntry<int, int> getPlayerRating(int category, int charId)
	{
		if (_ranking.get(category) == null)
		{
			return new SimpleEntry<>(0, 0);
		}
		
		Optional<Entry<int, StatSet>> player = _ranking.get(category).entrySet().stream().filter(it => it.getValue().getInt("charId") == charId).findFirst();
		if (player.isPresent())
		{
			if (player.get().getValue() == null)
			{
				return new SimpleEntry<>(0, 0);
			}
			return new SimpleEntry<>(player.get().getKey(), player.get().getValue().getInt("points"));
		}
		
		return new SimpleEntry<>(0, 0);
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