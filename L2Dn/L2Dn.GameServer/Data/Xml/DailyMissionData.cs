using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
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
		
		string filePath = Path.Combine(Config.DATAPACK_ROOT_PATH, "data/DailyMission.xml");
		using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		XDocument document = XDocument.Load(stream);
		document.Root?.Elements("reward").ForEach(loadElement);
		
		_dailyMissionData.Clear();
		foreach (List<DailyMissionDataHolder> missionList in _dailyMissionRewards.values())
		{
			_dailyMissionData.AddRange(missionList);
		}
		
		_isAvailable = !_dailyMissionRewards.isEmpty();
		
		LOGGER.Info(GetType().Name + ": Loaded " + _dailyMissionRewards.size() + " one day rewards.");
	}

	private void loadElement(XElement element)
	{
		DailyMissionDataHolder holder = new DailyMissionDataHolder(element);
		_dailyMissionRewards.computeIfAbsent(holder.getId(), k => new()).add(holder);
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