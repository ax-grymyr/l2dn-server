namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class TimedHuntingZoneHolder
{
	private readonly int _id;
	private readonly String _name;
	private readonly int _initialTime;
	private readonly int _maximumAddedTime;
	private readonly int _resetDelay;
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
	private readonly Location _enterLocation;
	private readonly Location _subEnterLocation1;
	private readonly Location _subEnterLocation2;
	private readonly Location _subEnterLocation3;
	private readonly Location _exitLocation;
	private readonly List<MapHolder> _maps = new();

	public TimedHuntingZoneHolder(int id, String name, int initialTime, int maximumAddedTime, int resetDelay,
		int entryItemId, int entryFee, int minLevel, int maxLevel, int remainRefillTime, int refillTimeMax,
		bool pvpZone, bool noPvpZone, int instanceId, bool soloInstance, bool weekly, bool useWorldPrefix,
		bool zonePremiumUserOnly, Location enterLocation, Location subEnterLocation1, Location subEnterLocation2,
		Location subEnterLocation3, Location exitLocation)
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
		_enterLocation = enterLocation;
		_subEnterLocation1 = subEnterLocation1;
		_subEnterLocation2 = subEnterLocation2;
		_subEnterLocation3 = subEnterLocation3;
		_exitLocation = exitLocation;
		_maps.Add(new MapHolder(getMapX(_enterLocation), getMapY(_enterLocation)));
		if (_subEnterLocation1 != null)
		{
			_maps.Add(new MapHolder(getMapX(_subEnterLocation1), getMapY(_subEnterLocation1)));
		}

		if (_subEnterLocation2 != null)
		{
			_maps.Add(new MapHolder(getMapX(_subEnterLocation2), getMapY(_subEnterLocation2)));
		}

		if (_subEnterLocation3 != null)
		{
			_maps.Add(new MapHolder(getMapX(_subEnterLocation3), getMapY(_subEnterLocation3)));
		}
	}

	private int getMapY(Location location)
	{
		return ((location.getY() - World.WORLD_Y_MIN) >> 15) + World.TILE_Y_MIN;
	}

	private int getMapX(Location location)
	{
		return ((location.getX() - World.WORLD_X_MIN) >> 15) + World.TILE_X_MIN;
	}

	public int getZoneId()
	{
		return _id;
	}

	public String getZoneName()
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

	public int getResetDelay()
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

	public Location getEnterLocation()
	{
		return _enterLocation;
	}

	public List<MapHolder> getMaps()
	{
		return _maps;
	}

	public Location getExitLocation()
	{
		return _exitLocation;
	}
}