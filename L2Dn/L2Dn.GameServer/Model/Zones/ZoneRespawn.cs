using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Zones;

/**
 * Abstract zone with spawn locations
 * @author DS, Nyaran (rework 10/07/2011)
 */
public abstract class ZoneRespawn: ZoneType
{
	private List<Location> _spawnLocs = null;
	private List<Location> _otherSpawnLocs = null;
	private List<Location> _chaoticSpawnLocs = null;
	private List<Location> _banishSpawnLocs = null;

	protected ZoneRespawn(int id): base(id)
	{
	}

	public virtual void parseLoc(int x, int y, int z, String type)
	{
		if ((type == null) || type.isEmpty())
		{
			addSpawn(x, y, z);
		}
		else
		{
			switch (type)
			{
				case "other":
				{
					addOtherSpawn(x, y, z);
					break;
				}
				case "chaotic":
				{
					addChaoticSpawn(x, y, z);
					break;
				}
				case "banish":
				{
					addBanishSpawn(x, y, z);
					break;
				}
				default:
				{
					LOGGER.Warn(GetType().Name + ": Unknown location type: " + type);
					break;
				}
			}
		}
	}

	public void addSpawn(int x, int y, int z)
	{
		if (_spawnLocs == null)
		{
			_spawnLocs = new();
		}

		_spawnLocs.add(new Location(x, y, z));
	}

	public void addOtherSpawn(int x, int y, int z)
	{
		if (_otherSpawnLocs == null)
		{
			_otherSpawnLocs = new();
		}

		_otherSpawnLocs.add(new Location(x, y, z));
	}

	public void addChaoticSpawn(int x, int y, int z)
	{
		if (_chaoticSpawnLocs == null)
		{
			_chaoticSpawnLocs = new();
		}

		_chaoticSpawnLocs.add(new Location(x, y, z));
	}

	public void addBanishSpawn(int x, int y, int z)
	{
		if (_banishSpawnLocs == null)
		{
			_banishSpawnLocs = new();
		}

		_banishSpawnLocs.add(new Location(x, y, z));
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

		return _spawnLocs.get(0);
	}

	public Location getOtherSpawnLoc()
	{
		if (_otherSpawnLocs != null)
		{
			if (Config.RANDOM_RESPAWN_IN_TOWN_ENABLED)
			{
				return _otherSpawnLocs.get(Rnd.get(_otherSpawnLocs.size()));
			}

			return _otherSpawnLocs.get(0);
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

			return _chaoticSpawnLocs.get(0);
		}

		return getSpawnLoc();
	}

	public virtual Location getBanishSpawnLoc()
	{
		if (_banishSpawnLocs != null)
		{
			if (Config.RANDOM_RESPAWN_IN_TOWN_ENABLED)
			{
				return _banishSpawnLocs.get(Rnd.get(_banishSpawnLocs.size()));
			}

			return _banishSpawnLocs.get(0);
		}

		return getSpawnLoc();
	}
}