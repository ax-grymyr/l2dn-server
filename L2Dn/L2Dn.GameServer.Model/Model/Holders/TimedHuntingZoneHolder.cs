using System.Collections.Immutable;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class TimedHuntingZoneHolder
{
	private readonly int _id;
	private readonly string _name;
	private readonly int _initialTime;
	private readonly int _maximumAddedTime;
	private readonly TimeSpan _resetDelay;
	private readonly int _entryItemId;
	private readonly int _entryFee;
	private readonly int _minLevel;
	private readonly int _maxLevel;
	private readonly int _remainRefillTime;
	private readonly int _refillTimeMax;
	private readonly bool _pvpZone;
	private readonly bool _noPvpZone;
	private readonly int _instanceId;
	private readonly bool _soloInstance;
	private readonly bool _weekly;
	private readonly bool _useWorldPrefix;
	private readonly bool _zonePremiumUserOnly;
	private readonly ImmutableArray<Location3D> _enterLocations;
	private readonly Location3D? _exitLocation;
	private readonly ImmutableArray<MapHolder> _maps;

	public TimedHuntingZoneHolder(int id, string name, int initialTime, int maximumAddedTime, TimeSpan resetDelay,
		int entryItemId, int entryFee, int minLevel, int maxLevel, int remainRefillTime, int refillTimeMax,
		bool pvpZone, bool noPvpZone, int instanceId, bool soloInstance, bool weekly, bool useWorldPrefix,
		bool zonePremiumUserOnly, ImmutableArray<Location3D> enterLocations, Location3D? exitLocation)
	{
		_id = id;
		_name = name;
		_initialTime = initialTime;
		_maximumAddedTime = maximumAddedTime;
		_resetDelay = resetDelay;
		_entryItemId = entryItemId;
		_entryFee = entryFee;
		_minLevel = minLevel;
		_maxLevel = maxLevel;
		_remainRefillTime = remainRefillTime;
		_refillTimeMax = refillTimeMax;
		_pvpZone = pvpZone;
		_noPvpZone = noPvpZone;
		_instanceId = instanceId;
		_soloInstance = soloInstance;
		_weekly = weekly;
		_useWorldPrefix = useWorldPrefix;
		_zonePremiumUserOnly = zonePremiumUserOnly;
		_enterLocations = enterLocations;
		_exitLocation = exitLocation;
		_maps = enterLocations.Select(loc => new MapHolder(getMapX(loc.X), getMapY(loc.Y))).ToImmutableArray();
	}

	private static int getMapY(int y)
	{
		return ((y - World.WORLD_Y_MIN) >> 15) + World.TILE_Y_MIN;
	}

	private static int getMapX(int x)
	{
		return ((x - World.WORLD_X_MIN) >> 15) + World.TILE_X_MIN;
	}

	public int getZoneId()
	{
		return _id;
	}

	public string getZoneName()
	{
		return _name;
	}

	public int getInitialTime()
	{
		return _initialTime;
	}

	public int getMaximumAddedTime()
	{
		return _maximumAddedTime;
	}

	public TimeSpan getResetDelay()
	{
		return _resetDelay;
	}

	public int getEntryItemId()
	{
		return _entryItemId;
	}

	public int getEntryFee()
	{
		return _entryFee;
	}

	public int getMinLevel()
	{
		return _minLevel;
	}

	public int getMaxLevel()
	{
		return _maxLevel;
	}

	public int getRemainRefillTime()
	{
		return _remainRefillTime;
	}

	public int getRefillTimeMax()
	{
		return _refillTimeMax;
	}

	public bool isPvpZone()
	{
		return _pvpZone;
	}

	public bool isNoPvpZone()
	{
		return _noPvpZone;
	}

	public int getInstanceId()
	{
		return _instanceId;
	}

	public bool isSoloInstance()
	{
		return _soloInstance;
	}

	public bool isWeekly()
	{
		return _weekly;
	}

	public bool useWorldPrefix()
	{
		return _useWorldPrefix;
	}

	public bool zonePremiumUserOnly()
	{
		return _zonePremiumUserOnly;
	}

	public Location3D getEnterLocation()
	{
		return _enterLocations[0];
	}

	public ImmutableArray<MapHolder> getMaps()
	{
		return _maps;
	}

	public Location3D? getExitLocation()
	{
		return _exitLocation;
	}
}