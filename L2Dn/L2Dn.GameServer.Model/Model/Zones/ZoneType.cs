using L2Dn.Events;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Zones;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model;
using L2Dn.Model.Enums;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Model.Zones;

/**
 * Abstract base class for any zone type handles basic operations.
 * @author durgus
 */
public abstract class ZoneType: IEventContainerProvider
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(ZoneType));
	
	private readonly int _id;
	protected ZoneForm _zone;
	protected List<ZoneForm> _blockedZones;
	private readonly Map<int, Creature> _characterList = new();
	private readonly EventContainer _eventContainer;
	
	/** Parameters to affect specific characters */
	private bool _checkAffected;
	private string _name;
	private int _minLevel;
	private int _maxLevel;
	private Race[] _race;
	private CharacterClass[] _class;
	private int _classType;
	private InstanceType _target = InstanceType.Creature; // default all chars
	private bool _allowStore;
	protected bool _enabled;
	private AbstractZoneSettings _settings;
	private int _instanceTemplateId;
	private Map<int, bool> _enabledInInstance;
	
	protected ZoneType(int id)
	{
		_eventContainer = new($"Zone template {id}", GlobalEvents.Global);
		_id = id;
		_minLevel = 0;
		_maxLevel = 0xFF;
		_classType = 0;
		_race = null;
		_class = null;
		_allowStore = true;
		_enabled = true;
	}

	public EventContainer Events => _eventContainer;
	
	/**
	 * @return Returns the id.
	 */
	public int getId()
	{
		return _id;
	}
	
	/**
	 * Setup new parameters for this zone
	 * @param name
	 * @param value
	 */
	public virtual void setParameter(string name, string value)
	{
		_checkAffected = true;
		
		// Zone name
		if (name.equals("name"))
		{
			_name = value;
		}
		// Minimum level
		else if (name.equals("affectedLvlMin"))
		{
			_minLevel = int.Parse(value);
		}
		// Maximum level
		else if (name.equals("affectedLvlMax"))
		{
			_maxLevel = int.Parse(value);
		}
		// Affected Races
		else if (name.equals("affectedRace"))
		{
			// Create a new array holding the affected race
			if (_race == null)
			{
				_race = new Race[1];
				_race[0] = (Race)int.Parse(value);
			}
			else
			{
				Race[] temp = new Race[_race.Length + 1];
				int i = 0;
				for (; i < _race.Length; i++)
					temp[i] = _race[i];
				
				temp[i] = (Race)int.Parse(value);
				_race = temp;
			}
		}
		// Affected classes
		else if (name.equals("affectedClassId"))
		{
			// Create a new array holding the affected classIds
			if (_class == null)
			{
				_class = new CharacterClass[1];
				_class[0] = (CharacterClass)int.Parse(value);
			}
			else
			{
				CharacterClass[] temp = new CharacterClass[_class.Length + 1];
				int i = 0;
				for (; i < _class.Length; i++)
					temp[i] = _class[i];
				
				temp[i] = (CharacterClass)int.Parse(value);
				_class = temp;
			}
		}
		// Affected class type
		else if (name.equals("affectedClassType"))
		{
			if (value.equals("Fighter"))
			{
				_classType = 1;
			}
			else
			{
				_classType = 2;
			}
		}
		else if (name.equals("targetClass"))
		{
			_target = Enum.Parse<InstanceType>(value);
		}
		else if (name.equals("allowStore"))
		{
			_allowStore = bool.Parse(value);
		}
		else if (name.equals("default_enabled"))
		{
			_enabled = bool.Parse(value);
		}
		else if (name.equals("instanceId"))
		{
			_instanceTemplateId = int.Parse(value);
		}
		else
		{
			LOGGER.Info(GetType().Name + ": Unknown parameter - " + name + " in zone: " + _id);
		}
	}
	
	/**
	 * @param creature the player to verify.
	 * @return {@code true} if the given character is affected by this zone, {@code false} otherwise.
	 */
	private bool isAffected(Creature creature)
	{
		// Check instance
		Instance world = creature.getInstanceWorld();
		if (world != null)
		{
			if (world.getTemplateId() != _instanceTemplateId)
			{
				return false;
			}
			if (!isEnabled(creature.getInstanceId()))
			{
				return false;
			}
		}
		else if (_instanceTemplateId > 0)
		{
			return false;
		}
		
		// Check level
		if ((creature.getLevel() < _minLevel) || (creature.getLevel() > _maxLevel))
		{
			if (creature.isPlayer())
			{
				creature.getActingPlayer().sendPacket(new ExShowScreenMessagePacket(
					SystemMessageId.YOU_CANNOT_ENTER_AS_YOUR_LEVEL_DOES_NOT_MEET_THE_REQUIREMENTS,
					ExShowScreenMessagePacket.TOP_CENTER, 10000));
			}
			
			return false;
		}
		
		// check obj class
		if (!creature.isInstanceTypes(_target))
		{
			return false;
		}
		
		if (creature.isPlayer())
		{
			// Check class type
			if (_classType != 0)
			{
				if (((Player) creature).isMageClass())
				{
					if (_classType == 1)
					{
						return false;
					}
				}
				else if (_classType == 2)
				{
					return false;
				}
			}
			
			// Check race
			if (_race != null)
			{
				bool ok = false;
				foreach (Race element in _race)
				{
					if (creature.getRace() == element)
					{
						ok = true;
						break;
					}
				}
				
				if (!ok)
				{
					return false;
				}
			}
			
			// Check class
			if (_class != null)
			{
				bool ok = false;
				foreach (CharacterClass _clas in _class)
				{
					if (((Player) creature).getClassId() == _clas)
					{
						ok = true;
						break;
					}
				}
				
				if (!ok)
				{
					return false;
				}
			}
		}
		return true;
	}
	
	/**
	 * Set the zone for this ZoneType Instance
	 * @param zone
	 */
	public void setZone(ZoneForm zone)
	{
		if (_zone != null)
		{
			throw new InvalidOperationException("Zone already set");
		}
		_zone = zone;
	}
	
	/**
	 * Returns this zones zone form.
	 * @return {@link #_zone}
	 */
	public ZoneForm getZone()
	{
		return _zone;
	}
	
	public void setBlockedZones(List<ZoneForm> blockedZones)
	{
		if (_blockedZones != null)
		{
			throw new InvalidOperationException("Blocked zone already set");
		}
		_blockedZones = blockedZones;
	}
	
	public List<ZoneForm> getBlockedZones()
	{
		return _blockedZones;
	}
	
	/**
	 * Set the zone name.
	 * @param name
	 */
	public void setName(string name)
	{
		_name = name;
	}
	
	/**
	 * Returns zone name
	 * @return
	 */
	public string getName()
	{
		return _name;
	}
	
	/**
	 * Checks if the given coordinates are within the zone, ignores instanceId check.
	 * @param x
	 * @param y
	 * @param z
	 * @return
	 */
	public bool isInsideZone(int x, int y, int z)
	{
		return (_zone != null) && _zone.isInsideZone(x, y, z) && !isInsideBlockedZone(x, y, z);
	}
	
	/**
	 * @param x
	 * @param y
	 * @param z
	 * @return {@code true} if this location is within blocked zone boundaries, {@code false} otherwise.
	 */
	public bool isInsideBlockedZone(int x, int y, int z)
	{
		if ((_blockedZones == null) || _blockedZones.isEmpty())
		{
			return false;
		}
		
		foreach (ZoneForm zone in _blockedZones)
		{
			if (zone.isInsideZone(x, y, z))
			{
				return true;
			}
		}
		
		return false;
	}
	
	/**
	 * Checks if the given coordinates are within zone's plane
	 * @param x
	 * @param y
	 * @return
	 */
	public bool isInsideZone(Location2D location)
	{
		return isInsideZone(new Location3D(location.X, location.Y, _zone.getHighZ()));
	}
	
	/**
	 * Checks if the given coordinates are within the zone, ignores instanceId check
	 * @param loc
	 * @return
	 */
	public bool isInsideZone(Location3D location)
	{
		return isInsideZone(location.X, location.Y, location.Z);
	}
	
	/**
	 * Checks if the given object is inside the zone.
	 * @param object
	 * @return
	 */
	public bool isInsideZone(WorldObject obj)
	{
		return isInsideZone(obj.getX(), obj.getY(), obj.getZ());
	}
	
	public double getDistanceToZone(int x, int y)
	{
		return _zone.getDistanceToZone(x, y);
	}
	
	public double getDistanceToZone(WorldObject obj)
	{
		return _zone.getDistanceToZone(obj.getX(), obj.getY());
	}
	
	public void revalidateInZone(Creature creature)
	{
		// If the object is inside the zone...
		if (isInsideZone(creature))
		{
			// If the character can't be affected by this zone return
			if (_checkAffected && !isAffected(creature))
			{
				return;
			}
			
			if (_characterList.TryAdd(creature.getObjectId(), creature))
			{
				// Notify to scripts.
				if (_eventContainer.HasSubscribers<OnZoneEnter>())
				{
					_eventContainer.NotifyAsync(new OnZoneEnter(creature, this));
				}
				
				// Notify Zone implementation.
				onEnter(creature);
			}
		}
		else
		{
			removeCharacter(creature);
		}
	}
	
	/**
	 * Force fully removes a character from the zone Should use during teleport / logoff
	 * @param creature
	 */
	public void removeCharacter(Creature creature)
	{
		// Was the character inside this zone?
		if (_characterList.containsKey(creature.getObjectId()))
		{
			// Notify to scripts.
			if (_eventContainer.HasSubscribers<OnZoneExit>())
			{
				_eventContainer.NotifyAsync(new OnZoneExit(creature, this));
			}
			
			// Unregister player.
			_characterList.remove(creature.getObjectId());
			
			// Notify Zone implementation.
			onExit(creature);
		}
	}
	
	/**
	 * Will scan the zones char list for the character
	 * @param creature
	 * @return
	 */
	public bool isCharacterInZone(Creature creature)
	{
		return _characterList.containsKey(creature.getObjectId());
	}
	
	public virtual AbstractZoneSettings getSettings()
	{
		return _settings;
	}
	
	public void setSettings(AbstractZoneSettings settings)
	{
		if (_settings != null)
		{
			_settings.clear();
		}
		_settings = settings;
	}
	
	protected abstract void onEnter(Creature creature);
	
	protected abstract void onExit(Creature creature);
	
	public virtual void onDieInside(Creature creature)
	{
	}
	
	public void onReviveInside(Creature creature)
	{
	}
	
	public virtual void onPlayerLoginInside(Player player)
	{
	}
	
	public virtual void onPlayerLogoutInside(Player player)
	{
	}
	
	public Map<int, Creature> getCharacters()
	{
		return _characterList;
	}
	
	public ICollection<Creature> getCharactersInside()
	{
		return _characterList.values();
	}
	
	public List<Player> getPlayersInside()
	{
		List<Player> players = new();
		foreach (Creature ch in _characterList.values())
		{
			if ((ch != null) && ch.isPlayer())
			{
				players.add(ch.getActingPlayer());
			}
		}
		return players;
	}
	
	/**
	 * Broadcasts packet to all players inside the zone
	 * @param packet
	 */
	public void broadcastPacket<TPacket>(TPacket packet)
		where TPacket: struct, IOutgoingPacket
	{
		if (_characterList.isEmpty())
		{
			return;
		}
		
		foreach (Creature creature in _characterList.values())
		{
			if ((creature != null) && creature.isPlayer())
			{
				creature.sendPacket(packet);
			}
		}
	}
	
	public InstanceType getTargetType()
	{
		return _target;
	}
	
	public void setTargetType(InstanceType type)
	{
		_target = type;
		_checkAffected = true;
	}
	
	public bool getAllowStore()
	{
		return _allowStore;
	}
	
	public int getInstanceTemplateId()
	{
		return _instanceTemplateId;
	}
	
	public override string ToString()
	{
		return GetType().Name + "[" + _id + "]";
	}
	
	public void visualizeZone(int z)
	{
		_zone.visualizeZone(z);
	}
	
	public virtual void setEnabled(bool value)
	{
		_enabled = value;
	}
	
	public bool isEnabled()
	{
		return _enabled;
	}
	
	public void setEnabled(bool state, int instanceId)
	{
		if (_enabledInInstance == null)
		{
			lock (this)
			{
				if (_enabledInInstance == null)
				{
					_enabledInInstance = new();
				}
			}
		}
		
		_enabledInInstance.put(instanceId, state);
	}
	
	public bool isEnabled(int instanceId)
	{
		if (_enabledInInstance != null)
		{
			return _enabledInInstance.getOrDefault(instanceId, _enabled);
		}
		return _enabled;
	}
	
	public virtual void oustAllPlayers()
	{
		foreach (Creature obj in _characterList.values())
		{
			if ((obj != null) && obj.isPlayer() && obj.getActingPlayer().isOnline())
			{
				obj.getActingPlayer().teleToLocation(TeleportWhereType.TOWN);
			}
		}
	}
	
	/**
	 * @param loc
	 */
	public void movePlayersTo(LocationHeading loc)
	{
		if (_characterList.isEmpty())
		{
			return;
		}
		
		foreach (Creature creature in _characterList.values())
		{
			if ((creature != null) && creature.isPlayer())
			{
				Player player = creature.getActingPlayer();
				if (player.isOnline())
				{
					player.teleToLocation(loc);
				}
			}
		}
	}
}