﻿using System.Globalization;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model;

public class DailyMissionDataHolder
{
	private readonly int _id;
	private readonly List<ItemHolder> _rewardsItems = new();
	private readonly List<CharacterClass> _classRestriction = new();
	private readonly int _requiredCompletions;
	private readonly StatSet _params = new();
	private readonly bool _dailyReset;
	private readonly bool _isOneTime;
	private readonly bool _isMainClassOnly;
	private readonly bool _isDualClassOnly;
	private readonly bool _isDisplayedWhenNotAvailable;
	private readonly AbstractDailyMissionHandler? _handler;
	private readonly MissionResetType _missionResetSlot;
	
	public DailyMissionDataHolder(XElement element)
	{
		_id = element.GetAttributeValueAsInt32("id");
		_requiredCompletions = element.Attribute("requiredCompletion").GetInt32(0);
		_dailyReset = element.Attribute("dailyReset").GetBoolean(true);
		_isOneTime = element.Attribute("isOneTime").GetBoolean(true);
		_isMainClassOnly = element.Attribute("isMainClassOnly").GetBoolean(true);
		_isDualClassOnly = element.Attribute("isDualClassOnly").GetBoolean(false);
		_isDisplayedWhenNotAvailable = element.Attribute("isDisplayedWhenNotAvailable").GetBoolean(true);
		_missionResetSlot = element.Attribute("duration").GetEnum(MissionResetType.DAY);

		// reward items
		element.Elements("items").Elements("item").ForEach(el =>
		{
			int itemId = el.GetAttributeValueAsInt32("id");
			int itemCount = el.GetAttributeValueAsInt32("count");
			if ((itemId == AbstractDailyMissionHandler.MISSION_LEVEL_POINTS) && (MissionLevel.getInstance().getCurrentSeason() <= 0))
			{
				return;
			}
				
			_rewardsItems.add(new ItemHolder(itemId, itemCount));
		});
		
		// class restrictions
		element.Elements("classId").ForEach(el =>
		{
			CharacterClass classId = (CharacterClass)(int)el;
			_classRestriction.add(classId);
		});
		
		// handler
		XElement? handlerEl = element.Elements("handler").SingleOrDefault();
		if (handlerEl is not null)
		{
			string handlerName = handlerEl.GetAttributeValueAsString("name");
			Func<DailyMissionDataHolder, AbstractDailyMissionHandler> handler =
				DailyMissionHandler.getInstance().getHandler(handlerName);

			handlerEl.Elements("param").ForEach(paramEl =>
			{
				string paramName = paramEl.GetAttributeValueAsString("name");
				string paramValue = paramEl.Value;
				_params.set(paramName, paramValue);
			});
			
			_handler = handler != null ? handler(this) : null;
		}
	}
	
	public int getId()
	{
		return _id;
	}
	
	public List<CharacterClass> getClassRestriction()
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
		
		DailyMissionStatus status = getStatus(player);
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
			if ((_missionResetSlot == MissionResetType.WEEK) && (DateTime.Now.DayOfWeek == DayOfWeek.Monday))
			{
				_handler.reset();
			}
			else if ((_missionResetSlot == MissionResetType.MONTH) && (DateTime.Now.Day == 1))
			{
				_handler.reset();
			}
			else if ((_missionResetSlot == MissionResetType.WEEKEND) && (DateTime.Now.DayOfWeek == DayOfWeek.Saturday))
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