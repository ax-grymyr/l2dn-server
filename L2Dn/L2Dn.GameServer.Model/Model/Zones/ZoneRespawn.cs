using System.Collections.Immutable;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Zones;

/**
 * Abstract zone with spawn locations
 * @author DS, Nyaran (rework 10/07/2011)
 */
public abstract class ZoneRespawn(int id): ZoneType(id)
{
	private ImmutableArray<Location3D> _spawnLocs = ImmutableArray<Location3D>.Empty;
	private ImmutableArray<Location3D> _otherSpawnLocs = ImmutableArray<Location3D>.Empty;
	private ImmutableArray<Location3D> _chaoticSpawnLocs = ImmutableArray<Location3D>.Empty;
	private ImmutableArray<Location3D> _banishSpawnLocs = ImmutableArray<Location3D>.Empty;

	public virtual void parseLoc(Location3D location, string type)
	{
		if (string.IsNullOrEmpty(type))
		{
			addSpawn(location);
		}
		else
		{
			switch (type)
			{
				case "other":
				{
					addOtherSpawn(location);
					break;
				}
				case "chaotic":
				{
					addChaoticSpawn(location);
					break;
				}
				case "banish":
				{
					addBanishSpawn(location);
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

	public void addSpawn(Location3D location)
	{
		_spawnLocs = _spawnLocs.Add(location);
	}

	public void addOtherSpawn(Location3D location)
	{
		_otherSpawnLocs = _otherSpawnLocs.Add(location);
	}

	public void addChaoticSpawn(Location3D location)
	{
		_chaoticSpawnLocs = _chaoticSpawnLocs.Add(location);
	}

	public void addBanishSpawn(Location3D location)
	{
		_banishSpawnLocs = _banishSpawnLocs.Add(location);
	}

	public ImmutableArray<Location3D> getSpawns()
	{
		return _spawnLocs;
	}

	public Location3D getSpawnLoc()
	{
		return Config.RANDOM_RESPAWN_IN_TOWN_ENABLED ? _spawnLocs[Rnd.get(_spawnLocs.Length)] : _spawnLocs[0];
	}

	public Location3D getOtherSpawnLoc()
	{
		if (_otherSpawnLocs.Length != 0)
		{
			return Config.RANDOM_RESPAWN_IN_TOWN_ENABLED
				? _otherSpawnLocs[Rnd.get(_otherSpawnLocs.Length)]
				: _otherSpawnLocs[0];
		}

		return getSpawnLoc();
	}

	public Location3D getChaoticSpawnLoc()
	{
		if (_chaoticSpawnLocs.Length != 0)
		{
			return Config.RANDOM_RESPAWN_IN_TOWN_ENABLED
				? _chaoticSpawnLocs[Rnd.get(_chaoticSpawnLocs.Length)]
				: _chaoticSpawnLocs[0];
		}

		return getSpawnLoc();
	}

	public virtual Location3D getBanishSpawnLoc()
	{
		if (_banishSpawnLocs.Length != 0)
		{
			return Config.RANDOM_RESPAWN_IN_TOWN_ENABLED
				? _banishSpawnLocs[Rnd.get(_banishSpawnLocs.Length)]
				: _banishSpawnLocs[0];
		}

		return getSpawnLoc();
	}
}