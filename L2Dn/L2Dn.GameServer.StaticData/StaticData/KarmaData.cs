using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.StaticData.Xml.PcKarmaIncrease;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class KarmaData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(KarmaData));
    private static ImmutableArray<double> _karmaTable = ImmutableArray<double>.Empty;

    private KarmaData()
    {
    }

    public static KarmaData Instance { get; } = new();

    public void Load()
    {
        Dictionary<int, double> values = XmlLoader.
            LoadXmlDocument<XmlPcKarmaIncreaseData>("stats/chars/pcKarmaIncrease.xml").
            Levels.ToDictionary(el => el.Level, el => el.Value);

        _karmaTable = values.ToValueArray().ToImmutableArray();

        _logger.Info($"{nameof(KarmaData)}: Loaded {values.Count} karma modifiers.");
    }

    /// <summary>
    /// The modifier used to calculate karma lost upon death.
    /// </summary>
    public double GetMultiplier(int level) => _karmaTable[level];
}