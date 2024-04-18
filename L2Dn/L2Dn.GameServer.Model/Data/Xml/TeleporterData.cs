using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Teleporters;
using L2Dn.Model.DataPack;
using NLog;
using TeleportLocation = L2Dn.GameServer.Model.Teleporters.TeleportLocation;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class TeleporterData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(TeleporterData));

	private readonly record struct TeleportKey(int NpcId, string ListName);

	private FrozenDictionary<TeleportKey, TeleportHolder> _teleportData =
		FrozenDictionary<TeleportKey, TeleportHolder>.Empty;
	
	private TeleporterData()
	{
		load();
	}
	
	public void load()
	{
		Dictionary<TeleportKey, TeleportHolder> teleportData = new Dictionary<TeleportKey, TeleportHolder>();
		LoadXmlDocuments<XmlTeleportData>(DataFileLocation.Data, "teleporters", true)
			.SelectMany(data => data.Document.Npcs)
			.SelectMany(npc =>
			{
				List<int> npcs = [npc.Id];
				npcs.AddRange(npc.Npcs.Select(n => n.Id));

				List<TeleportHolder> teleports = npc.Teleports.Select(teleport =>
				{
					TeleportType type = Enum.Parse<TeleportType>(teleport.Type);
					string name = teleport.NameSpecified ? teleport.Name : teleport.Type.ToString();

					ImmutableArray<TeleportLocation> locations = teleport.Locations.Select((location, index)
						=> new TeleportLocation(index, location)).ToImmutableArray();

					TeleportHolder holder = new(name, type, locations);
					return holder;
				}).ToList();

				return npcs.SelectMany(n => teleports.Select(t => (Key: new TeleportKey(n, t.getName()), Value: t)));
			})
			.ForEach(t =>
			{
				if (!teleportData.TryAdd(t.Key, t.Value))
				{
					_logger.Error(nameof(TeleporterData) + ": Duplicate teleport list (" + t.Key.ListName +
					              ") has been found for NPC: " +
					              t.Key.NpcId);
				}
			});

		int teleportersCount = teleportData.Select(t => t.Key.NpcId).Distinct().Count();

		_teleportData = teleportData.ToFrozenDictionary();
		
		_logger.Info(GetType().Name + ": Loaded " + teleportersCount + " npc teleporters.");
	}
	
	/**
	 * Gets teleport data for specified NPC and list name
	 * @param npcId template id of teleporter
	 * @param listName name of teleport list
	 * @return {@link TeleportHolder} if found otherwise {@code null}
	 */
	public TeleportHolder? getHolder(int npcId, string listName)
	{
		return _teleportData.GetValueOrDefault(new TeleportKey(npcId, listName));
	}
	
	/**
	 * Gets the single instance of TeleportersData.
	 * @return single instance of TeleportersData
	 */
	public static TeleporterData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly TeleporterData INSTANCE = new();
	}
}