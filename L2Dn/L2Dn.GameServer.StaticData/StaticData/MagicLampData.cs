using System.Collections.Immutable;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.StaticData.Xml.MagicLamp;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class MagicLampData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(MagicLampData));
    private ImmutableArray<MagicLampDataHolder> _lamps = ImmutableArray<MagicLampDataHolder>.Empty;

    private MagicLampData()
    {
    }

    public static MagicLampData Instance { get; } = new();

    public void Load()
    {
        XmlMagicLampData xmlMagicLampData = XmlLoader.LoadXmlDocument<XmlMagicLampData>("MagicLampData.xml");
        _lamps = xmlMagicLampData.LevelRanges.SelectMany(x =>
                x.Lamps.Select(y => new MagicLampDataHolder(y.Type, y.Exp, y.Sp, y.Chance, x.FromLevel, x.ToLevel))).
            ToImmutableArray();

        _logger.Info($"{nameof(MagicLampData)}: Loaded {_lamps.Length} magic lamps exp types.");
    }

    public ImmutableArray<MagicLampDataHolder> Lamps => _lamps;
}