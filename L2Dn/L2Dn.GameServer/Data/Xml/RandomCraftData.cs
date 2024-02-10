using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Mode, Mobius
 */
public class RandomCraftData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(RandomCraftData));
	private static readonly Map<int, RandomCraftExtractDataHolder> EXTRACT_DATA = new();
	private static readonly Map<int, RandomCraftRewardDataHolder> REWARD_DATA = new();
	
	private List<RandomCraftRewardDataHolder> _randomRewards = null;
	private int _randomRewardIndex = 0;
	
	protected RandomCraftData()
	{
		load();
	}
	
	public void load()
	{
		EXTRACT_DATA.clear();
		parseDatapackFile("data/RandomCraftExtractData.xml");
		int extractCount = EXTRACT_DATA.size();
		if (extractCount > 0)
		{
			LOGGER.Info(GetType().Name + ": Loaded " + extractCount + " extraction data.");
		}
		
		REWARD_DATA.clear();
		parseDatapackFile("data/RandomCraftRewardData.xml");
		int rewardCount = REWARD_DATA.size();
		if (rewardCount > 4)
		{
			LOGGER.Info(GetType().Name + ": Loaded " + rewardCount + " rewards.");
		}
		else if (rewardCount > 0)
		{
			LOGGER.Info(GetType().Name + ": Random craft rewards should be more than " + rewardCount + ".");
			REWARD_DATA.clear();
		}
		
		randomizeRewards();
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, "extract", extractNode =>
		{
			forEach(extractNode, "item", itemNode =>
			{
				StatSet stats = new StatSet(parseAttributes(itemNode));
				int itemId = stats.getInt("id");
				long points = stats.getLong("points");
				long fee = stats.getLong("fee");
				EXTRACT_DATA.put(itemId, new RandomCraftExtractDataHolder(points, fee));
			});
		}));
		
		forEach(doc, "list", listNode => forEach(listNode, "rewards", rewardNode =>
		{
			forEach(rewardNode, "item", itemNode =>
			{
				StatSet stats = new StatSet(parseAttributes(itemNode));
				int itemId = stats.getInt("id");
				ItemTemplate item = ItemData.getInstance().getTemplate(itemId);
				if (item == null)
				{
					LOGGER.Warn(GetType().Name + " unexisting item reward: " + itemId);
				}
				else
				{
					REWARD_DATA.put(itemId, new RandomCraftRewardDataHolder(stats.getInt("id"), stats.getLong("count", 1), Math.min(100, Math.max(0.00000000000001, stats.getDouble("chance", 100))), stats.getBoolean("announce", false)));
				}
			});
		}));
	}
	
	public bool isEmpty()
	{
		return REWARD_DATA.isEmpty();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)] 
	public RandomCraftRewardItemHolder getNewReward()
	{
		RandomCraftRewardDataHolder reward = null;
		double random = Rnd.get(100d);
		while (!REWARD_DATA.isEmpty())
		{
			if (_randomRewardIndex == (REWARD_DATA.size() - 1))
			{
				randomizeRewards();
			}
			_randomRewardIndex++;
			
			reward = _randomRewards.get(_randomRewardIndex);
			if (random < reward.getChance())
			{
				return new RandomCraftRewardItemHolder(reward.getItemId(), reward.getCount(), false, 20);
			}
		}
		return null;
	}
	
	private void randomizeRewards()
	{
		_randomRewardIndex = -1;
		_randomRewards = new(REWARD_DATA.values());
		Collections.shuffle(_randomRewards);
	}
	
	public bool isAnnounce(int id)
	{
		RandomCraftRewardDataHolder holder = REWARD_DATA.get(id);
		if (holder == null)
		{
			return false;
		}
		return holder.isAnnounce();
	}
	
	public long getPoints(int id)
	{
		RandomCraftExtractDataHolder holder = EXTRACT_DATA.get(id);
		if (holder == null)
		{
			return 0;
		}
		return holder.getPoints();
	}
	
	public long getFee(int id)
	{
		RandomCraftExtractDataHolder holder = EXTRACT_DATA.get(id);
		if (holder == null)
		{
			return 0;
		}
		return holder.getFee();
	}
	
	public static RandomCraftData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly RandomCraftData INSTANCE = new();
	}
}