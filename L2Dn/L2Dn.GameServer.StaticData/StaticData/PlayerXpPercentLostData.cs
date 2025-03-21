using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.StaticData;

/// <summary>
/// This class holds the Player Xp Percent Lost Data for each level for players.
/// </summary>
public sealed class PlayerXpPercentLostData: DataReaderBase
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(PlayerXpPercentLostData));
    private ImmutableArray<double> _playerXpPercentLost = ImmutableArray<double>.Empty;

    private PlayerXpPercentLostData()
    {
    }

    public static PlayerXpPercentLostData Instance { get; } = new();

    public void Load()
    {
        _playerXpPercentLost = XmlLoader.
            LoadXmlDocument<XmlPlayerXpPercentLostList>("stats/chars/playerXpPercentLost.xml").
            Levels.ToDictionary(x => x.Level, x => x.Value).ToValueArray().ToImmutableArray();
    }

    public double GetXpPercent(int level)
    {
        if (level > _playerXpPercentLost.Length)
        {
            _logger.Warn("Require to high level inside PlayerXpPercentLostData (" + level + ")");
            return 1.0;
        }

        return _playerXpPercentLost[level];
    }
}