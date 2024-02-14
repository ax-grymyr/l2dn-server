using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Index
 */
public class MissionLevel
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(MissionLevel));
	
	private readonly Map<int, MissionLevelHolder> _template = new();
	private int _currentSeason;
	
	protected MissionLevel()
	{
		load();
	}
	
	public void load()
	{
		_template.clear();
		
		string filePath = Path.Combine(Config.DATAPACK_ROOT_PATH, "data/MissionLevel.xml");
		using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		XDocument document = XDocument.Load(stream);
		document.Elements("list").Elements("current").ForEach(el => _currentSeason = el.Attribute("season").GetInt32());
		document.Elements("list").Elements("missionLevel").ForEach(parseElement);
		
		if (_currentSeason > 0)
		{
			LOGGER.Info(GetType().Name + ": Loaded " + _template.size() + " seasons.");
		}
		else
		{
			_template.clear();
		}
	}

	private void parseElement(XElement element)
	{
		int season = element.Attribute("season").GetInt32();
		int maxLevel = element.Attribute("maxLevel").GetInt32();
		bool bonusRewardIsAvailable = element.Attribute("bonusRewardIsAvailable").GetBoolean();
		bool bonusRewardByLevelUP = element.Attribute("bonusRewardByLevelUP").GetBoolean();

		Map<int, ItemHolder> keyReward = new();
		Map<int, ItemHolder> normalReward = new();
		Map<int, int> xpForLevel = new();
		ItemHolder specialReward = null;
		ItemHolder bonusReward = null;

		element.Elements("expTable").Elements("exp").ForEach(el =>
		{
			int level = el.Attribute("level").GetInt32();
			int amount = el.Attribute("amount").GetInt32();
			xpForLevel.put(level, amount);
		});

		element.Elements("baseRewards").Elements("baseReward").ForEach(el =>
		{
			int level = el.Attribute("level").GetInt32();
			int itemId = el.Attribute("itemId").GetInt32();
			long itemCount = el.Attribute("itemCount").GetInt64();
			normalReward.put(level, new ItemHolder(itemId, itemCount));
		});

		element.Elements("keyRewards").Elements("keyReward").ForEach(el =>
		{
			int level = el.Attribute("level").GetInt32();
			int itemId = el.Attribute("itemId").GetInt32();
			long itemCount = el.Attribute("itemCount").GetInt64();
			keyReward.put(level, new ItemHolder(itemId, itemCount));
		});

		element.Elements("specialReward").ForEach(el =>
		{
			int itemId = el.Attribute("itemId").GetInt32();
			long itemCount = el.Attribute("itemCount").GetInt64();
			specialReward = new ItemHolder(itemId, itemCount);
		});

		element.Elements("bonusReward").ForEach(el =>
		{
			int itemId = el.Attribute("itemId").GetInt32();
			long itemCount = el.Attribute("itemCount").GetInt64();
			bonusReward = new ItemHolder(itemId, itemCount);
		});

		int bonusLevel = normalReward.Count == 0 ? maxLevel : normalReward.Keys.Max();
		if (bonusLevel == maxLevel)
			bonusLevel--;

		_template.put(season,
			new MissionLevelHolder(maxLevel, bonusLevel + 1, xpForLevel, normalReward, keyReward, specialReward,
				bonusReward, bonusRewardByLevelUP, bonusRewardIsAvailable));
	}

	public int getCurrentSeason()
	{
		return _currentSeason;
	}
	
	public MissionLevelHolder getMissionBySeason(int season)
	{
		return _template.getOrDefault(season, null);
	}
	
	public static MissionLevel getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly MissionLevel INSTANCE = new();
	}
}