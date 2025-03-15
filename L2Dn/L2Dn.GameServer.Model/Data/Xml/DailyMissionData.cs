using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.StaticData;
using L2Dn.Model;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Sdw, Mobius
 */
public sealed class DailyMissionData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(DailyMissionData));

	private FrozenDictionary<int, DailyMissionDataHolder> _dailyMissionRewards =
		FrozenDictionary<int, DailyMissionDataHolder>.Empty;

	private DailyMissionData()
	{
		load();
	}

	public void load()
	{
		Dictionary<int, DailyMissionDataHolder> dailyMissionRewards = [];

		XmlDailyMissionData document = LoadXmlDocument<XmlDailyMissionData>(DataFileLocation.Data, "DailyMission.xml");
		bool missionSeasonStarted = MissionLevel.getInstance().getCurrentSeason() <= 0; // TODO Must be handled somewhere else
		foreach (XmlDailyMission xmlDailyMission in document.DailyMissions)
		{
			ImmutableArray<CharacterClass> characterClasses =
				xmlDailyMission.ClassIds.Select(x => (CharacterClass)x).ToImmutableArray();

			// TODO check item ids
			ImmutableArray<ItemHolder> rewardItems = xmlDailyMission.Rewards
				.Where(x => missionSeasonStarted || x.Id != AbstractDailyMissionHandler.MISSION_LEVEL_POINTS)
				.Select(x => new ItemHolder(x.Id, x.Count)).ToImmutableArray();

			Func<DailyMissionDataHolder, AbstractDailyMissionHandler>? handlerFactory = null;
			StatSet handlerParams = StatSet.EMPTY_STATSET;
			if (xmlDailyMission.Handler is not null)
			{
				handlerFactory = DailyMissionHandler.getInstance().getHandler(xmlDailyMission.Handler.Name);
				if (handlerFactory is null)
				{
					_logger.Error($"{GetType().Name}: Unknown handler '{xmlDailyMission.Handler.Name}' for daily mission id={xmlDailyMission.Id}.");
					continue;
				}

				handlerParams = new();
				foreach (XmlDailyMissionHandlerParam xmlDailyMissionHandlerParam in xmlDailyMission.Handler.Parameters)
					handlerParams.set(xmlDailyMissionHandlerParam.Name, xmlDailyMissionHandlerParam.Value);
			}

			DailyMissionDataHolder holder = new(xmlDailyMission.Id, xmlDailyMission.RequiredCompletion,
				xmlDailyMission.DailyReset, xmlDailyMission.IsOneTime, xmlDailyMission.IsMainClassOnly,
				xmlDailyMission.IsDualClassOnly, xmlDailyMission.IsDisplayedWhenNotAvailable, xmlDailyMission.Duration,
				characterClasses, rewardItems, handlerFactory, handlerParams);

			if (!dailyMissionRewards.TryAdd(holder.getId(), holder))
				_logger.Error($"{GetType().Name}: Duplicated daily mission id={holder.getId()}.");
		}

		_dailyMissionRewards = dailyMissionRewards.ToFrozenDictionary();

		_logger.Info(GetType().Name + ": Loaded " + _dailyMissionRewards.Count + " one day rewards.");
	}

	public ImmutableArray<DailyMissionDataHolder> getDailyMissionData()
	{
		return _dailyMissionRewards.Values;
	}

	public List<DailyMissionDataHolder> getDailyMissionData(Player player)
	{
		List<DailyMissionDataHolder> missionData = [];
		foreach (DailyMissionDataHolder mission in _dailyMissionRewards.Values)
		{
			if (mission.isDisplayable(player))
				missionData.Add(mission);
		}

		return missionData;
	}

	public DailyMissionDataHolder? getDailyMissionData(int id)
	{
		return _dailyMissionRewards.GetValueOrDefault(id);
	}

	public bool isAvailable()
	{
		return _dailyMissionRewards.Count != 0;
	}

	/**
	 * Gets the single instance of DailyMissionData.
	 * @return single instance of DailyMissionData
	 */
	public static DailyMissionData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly DailyMissionData INSTANCE = new();
	}
}