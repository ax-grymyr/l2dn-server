using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Spawns;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Model.Zones.Forms;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class SpawnData: DataReaderBase
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(SpawnData));
	
	private readonly Set<SpawnTemplate> _spawns = new();
	
	protected SpawnData()
	{
		load();
	}
	
	public void load()
	{
		LoadXmlDocuments(DataFileLocation.Data, "spawns", true).ForEach(t =>
		{
			t.Document.Elements("list").Elements("skillTree").ForEach(x => loadElement(t.FilePath, x));
		});

		LOGGER.Info(GetType().Name + ": Loaded " +
		            _spawns.SelectMany(c => c.getGroups()).SelectMany(c => c.getSpawns()).Count() + " spawns");
	}

	public void loadElement(string filePath, XElement element)
	{
		try
		{
			parseSpawn(element, Path.GetFileNameWithoutExtension(filePath), _spawns);
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Error while processing spawn in file: " + filePath, e);
		}
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
			Set<ScheduledFuture> jobs = new();
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
				foreach (ScheduledFuture job in jobs)
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

	public void parseSpawn(XElement element, string fileName, ICollection<SpawnTemplate> spawns)
	{
		SpawnTemplate spawnTemplate = new SpawnTemplate(new StatSet(element), fileName);
		SpawnGroup defaultGroup = null;
		foreach (XElement innerNode in element.Elements())
		{
			switch (innerNode.Name.LocalName)
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
	private void parseTerritories(XElement innerNode, string fileName, ITerritorized spawnTemplate)
	{
		innerNode.Elements().ForEach(territoryNode =>
		{
			string name = territoryNode.Attribute("name").GetString(fileName + "_" + (spawnTemplate.getTerritories().size() + 1));
			int minZ = territoryNode.Attribute("minZ").GetInt32();
			int maxZ = territoryNode.Attribute("maxZ").GetInt32();
			List<int> xNodes = new();
			List<int> yNodes = new();
			territoryNode.Elements("node").ForEach(node =>
			{
				xNodes.add(node.Attribute("x").GetInt32());
				yNodes.add(node.Attribute("y").GetInt32());
			});
			int[] x = xNodes.ToArray();
			int[] y = yNodes.ToArray();
			
			// Support for multiple spawn zone types.
			ZoneForm zoneForm = null;
			String zoneShape = territoryNode.Attribute("shape").GetString("NPoly");
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
					int zoneRad = territoryNode.Attribute("rad").GetInt32();
					zoneForm = new ZoneCylinder(x[0], y[0], minZ, maxZ, zoneRad);
					break;
				}
			}
			
			switch (territoryNode.Name.LocalName)
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
	
	private void parseGroup(XElement n, SpawnTemplate spawnTemplate)
	{
		SpawnGroup group = new SpawnGroup(new StatSet(n));
		n.Elements().ForEach(npcNode =>
		{
			switch (npcNode.Name.LocalName)
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
	private void parseNpc(XElement n, SpawnTemplate spawnTemplate, SpawnGroup group)
	{
		NpcSpawnTemplate npcTemplate = new NpcSpawnTemplate(spawnTemplate, group, new StatSet(n));
		NpcTemplate template = NpcData.getInstance().getTemplate(npcTemplate.getId());
		if (template == null)
		{
			LOGGER.Warn(GetType().Name + ": Requested spawn for non existing npc: " + npcTemplate.getId() +
			            " in file: " + spawnTemplate.getFile());
			return;
		}

		if (template.isType("Servitor") || template.isType("Pet"))
		{
			LOGGER.Warn(GetType().Name + ": Requested spawn for " + template.getType() + " " + template.getName() +
			            "(" + template.getId() + ") file: " + spawnTemplate.getFile());
			return;
		}

		if (!Config.FAKE_PLAYERS_ENABLED && template.isFakePlayer())
		{
			return;
		}
		
		foreach (XElement d in n.Elements())
		{
			if ("parameters".equalsIgnoreCase(d.Name.LocalName))
			{
				parseParameters(d, npcTemplate);
			}
			else if ("minions".equalsIgnoreCase(d.Name.LocalName))
			{
				parseMinions(d, npcTemplate);
			}
			else if ("locations".equalsIgnoreCase(d.Name.LocalName))
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
	private void parseLocations(XElement n, NpcSpawnTemplate npcTemplate)
	{
		foreach (XElement d in n.Elements())
		{
			if ("location".equalsIgnoreCase(d.Name.LocalName))
			{
				int x = d.Attribute("x").GetInt32();
				int y = d.Attribute("y").GetInt32();
				int z = d.Attribute("z").GetInt32();
				int heading = d.Attribute("heading").GetInt32(0);
				double chance = d.Attribute("chance").GetDouble();
				npcTemplate.addSpawnLocation(new ChanceLocation(x, y, z, heading, chance));
			}
		}
	}
	
	/**
	 * @param n
	 * @param npcTemplate
	 */
	private void parseParameters(XElement n, IParameterized<StatSet> npcTemplate)
	{
		Map<String, Object> @params = parseParameters(n);
		npcTemplate.setParameters(!@params.isEmpty() ? new StatSet(@params) : StatSet.EMPTY_STATSET);
	}
	
	/**
	 * @param n
	 * @param npcTemplate
	 */
	private void parseMinions(XElement n, NpcSpawnTemplate npcTemplate)
	{
		n.Elements("minion").ForEach(minionNode => npcTemplate.addMinion(new MinionHolder(new StatSet(minionNode))));
	}
	
	private static Map<String, Object> parseParameters(XElement element)
	{
		Map<String, Object> parameters = new();
		
		element.Elements("param").ForEach(el =>
		{
			string name = el.Attribute("name").GetString();
			string value = el.Attribute("value").GetString();
			parameters.put(name, value);
		});
		
		element.Elements("skill").ForEach(el =>
		{
			string name = el.Attribute("name").GetString();
			int id = el.Attribute("id").GetInt32();
			int level = el.Attribute("level").GetInt32();
			parameters.put(name, new SkillHolder(id, level));
		});
		
		element.Elements("location").ForEach(el =>
		{
			string name = el.Attribute("name").GetString();
			int x = el.Attribute("x").GetInt32();
			int y = el.Attribute("y").GetInt32();
			int z = el.Attribute("z").GetInt32();
			int heading = el.Attribute("heading").GetInt32(0);
			parameters.put(name, new Location(x, y, z, heading));
		});
		
		element.Elements("minions").ForEach(el =>
		{
			List<MinionHolder> minions = new();
			el.Elements("npc").ForEach(e =>
			{
				int id = el.Attribute("id").GetInt32();
				int count = el.Attribute("count").GetInt32();
				int max = el.Attribute("max").GetInt32(0);
				int respawnTime = el.Attribute("respawnTime").GetInt32();
				int weightPoint = el.Attribute("weightPoint").GetInt32(0);
				minions.add(new MinionHolder(id, count, max, respawnTime, weightPoint));
			});
					
			if (!minions.isEmpty())
				parameters.put(el.Attribute("name").GetString(), minions);
		});

		return parameters;
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