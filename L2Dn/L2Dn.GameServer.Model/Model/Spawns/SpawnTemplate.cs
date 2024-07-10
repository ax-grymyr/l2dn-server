using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Quests;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Spawns;

public class SpawnTemplate: ITerritorized, IParameterized<StatSet>
{
	private readonly string _name;
	private readonly string _ai;
	private readonly bool _spawnByDefault;
	private readonly string _filePath;
	private List<SpawnTerritory> _territories;
	private List<BannedSpawnTerritory> _bannedTerritories;
	private readonly List<SpawnGroup> _groups = [];
	private StatSet _parameters;

	public SpawnTemplate(string name, string ai, bool spawnByDefault, string filePath)
	{
		_name = name;
		_ai = ai;
		_spawnByDefault = spawnByDefault;
		_filePath = filePath;
	}
	
	public string getName()
	{
		return _name;
	}
	
	public string getAI()
	{
		return _ai;
	}
	
	public bool isSpawningByDefault()
	{
		return _spawnByDefault;
	}
	
	public string getFile()
	{
		return _filePath;
	}
	
	public void addTerritory(SpawnTerritory territory)
	{
		if (_territories == null)
		{
			_territories = new();
		}
		_territories.Add(territory);
	}
	
	public List<SpawnTerritory> getTerritories()
	{
		return _territories != null ? _territories : new();
	}
	
	public void addBannedTerritory(BannedSpawnTerritory territory)
	{
		if (_bannedTerritories == null)
		{
			_bannedTerritories = new();
		}
		_bannedTerritories.Add(territory);
	}
	
	public List<BannedSpawnTerritory> getBannedTerritories()
	{
		return _bannedTerritories != null ? _bannedTerritories : new();
	}
	
	public void addGroup(SpawnGroup group)
	{
		_groups.Add(group);
	}
	
	public List<SpawnGroup> getGroups()
	{
		return _groups;
	}
	
	public List<SpawnGroup> getGroupsByName(string name)
	{
		List<SpawnGroup> result = new();
		foreach (SpawnGroup group in _groups)
		{
			if (group.getName() != null && group.getName().equalsIgnoreCase(name))
			{
				result.add(group);
			}
		}
		return result;
	}
	
	public StatSet getParameters()
	{
		return _parameters;
	}
	
	public void setParameters(StatSet parameters)
	{
		_parameters = parameters;
	}
	
	public void notifyEvent(Action<Quest> @event)
	{
		if (_ai != null)
		{
			Quest script = QuestManager.getInstance().getQuest(_ai);
			if (script != null)
			{
				@event(script);
			}
		}
	}
	
	public void spawn(Predicate<SpawnGroup> groupFilter, Instance instance)
	{
		foreach (SpawnGroup group in _groups)
		{
			if (groupFilter(group))
			{
				group.spawnAll(instance);
			}
		}
	}
	
	public void spawnAll()
	{
		spawnAll(null);
	}
	
	public void spawnAll(Instance instance)
	{
		spawn(g => g.isSpawningByDefault(), instance);
	}
	
	public void notifyActivate()
	{
		notifyEvent(script => script.onSpawnActivate(this));
	}
	
	public void spawnAllIncludingNotDefault(Instance instance)
	{
		_groups.ForEach(group => group.spawnAll(instance));
	}
	
	public void despawn(Predicate<SpawnGroup> groupFilter)
	{
		foreach (SpawnGroup group in _groups)
		{
			if (groupFilter(group))
			{
				group.despawnAll();
			}
		}
		notifyEvent(script => script.onSpawnDeactivate(this));
	}
	
	public void despawnAll()
	{
		_groups.ForEach(g => g.despawnAll());
		notifyEvent(script => script.onSpawnDeactivate(this));
	}
	
	public SpawnTemplate clone()
	{
		SpawnTemplate template = new SpawnTemplate(_name, _ai, _spawnByDefault, _filePath);
		
		// Clone parameters
		template.setParameters(_parameters);
		
		// Clone banned territories
		foreach (BannedSpawnTerritory territory in getBannedTerritories())
		{
			template.addBannedTerritory(territory);
		}
		
		// Clone territories
		foreach (SpawnTerritory territory in getTerritories())
		{
			template.addTerritory(territory);
		}
		
		// Clone groups
		foreach (SpawnGroup group in _groups)
		{
			template.addGroup(group.clone());
		}
		
		return template;
	}
}