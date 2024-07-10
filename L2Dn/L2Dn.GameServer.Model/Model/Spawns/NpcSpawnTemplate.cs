using L2Dn.Extensions;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model.DataPack;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Spawns;

public class NpcSpawnTemplate: IParameterized<StatSet>
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(NpcSpawnTemplate));
	
	private readonly int _id;
	private readonly int _count;
	private readonly TimeSpan? _respawnTime;
	private readonly TimeSpan _respawnTimeRandom;
	private readonly SchedulingPattern? _respawnPattern;
	private readonly int _chaseRange;
	private readonly List<ChanceLocation> _locations = [];
	private readonly SpawnTerritory? _zone;
	private readonly StatSet _parameters;
	private readonly bool _spawnAnimation;
	private readonly bool _saveInDb;
	private readonly string _dbName;
	private readonly List<MinionHolder> _minions = [];
	private readonly SpawnTemplate _spawnTemplate;
	private readonly SpawnGroup _group;
	private readonly Set<Npc> _spawnedNpcs = [];
	
	private NpcSpawnTemplate(NpcSpawnTemplate template)
	{
		_spawnTemplate = template._spawnTemplate;
		_group = template._group;
		_id = template._id;
		_count = template._count;
		_respawnTime = template._respawnTime;
		_respawnTimeRandom = template._respawnTimeRandom;
		_respawnPattern = template._respawnPattern;
		_chaseRange = template._chaseRange;
		_spawnAnimation = template._spawnAnimation;
		_saveInDb = template._saveInDb;
		_dbName = template._dbName;
		_locations = template._locations;
		_zone = template._zone;
		_parameters = template._parameters;
		_minions = template._minions;
	}
	
	public NpcSpawnTemplate(SpawnTemplate spawnTemplate, SpawnGroup group, XmlSpawnNpc npc)
	{
		_spawnTemplate = spawnTemplate;
		_group = group;
		_id = npc.Id;
		_count = npc.Count;
		_respawnTime = string.IsNullOrEmpty(npc.RespawnTime)
			? TimeSpan.Zero
			: TimeUtil.ParseDuration(npc.RespawnTime);
		
		_respawnTimeRandom = string.IsNullOrEmpty(npc.RespawnRandom)
			? TimeSpan.Zero
			: TimeUtil.ParseDuration(npc.RespawnRandom);
		
		_respawnPattern = string.IsNullOrEmpty(npc.RespawnPattern) ? null : new SchedulingPattern(npc.RespawnPattern);
		_chaseRange = npc.ChaseRange;
		_spawnAnimation = npc.SpawnAnimation;
		_saveInDb = npc.DbSave;
		_dbName = npc.DbName;
		_parameters = mergeParameters(spawnTemplate, group);
		
		int x = npc.X;
		int y = npc.Y;
		int z = npc.Z;
		if (npc is { XSpecified: true, YSpecified: true, ZSpecified: true })
		{
			_locations.add(new ChanceLocation(new Location(x, y, z, npc.Heading), 100));
		}
		else
		{
			if (npc.XSpecified || npc.YSpecified || npc.ZSpecified)
			{
				throw new InvalidOperationException(
					$"Spawn with partially declared and x: {processParam(x)} y: {processParam(y)} z: {processParam(z)}!");
			}
			
			string zoneName = npc.Zone;
			if (!string.IsNullOrEmpty(zoneName))
			{
				SpawnTerritory zone = ZoneManager.getInstance().getSpawnTerritory(zoneName);
				if (zone == null)
					throw new InvalidOperationException("Spawn with non existing zone requested " + zoneName);

				_zone = zone;
			}
		}
		
		mergeParameters(spawnTemplate, group);
	}
	
	private StatSet mergeParameters(SpawnTemplate spawnTemplate, SpawnGroup group)
	{
		if (_parameters == null && spawnTemplate.getParameters() == null && group.getParameters() == null)
		{
			return null;
		}
		
		StatSet set = new StatSet();
		if (spawnTemplate.getParameters() != null)
		{
			set.merge(spawnTemplate.getParameters());
		}
		if (group.getParameters() != null)
		{
			set.merge(group.getParameters());
		}
		if (_parameters != null)
		{
			set.merge(_parameters);
		}
		return set;
	}
	
	public void addSpawnLocation(ChanceLocation loc)
	{
		_locations.add(loc);
	}
	
	public SpawnTemplate getSpawnTemplate()
	{
		return _spawnTemplate;
	}
	
	public SpawnGroup getGroup()
	{
		return _group;
	}
	
	private string processParam(int value)
	{
		return value != int.MaxValue ? value.ToString() : "undefined";
	}
	
	public int getId()
	{
		return _id;
	}
	
	public int getCount()
	{
		return _count;
	}
	
	public TimeSpan? getRespawnTime()
	{
		return _respawnTime;
	}
	
	public TimeSpan getRespawnTimeRandom()
	{
		return _respawnTimeRandom;
	}
	
	public SchedulingPattern getRespawnPattern()
	{
		return _respawnPattern;
	}
	
	public int getChaseRange()
	{
		return _chaseRange;
	}
	
	public List<ChanceLocation> getLocation()
	{
		return _locations;
	}
	
	public SpawnTerritory getZone()
	{
		return _zone;
	}
	
	public StatSet getParameters()
	{
		return _parameters;
	}
	
	public void setParameters(StatSet parameters)
	{
		_parameters.merge(parameters);
	}
	
	public bool hasSpawnAnimation()
	{
		return _spawnAnimation;
	}
	
	public bool hasDBSave()
	{
		return _saveInDb;
	}
	
	public string getDBName()
	{
		return _dbName;
	}
	
	public List<MinionHolder> getMinions()
	{
		return _minions;
	}
	
	public void addMinion(MinionHolder minion)
	{
		_minions.add(minion);
	}
	
	public Set<Npc> getSpawnedNpcs()
	{
		return _spawnedNpcs;
	}
	
	public Location? getSpawnLocation()
	{
		if (_locations.Count != 0)
		{
			double locRandom = 100 * Rnd.nextDouble();
			double cumulativeChance = 0;
			foreach (ChanceLocation loc in _locations)
			{
				if (locRandom <= (cumulativeChance += loc.getChance()))
					return loc.Location;
			}

			_logger.Warn("Couldn't match location by chance turning first...");
			return null;
		}

		if (_zone != null)
		{
			Location3D loc = _zone.getRandomPoint();
			return new Location(loc, -1);
		}

		if (_group.getTerritories().Count != 0)
		{
			SpawnTerritory territory = _group.getTerritories().GetRandomElement();
			for (int i = 0; i < 100; i++)
			{
				Location3D loc = territory.getRandomPoint();
				if (_group.getBannedTerritories().Count == 0)
				{
					return new Location(loc, -1);
				}

				bool insideBannedTerritory = false;
				foreach (BannedSpawnTerritory bannedTerritory in _group.getBannedTerritories())
				{
					if (bannedTerritory.isInsideZone(loc.X, loc.Y, loc.Z))
					{
						insideBannedTerritory = true;
						break;
					}
				}

				if (!insideBannedTerritory)
				{
					return new Location(loc, -1);
				}
			}
		}
		else if (_spawnTemplate.getTerritories().Count != 0)
		{
			SpawnTerritory territory = _spawnTemplate.getTerritories().GetRandomElement();
			for (int i = 0; i < 100; i++)
			{
				Location3D loc = territory.getRandomPoint();
				if (_spawnTemplate.getBannedTerritories().Count == 0)
				{
					return new Location(loc, -1);
				}

				bool insideBannedTerritory = false;
				foreach (BannedSpawnTerritory bannedTerritory in _spawnTemplate.getBannedTerritories())
				{
					if (bannedTerritory.isInsideZone(loc.X, loc.Y, loc.Z))
					{
						insideBannedTerritory = true;
						break;
					}
				}

				if (!insideBannedTerritory)
				{
					return new Location(loc, -1);
				}
			}
		}

		return null;
	}
	
	public void spawn()
	{
		spawn(null);
	}
	
	public void spawn(Instance instance)
	{
		try
		{
			NpcTemplate npcTemplate = NpcData.getInstance().getTemplate(_id);
			if (npcTemplate == null)
			{
				_logger.Warn("Attempting to spawn unexisting npc id: " + _id + " file: " + _spawnTemplate.getFile() + " spawn: " + _spawnTemplate.getName() + " group: " + _group.getName());
				return;
			}
			
			if (npcTemplate.isType("Defender"))
			{
				_logger.Warn("Attempting to spawn npc id: " + _id + " type: " + npcTemplate.getType() + " file: " + _spawnTemplate.getFile() + " spawn: " + _spawnTemplate.getName() + " group: " + _group.getName());
				return;
			}
			
			for (int i = 0; i < _count; i++)
			{
				spawnNpc(npcTemplate, instance);
			}
		}
		catch (Exception e)
		{
			_logger.Warn("Couldn't spawn npc " + _id + e);
		}
	}
	
	/**
	 * @param npcTemplate
	 * @param instance
	 * @throws ClassCastException
	 * @throws NoSuchMethodException
	 * @throws ClassNotFoundException
	 */
	private void spawnNpc(NpcTemplate npcTemplate, Instance instance)
	{
		Spawn spawn = new Spawn(npcTemplate);
		Location? loc = getSpawnLocation();
		if (loc == null)
		{
			_logger.Warn("Couldn't initialize new spawn, no location found!");
			return;
		}
		
		spawn.setInstanceId(instance != null ? instance.getId() : 0);
		spawn.setAmount(1);
		spawn.Location = loc.Value;
		TimeSpan respawn = TimeSpan.Zero;
		TimeSpan respawnRandom = TimeSpan.Zero;
		SchedulingPattern respawnPattern = null;
		if (_respawnTime != null)
		{
			respawn = _respawnTime.Value;
		}
		if (_respawnTimeRandom != null)
		{
			respawnRandom = _respawnTimeRandom;
		}
		if (_respawnPattern != null)
		{
			respawnPattern = _respawnPattern;
		}
		
		if (respawn > TimeSpan.Zero || respawnPattern != null)
		{
			spawn.setRespawnDelay(respawn, respawnRandom);
			spawn.setRespawnPattern(respawnPattern);
			spawn.startRespawn();
		}
		else
		{
			spawn.stopRespawn();
		}
		
		spawn.setSpawnTemplate(this);
		
		if (_saveInDb)
		{
			if (!DbSpawnManager.getInstance().isDefined(_id))
			{
				Npc spawnedNpc = DbSpawnManager.getInstance().addNewSpawn(spawn, true);
				if (spawnedNpc != null && spawnedNpc.isMonster() && _minions != null)
				{
					((Monster) spawnedNpc).getMinionList().spawnMinions(_minions);
				}
				
				_spawnedNpcs.add(spawnedNpc);
			}
		}
		else
		{
			Npc npc = spawn.doSpawn(_spawnAnimation);
			if (npc.isMonster() && _minions != null)
			{
				((Monster) npc).getMinionList().spawnMinions(_minions);
			}
			_spawnedNpcs.add(npc);
			
			SpawnTable.getInstance().addNewSpawn(spawn, false);
		}
	}
	
	public void despawn()
	{
		_spawnedNpcs.ForEach(npc =>
		{
			npc.getSpawn().stopRespawn();
			SpawnTable.getInstance().deleteSpawn(npc.getSpawn(), false);
			npc.deleteMe();
		});

		_spawnedNpcs.Clear();
	}
	
	public void notifySpawnNpc(Npc npc)
	{
		_spawnTemplate.notifyEvent(@event => @event.onSpawnNpc(_spawnTemplate, _group, npc));
	}
	
	public void notifyDespawnNpc(Npc npc)
	{
		_spawnTemplate.notifyEvent(@event => @event.onSpawnDespawnNpc(_spawnTemplate, _group, npc));
	}
	
	public void notifyNpcDeath(Npc npc, Creature killer)
	{
		_spawnTemplate.notifyEvent(@event => @event.onSpawnNpcDeath(_spawnTemplate, _group, npc, killer));
	}
	
	public NpcSpawnTemplate clone()
	{
		return new NpcSpawnTemplate(this);
	}
}