using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Mobius
 */
public class AttendanceRewardData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AttendanceRewardData));
	private readonly List<ItemHolder> _rewards = new();
	private int _rewardsCount = 0;
	
	protected AttendanceRewardData()
	{
		load();
	}
	
	public void load()
	{
		if (Config.ENABLE_ATTENDANCE_REWARDS)
		{
			_rewards.Clear();
			parseDatapackFile("data/AttendanceRewards.xml");
			_rewardsCount = _rewards.Count;
			LOGGER.Info(GetType().Name + ": Loaded " + _rewardsCount + " rewards.");
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
			if (ItemData.getInstance().getTemplate(itemId) == null)
			{
				LOGGER.Info(GetType().Name + ": Item with id " + itemId + " does not exist.");
			}
			else
			{
				_rewards.Add(new ItemHolder(itemId, itemCount));
			}
		}));
	}
	
	public List<ItemHolder> getRewards()
	{
		return _rewards;
	}
	
	public int getRewardsCount()
	{
		return _rewardsCount;
	}
	
	public static AttendanceRewardData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly AttendanceRewardData INSTANCE = new();
	}
}