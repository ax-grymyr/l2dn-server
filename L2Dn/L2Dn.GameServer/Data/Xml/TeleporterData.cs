using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Teleporters;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class TeleporterData
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
		parseDatapackDirectory("data/teleporters", true);
		LOGGER.Info(GetType().Name + ": Loaded " + _teleporters.size() + " npc teleporters.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", list => forEach(list, "npc", npc =>
		{
			Map<String, TeleportHolder> teleList = new();
			// Parse npc node child
			int npcId = parseInteger(npc.getAttributes(), "id");
			forEach(npc, node =>
			{
				switch (node.getNodeName())
				{
					case "teleport":
					{
						NamedNodeMap nodeAttrs = node.getAttributes();
						// Parse attributes
						TeleportType type = parseEnum(nodeAttrs, TeleportType.class, "type");
						String name = parseString(nodeAttrs, "name", type.name());
						// Parse locations
						TeleportHolder holder = new TeleportHolder(name, type);
						forEach(node, "location", location => holder.registerLocation(new StatSet(parseAttributes(location))));
						// Register holder
						if (teleList.putIfAbsent(name, holder) != null)
						{
							LOGGER.Warn("Duplicate teleport list (" + name + ") has been found for NPC: " + npcId);
						}
						break;
					}
					case "npcs":
					{
						forEach(node, "npc", npcNode =>
						{
							int id = parseInteger(npcNode.getAttributes(), "id");
							registerTeleportList(id, teleList);
						});
						break;
					}
				}
			});
			registerTeleportList(npcId, teleList);
		}));
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