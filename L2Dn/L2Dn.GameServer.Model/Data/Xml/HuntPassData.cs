using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Serenitty
 */
public class HuntPassData: DataReaderBase
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
			
			XDocument document = LoadXmlDocument(DataFileLocation.Data, "HuntPass.xml");
			document.Elements("hitConditionBonus").Elements("item").ForEach(parseElement);
			
			_rewardCount = _rewards.Count;
			_premiumRewardCount = _premiumRewards.Count;
			LOGGER.Info(GetType().Name + ": Loaded " + _rewardCount + " HuntPass rewards.");
		}
		else
		{
			LOGGER.Info(GetType().Name + ": Disabled.");
		}
	}
	
	private void parseElement(XElement element)
	{
		int itemId = element.GetAttributeValueAsInt32("id");
		int itemCount = element.GetAttributeValueAsInt32("count");
		int premiumitemId = element.GetAttributeValueAsInt32("premiumId");
		int premiumitemCount = element.GetAttributeValueAsInt32("premiumCount");
		if (ItemData.getInstance().getTemplate(itemId) == null)
		{
			LOGGER.Error(GetType().Name + ": Item with id " + itemId + " does not exist.");
		}
		else
		{
			_rewards.add(new ItemHolder(itemId, itemCount));
			_premiumRewards.add(new ItemHolder(premiumitemId, premiumitemCount));
		}
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