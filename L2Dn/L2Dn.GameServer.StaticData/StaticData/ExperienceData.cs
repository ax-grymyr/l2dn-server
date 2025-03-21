using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.StaticData;

/// <summary>
/// This class holds the Experience points for each level for players and pets.
/// </summary>
public sealed class ExperienceData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(ExperienceData));
    private ImmutableArray<long> _expTable = ImmutableArray<long>.Empty;
    private ImmutableArray<double> _trainingRateTable = ImmutableArray<double>.Empty;

    private int _maxLevel;
    private int _maxPetLevel;

    /**
     * Instantiates a new experience table.
     */
    private ExperienceData()
    {
    }

    public static ExperienceData Instance { get; } = new();

    /// <summary>
    /// The maximum level acquirable by a player.
    /// </summary>
    public int MaxLevel => _maxLevel;

    /// <summary>
    /// The maximum level acquirable by a pet.
    /// </summary>
    public int MaxPetLevel => _maxPetLevel;

    public void Load()
    {
        XmlExperienceList document = XmlLoader.LoadXmlDocument<XmlExperienceList>("stats/experience.xml");

        int maxLevel = document.MaxLevel;
        int maxPetLevel = document.MaxPetLevel;
        if (maxLevel > Config.Character.PLAYER_MAXIMUM_LEVEL)
            maxLevel = Config.Character.PLAYER_MAXIMUM_LEVEL;

        if (maxPetLevel > maxLevel + 1)
            maxPetLevel = maxLevel + 1; // Pet level should not exceed owner level.

        _maxLevel = maxLevel;
        _maxPetLevel = maxPetLevel;
        _expTable = document.Levels.ToDictionary(x => x.Level, x => x.ToLevel).ToValueArray().ToImmutableArray();
        _trainingRateTable = document.Levels.ToDictionary(x => x.Level, x => x.TrainingRate).ToValueArray().
            ToImmutableArray();

        _logger.Info($"{nameof(ExperienceData)}: Loaded {_expTable.Length} levels.");
        _logger.Info($"{nameof(ExperienceData)}: Max Player Level is {maxLevel - 1}.");
        _logger.Info($"{nameof(ExperienceData)}: Max Pet Level is {maxPetLevel - 1}.");
    }

    /// <summary>
    /// Gets the exp for level.
    /// </summary>
    /// <param name="level">The level required.</param>
    /// <returns>The experience points required to reach the given level.</returns>
    public long GetExpForLevel(int level) => _expTable[Math.Min(level, Config.Character.PLAYER_MAXIMUM_LEVEL)];

    public double GetTrainingRate(int level) =>
        _trainingRateTable[Math.Min(level, Config.Character.PLAYER_MAXIMUM_LEVEL)];
}