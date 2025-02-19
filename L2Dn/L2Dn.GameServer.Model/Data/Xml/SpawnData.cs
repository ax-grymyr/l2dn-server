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
using L2Dn.Geometry;
using L2Dn.Model.DataPack;
using L2Dn.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class SpawnData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(SpawnData));

	private readonly Set<SpawnTemplate> _spawns = new();

	private SpawnData()
	{
		load();
	}

	public void load()
	{
		LoadXmlDocuments<XmlSpawnList>(DataFileLocation.Data, "spawns", true)
			.SelectMany(t => t.Document.Spawns.Select(s => (t.FilePath, Spawn: s)))
			.ForEach(t => LoadSpawn(t.FilePath, t.Spawn));

		_logger.Info(GetType().Name + ": Loaded " +
		             _spawns.Select(c => c.getGroups().Sum(gr => gr.getSpawns().Count)).Sum() + " spawns");
	}

	private void LoadSpawn(string filePath, XmlSpawn spawn)
	{
		try
		{
			parseSpawn(spawn, Path.GetFileNameWithoutExtension(filePath), _spawns);
		}
		catch (Exception e)
		{
			_logger.Warn(GetType().Name + ": Error while processing spawn in file: " + filePath, e);
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

		_logger.Info(GetType().Name + ": Initializing spawns...");
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
					if (job.isDone() || job.isCancelled())
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

		_logger.Info(GetType().Name + ": All spawns has been initialized!");
	}

	/**
	 * Removing all spawns
	 */
	public void despawnAll()
	{
		_logger.Info(GetType().Name + ": Removing all spawns...");
		_spawns.ForEach(x => x.despawnAll());
		_logger.Info(GetType().Name + ": All spawns has been removed!");
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
				result.Add(spawnTemplate);
			}
		}
		return result;
	}

	public SpawnTemplate? getSpawnByName(string name)
	{
		foreach (SpawnTemplate spawn in _spawns)
		{
			if (spawn.getName() != null && spawn.getName().equalsIgnoreCase(name))
			{
				return spawn;
			}
		}
		return null;
	}

	public SpawnGroup? getSpawnGroupByName(string name)
	{
		foreach (SpawnTemplate spawnTemplate in _spawns)
		{
			foreach (SpawnGroup group in spawnTemplate.getGroups())
			{
				if (group.getName() != null && group.getName().equalsIgnoreCase(name))
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
						result.Add(spawn);
					}
				}
			}
		}
		return result;
	}

	internal void parseSpawn(XmlSpawn spawn, string fileName, ICollection<SpawnTemplate> spawns)
	{
		SpawnTemplate spawnTemplate = new(spawn.Name, spawn.Ai, spawn.SpawnByDefault, fileName);
		parseTerritories(spawn.Territories, fileName, spawnTemplate);

		foreach (XmlSpawnGroup group in spawn.Groups)
			parseGroup(group, spawnTemplate);

		if (spawn.Npcs.Count != 0)
		{
			// One static group for all npcs outside group scope
			SpawnGroup defaultGroup = new(spawn.Name, spawn.SpawnByDefault);
			foreach (XmlSpawnNpc npc in spawn.Npcs)
				parseNpc(npc, spawnTemplate, defaultGroup);

			spawnTemplate.addGroup(defaultGroup);
		}

		parseParameters(spawn.Parameters, spawnTemplate);

		spawns.Add(spawnTemplate);
	}

	/**
	 * @param innerNode
	 * @param file
	 * @param spawnTemplate
	 */
	private void parseTerritories(List<XmlSpawnTerritoryBase> territories, string fileName, ITerritorized spawnTemplate)
	{
		foreach (XmlSpawnTerritoryBase territory in territories)
		{
			string name = territory.Name;
			if (string.IsNullOrEmpty(name))
				name = fileName + "_" + (spawnTemplate.getTerritories().Count + 1);

			int minZ = territory.MinZ;
			int maxZ = territory.MaxZ;
			int[] x = territory.Nodes.Select(n => n.X).ToArray();
			int[] y = territory.Nodes.Select(n => n.Y).ToArray();

			// Support for multiple spawn zone types.
			ZoneForm zoneForm;
			XmlSpawnTerritoryShape zoneShape = territory.ShapeSpecified ? territory.Shape : XmlSpawnTerritoryShape.NPoly;
			switch (zoneShape)
			{
				case XmlSpawnTerritoryShape.Cuboid:
				{
					zoneForm = new ZoneCuboid(x[0], x[1], y[0], y[1], minZ, maxZ);
					break;
				}
				case XmlSpawnTerritoryShape.NPoly:
				{
					zoneForm = new ZoneNPoly(x, y, minZ, maxZ);
					break;
				}
				case XmlSpawnTerritoryShape.Cylinder:
				{
					int zoneRad = territory.Radius;
					zoneForm = new ZoneCylinder(x[0], y[0], minZ, maxZ, zoneRad);
					break;
				}
				default:
				{
					_logger.Error(nameof(SpawnData) + $": Invalid territory type in file '{fileName}'");
					continue;
				}
			}

			switch (territory)
			{
				case XmlSpawnTerritory:
					spawnTemplate.addTerritory(new SpawnTerritory(name, zoneForm));
					break;
				case XmlSpawnBannedTerritory:
					spawnTemplate.addBannedTerritory(new BannedSpawnTerritory(name, zoneForm));
					break;
				default:
					_logger.Error(nameof(SpawnData) + $": Invalid territory type in file '{fileName}'");
					break;
			}
		}
	}

	private void parseGroup(XmlSpawnGroup xmlGroup, SpawnTemplate spawnTemplate)
	{
		SpawnGroup group = new(xmlGroup.Name, xmlGroup.SpawnByDefault);
		parseTerritories(xmlGroup.Territories, spawnTemplate.getFile(), group);
		foreach (XmlSpawnNpc npc in xmlGroup.Npcs)
			parseNpc(npc, spawnTemplate, group);

		spawnTemplate.addGroup(group);
	}

	/**
	 * @param n
	 * @param spawnTemplate
	 * @param group
	 */
	private void parseNpc(XmlSpawnNpc npc, SpawnTemplate spawnTemplate, SpawnGroup group)
	{
		NpcSpawnTemplate npcTemplate = new(spawnTemplate, group, npc);
		NpcTemplate? template = NpcData.getInstance().getTemplate(npcTemplate.getId());
		if (template == null)
		{
			_logger.Warn(GetType().Name + ": Requested spawn for non existing npc: " + npcTemplate.getId() +
			            " in file: " + spawnTemplate.getFile());

			return;
		}

		if (template.isType("Servitor") || template.isType("Pet"))
		{
			_logger.Warn(GetType().Name + ": Requested spawn for " + template.getType() + " " + template.getName() +
			            "(" + template.getId() + ") file: " + spawnTemplate.getFile());

			return;
		}

		if (!Config.FAKE_PLAYERS_ENABLED && template.isFakePlayer())
			return;

		parseParameters(npc.Parameters, npcTemplate);
		parseMinions(npc.Minions, npcTemplate);
		parseLocations(npc.Locations, npcTemplate);
		group.addSpawn(npcTemplate);
	}

	/**
	 * @param n
	 * @param npcTemplate
	 */
	private static void parseLocations(List<XmlSpawnNpcLocation> locations, NpcSpawnTemplate npcTemplate)
	{
		foreach (XmlSpawnNpcLocation location in locations)
		{
			npcTemplate.addSpawnLocation(new ChanceLocation(
				new Location(location.X, location.Y, location.Z, location.Heading),
				location.Chance));
		}
	}

	/**
	 * @param n
	 * @param npcTemplate
	 */
	private void parseParameters(List<XmlParameter> parameters, IParameterized<StatSet> npcTemplate)
	{
		StatSet statSet = new();
		foreach (XmlParameter parameter in parameters)
		{
			switch (parameter)
			{
				case XmlParameterString paramString:
					statSet.set(parameter.Name, paramString.Value);
					break;

				case XmlParameterSkill paramSkill:
					statSet.set(parameter.Name, new SkillHolder(paramSkill.Id, paramSkill.Level));
					break;

				case XmlSpawnNpcParameterMinions paramMinions:
					List<MinionHolder> minions = paramMinions.Npcs.Select(m
						=> new MinionHolder(m.Id, m.Count, m.MaxCount, TimeSpan.FromMilliseconds(m.RespawnTime),
							m.WeightPoint)).ToList();

					if (minions.Count != 0)
						statSet.set(parameter.Name, minions);

					break;

				default:
					_logger.Error(nameof(SpawnData) + $": Invalid parameter type {parameter.GetType()}");
					break;
			}
		}

		npcTemplate.setParameters(!statSet.isEmpty() ? statSet : StatSet.EMPTY_STATSET);
	}

	/**
	 * @param n
	 * @param npcTemplate
	 */
	private void parseMinions(List<XmlSpawnNpcMinion> minions, NpcSpawnTemplate npcTemplate)
	{
		foreach (XmlSpawnNpcMinion minion in minions)
		{
			// TODO: support for random time
			npcTemplate.addMinion(new MinionHolder(minion.Id, minion.Count, minion.MaxCount,
				TimeUtil.ParseDuration(minion.RespawnTime), minion.WeightPoint));
		}
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