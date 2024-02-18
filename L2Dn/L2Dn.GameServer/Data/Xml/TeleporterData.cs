using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Teleporters;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class TeleporterData: DataReaderBase
{
	// Logger instance
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(TeleporterData));
	// Teleporter data
	private readonly Map<int, Map<String, TeleportHolder>> _teleporters = new();
	
	protected TeleporterData()
	{
		load();
	}
	
	public void load()
	{
		_teleporters.clear();
		
		LoadXmlDocuments(DataFileLocation.Data, "teleporters", true).ForEach(t =>
		{
			t.Document.Elements("list").Elements("npc").ForEach(x => loadElement(t.FilePath, x));
		});
		
		LOGGER.Info(GetType().Name + ": Loaded " + _teleporters.size() + " npc teleporters.");
	}

	private void loadElement(string filePath, XElement element)
	{
		Map<String, TeleportHolder> teleList = new();

		// Parse npc node child
		int npcId = element.Attribute("id").GetInt32();

		element.Elements("npcs").Elements("npc").Select(e => e.Attribute("id").GetInt32())
			.ForEach(npcId => registerTeleportList(npcId, teleList));

		element.Elements("teleport").ForEach(el =>
		{
			TeleportType type = el.Attribute("type").GetEnum<TeleportType>();
			string name = el.Attribute("name").GetString(type.ToString());

			// Parse locations
			TeleportHolder holder = new TeleportHolder(name, type);

			el.Elements("location").ForEach(e => { holder.registerLocation(new StatSet(e)); });

			// Register holder
			if (teleList.putIfAbsent(name, holder) != null)
			{
				LOGGER.Error("Duplicate teleport list (" + name + ") has been found for NPC: " + npcId);
			}
		});

		registerTeleportList(npcId, teleList);
	}

	public int getTeleporterCount()
	{
		return _teleporters.size();
	}
	
	/**
	 * Register teleport data to global teleport list holder. Also show warning when any duplicate occurs.
	 * @param npcId template id of teleporter
	 * @param teleList teleport data to register
	 */
	private void registerTeleportList(int npcId, Map<String, TeleportHolder> teleList)
	{
		_teleporters.put(npcId, teleList);
	}
	
	/**
	 * Gets teleport data for specified NPC and list name
	 * @param npcId template id of teleporter
	 * @param listName name of teleport list
	 * @return {@link TeleportHolder} if found otherwise {@code null}
	 */
	public TeleportHolder getHolder(int npcId, String listName)
	{
		return _teleporters.getOrDefault(npcId, new()).get(listName);
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