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
public class MissionLevel: DataReaderBase
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
		_template.Clear();
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "MissionLevel.xml");
		document.Elements("list").Elements("current").ForEach(el => _currentSeason = el.GetAttributeValueAsInt32("season"));
		document.Elements("list").Elements("missionLevel").ForEach(parseElement);
		
		if (_currentSeason > 0)
		{
			LOGGER.Info(GetType().Name + ": Loaded " + _template.size() + " seasons.");
		}
		else
		{
			_template.Clear();
		}
	}

	private void parseElement(XElement element)
	{
		int season = element.GetAttributeValueAsInt32("season");
		int maxLevel = element.GetAttributeValueAsInt32("maxLevel");
		bool bonusRewardIsAvailable = element.GetAttributeValueAsBoolean("bonusRewardIsAvailable");
		bool bonusRewardByLevelUP = element.GetAttributeValueAsBoolean("bonusRewardByLevelUP");

		Map<int, ItemHolder> keyReward = new();
		Map<int, ItemHolder> normalReward = new();
		Map<int, int> xpForLevel = new();
		ItemHolder specialReward = null;
		ItemHolder bonusReward = null;

		element.Elements("expTable").Elements("exp").ForEach(el =>
		{
			int level = el.GetAttributeValueAsInt32("level");
			int amount = el.GetAttributeValueAsInt32("amount");
			xpForLevel.put(level, amount);
		});

		element.Elements("baseRewards").Elements("baseReward").ForEach(el =>
		{
			int level = el.GetAttributeValueAsInt32("level");
			int itemId = el.GetAttributeValueAsInt32("itemId");
			long itemCount = el.GetAttributeValueAsInt64("itemCount");
			normalReward.put(level, new ItemHolder(itemId, itemCount));
		});

		element.Elements("keyRewards").Elements("keyReward").ForEach(el =>
		{
			int level = el.GetAttributeValueAsInt32("level");
			int itemId = el.GetAttributeValueAsInt32("itemId");
			long itemCount = el.GetAttributeValueAsInt64("itemCount");
			keyReward.put(level, new ItemHolder(itemId, itemCount));
		});

		element.Elements("specialReward").ForEach(el =>
		{
			int itemId = el.GetAttributeValueAsInt32("itemId");
			long itemCount = el.GetAttributeValueAsInt64("itemCount");
			specialReward = new ItemHolder(itemId, itemCount);
		});

		element.Elements("bonusReward").ForEach(el =>
		{
			int itemId = el.GetAttributeValueAsInt32("itemId");
			long itemCount = el.GetAttributeValueAsInt64("itemCount");
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
	
	public MissionLevelHolder? getMissionBySeason(int season)
	{
		return _template.GetValueOrDefault(season);
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