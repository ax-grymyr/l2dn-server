using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.StaticData.Xml.Sayune;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class SayuneData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(SayuneData));

    private FrozenDictionary<int, SayuneEntry> _maps = FrozenDictionary<int, SayuneEntry>.Empty;

    private SayuneData()
    {
    }

    public static SayuneData Instance { get; } = new();

    public void Load()
    {
        XmlSayuneData xmlSayuneData = XmlLoader.LoadXmlDocument<XmlSayuneData>("SayuneData.xml");

        _maps = xmlSayuneData.Items.Select(xmlSayuneItem => new SayuneEntry(xmlSayuneItem)).
            ToFrozenDictionary(x => x.Id);

        _logger.Info(GetType().Name + ": Loaded " + _maps.Count + " maps.");
    }

    public SayuneEntry? GetMap(int id) => _maps.GetValueOrDefault(id);
    public ImmutableArray<SayuneEntry> Maps => _maps.Values;
}