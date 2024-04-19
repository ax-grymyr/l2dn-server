using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Spawns;

public class SpawnGroup: ITerritorized, IParameterized<StatSet>
{
	private readonly string _name;
	private readonly bool _spawnByDefault;
	private readonly List<SpawnTerritory> _territories = [];
	private readonly List<BannedSpawnTerritory> _bannedTerritories = [];
	private readonly List<NpcSpawnTemplate> _spawns = [];
	private readonly StatSet _parameters = new();

	public SpawnGroup(string name, bool spawnByDefault)
	{
		_name = name;
		_spawnByDefault = spawnByDefault;
	}

	public string getName()
	{
		return _name;
	}

	public bool isSpawningByDefault()
	{
		return _spawnByDefault;
	}

	public void addSpawn(NpcSpawnTemplate template)
	{
		_spawns.Add(template);
	}

	public List<NpcSpawnTemplate> getSpawns()
	{
		return _spawns;
	}

	public void addTerritory(SpawnTerritory territory)
	{
		_territories.Add(territory);
	}

	public List<SpawnTerritory> getTerritories()
	{
		return _territories;
	}

	public void addBannedTerritory(BannedSpawnTerritory territory)
	{
		_bannedTerritories.Add(territory);
	}

	public List<BannedSpawnTerritory> getBannedTerritories()
	{
		return _bannedTerritories;
	}

	public StatSet getParameters()
	{
		return _parameters;
	}

	public void setParameters(StatSet parameters)
	{
		_parameters.merge(parameters);
	}

	public List<NpcSpawnTemplate> getSpawnsById(int id)
	{
		List<NpcSpawnTemplate> result = new();
		foreach (NpcSpawnTemplate spawn in _spawns)
		{
			if (spawn.getId() == id)
			{
				result.Add(spawn);
			}
		}

		return result;
	}

	public void spawnAll()
	{
		spawnAll(null);
	}

	public void spawnAll(Instance instance)
	{
		_spawns.forEach(template => template.spawn(instance));
	}

	public void despawnAll()
	{
		_spawns.forEach(template => template.despawn());
	}

	public SpawnGroup clone()
	{
		SpawnGroup group = new SpawnGroup(_name, _spawnByDefault);

		// Clone banned territories
		foreach (BannedSpawnTerritory territory in getBannedTerritories())
		{
			group.addBannedTerritory(territory);
		}

		// Clone territories
		foreach (SpawnTerritory territory in getTerritories())
		{
			group.addTerritory(territory);
		}

		// Clone spawns
		foreach (NpcSpawnTemplate spawn in _spawns)
		{
			group.addSpawn(spawn.clone());
		}

		return group;
	}
}