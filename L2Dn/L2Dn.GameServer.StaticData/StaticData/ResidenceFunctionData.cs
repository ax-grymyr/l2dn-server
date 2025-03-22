using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.StaticData.Xml.ResidenceFunctions;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class ResidenceFunctionData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(ResidenceFunctionData));

    // Key: id, level
    private FrozenDictionary<(int, int), ResidenceFunctionTemplate> _functionsByIdLevel =
        FrozenDictionary<(int, int), ResidenceFunctionTemplate>.Empty;

    private FrozenDictionary<int, ImmutableArray<ResidenceFunctionTemplate>> _functions =
        FrozenDictionary<int, ImmutableArray<ResidenceFunctionTemplate>>.Empty;

    private ResidenceFunctionData()
    {
    }

    public static ResidenceFunctionData Instance { get; } = new();

    public void Load()
    {
        XmlResidenceFunctionList xmlResidenceFunctionList =
            XmlLoader.LoadXmlDocument<XmlResidenceFunctionList>("ResidenceFunctions.xml");

        _functionsByIdLevel = xmlResidenceFunctionList.Items.SelectMany(item =>
                item.Functions.Select(f => new ResidenceFunctionTemplate(item.Id, f.Level, item.Type,
                    new ItemHolder(f.ItemId, f.ItemCount), TimeUtil.ParseDuration(f.Duration), f.Value))).
            ToFrozenDictionary(x => (x.Id, x.Level));

        _functions = _functionsByIdLevel.Values.GroupBy(x => x.Id).
            Select(g => KeyValuePair.Create(g.Key, g.OrderBy(x => x.Level).ToImmutableArray())).
            ToFrozenDictionary();

        _logger.Info($"{nameof(ResidenceFunctionData)}: Loaded {_functions.Count} functions.");
    }

    public ResidenceFunctionTemplate? GetFunction(int id, int level) =>
        _functionsByIdLevel.GetValueOrDefault((id, level));

    public ImmutableArray<ResidenceFunctionTemplate> GetFunctions(int id) =>
        _functions.GetValueOrDefault(id, ImmutableArray<ResidenceFunctionTemplate>.Empty);
}