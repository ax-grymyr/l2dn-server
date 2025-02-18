using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;

namespace L2Dn.GameServer.InstanceManagers;

public class FortManager
{
	public const int ORC_FORTRESS = 122;
	public const int ORC_FORTRESS_FLAG = 93331; // 9819
	public const int FLAG_MAX_COUNT = 3;
	public const int ORC_FORTRESS_FLAGPOLE_ID = 23170500;
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(FortManager));

	private static readonly Map<int, Fort> _forts = new();

	public Fort? findNearestFort(WorldObject obj, long maxDistanceValue = long.MaxValue)
	{
		Fort? nearestFort = getFort(obj);
		if (nearestFort == null)
		{
			long maxDistance = maxDistanceValue;
			foreach (Fort fort in _forts.Values)
			{
				double distance = fort.getDistance(obj);
				if (maxDistance > distance)
				{
					maxDistance = (long) distance;
					nearestFort = fort;
				}
			}
		}
		return nearestFort;
	}

	public Fort? getFortById(int fortId)
	{
		foreach (Fort f in _forts.Values)
		{
			if (f.getResidenceId() == fortId)
			{
				return f;
			}
		}
		return null;
	}

	public Fort? getFortByOwner(Clan clan)
	{
		if (clan == null)
		{
			return null;
		}
		foreach (Fort f in _forts.Values)
		{
			if (f.getOwnerClan() == clan)
			{
				return f;
			}
		}
		return null;
	}

	public Fort? getFort(string name)
	{
		foreach (Fort f in _forts.Values)
		{
			if (f.getName().equalsIgnoreCase(name.Trim()))
			{
				return f;
			}
		}
		return null;
	}

	public Fort? getFort(Location3D location)
	{
		foreach (Fort f in _forts.Values)
		{
			if (f.checkIfInZone(location))
			{
				return f;
			}
		}
		return null;
	}

	public Fort? getFort(WorldObject activeObject)
	{
		// TODO: Make this more abstract?
		// return getFort(activeObject.getX(), activeObject.getY(), activeObject.getZ());
		return getFortById(ORC_FORTRESS);
	}

	public ICollection<Fort> getForts()
	{
		return _forts.Values;
	}

	public void loadInstances()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (var fort in ctx.Forts.Select(f => new { f.Id, f.Name }).OrderBy(f => f.Id))
			{
				_forts.put(fort.Id, new Fort(fort.Id, fort.Name));
			}

			LOGGER.Info(GetType().Name +": Loaded " + _forts.Values.Count + " fortress.");
			foreach (Fort fort in _forts.Values)
			{
				fort.getSiege().loadSiegeGuard();
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception: loadFortData(): " + e);
		}
	}

	public void activateInstances()
	{
		foreach (Fort fort in _forts.Values)
		{
			fort.activateInstance();
		}
	}

	public static FortManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly FortManager INSTANCE = new();
	}
}