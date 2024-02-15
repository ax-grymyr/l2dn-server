using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Holders;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Mobius
 */
public class AttendanceRewardData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AttendanceRewardData));
	private readonly List<ItemHolder> _rewards = new();
	private int _rewardsCount;
	
	protected AttendanceRewardData()
	{
		load();
	}
	
	public void load()
	{
		if (Config.ENABLE_ATTENDANCE_REWARDS)
		{
			_rewards.Clear();

			XDocument document = LoadXmlDocument(DataFileLocation.Data, "AttendanceRewards.xml");
			document.Elements("list").Elements("item").ForEach(loadElement);
			
			_rewardsCount = _rewards.Count;
			LOGGER.Info(GetType().Name + ": Loaded " + _rewardsCount + " rewards.");
		}
		else
		{
			LOGGER.Info(GetType().Name + ": Disabled.");
		}
	}

	private void loadElement(XElement element)
	{
		int itemId = element.Attribute("id").GetInt32();
		int itemCount = element.Attribute("count").GetInt32();
		if (ItemData.getInstance().getTemplate(itemId) == null)
		{
			LOGGER.Info(GetType().Name + ": Item with id " + itemId + " does not exist.");
		}
		else
		{
			_rewards.Add(new ItemHolder(itemId, itemCount));
		}
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