using System.Globalization;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model;

public class DailyMissionDataHolder
{
	private readonly int _id;
	private readonly List<ItemHolder> _rewardsItems;
	private readonly List<ClassId> _classRestriction;
	private readonly int _requiredCompletions;
	private readonly StatSet _params;
	private readonly bool _dailyReset;
	private readonly bool _isOneTime;
	private readonly bool _isMainClassOnly;
	private readonly bool _isDualClassOnly;
	private readonly bool _isDisplayedWhenNotAvailable;
	private readonly AbstractDailyMissionHandler _handler;
	private readonly MissionResetType _missionResetSlot;
	
	public DailyMissionDataHolder(StatSet set)
	{
		Func<DailyMissionDataHolder, AbstractDailyMissionHandler> handler = DailyMissionHandler.getInstance().getHandler(set.getString("handler"));
		_id = set.getInt("id");
		_requiredCompletions = set.getInt("requiredCompletion", 0);
		_rewardsItems = set.getList<ItemHolder>("items");
		_classRestriction = set.getList<ClassId>("classRestriction");
		_params = set.getObject<StatSet>("params");
		_dailyReset = set.getBoolean("dailyReset", true);
		_isOneTime = set.getBoolean("isOneTime", true);
		_isMainClassOnly = set.getBoolean("isMainClassOnly", true);
		_isDualClassOnly = set.getBoolean("isDualClassOnly", false);
		_isDisplayedWhenNotAvailable = set.getBoolean("isDisplayedWhenNotAvailable", true);
		_handler = handler != null ? handler(this) : null;
		_missionResetSlot = set.getEnum("duration", MissionResetType.DAY);
	}
	
	public int getId()
	{
		return _id;
	}
	
	public List<ClassId> getClassRestriction()
	{
		return _classRestriction;
	}
	
	public List<ItemHolder> getRewards()
	{
		return _rewardsItems;
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
		if (!_classRestriction.isEmpty() && !_classRestriction.Contains(player.getClassId()))
		{
			return false;
		}
		
		int status = getStatus(player);
		if (!isDisplayedWhenNotAvailable() && (status == DailyMissionStatus.NOT_AVAILABLE))
		{
			return false;
		}
		
		// Show only if its repeatable, recently completed or incompleted that has met the checks above.
		return (!isOneTime() || isRecentlyCompleted(player) || (status != DailyMissionStatus.COMPLETED));
	}
	
	public void requestReward(Player player)
	{
		if ((_handler != null) && isDisplayable(player))
		{
			_handler.requestReward(player);
		}
	}
	
	public DailyMissionStatus getStatus(Player player)
	{
		return _handler != null ? _handler.getStatus(player) : DailyMissionStatus.NOT_AVAILABLE;
	}
	
	public DailyMissionStatus getProgress(Player player)
	{
		return _handler != null ? _handler.getProgress(player) : DailyMissionStatus.NOT_AVAILABLE;
	}
	
	public bool isRecentlyCompleted(Player player)
	{
		return (_handler != null) && _handler.isRecentlyCompleted(player);
	}
	
	public void reset()
	{
		if (_handler != null)
		{
			if ((_missionResetSlot == MissionResetType.WEEK) && (Calendar.getInstance().get(Calendar.DAY_OF_WEEK) == Calendar.MONDAY))
			{
				_handler.reset();
			}
			else if ((_missionResetSlot == MissionResetType.MONTH) && (Calendar.getInstance().get(Calendar.DAY_OF_MONTH) == 1))
			{
				_handler.reset();
			}
			else if ((_missionResetSlot == MissionResetType.WEEKEND) && (Calendar.getInstance().get(Calendar.DAY_OF_WEEK) == Calendar.SATURDAY))
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
