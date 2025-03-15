using System.Collections.Immutable;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.StaticData;
using L2Dn.Model.Xml;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Data.Xml;

public sealed class AttendanceRewardData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(AttendanceRewardData));
	private ImmutableArray<ItemHolder> _rewards = ImmutableArray<ItemHolder>.Empty;

	private AttendanceRewardData()
	{
		load();
	}

	public void load()
	{
		if (Config.Attendance.ENABLE_ATTENDANCE_REWARDS)
		{
			static bool CheckItem(XmlAttendanceReward item)
			{
				bool itemExists = ItemData.getInstance().getTemplate(item.Id) is not null;
				if (!itemExists)
					_logger.Info(nameof(AttendanceRewardData) + ": Item with id " + item.Id + " does not exist.");

				return itemExists;
			}

			_rewards = LoadXmlDocument<XmlAttendanceRewardList>(DataFileLocation.Data, "AttendanceRewards.xml")
				.Items.Where(CheckItem).Select(item => new ItemHolder(item.Id, item.Count))
				.ToImmutableArray();

			_logger.Info(GetType().Name + ": Loaded " + _rewards.Length + " rewards.");
		}
		else
		{
			_logger.Info(GetType().Name + ": Disabled.");
		}
	}

	public ImmutableArray<ItemHolder> getRewards()
	{
		return _rewards;
	}

	public int getRewardsCount()
	{
		return _rewards.Length;
	}

	public static AttendanceRewardData getInstance()
	{
		return SingletonHolder.Instance;
	}

	private static class SingletonHolder
	{
		public static readonly AttendanceRewardData Instance = new();
	}
}