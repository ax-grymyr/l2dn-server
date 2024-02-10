using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Sdw, Mobius
 */
public class DailyMissionData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(DailyMissionData));
	
	private readonly Map<int, List<DailyMissionDataHolder>> _dailyMissionRewards = new();
	private readonly List<DailyMissionDataHolder> _dailyMissionData = new();
	private bool _isAvailable;
	
	protected DailyMissionData()
	{
		load();
	}
	
	public void load()
	{
		_dailyMissionRewards.clear();
		parseDatapackFile("data/DailyMission.xml");
		
		_dailyMissionData.Clear();
		foreach (List<DailyMissionDataHolder> missionList in _dailyMissionRewards.values())
		{
			_dailyMissionData.addAll(missionList);
		}
		
		_isAvailable = !_dailyMissionRewards.isEmpty();
		
		LOGGER.Info(GetType().Name + ": Loaded " + _dailyMissionRewards.size() + " one day rewards.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, "reward", rewardNode =>
		{
			StatSet set = new StatSet(parseAttributes(rewardNode));
			List<ItemHolder> items = new();
			forEach(rewardNode, "items", itemsNode => forEach(itemsNode, "item", itemNode =>
			{
				int itemId = parseInteger(itemNode.getAttributes(), "id");
				int itemCount = parseInteger(itemNode.getAttributes(), "count");
				if ((itemId == AbstractDailyMissionHandler.MISSION_LEVEL_POINTS) && (MissionLevel.getInstance().getCurrentSeason() <= 0))
				{
					return;
				}
				items.add(new ItemHolder(itemId, itemCount));
			}));
			
			set.set("items", items);
			
			List<ClassId> classRestriction = new();
			forEach(rewardNode, "classId", classRestrictionNode => classRestriction.add(ClassId.getClassId(int.Parse(classRestrictionNode.getTextContent()))));
			set.set("classRestriction", classRestriction);
			
			// Initial values in case handler doesn't exists
			set.set("handler", "");
			set.set("params", StatSet.EMPTY_STATSET);
			
			// Parse handler and parameters
			forEach(rewardNode, "handler", handlerNode =>
			{
				set.set("handler", parseString(handlerNode.getAttributes(), "name"));
				
				StatSet @params = new StatSet();
				set.set("params", @params);
				forEach(handlerNode, "param", paramNode => @params.set(parseString(paramNode.getAttributes(), "name"), paramNode.getTextContent()));
			});
			
			DailyMissionDataHolder holder = new DailyMissionDataHolder(set);
			_dailyMissionRewards.computeIfAbsent(holder.getId(), k => new()).add(holder);
		}));
	}
	
	public ICollection<DailyMissionDataHolder> getDailyMissionData()
	{
		return _dailyMissionData;
	}
	
	public ICollection<DailyMissionDataHolder> getDailyMissionData(Player player)
	{
		List<DailyMissionDataHolder> missionData = new();
		foreach (DailyMissionDataHolder mission in _dailyMissionData)
		{
			if (mission.isDisplayable(player))
			{
				missionData.add(mission);
			}
		}
		return missionData;
	}
	
	public ICollection<DailyMissionDataHolder> getDailyMissionData(int id)
	{
		return _dailyMissionRewards.get(id);
	}
	
	public bool isAvailable()
	{
		return _isAvailable;
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