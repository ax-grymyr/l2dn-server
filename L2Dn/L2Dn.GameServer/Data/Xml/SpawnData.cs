using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Spawns;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Model.Zones.Forms;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class SpawnData
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(SpawnData));
	
	private readonly Set<SpawnTemplate> _spawns = new();
	
	protected SpawnData()
	{
		load();
	}
	
	public void load()
	{
		parseDatapackDirectory("data/spawns", true);
		LOGGER.Info(GetType().Name + ": Loaded " + _spawns.stream().flatMap(c => c.getGroups().stream()).flatMap(c => c.getSpawns().stream()).count() + " spawns");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, "spawn", spawnNode =>
		{
			try
			{
				parseSpawn(spawnNode, f, _spawns);
			}
			catch (Exception e)
			{
				LOGGER.Warn(GetType().Name + ": Error while processing spawn in file: " + f.getAbsolutePath(), e);
			}
		}));
	}
	
	/**
	 * Initializing all spawns
	 */
	public void init()
	{
		if (Config.ALT_DEV_NO_SPAWNS)
		{
			return;
		}
		
		LOGGER.Info(GetType().Name + ": Initializing spawns...");
		if (Config.THREADS_FOR_LOADING)
		{
			Collection<ScheduledFuture<?>> jobs = ConcurrentHashMap.newKeySet();
			foreach (SpawnTemplate template in _spawns)
			{
				if (template.isSpawningByDefault())
				{
					jobs.add(ThreadPool.schedule(() =>
					{
						template.spawnAll(null);
						template.notifyActivate();
					}, 0));
				}
			}
			while (!jobs.isEmpty())
			{
				foreach (ScheduledFuture<?> job in jobs)
				{
					if ((job == null) || job.isDone() || job.isCancelled())
					{
						jobs.remove(job);
					}
				}
			}
		}
		else
		{
			foreach (SpawnTemplate template in _spawns)
			{
				if (template.isSpawningByDefault())
				{
					template.spawnAll(null);
					template.notifyActivate();
				}
			}
		}
		
		LOGGER.Info(GetType().Name + ": All spawns has been initialized!");
	}
	
	/**
	 * Removing all spawns
	 */
	public void despawnAll()
	{
		LOGGER.Info(GetType().Name + ": Removing all spawns...");
		_spawns.forEach(x => x.despawnAll());
		LOGGER.Info(GetType().Name + ": All spawns has been removed!");
	}
	
	public ICollection<SpawnTemplate> getSpawns()
	{
		return _spawns;
	}
	
	public List<SpawnTemplate> getSpawns(Predicate<SpawnTemplate> condition)
	{
		List<SpawnTemplate> result = new();
		foreach (SpawnTemplate spawnTemplate in _spawns)
		{
			if (condition(spawnTemplate))
			{
				result.add(spawnTemplate);
			}
		}
		return result;
	}
	
	public SpawnTemplate getSpawnByName(String name)
	{
		foreach (SpawnTemplate spawn in _spawns)
		{
			if ((spawn.getName() != null) && spawn.getName().equalsIgnoreCase(name))
			{
				return spawn;
			}
		}
		return null;
	}
	
	public SpawnGroup getSpawnGroupByName(String name)
	{
		foreach (SpawnTemplate spawnTemplate in _spawns)
		{
			foreach (SpawnGroup group in spawnTemplate.getGroups())
			{
				if ((group.getName() != null) && group.getName().equalsIgnoreCase(name))
				{
					return group;
				}
			}
		}
		return null;
	}
	
	public List<NpcSpawnTemplate> getNpcSpawns(Predicate<NpcSpawnTemplate> condition)
	{
		List<NpcSpawnTemplate> result = new();
		foreach (SpawnTemplate template in _spawns)
		{
			foreach (SpawnGroup group in template.getGroups())
			{
				foreach (NpcSpawnTemplate spawn in group.getSpawns())
				{
					if (condition(spawn))
					{
						result.add(spawn);
					}
				}
			}
		}
		return result;
	}
	
	public void parseSpawn(Node spawnsNode, File file, ICollection<SpawnTemplate> spawns)
	{
		SpawnTemplate spawnTemplate = new SpawnTemplate(new StatSet(parseAttributes(spawnsNode)), file);
		SpawnGroup defaultGroup = null;
		for (Node innerNode = spawnsNode.getFirstChild(); innerNode != null; innerNode = innerNode.getNextSibling())
		{
			switch (innerNode.getNodeName())
			{
				case "territories":
				{
					parseTerritories(innerNode, spawnTemplate.getFile(), spawnTemplate);
					break;
				}
				case "group":
				{
					parseGroup(innerNode, spawnTemplate);
					break;
				}
				case "npc":
				{
					if (defaultGroup == null)
					{
						defaultGroup = new SpawnGroup(StatSet.EMPTY_STATSET);
					}
					parseNpc(innerNode, spawnTemplate, defaultGroup);
					break;
				}
				case "parameters":
				{
					parseParameters(innerNode, spawnTemplate);
					break;
				}
			}
		}
		
		// One static group for all npcs outside group scope
		if (defaultGroup != null)
		{
			spawnTemplate.addGroup(defaultGroup);
		}
		spawns.Add(spawnTemplate);
	}
	
	/**
	 * @param innerNode
	 * @param file
	 * @param spawnTemplate
	 */
	private void parseTerritories(Node innerNode, File file, ITerritorized spawnTemplate)
	{
		forEach(innerNode, IXmlReader::isNode, territoryNode =>
		{
			String name = parseString(territoryNode.getAttributes(), "name", file.getName() + "_" + (spawnTemplate.getTerritories().size() + 1));
			int minZ = parseInteger(territoryNode.getAttributes(), "minZ");
			int maxZ = parseInteger(territoryNode.getAttributes(), "maxZ");
			List<int> xNodes = new();
			List<int> yNodes = new();
			forEach(territoryNode, "node", node =>
			{
				xNodes.add(parseInteger(node.getAttributes(), "x"));
				yNodes.add(parseInteger(node.getAttributes(), "y"));
			});
			int[] x = xNodes.stream().mapToInt(int::valueOf).toArray();
			int[] y = yNodes.stream().mapToInt(int::valueOf).toArray();
			
			// Support for multiple spawn zone types.
			ZoneForm zoneForm = null;
			String zoneShape = parseString(territoryNode.getAttributes(), "shape", "NPoly");
			switch (zoneShape)
			{
				case "Cuboid":
				{
					zoneForm = new ZoneCuboid(x[0], x[1], y[0], y[1], minZ, maxZ);
					break;
				}
				case "NPoly":
				{
					zoneForm = new ZoneNPoly(x, y, minZ, maxZ);
					break;
				}
				case "Cylinder":
				{
					int zoneRad = int.Parse(territoryNode.getAttributes().getNamedItem("rad").getNodeValue());
					zoneForm = new ZoneCylinder(x[0], y[0], minZ, maxZ, zoneRad);
					break;
				}
			}
			
			switch (territoryNode.getNodeName())
			{
				case "territory":
				{
					spawnTemplate.addTerritory(new SpawnTerritory(name, zoneForm));
					break;
				}
				case "banned_territory":
				{
					spawnTemplate.addBannedTerritory(new BannedSpawnTerritory(name, zoneForm));
					break;
				}
			}
		});
	}
	
	private void parseGroup(Node n, SpawnTemplate spawnTemplate)
	{
		SpawnGroup group = new SpawnGroup(new StatSet(parseAttributes(n)));
		forEach(n, IXmlReader::isNode, npcNode =>
		{
			switch (npcNode.getNodeName())
			{
				case "territories":
				{
					parseTerritories(npcNode, spawnTemplate.getFile(), group);
					break;
				}
				case "npc":
				{
					parseNpc(npcNode, spawnTemplate, group);
					break;
				}
			}
		});
		spawnTemplate.addGroup(group);
	}
	
	/**
	 * @param n
	 * @param spawnTemplate
	 * @param group
	 */
	private void parseNpc(Node n, SpawnTemplate spawnTemplate, SpawnGroup group)
	{
		NpcSpawnTemplate npcTemplate = new NpcSpawnTemplate(spawnTemplate, group, new StatSet(parseAttributes(n)));
		NpcTemplate template = NpcData.getInstance().getTemplate(npcTemplate.getId());
		if (template == null)
		{
			LOGGER.Warn(GetType().Name + ": Requested spawn for non existing npc: " + npcTemplate.getId() + " in file: " + spawnTemplate.getFile().getName());
			return;
		}
		
		if (template.isType("Servitor") || template.isType("Pet"))
		{
			LOGGER.Warn(GetType().Name + ": Requested spawn for " + template.getType() + " " + template.getName() + "(" + template.getId() + ") file: " + spawnTemplate.getFile().getName());
			return;
		}
		
		if (!Config.FAKE_PLAYERS_ENABLED && template.isFakePlayer())
		{
			return;
		}
		
		for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
		{
			if ("parameters".equalsIgnoreCase(d.getNodeName()))
			{
				parseParameters(d, npcTemplate);
			}
			else if ("minions".equalsIgnoreCase(d.getNodeName()))
			{
				parseMinions(d, npcTemplate);
			}
			else if ("locations".equalsIgnoreCase(d.getNodeName()))
			{
				parseLocations(d, npcTemplate);
			}
		}
		group.addSpawn(npcTemplate);
	}
	
	/**
	 * @param n
	 * @param npcTemplate
	 */
	private void parseLocations(Node n, NpcSpawnTemplate npcTemplate)
	{
		for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
		{
			if ("location".equalsIgnoreCase(d.getNodeName()))
			{
				int x = parseInteger(d.getAttributes(), "x");
				int y = parseInteger(d.getAttributes(), "y");
				int z = parseInteger(d.getAttributes(), "z");
				int heading = parseInteger(d.getAttributes(), "heading", 0);
				double chance = parseDouble(d.getAttributes(), "chance");
				npcTemplate.addSpawnLocation(new ChanceLocation(x, y, z, heading, chance));
			}
		}
	}
	
	/**
	 * @param n
	 * @param npcTemplate
	 */
	private void parseParameters(Node n, IParameterized<StatSet> npcTemplate)
	{
		Map<String, Object> @params = parseParameters(n);
		npcTemplate.setParameters(!@params.isEmpty() ? new StatSet(Collections.unmodifiableMap(@params)) : StatSet.EMPTY_STATSET);
	}
	
	/**
	 * @param n
	 * @param npcTemplate
	 */
	private void parseMinions(Node n, NpcSpawnTemplate npcTemplate)
	{
		forEach(n, "minion", minionNode => npcTemplate.addMinion(new MinionHolder(new StatSet(parseAttributes(minionNode)))));
	}
	
	/**
	 * Gets the single instance of SpawnsData.
	 * @return single instance of SpawnsData
	 */
	public static SpawnData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly SpawnData INSTANCE = new();
	}
}