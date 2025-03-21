using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.StaticData.Xml.ClanLevels;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class ClanLevelData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(ClanLevelData));
    private ImmutableArray<int> _clanExp = [0]; // 0th level

    private ClanLevelData()
    {
    }

    public static ClanLevelData Instance { get; } = new();

    public void Load()
    {
        XmlClanLevelList document = XmlLoader.LoadXmlDocument<XmlClanLevelList>("ClanLevelData.xml");
        _clanExp = document.ClanLevels.Count != 0
            ? document.ClanLevels.ToDictionary(x => x.Level, x => x.Exp).ToValueArray().ToImmutableArray()
            : [0];

        // TODO Add checks for duplicated levels and that exp must increase with each level.

        _logger.Info($"{nameof(ClanLevelData)}: Loaded {_clanExp.Length - 1} clan level data.");
    }

    public int GetLevelExp(int clanLevel) => _clanExp[clanLevel];
    public int MaxLevel => _clanExp.Length - 1;
    public int MaxExp => _clanExp[^1];
}