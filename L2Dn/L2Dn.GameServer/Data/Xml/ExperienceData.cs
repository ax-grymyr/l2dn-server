using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class holds the Experience points for each level for players and pets.
 * @author mrTJO
 */
public class ExperienceData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ExperienceData));
	
	private readonly Map<int, long> _expTable = new();
	private readonly Map<int, Double> _traningRateTable = new();
	
	private int MAX_LEVEL;
	private int MAX_PET_LEVEL;
	
	/**
	 * Instantiates a new experience table.
	 */
	protected ExperienceData()
	{
		load();
	}
	
	public void load()
	{
		_expTable.clear();
		_traningRateTable.clear();
		
		string filePath = Path.Combine(Config.DATAPACK_ROOT_PATH, "data/stats/experience.xml");
		using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		XDocument document = XDocument.Load(stream);
		document.Elements("table").ForEach(el =>
		{
			MAX_LEVEL = el.Attribute("maxLevel").GetInt32() + 1;
			MAX_PET_LEVEL = el.Attribute("maxPetLevel").GetInt32() + 1;
			if (MAX_LEVEL > Config.PLAYER_MAXIMUM_LEVEL)
			{
				MAX_LEVEL = Config.PLAYER_MAXIMUM_LEVEL;
			}
			if (MAX_PET_LEVEL > (MAX_LEVEL + 1))
			{
				MAX_PET_LEVEL = MAX_LEVEL + 1; // Pet level should not exceed owner level.
			}
			
			el.Elements("experience").ForEach(parseElement);
		});
		
		LOGGER.Info(GetType().Name + ": Loaded " + _expTable.size() + " levels.");
		LOGGER.Info(GetType().Name + ": Max Player Level is " + (MAX_LEVEL - 1) + ".");
		LOGGER.Info(GetType().Name + ": Max Pet Level is " + (MAX_PET_LEVEL - 1) + ".");
	}
	
	private void parseElement(XElement element)
	{
		int maxLevel = element.Attribute("level").GetInt32();
		if (maxLevel > Config.PLAYER_MAXIMUM_LEVEL)
		{
			return;
		}

		long toLevel = element.Attribute("tolevel").GetInt64();
		double trainingRate = element.Attribute("trainingRate").GetDouble();
		_expTable.put(maxLevel, toLevel);
		_traningRateTable.put(maxLevel, trainingRate);
	}
	
	/**
	 * Gets the exp for level.
	 * @param level the level required.
	 * @return the experience points required to reach the given level.
	 */
	public long getExpForLevel(int level)
	{
		if (level > Config.PLAYER_MAXIMUM_LEVEL)
		{
			return _expTable.get(Config.PLAYER_MAXIMUM_LEVEL);
		}
		return _expTable.get(level);
	}
	
	public double getTrainingRate(int level)
	{
		if (level > Config.PLAYER_MAXIMUM_LEVEL)
		{
			return _traningRateTable.get(Config.PLAYER_MAXIMUM_LEVEL);
		}
		return _traningRateTable.get(level);
	}
	
	/**
	 * Gets the max level.
	 * @return the maximum level acquirable by a player.
	 */
	public int getMaxLevel()
	{
		return MAX_LEVEL;
	}
	
	/**
	 * Gets the max pet level.
	 * @return the maximum level acquirable by a pet.
	 */
	public int getMaxPetLevel()
	{
		return MAX_PET_LEVEL;
	}
	
	/**
	 * Gets the single instance of ExperienceTable.
	 * @return single instance of ExperienceTable
	 */
	public static ExperienceData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly ExperienceData INSTANCE = new();
	}
}