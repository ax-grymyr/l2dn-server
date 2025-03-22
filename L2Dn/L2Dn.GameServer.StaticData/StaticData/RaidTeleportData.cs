using System.Collections.Frozen;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.StaticData.Xml.Teleports;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class RaidTeleportData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(RaidTeleportData));
    private FrozenDictionary<int, TeleportListHolder> _teleports = FrozenDictionary<int, TeleportListHolder>.Empty;

    private RaidTeleportData()
    {
    }

    public static RaidTeleportData Instance { get; } = new();

    public void Load()
    {
        _teleports = XmlLoader.LoadXmlDocument<XmlTeleportList>("RaidTeleportListData.xml").Teleports.
            Select(xmlTeleport => new TeleportListHolder(xmlTeleport.Id,
                new Location3D(xmlTeleport.X, xmlTeleport.Y, xmlTeleport.Z),
                xmlTeleport.Price, xmlTeleport.Special)).
            ToFrozenDictionary(t => t.TeleportId);

        _logger.Info($"{nameof(TeleportData)}: Loaded {_teleports.Count} raid teleport locations.");
    }

    public TeleportListHolder? GetTeleport(int teleportId) => _teleports.GetValueOrDefault(teleportId);
}