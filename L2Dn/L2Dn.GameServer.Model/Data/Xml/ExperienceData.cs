using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/// <summary>
/// This class holds the Experience points for each level for players and pets.
/// </summary>
public class ExperienceData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(ExperienceData));
	private static ImmutableArray<long> _expTable = ImmutableArray<long>.Empty;
	private static ImmutableArray<double> _trainingRateTable = ImmutableArray<double>.Empty;
	
	private static int _maxLevel;
	private static int _maxPetLevel;
	
	/**
	 * Instantiates a new experience table.
	 */
	protected ExperienceData()
	{
		load();
	}
	
	public void load()
	{
		XmlExperienceData document = LoadXmlDocument<XmlExperienceData>(DataFileLocation.Data, "stats/experience.xml");

		int maxLevel = document.MaxLevel;
		int maxPetLevel = document.MaxPetLevel;
		if (maxLevel > Config.PLAYER_MAXIMUM_LEVEL)
			maxLevel = Config.PLAYER_MAXIMUM_LEVEL;

		if (maxPetLevel > maxLevel + 1)
			maxPetLevel = maxLevel + 1; // Pet level should not exceed owner level.

		_maxLevel = maxLevel;
		_maxPetLevel = maxPetLevel;
		_expTable = document.Levels.ToDictionary(x => x.Level, x => x.ToLevel).ToValueArray().ToImmutableArray();
		_trainingRateTable = document.Levels.ToDictionary(x => x.Level, x => x.TrainingRate).ToValueArray()
			.ToImmutableArray();
		
		_logger.Info($"{GetType().Name}: Loaded {_expTable.Length} levels.");
		_logger.Info($"{GetType().Name}: Max Player Level is {maxLevel - 1}.");
		_logger.Info($"{GetType().Name}: Max Pet Level is {maxPetLevel - 1}.");
	}
	
	/**
	 * Gets the exp for level.
	 * @param level the level required.
	 * @return the experience points required to reach the given level.
	 */
	public long getExpForLevel(int level)
	{
		if (level > Config.PLAYER_MAXIMUM_LEVEL)
			return _expTable[Config.PLAYER_MAXIMUM_LEVEL];
		
		return _expTable[level];
	}
	
	public double getTrainingRate(int level)
	{
		if (level > Config.PLAYER_MAXIMUM_LEVEL)
			return _trainingRateTable[Config.PLAYER_MAXIMUM_LEVEL];

		return _trainingRateTable[level];
	}
	
	/**
	 * Gets the max level.
	 * @return the maximum level acquirable by a player.
	 */
	public int getMaxLevel()
	{
		return _maxLevel;
	}
	
	/**
	 * Gets the max pet level.
	 * @return the maximum level acquirable by a pet.
	 */
	public int getMaxPetLevel()
	{
		return _maxPetLevel;
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