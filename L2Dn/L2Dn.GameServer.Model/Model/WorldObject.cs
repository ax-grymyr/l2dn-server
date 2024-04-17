using System.Runtime.CompilerServices;
using System.Text;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Model;

/**
 * Base class for all interactive objects.
 */
public abstract class WorldObject: IIdentifiable, INamable, ISpawnable, IUniqueId, IDecayable, IPositionable
{
	/** Name */
	private string _name;
	/** Object ID */
	private int _objectId;
	/** World Region */
	private WorldRegion _worldRegion;
	/** Location */
	private Location _location = new(0, 0, -10000);
	/** Instance */
	private Instance _instance;
	/** Instance type */
	private InstanceType _instanceType;
	private bool _isSpawned;
	private bool _isInvisible;
	private bool _isTargetable = true;
	private Map<string, object>? _scripts;
	
	public WorldObject(int objectId)
	{
		setInstanceType(InstanceType.WorldObject);
		_objectId = objectId;
	}

	public abstract int getId();
	
	/**
	 * Gets the instance type of object.
	 * @return the instance type
	 */
	public InstanceType getInstanceType()
	{
		return _instanceType;
	}
	
	/**
	 * Sets the instance type.
	 * @param newInstanceType the instance type to set
	 */
	protected void setInstanceType(InstanceType newInstanceType)
	{
		_instanceType = newInstanceType;
	}
	
	/**
	 * Verifies if object is of any given instance types.
	 * @param instanceTypes the instance types to verify
	 * @return {@code true} if object is of any given instance types, {@code false} otherwise
	 */
	public bool isInstanceTypes(params InstanceType[] instanceTypes)
	{
		return instanceTypes.Any(t => _instanceType.IsType(t));
	}
	
	/**
	 * Verifies if object is of any given instance types.
	 * @param instanceTypes the instance types to verify
	 * @return {@code true} if object is of any given instance types, {@code false} otherwise
	 */
	public bool isInstanceTypes(InstanceType instanceType)
	{
		return _instanceType.IsType(instanceType);
	}
	
	public void onAction(Player player)
	{
		onAction(player, true);
	}
	
	public virtual void onAction(Player player, bool interact)
	{
		IActionHandler handler = ActionHandler.getInstance().getHandler(getInstanceType());
		if (handler != null)
		{
			handler.action(player, this, interact);
		}
		
		player.sendPacket(ActionFailedPacket.STATIC_PACKET);
	}
	
	public virtual void onActionShift(Player player)
	{
		IActionShiftHandler handler = ActionShiftHandler.getInstance().getHandler(getInstanceType());
		if (handler != null)
		{
			handler.action(player, this, true);
		}
		
		player.sendPacket(ActionFailedPacket.STATIC_PACKET);
	}
	
	public virtual void onForcedAttack(Player player)
	{
		player.sendPacket(ActionFailedPacket.STATIC_PACKET);
	}
	
	public virtual void onSpawn()
	{
	}
	
	public virtual bool decayMe()
	{
		_isSpawned = false;
		World.getInstance().removeVisibleObject(this, _worldRegion);
		World.getInstance().removeObject(this);
		return true;
	}
	
	public virtual void refreshId()
	{
		World.getInstance().removeObject(this);
		IdManager.getInstance().releaseId(getObjectId());
		_objectId = IdManager.getInstance().getNextId();
	}
	
	public virtual bool spawnMe()
	{
		lock (this)
		{
			// Set the x,y,z position of the WorldObject spawn and update its _worldregion
			_isSpawned = true;
			setWorldRegion(World.getInstance().getRegion(this));
			
			// Add the WorldObject spawn in the _allobjects of World
			World.getInstance().addObject(this);
			
			// Add the WorldObject spawn to _visibleObjects and if necessary to _allplayers of its WorldRegion
			_worldRegion.addVisibleObject(this);
		}
		
		// this can synchronize on others instances, so it's out of synchronized, to avoid deadlocks
		// Add the WorldObject spawn in the world as a visible object
		World.getInstance().addVisibleObject(this, getWorldRegion());
		
		onSpawn();
		
		return true;
	}
	
	public void spawnMe(int x, int y, int z)
	{
		lock (this)
		{
			int spawnX = x;
			if (spawnX > World.WORLD_X_MAX)
			{
				spawnX = World.WORLD_X_MAX - 5000;
			}
			if (spawnX < World.WORLD_X_MIN)
			{
				spawnX = World.WORLD_X_MIN + 5000;
			}
			
			int spawnY = y;
			if (spawnY > World.WORLD_Y_MAX)
			{
				spawnY = World.WORLD_Y_MAX - 5000;
			}
			if (spawnY < World.WORLD_Y_MIN)
			{
				spawnY = World.WORLD_Y_MIN + 5000;
			}
			
			// Set the x,y,z position of the WorldObject. If flagged with _isSpawned, setXYZ will automatically update world region, so avoid that.
			setXYZ(spawnX, spawnY, z);
		}
		
		// Spawn and update its _worldregion
		spawnMe();
	}
	
	/**
	 * Verify if object can be attacked.
	 * @return {@code true} if object can be attacked, {@code false} otherwise
	 */
	public virtual bool canBeAttacked()
	{
		return false;
	}
	
	public abstract bool isAutoAttackable(Creature attacker);
	
	public bool isSpawned()
	{
		return _isSpawned;
	}
	
	public void setSpawned(bool value)
	{
		_isSpawned = value;
	}
	
	public virtual String getName()
	{
		return _name;
	}
	
	public virtual void setName(String value)
	{
		_name = value;
	}
	
	public virtual int getObjectId()
	{
		return _objectId;
	}
	
	public abstract void sendInfo(Player player);
	
	public virtual void sendPacket<TPacket>(TPacket packet)
		where TPacket: struct, IOutgoingPacket 
	{
	}
	
	public virtual void sendPacket(SystemMessageId id)
	{
	}
	
	public virtual Player getActingPlayer()
	{
		return null;
	}
	
	/**
	 * Verify if object is instance of Attackable.
	 * @return {@code true} if object is instance of Attackable, {@code false} otherwise
	 */
	public virtual bool isAttackable()
	{
		return false;
	}
	
	/**
	 * Verify if object is instance of Creature.
	 * @return {@code true} if object is instance of Creature, {@code false} otherwise
	 */
	public virtual bool isCreature()
	{
		return false;
	}
	
	/**
	 * Verify if object is instance of Door.
	 * @return {@code true} if object is instance of Door, {@code false} otherwise
	 */
	public virtual bool isDoor()
	{
		return false;
	}
	
	/**
	 * Verify if object is instance of Artefact.
	 * @return {@code true} if object is instance of Artefact, {@code false} otherwise
	 */
	public virtual bool isArtefact()
	{
		return false;
	}
	
	/**
	 * Verify if object is instance of Monster.
	 * @return {@code true} if object is instance of Monster, {@code false} otherwise
	 */
	public virtual bool isMonster()
	{
		return false;
	}
	
	/**
	 * Verify if object is instance of Npc.
	 * @return {@code true} if object is instance of Npc, {@code false} otherwise
	 */
	public virtual bool isNpc()
	{
		return false;
	}
	
	/**
	 * Verify if object is instance of Pet.
	 * @return {@code true} if object is instance of Pet, {@code false} otherwise
	 */
	public virtual bool isPet()
	{
		return false;
	}
	
	/**
	 * Verify if object is instance of Player.
	 * @return {@code true} if object is instance of Player, {@code false} otherwise
	 */
	public virtual bool isPlayer()
	{
		return false;
	}
	
	/**
	 * Verify if object is instance of Playable.
	 * @return {@code true} if object is instance of Playable, {@code false} otherwise
	 */
	public virtual bool isPlayable()
	{
		return false;
	}
	
	/**
	 * Verify if object is a fake player.
	 * @return {@code true} if object is a fake player, {@code false} otherwise
	 */
	public virtual bool isFakePlayer()
	{
		return false;
	}
	
	/**
	 * Verify if object is instance of Servitor.
	 * @return {@code true} if object is instance of Servitor, {@code false} otherwise
	 */
	public virtual bool isServitor()
	{
		return false;
	}
	
	/**
	 * Verify if object is instance of Summon.
	 * @return {@code true} if object is instance of Summon, {@code false} otherwise
	 */
	public virtual bool isSummon()
	{
		return false;
	}
	
	/**
	 * Verify if object is instance of Trap.
	 * @return {@code true} if object is instance of Trap, {@code false} otherwise
	 */
	public virtual bool isTrap()
	{
		return false;
	}
	
	/**
	 * Verify if object is instance of Cubic.
	 * @return {@code true} if object is instance of Cubic, {@code false} otherwise
	 */
	public virtual bool isCubic()
	{
		return false;
	}
	
	/**
	 * Verify if object is instance of Item.
	 * @return {@code true} if object is instance of Item, {@code false} otherwise
	 */
	public virtual bool isItem()
	{
		return false;
	}
	
	/**
	 * Verifies if the object is a walker NPC.
	 * @return {@code true} if object is a walker NPC, {@code false} otherwise
	 */
	public virtual bool isWalker()
	{
		return false;
	}
	
	/**
	 * Verifies if this object is a vehicle.
	 * @return {@code true} if object is Vehicle, {@code false} otherwise
	 */
	public virtual bool isVehicle()
	{
		return false;
	}
	
	/**
	 * Verifies if this object is a fence.
	 * @return {@code true} if object is Fence, {@code false} otherwise
	 */
	public virtual bool isFence()
	{
		return false;
	}
	
	public virtual void setTargetable(bool targetable)
	{
		if (_isTargetable != targetable)
		{
			_isTargetable = targetable;
			if (!targetable)
			{
				World.getInstance().forEachVisibleObject<Creature>(this, creature =>
				{
					if (creature.getTarget() == this)
					{
						creature.setTarget(null);
						creature.abortAttack();
						creature.abortCast();
					}
				});
			}
		}
	}
	
	/**
	 * @return {@code true} if the object can be targetted by other players, {@code false} otherwise.
	 */
	public virtual bool isTargetable()
	{
		return _isTargetable;
	}
	
	/**
	 * Check if the object is in the given zone Id.
	 * @param zone the zone Id to check
	 * @return {@code true} if the object is in that zone Id
	 */
	public virtual bool isInsideZone(ZoneId zone)
	{
		return false;
	}
	
	/**
	 * @param <T>
	 * @param script
	 * @return
	 */
	public T addScript<T>(T script)
		where T: class
	{
		if (_scripts == null)
		{
			// Double-checked locking
			lock (this)
			{
				if (_scripts == null)
				{
					_scripts = new();
				}
			}
		}
		_scripts.put(script.GetType().Name, script);
		return script;
	}
	
	/**
	 * @param <T>
	 * @param script
	 * @return
	 */
	public T? removeScript<T>()
		where T: class
	{
		if (_scripts == null)
		{
			return null;
		}
		return (T?) _scripts.remove(typeof(T).Name);
	}
	
	/**
	 * @param <T>
	 * @param script
	 * @return
	 */
	public T? getScript<T>()
		where T: class
	{
		if (_scripts == null)
		{
			return null;
		}
		return (T?) _scripts.get(typeof(T).Name);
	}
	
	public virtual void removeStatusListener(Creature @object)
	{
	}
	
	public void setXYZInvisible(int x, int y, int z)
	{
		int correctX = x;
		if (correctX > World.WORLD_X_MAX)
		{
			correctX = World.WORLD_X_MAX - 5000;
		}
		if (correctX < World.WORLD_X_MIN)
		{
			correctX = World.WORLD_X_MIN + 5000;
		}
		
		int correctY = y;
		if (correctY > World.WORLD_Y_MAX)
		{
			correctY = World.WORLD_Y_MAX - 5000;
		}
		if (correctY < World.WORLD_Y_MIN)
		{
			correctY = World.WORLD_Y_MIN + 5000;
		}
		
		setXYZ(correctX, correctY, z);
		setSpawned(false);
	}
	
	public void setLocationInvisible(ILocational loc)
	{
		setXYZInvisible(loc.getX(), loc.getY(), loc.getZ());
	}
	
	public WorldRegion getWorldRegion()
	{
		return _worldRegion;
	}
	
	public void setWorldRegion(WorldRegion region)
	{
		if ((region == null) && (_worldRegion != null))
		{
			_worldRegion.removeVisibleObject(this);
		}
		_worldRegion = region;
	}
	
	/**
	 * Gets the X coordinate.
	 * @return the X coordinate
	 */
	public virtual int getX()
	{
		return _location.getX();
	}
	
	/**
	 * Gets the Y coordinate.
	 * @return the Y coordinate
	 */
	public virtual int getY()
	{
		return _location.getY();
	}
	
	/**
	 * Gets the Z coordinate.
	 * @return the Z coordinate
	 */
	public virtual int getZ()
	{
		return _location.getZ();
	}
	
	/**
	 * Gets the heading.
	 * @return the heading
	 */
	public virtual int getHeading()
	{
		return _location.getHeading();
	}
	
	/**
	 * Gets the instance ID.
	 * @return the instance ID
	 */
	public virtual int getInstanceId()
	{
		Instance instance = _instance;
		return (instance != null) ? instance.getId() : 0;
	}
	
	/**
	 * Check if object is inside instance world.
	 * @return {@code true} when object is inside any instance world, otherwise {@code false}
	 */
	public virtual bool isInInstance()
	{
		return _instance != null;
	}
	
	/**
	 * Get instance world where object is currently located.
	 * @return {@link Instance} if object is inside instance world, otherwise {@code null}
	 */
	public virtual Instance getInstanceWorld()
	{
		return _instance;
	}
	
	/**
	 * Gets the location object.
	 * @return the location object
	 */
	public virtual Location getLocation()
	{
		return _location;
	}
	
	/**
	 * Sets the x, y, z coordinate.
	 * @param newX the X coordinate
	 * @param newY the Y coordinate
	 * @param newZ the Z coordinate
	 */
	public virtual void setXYZ(int newX, int newY, int newZ)
	{
		_location.setXYZ(newX, newY, newZ);
		
		if (_isSpawned)
		{
			WorldRegion newRegion = World.getInstance().getRegion(this);
			if ((newRegion != null) && (newRegion != _worldRegion))
			{
				if (_worldRegion != null)
				{
					_worldRegion.removeVisibleObject(this);
				}
				newRegion.addVisibleObject(this);
				World.getInstance().switchRegion(this, newRegion);
				setWorldRegion(newRegion);
			}
		}
	}
	
	/**
	 * Sets the x, y, z coordinate.
	 * @param loc the location object
	 */
	public virtual void setXYZ(ILocational loc)
	{
		setXYZ(loc.getX(), loc.getY(), loc.getZ());
	}
	
	/**
	 * Sets heading of object.
	 * @param newHeading the new heading
	 */
	public virtual void setHeading(int newHeading)
	{
		_location.setHeading(newHeading);
	}
	
	/**
	 * Sets instance for current object by instance ID.
	 * @param id ID of instance world which should be set (0 means normal world)
	 */
	public void setInstanceById(int id)
	{
		Instance instance = InstanceManager.getInstance().getInstance(id);
		if ((id != 0) && (instance == null))
		{
			return;
		}
		setInstance(instance);
	}
	
	/**
	 * Sets instance where current object belongs.
	 * @param newInstance new instance world for object
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void setInstance(Instance newInstance)
	{
		// Check if new and old instances are identical
		if (_instance == newInstance)
		{
			return;
		}
		
		// Leave old instance
		if (_instance != null)
		{
			_instance.onInstanceChange(this, false);
		}
		
		// Set new instance
		_instance = newInstance;
		
		// Enter into new instance
		if (newInstance != null)
		{
			newInstance.onInstanceChange(this, true);
		}
	}
	
	/**
	 * Sets location of object.
	 * @param loc the location object
	 */
	public virtual void setLocation(Location loc)
	{
		_location.setXYZ(loc.getX(), loc.getY(), loc.getZ());
		_location.setHeading(loc.getHeading());
	}
	
	/**
	 * Calculates 2D distance between this WorldObject and given x, y, z.
	 * @param x the X coordinate
	 * @param y the Y coordinate
	 * @param z the Z coordinate
	 * @return distance between object and given x, y, z.
	 */
	public double calculateDistance2D(int x, int y, int z)
	{
		return Math.Sqrt(Math.Pow(x - getX(), 2) + Math.Pow(y - getY(), 2));
	}
	
	/**
	 * Calculates the 2D distance between this WorldObject and given location.
	 * @param loc the location object
	 * @return distance between object and given location.
	 */
	public double calculateDistance2D(ILocational loc)
	{
		return calculateDistance2D(loc.getX(), loc.getY(), loc.getZ());
	}
	
	/**
	 * Calculates the 3D distance between this WorldObject and given x, y, z.
	 * @param x the X coordinate
	 * @param y the Y coordinate
	 * @param z the Z coordinate
	 * @return distance between object and given x, y, z.
	 */
	public double calculateDistance3D(int x, int y, int z)
	{
		return Math.Sqrt(Math.Pow(x - getX(), 2) + Math.Pow(y - getY(), 2) + Math.Pow(z - getZ(), 2));
	}
	
	/**
	 * Calculates 3D distance between this WorldObject and given location.
	 * @param loc the location object
	 * @return distance between object and given location.
	 */
	public double calculateDistance3D(ILocational loc)
	{
		return calculateDistance3D(loc.getX(), loc.getY(), loc.getZ());
	}
	
	/**
	 * Calculates the non squared 2D distance between this WorldObject and given x, y, z.
	 * @param x the X coordinate
	 * @param y the Y coordinate
	 * @param z the Z coordinate
	 * @return distance between object and given x, y, z.
	 */
	public double calculateDistanceSq2D(int x, int y, int z)
	{
		return Math.Pow(x - getX(), 2) + Math.Pow(y - getY(), 2);
	}
	
	/**
	 * Calculates the non squared 2D distance between this WorldObject and given location.
	 * @param loc the location object
	 * @return distance between object and given location.
	 */
	public double calculateDistanceSq2D(ILocational loc)
	{
		return calculateDistanceSq2D(loc.getX(), loc.getY(), loc.getZ());
	}
	
	/**
	 * Calculates the non squared 3D distance between this WorldObject and given x, y, z.
	 * @param x the X coordinate
	 * @param y the Y coordinate
	 * @param z the Z coordinate
	 * @return distance between object and given x, y, z.
	 */
	public double calculateDistanceSq3D(int x, int y, int z)
	{
		return Math.Pow(x - getX(), 2) + Math.Pow(y - getY(), 2) + Math.Pow(z - getZ(), 2);
	}
	
	/**
	 * Calculates the non squared 3D distance between this WorldObject and given location.
	 * @param loc the location object
	 * @return distance between object and given location.
	 */
	public double calculateDistanceSq3D(ILocational loc)
	{
		return calculateDistanceSq3D(loc.getX(), loc.getY(), loc.getZ());
	}
	
	/**
	 * Calculates the angle in degrees from this object to the given object.<br>
	 * The return value can be described as how much this object has to turn<br>
	 * to have the given object directly in front of it.
	 * @param target the object to which to calculate the angle
	 * @return the angle this object has to turn to have the given object in front of it
	 */
	public double calculateDirectionTo(ILocational target)
	{
		return Util.calculateAngleFrom(this, target);
	}
	
	/**
	 * @return {@code true} if this object is invisible, {@code false} otherwise.
	 */
	public bool isInvisible()
	{
		return _isInvisible;
	}
	
	/**
	 * Sets this object as invisible or not
	 * @param invisible
	 */
	public void setInvisible(bool invisible)
	{
		_isInvisible = invisible;
		
		if (invisible)
		{
			DeleteObjectPacket deletePacket = new(_objectId);
			World.getInstance().forEachVisibleObject<Player>(this, player =>
			{
				if (!isVisibleFor(player))
				{
					player.sendPacket(deletePacket);
				}
			});
		}
		
		// Broadcast information regarding the object to those which are suppose to see.
		broadcastInfo();
	}
	
	/**
	 * @param player
	 * @return {@code true} if player can see an invisible object if it's invisible, {@code false} otherwise.
	 */
	public virtual bool isVisibleFor(Player player)
	{
		return !_isInvisible || player.canOverrideCond(PlayerCondOverride.SEE_ALL_PLAYERS);
	}
	
	/**
	 * Broadcasts describing info to known players.
	 */
	public void broadcastInfo()
	{
		World.getInstance().forEachVisibleObject<Player>(this, player =>
		{
			if (isVisibleFor(player))
			{
				sendInfo(player);
			}
		});
	}
	
	public virtual bool isInvul()
	{
		return false;
	}
	
	public bool isInSurroundingRegion(WorldObject? worldObject)
	{
		if (worldObject == null)
		{
			return false;
		}
		
		WorldRegion worldRegion = worldObject.getWorldRegion();
		if (worldRegion == null)
		{
			return false;
		}
		
		if (_worldRegion == null)
		{
			return false;
		}
		
		return worldRegion.isSurroundingRegion(_worldRegion);
	}
	
	public override bool Equals(object? obj)
	{
		return obj is WorldObject worldObject && worldObject.getObjectId() == getObjectId();
	}
	
	public override string ToString()
	{
		StringBuilder sb = new();
		sb.Append(GetType().Name);
		sb.Append(':');
		sb.Append(_name);
		sb.Append('[');
		sb.Append(_objectId);
		sb.Append(']');
		return sb.ToString();
	}
}