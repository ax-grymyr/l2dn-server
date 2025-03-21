using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.StaticData.Xml.Teleporters;
using L2Dn.GameServer.Utilities;
using NLog;
using TeleportLocation = L2Dn.GameServer.Dto.TeleportLocation;

namespace L2Dn.GameServer.StaticData;

public sealed class TeleporterData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(TeleporterData));

    private FrozenDictionary<(int NpcId, string ListName), TeleportHolder> _teleportData =
        FrozenDictionary<(int NpcId, string ListName), TeleportHolder>.Empty;

    private TeleporterData()
    {
    }

    public static TeleporterData Instance { get; } = new();

    public void Load()
    {
        Dictionary<(int NpcId, string ListName), TeleportHolder> teleportData =
            new Dictionary<(int NpcId, string ListName), TeleportHolder>();

        XmlLoader.LoadXmlDocuments<XmlTeleporterList>("teleporters", true).SelectMany(doc => doc.Npcs).SelectMany(npc =>
        {
            List<int> npcs = [npc.Id];
            npcs.AddRange(npc.Npcs.Select(n => n.Id));

            List<TeleportHolder> teleports = npc.Teleports.Select(teleport =>
            {
                TeleportType type = teleport.Type;
                string name = teleport.NameSpecified ? teleport.Name : type.ToString();
                ImmutableArray<TeleportLocation> locations = teleport.Locations.Select((location, index)
                    => new TeleportLocation(index, location)).ToImmutableArray();

                TeleportHolder holder = new(name, type, locations);
                return holder;
            }).ToList();

            return npcs.SelectMany(n => teleports.Select(t => (Key: (NpcId: n, ListName: t.Name), Value: t)));
        }).ForEach(t =>
        {
            if (!teleportData.TryAdd(t.Key, t.Value))
            {
                _logger.Error($"{nameof(TeleporterData)}: Duplicate teleport list ({t.Key.ListName}) " +
                    $"has been found for NPC: {t.Key.NpcId}");
            }
        });

        int teleportersCount = teleportData.Select(t => t.Key.NpcId).Distinct().Count();

        _teleportData = teleportData.ToFrozenDictionary();

        _logger.Info($"{nameof(TeleporterData)}: Loaded {teleportersCount} npc teleporters.");
    }

    /// <summary>
    /// Gets teleport data for specified NPC and list name.
    /// </summary>
    public TeleportHolder? GetHolder(int npcId, string listName) => _teleportData.GetValueOrDefault((npcId, listName));
}