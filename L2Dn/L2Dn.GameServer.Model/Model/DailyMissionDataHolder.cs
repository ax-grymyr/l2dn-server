using System.Collections.Immutable;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model;

public class DailyMissionDataHolder
{
	private readonly int _id;
	private readonly ImmutableArray<CharacterClass> _classRestriction;
	private readonly int _requiredCompletions;
	private readonly bool _dailyReset;
	private readonly bool _isOneTime;
	private readonly bool _isMainClassOnly;
	private readonly bool _isDualClassOnly;
	private readonly bool _isDisplayedWhenNotAvailable;
	private readonly MissionResetType _missionResetType;
	private readonly ImmutableArray<ItemHolder> _rewardItems;
	private readonly AbstractDailyMissionHandler? _handler;
	private readonly StatSet _params;

	public DailyMissionDataHolder(int id, int requiredCompletions, bool dailyReset, bool isOneTime,
		bool isMainClassOnly, bool isDualClassOnly, bool isDisplayedWhenNotAvailable, MissionResetType missionResetType,
		ImmutableArray<CharacterClass> characterClasses, ImmutableArray<ItemHolder> rewardItems,
		Func<DailyMissionDataHolder, AbstractDailyMissionHandler>? handlerFactory, StatSet handlerParams)
	{
		_id = id;
		_requiredCompletions = requiredCompletions;
		_dailyReset = dailyReset;
		_isOneTime = isOneTime;
		_isMainClassOnly = isMainClassOnly;
		_isDualClassOnly = isDualClassOnly;
		_isDisplayedWhenNotAvailable = isDisplayedWhenNotAvailable;
		_missionResetType = missionResetType;
		_classRestriction = characterClasses;
		_rewardItems = rewardItems;
		_params = handlerParams;
		_handler = handlerFactory?.Invoke(this);
	}

	public int getId()
	{
		return _id;
	}

	public ImmutableArray<CharacterClass> getClassRestriction()
	{
		return _classRestriction;
	}

	public ImmutableArray<ItemHolder> getRewards()
	{
		return _rewardItems;
	}

	public int getRequiredCompletions()
	{
		return _requiredCompletions;
	}

	public StatSet getParams()
	{
		return _params;
	}

	public bool dailyReset()
	{
		return _dailyReset;
	}

	public bool isOneTime()
	{
		return _isOneTime;
	}

	public bool isMainClassOnly()
	{
		return _isMainClassOnly;
	}

	public bool isDualClassOnly()
	{
		return _isDualClassOnly;
	}

	public bool isDisplayedWhenNotAvailable()
	{
		return _isDisplayedWhenNotAvailable;
	}

	public bool isDisplayable(Player player)
	{
		// Check if its main class only
		if (isMainClassOnly() && (player.isSubClassActive() || player.isDualClassActive()))
		{
			return false;
		}

		// Check if its dual class only.
		if (isDualClassOnly() && !player.isDualClassActive())
		{
			return false;
		}

		// Check for specific class restrictions
		if (!_classRestriction.IsDefaultOrEmpty && !_classRestriction.Contains(player.getClassId()))
		{
			return false;
		}

		DailyMissionStatus status = getStatus(player);
		if (!isDisplayedWhenNotAvailable() && status == DailyMissionStatus.NOT_AVAILABLE)
		{
			return false;
		}

		// Show only if its repeatable, recently completed or incompleted that has met the checks above.
		return !isOneTime() || isRecentlyCompleted(player) || status != DailyMissionStatus.COMPLETED;
	}

	public void requestReward(Player player)
	{
		if (_handler != null && isDisplayable(player))
		{
			_handler.requestReward(player);
		}
	}

	public DailyMissionStatus getStatus(Player player)
	{
		if (_handler is null)
			return DailyMissionStatus.NOT_AVAILABLE;

		return player.getDailyMissions().getStatus(_id);
	}

	public int getProgress(Player player)
	{
		if (_handler is null)
			return 0;

		return _handler.getProgress(player);
	}

	public bool isRecentlyCompleted(Player player)
	{
		if (_handler is null)
			return false;

		return player.getDailyMissions().isRecentlyCompleted(_id);
	}

	public void reset()
	{
		if (_handler != null)
		{
			if (_missionResetType == MissionResetType.WEEK && DateTime.Now.DayOfWeek == DayOfWeek.Monday)
			{
				_handler.reset();
			}
			else if (_missionResetType == MissionResetType.MONTH && DateTime.Now.Day == 1)
			{
				_handler.reset();
			}
			else if (_missionResetType == MissionResetType.WEEKEND && DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
			{
				_handler.reset();
			}
			else if (_dailyReset)
			{
				_handler.reset();
			}
		}
	}
}