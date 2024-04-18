using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model;

public class MapRegion
{
	private readonly string _name;
	private readonly string _town;
	private readonly int _locId;
	private readonly int _bbs;
	private List<int[]> _maps;
	
	private List<Location> _spawnLocs;
	private List<Location> _otherSpawnLocs;
	private List<Location> _chaoticSpawnLocs;
	private List<Location> _banishSpawnLocs;
	
	private readonly Map<Race, string> _bannedRace = new();
	
	public MapRegion(string name, string town, int locId, int bbs)
	{
		_name = name;
		_town = town;
		_locId = locId;
		_bbs = bbs;
	}
	
	public string getName()
	{
		return _name;
	}
	
	public string getTown()
	{
		return _town;
	}
	
	public int getLocId()
	{
		return _locId;
	}
	
	public int getBbs()
	{
		return _bbs;
	}
	
	public void addMap(int x, int y)
	{
		if (_maps == null)
		{
			_maps = new();
		}
		
		_maps.Add(new int[]
		{
			x,
			y
		});
	}
	
	public List<int[]> getMaps()
	{
		return _maps;
	}
	
	public bool isZoneInRegion(int x, int y)
	{
		if (_maps == null)
		{
			return false;
		}
		
		foreach (int[] map in _maps)
		{
			if ((map[0] == x) && (map[1] == y))
			{
				return true;
			}
		}
		return false;
	}
	
	// Respawn
	public void addSpawn(int x, int y, int z)
	{
		if (_spawnLocs == null)
		{
			_spawnLocs = new();
		}
		
		_spawnLocs.Add(new Location(x, y, z));
	}
	
	public void addOtherSpawn(int x, int y, int z)
	{
		if (_otherSpawnLocs == null)
		{
			_otherSpawnLocs = new();
		}
		
		_otherSpawnLocs.Add(new Location(x, y, z));
	}
	
	public void addChaoticSpawn(int x, int y, int z)
	{
		if (_chaoticSpawnLocs == null)
		{
			_chaoticSpawnLocs = new();
		}
		
		_chaoticSpawnLocs.Add(new Location(x, y, z));
	}
	
	public void addBanishSpawn(int x, int y, int z)
	{
		if (_banishSpawnLocs == null)
		{
			_banishSpawnLocs = new();
		}
		
		_banishSpawnLocs.Add(new Location(x, y, z));
	}
	
	public List<Location> getSpawns()
	{
		return _spawnLocs;
	}
	
	public Location getSpawnLoc()
	{
		if (Config.RANDOM_RESPAWN_IN_TOWN_ENABLED)
		{
			return _spawnLocs.get(Rnd.get(_spawnLocs.size()));
		}
		return _spawnLocs[0];
	}
	
	public Location getOtherSpawnLoc()
	{
		if (_otherSpawnLocs != null)
		{
			if (Config.RANDOM_RESPAWN_IN_TOWN_ENABLED)
			{
				return _otherSpawnLocs.get(Rnd.get(_otherSpawnLocs.size()));
			}
			return _otherSpawnLocs[0];
		}
		return getSpawnLoc();
	}
	
	public Location getChaoticSpawnLoc()
	{
		if (_chaoticSpawnLocs != null)
		{
			if (Config.RANDOM_RESPAWN_IN_TOWN_ENABLED)
			{
				return _chaoticSpawnLocs.get(Rnd.get(_chaoticSpawnLocs.size()));
			}
			return _chaoticSpawnLocs[0];
		}
		return getSpawnLoc();
	}
	
	public Location getBanishSpawnLoc()
	{
		if (_banishSpawnLocs != null)
		{
			if (Config.RANDOM_RESPAWN_IN_TOWN_ENABLED)
			{
				return _banishSpawnLocs.get(Rnd.get(_banishSpawnLocs.size()));
			}
			return _banishSpawnLocs[0];
		}
		return getSpawnLoc();
	}
	
	public void addBannedRace(Race race, string point)
	{
		_bannedRace.put(race, point);
	}
	
	public Map<Race, string> getBannedRace()
	{
		return _bannedRace;
	}
}
