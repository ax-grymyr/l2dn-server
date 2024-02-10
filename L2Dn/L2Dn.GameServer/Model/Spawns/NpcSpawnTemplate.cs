using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Spawns;

public class NpcSpawnTemplate: IParameterized<StatSet>
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(NpcSpawnTemplate));
	
	private readonly int _id;
	private readonly int _count;
	private readonly TimeSpan _respawnTime;
	private readonly TimeSpan _respawnTimeRandom;
	private readonly SchedulingPattern _respawnPattern;
	private readonly int _chaseRange;
	private List<ChanceLocation> _locations;
	private SpawnTerritory _zone;
	private StatSet _parameters;
	private readonly bool _spawnAnimation;
	private readonly bool _saveInDB;
	private readonly String _dbName;
	private List<MinionHolder> _minions;
	private readonly SpawnTemplate _spawnTemplate;
	private readonly SpawnGroup _group;
	private readonly Set<Npc> _spawnedNpcs = new();
	
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
		_saveInDB = template._saveInDB;
		_dbName = template._dbName;
		_locations = template._locations;
		_zone = template._zone;
		_parameters = template._parameters;
		_minions = template._minions;
	}
	
	public NpcSpawnTemplate(SpawnTemplate spawnTemplate, SpawnGroup group, StatSet set)
	{
		_spawnTemplate = spawnTemplate;
		_group = group;
		_id = set.getInt("id");
		_count = set.getInt("count", 1);
		_respawnTime = set.getDuration("respawnTime", null);
		_respawnTimeRandom = set.getDuration("respawnRandom", null);
		String pattern = set.getString("respawnPattern", null);
		_respawnPattern = (pattern == null) || pattern.isEmpty() ? null : new SchedulingPattern(pattern);
		_chaseRange = set.getInt("chaseRange", 0);
		_spawnAnimation = set.getBoolean("spawnAnimation", false);
		_saveInDB = set.getBoolean("dbSave", false);
		_dbName = set.getString("dbName", null);
		_parameters = mergeParameters(spawnTemplate, group);
		
		int x = set.getInt("x", int.MaxValue);
		int y = set.getInt("y", int.MaxValue);
		int z = set.getInt("z", int.MaxValue);
		bool xDefined = x != int.MaxValue;
		bool yDefined = y != int.MaxValue;
		bool zDefined = z != int.MaxValue;
		if (xDefined && yDefined && zDefined)
		{
			_locations = new();
			_locations.add(new ChanceLocation(x, y, z, set.getInt("heading", 0), 100));
		}
		else
		{
			if (xDefined || yDefined || zDefined)
			{
				throw new InvalidOperationException(String.Format(
					"Spawn with partially declared and x: {0} y: {1} z: {2}!", processParam(x), processParam(y),
					processParam(z)));
			}
			
			String zoneName = set.getString("zone", null);
			if (zoneName != null)
			{
				SpawnTerritory zone = ZoneManager.getInstance().getSpawnTerritory(zoneName);
				if (zone == null)
				{
					throw new InvalidOperationException("Spawn with non existing zone requested " + zoneName);
				}
				_zone = zone;
			}
		}
		
		mergeParameters(spawnTemplate, group);
	}
	
	private StatSet mergeParameters(SpawnTemplate spawnTemplate, SpawnGroup group)
	{
		if ((_parameters == null) && (spawnTemplate.getParameters() == null) && (group.getParameters() == null))
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
		if (_locations == null)
		{
			_locations = new();
		}
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
	
	private String processParam(int value)
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
	
	public TimeSpan getRespawnTime()
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
		if (_parameters == null)
		{
			_parameters = parameters;
		}
		else
		{
			_parameters.merge(parameters);
		}
	}
	
	public bool hasSpawnAnimation()
	{
		return _spawnAnimation;
	}
	
	public bool hasDBSave()
	{
		return _saveInDB;
	}
	
	public String getDBName()
	{
		return _dbName;
	}
	
	public List<MinionHolder> getMinions()
	{
		return _minions != null ? _minions : new();
	}
	
	public void addMinion(MinionHolder minion)
	{
		if (_minions == null)
		{
			_minions = new();
		}
		_minions.add(minion);
	}
	
	public Set<Npc> getSpawnedNpcs()
	{
		return _spawnedNpcs;
	}
	
	public Location getSpawnLocation()
	{
		if (_locations != null)
		{
			double locRandom = (100 * Rnd.nextDouble());
			float cumulativeChance = 0;
			foreach (ChanceLocation loc in _locations)
			{
				if (locRandom <= (cumulativeChance += loc.getChance()))
				{
					return loc;
				}
			}
			LOGGER.Warn("Couldn't match location by chance turning first...");
			return null;
		}
		else if (_zone != null)
		{
			Location loc = _zone.getRandomPoint();
			loc.setHeading(-1);
			return loc;
		}
		else if (!_group.getTerritories().isEmpty())
		{
			SpawnTerritory territory = _group.getTerritories().get(Rnd.get(_group.getTerritories().size()));
			for (int i = 0; i < 100; i++)
			{
				Location loc = territory.getRandomPoint();
				if (_group.getBannedTerritories().isEmpty())
				{
					loc.setHeading(-1);
					return loc;
				}
				
				bool insideBannedTerritory = false;
				foreach (BannedSpawnTerritory bannedTerritory in _group.getBannedTerritories())
				{
					if (bannedTerritory.isInsideZone(loc.getX(), loc.getY(), loc.getZ()))
					{
						insideBannedTerritory = true;
						break;
					}
				}
				if (!insideBannedTerritory)
				{
					loc.setHeading(-1);
					return loc;
				}
			}
		}
		else if (!_spawnTemplate.getTerritories().isEmpty())
		{
			SpawnTerritory territory = _spawnTemplate.getTerritories().get(Rnd.get(_spawnTemplate.getTerritories().size()));
			for (int i = 0; i < 100; i++)
			{
				Location loc = territory.getRandomPoint();
				if (_spawnTemplate.getBannedTerritories().isEmpty())
				{
					loc.setHeading(-1);
					return loc;
				}
				
				bool insideBannedTerritory = false;
				foreach (BannedSpawnTerritory bannedTerritory in _spawnTemplate.getBannedTerritories())
				{
					if (bannedTerritory.isInsideZone(loc.getX(), loc.getY(), loc.getZ()))
					{
						insideBannedTerritory = true;
						break;
					}
				}
				if (!insideBannedTerritory)
				{
					loc.setHeading(-1);
					return loc;
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
				LOGGER.Warn("Attempting to spawn unexisting npc id: " + _id + " file: " + _spawnTemplate.getFile().getName() + " spawn: " + _spawnTemplate.getName() + " group: " + _group.getName());
				return;
			}
			
			if (npcTemplate.isType("Defender"))
			{
				LOGGER.Warn("Attempting to spawn npc id: " + _id + " type: " + npcTemplate.getType() + " file: " + _spawnTemplate.getFile().getName() + " spawn: " + _spawnTemplate.getName() + " group: " + _group.getName());
				return;
			}
			
			for (int i = 0; i < _count; i++)
			{
				spawnNpc(npcTemplate, instance);
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Couldn't spawn npc " + _id + e);
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
		Location loc = getSpawnLocation();
		if (loc == null)
		{
			LOGGER.Warn("Couldn't initialize new spawn, no location found!");
			return;
		}
		
		spawn.setInstanceId(instance != null ? instance.getId() : 0);
		spawn.setAmount(1);
		spawn.setXYZ(loc);
		spawn.setHeading(loc.getHeading());
		spawn.setLocation(loc);
		int respawn = 0;
		int respawnRandom = 0;
		SchedulingPattern respawnPattern = null;
		if (_respawnTime != null)
		{
			respawn = (int) _respawnTime.TotalSeconds;
		}
		if (_respawnTimeRandom != null)
		{
			respawnRandom = (int) _respawnTimeRandom.TotalSeconds;
		}
		if (_respawnPattern != null)
		{
			respawnPattern = _respawnPattern;
		}
		
		if ((respawn > 0) || (respawnPattern != null))
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
		
		if (_saveInDB)
		{
			if (!DBSpawnManager.getInstance().isDefined(_id))
			{
				Npc spawnedNpc = DBSpawnManager.getInstance().addNewSpawn(spawn, true);
				if ((spawnedNpc != null) && spawnedNpc.isMonster() && (_minions != null))
				{
					((Monster) spawnedNpc).getMinionList().spawnMinions(_minions);
				}
				
				_spawnedNpcs.add(spawnedNpc);
			}
		}
		else
		{
			Npc npc = spawn.doSpawn(_spawnAnimation);
			if (npc.isMonster() && (_minions != null))
			{
				((Monster) npc).getMinionList().spawnMinions(_minions);
			}
			_spawnedNpcs.add(npc);
			
			SpawnTable.getInstance().addNewSpawn(spawn, false);
		}
	}
	
	public void despawn()
	{
		_spawnedNpcs.forEach(npc =>
		{
			npc.getSpawn().stopRespawn();
			SpawnTable.getInstance().deleteSpawn(npc.getSpawn(), false);
			npc.deleteMe();
		});
		_spawnedNpcs.clear();
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