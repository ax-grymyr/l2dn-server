using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Serenitty
 */
public class HuntPassData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(HuntPassData));
	private readonly List<ItemHolder> _rewards = new();
	private readonly List<ItemHolder> _premiumRewards = new();
	private int _rewardCount = 0;
	private int _premiumRewardCount = 0;
	
	protected HuntPassData()
	{
		load();
	}
	
	public void load()
	{
		if (Config.ENABLE_HUNT_PASS)
		{
			_rewards.Clear();
			parseDatapackFile("data/HuntPass.xml");
			_rewardCount = _rewards.size();
			_premiumRewardCount = _premiumRewards.size();
			LOGGER.Info(GetType().Name + ": Loaded " + _rewardCount + " HuntPass rewards.");
		}
		else
		{
			LOGGER.Info(GetType().Name + ": Disabled.");
		}
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, "item", rewardNode =>
		{
			StatSet set = new StatSet(parseAttributes(rewardNode));
			int itemId = set.getInt("id");
			int itemCount = set.getInt("count");
			int premiumitemId = set.getInt("premiumId");
			int premiumitemCount = set.getInt("premiumCount");
			if (ItemData.getInstance().getTemplate(itemId) == null)
			{
				LOGGER.Info(GetType().Name + ": Item with id " + itemId + " does not exist.");
			}
			else
			{
				_rewards.add(new ItemHolder(itemId, itemCount));
				_premiumRewards.add(new ItemHolder(premiumitemId, premiumitemCount));
			}
		}));
	}
	
	public List<ItemHolder> getRewards()
	{
		return _rewards;
	}
	
	public int getRewardsCount()
	{
		return _rewardCount;
	}
	
	public List<ItemHolder> getPremiumRewards()
	{
		return _premiumRewards;
	}
	
	public int getPremiumRewardsCount()
	{
		return _premiumRewardCount;
	}
	
	public static HuntPassData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly HuntPassData INSTANCE = new();
	}
}