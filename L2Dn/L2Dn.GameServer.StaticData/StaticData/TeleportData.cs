using System.Collections.Frozen;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.StaticData.Xml.Teleports;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class TeleportData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(TeleportData));
    private FrozenDictionary<int, TeleportListHolder> _teleports = FrozenDictionary<int, TeleportListHolder>.Empty;

    private TeleportData()
    {
    }

    public static TeleportData Instance { get; } = new();

    public void Load()
    {
        _teleports = XmlLoader.LoadXmlDocument<XmlTeleportList>("TeleportListData.xml").Teleports.
            Select(xmlTeleport => new TeleportListHolder(xmlTeleport.Id,
                new Location3D(xmlTeleport.X, xmlTeleport.Y, xmlTeleport.Z),
                xmlTeleport.Price, xmlTeleport.Special)).
            ToFrozenDictionary(t => t.TeleportId);

        _logger.Info($"{nameof(TeleportData)}: Loaded {_teleports.Count} teleport locations.");
    }

    public TeleportListHolder? GetTeleport(int teleportId) => _teleports.GetValueOrDefault(teleportId);
}