using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
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

		{
			string filePath = Path.Combine(Config.DATAPACK_ROOT_PATH, "data/RandomCraftExtractData.xml");
			using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			XDocument document = XDocument.Load(stream);
			document.Elements("list").Elements("extract").Elements("item").ForEach(parseExtractElement);
		}

		int extractCount = EXTRACT_DATA.size();
		if (extractCount > 0)
		{
			LOGGER.Info(GetType().Name + ": Loaded " + extractCount + " extraction data.");
		}
		
		REWARD_DATA.clear();

		{
			string filePath = Path.Combine(Config.DATAPACK_ROOT_PATH, "data/RandomCraftRewardData.xml");
			using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			XDocument document = XDocument.Load(stream);
			document.Elements("list").Elements("rewards").Elements("item").ForEach(parseRewardElement);
		}
		
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

	private void parseExtractElement(XElement element)
	{
		int itemId = element.Attribute("id").GetInt32();
		long points = element.Attribute("points").GetInt64();
		long fee = element.Attribute("fee").GetInt64();
		EXTRACT_DATA.put(itemId, new RandomCraftExtractDataHolder(points, fee));
	}

	private void parseRewardElement(XElement element)
	{
		int itemId = element.Attribute("id").GetInt32();
		ItemTemplate item = ItemData.getInstance().getTemplate(itemId);
		if (item == null)
		{
			LOGGER.Warn(GetType().Name + " unexisting item reward: " + itemId);
		}
		else
		{
			long count = element.Attribute("count").GetInt64(1);
			double chance = element.Attribute("chance").GetDouble(100);
			bool announce = element.Attribute("announce").GetBoolean(false);

			REWARD_DATA.put(itemId,
				new RandomCraftRewardDataHolder(itemId, count, Math.Min(100, Math.Max(0.00000000000001, chance)),
					announce));
		}
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