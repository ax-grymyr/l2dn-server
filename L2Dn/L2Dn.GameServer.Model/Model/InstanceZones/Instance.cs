using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Events.Impl.Instances;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Spawns;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model.Enums;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.InstanceZones;

/**
 * Instance world.
 * @author malyelfik
 */
public class Instance : IIdentifiable, INamable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Instance));
	
	// Basic instance parameters
	private readonly int _id;
	private readonly InstanceTemplate _template;
	private readonly DateTime _startTime;
	private DateTime? _endTime;
	// Advanced instance parameters
	private readonly Set<int> _allowed = new(); // Player ids which can enter to instance
	private readonly Set<Player> _players = new(); // Players inside instance
	private readonly Set<Npc> _npcs = new(); // Spawned NPCs inside instance
	private readonly Map<int, Door> _doors = new(); // Spawned doors inside instance
	private readonly StatSet _parameters = new StatSet();
	// Timers
	private readonly Map<int, ScheduledFuture> _ejectDeadTasks = new();
	private ScheduledFuture _cleanUpTask = null;
	private ScheduledFuture _emptyDestroyTask = null;
	private readonly List<SpawnTemplate> _spawns;
	
	/**
	 * Create instance world.
	 * @param id ID of instance world
	 * @param template template of instance world
	 * @param player player who create instance world.
	 */
	public Instance(int id, InstanceTemplate template, Player player)
	{
		// Set basic instance info
		_id = id;
		_template = template;
		_startTime = DateTime.UtcNow;
		_spawns = new(template.getSpawns().Count);
		
		// Clone and add the spawn templates
		foreach (SpawnTemplate spawn in template.getSpawns())
		{
			_spawns.Add(spawn.clone());
		}
		
		// Register world to instance manager.
		InstanceManager.getInstance().register(this);
		
		// Set duration, spawns, status, etc..
		setDuration(_template.getDuration());
		setStatus(0);
		spawnDoors();
		
		// Initialize instance spawns.
		foreach (SpawnTemplate spawnTemplate in _spawns)
		{
			if (spawnTemplate.isSpawningByDefault())
			{
				spawnTemplate.spawnAll(this);
			}
		}
		
		// Notify DP scripts
		if (!isDynamic() && _template.Events.HasSubscribers<OnInstanceCreated>())
		{
			_template.Events.NotifyAsync(new OnInstanceCreated(this, player));
		}
	}
	
	public int getId()
	{
		return _id;
	}
	
	public string getName()
	{
		return _template.getName();
	}
	
	/**
	 * Check if instance has been created dynamically or have XML template.
	 * @return {@code true} if instance is dynamic or {@code false} if instance has static template
	 */
	public bool isDynamic()
	{
		return _template.getId() == -1;
	}
	
	/**
	 * Set instance world parameter.
	 * @param key parameter name
	 * @param value parameter value
	 */
	public void setParameter(string key, object value)
	{
		if (value == null)
		{
			_parameters.remove(key);
		}
		else
		{
			_parameters.set(key, value);
		}
	}
	
	/**
	 * Set instance world parameter.
	 * @param key parameter name
	 * @param value parameter value
	 */
	public void setParameter(string key, bool value)
	{
		_parameters.set(key, value);
	}
	
	/**
	 * Get instance world parameters.
	 * @return instance parameters
	 */
	public StatSet getParameters()
	{
		return _parameters;
	}
	
	/**
	 * Get status of instance world.
	 * @return instance status, otherwise 0
	 */
	public int getStatus()
	{
		return _parameters.getInt("INSTANCE_STATUS", 0);
	}
	
	/**
	 * Check if instance status is equal to {@code status}.
	 * @param status number used for status comparison
	 * @return {@code true} when instance status and {@code status} are equal, otherwise {@code false}
	 */
	public bool isStatus(int status)
	{
		return getStatus() == status;
	}
	
	/**
	 * Set status of instance world.
	 * @param value new world status
	 */
	public void setStatus(int value)
	{
		_parameters.set("INSTANCE_STATUS", value);
		
		if (_template.Events.HasSubscribers<OnInstanceStatusChange>())
		{
			_template.Events.NotifyAsync(new OnInstanceStatusChange(this, value));
		}
	}
	
	/**
	 * Increment instance world status
	 * @return new world status
	 */
	public int incStatus()
	{
		int status = getStatus() + 1;
		setStatus(status);
		return status;
	}
	
	/**
	 * Add player who can enter to instance.
	 * @param player player instance
	 */
	public void addAllowed(Player player)
	{
		if (!_allowed.Contains(player.getObjectId()))
		{
			_allowed.add(player.getObjectId());
		}
	}
	
	/**
	 * Check if player can enter to instance.
	 * @param player player itself
	 * @return {@code true} when can enter, otherwise {@code false}
	 */
	public bool isAllowed(Player player)
	{
		return _allowed.Contains(player.getObjectId());
	}
	
	/**
	 * Returns all players who can enter to instance.
	 * @return allowed players list
	 */
	public List<Player> getAllowed()
	{
		List<Player> allowed = new(_allowed.size());
		foreach (int playerId in _allowed)
		{
			Player player = World.getInstance().getPlayer(playerId);
			if (player != null)
			{
				allowed.Add(player);
			}
		}
		return allowed;
	}
	
	/**
	 * Add player to instance
	 * @param player player instance
	 */
	public void addPlayer(Player player)
	{
		_players.add(player);
		if (_emptyDestroyTask != null)
		{
			_emptyDestroyTask.cancel(false);
			_emptyDestroyTask = null;
		}
	}
	
	/**
	 * Remove player from instance.
	 * @param player player instance
	 */
	public void removePlayer(Player player)
	{
		_players.remove(player);
		if (_players.isEmpty())
		{
			TimeSpan emptyTime = _template.getEmptyDestroyTime();
			if ((_template.getDuration() == TimeSpan.Zero) || (emptyTime == TimeSpan.Zero))
			{
				destroy();
			}
			else if ((emptyTime >= TimeSpan.Zero) && (_emptyDestroyTask == null) && (getRemainingTime() < emptyTime))
			{
				_emptyDestroyTask = ThreadPool.schedule(destroy, emptyTime);
			}
		}
	}
	
	/**
	 * Check if player is inside instance.
	 * @param player player to be checked
	 * @return {@code true} if player is inside, otherwise {@code false}
	 */
	public bool containsPlayer(Player player)
	{
		return _players.Contains(player);
	}
	
	/**
	 * Get all players inside instance.
	 * @return players within instance
	 */
	public Set<Player> getPlayers()
	{
		return _players;
	}
	
	/**
	 * Get count of players inside instance.
	 * @return players count inside instance
	 */
	public int getPlayersCount()
	{
		return _players.size();
	}
	
	/**
	 * Get first found player from instance world.<br>
	 * <i>This method is useful for instances with one player inside.</i>
	 * @return first found player, otherwise {@code null}
	 */
	public Player getFirstPlayer()
	{
		foreach (Player player in _players)
		{
			return player;
		}
		return null;
	}
	
	/**
	 * Get player by ID from instance.
	 * @param id objectId of player
	 * @return first player by ID, otherwise {@code null}
	 */
	public Player getPlayerById(int id)
	{
		foreach (Player player in _players)
		{
			if (player.getObjectId() == id)
			{
				return player;
			}
		}
		return null;
	}
	
	/**
	 * Get all players from instance world inside specified radius.
	 * @param object location of target
	 * @param radius radius around target
	 * @return players within radius
	 */
	public List<Player> getPlayersInsideRadius(Location3D location, int radius)
	{
		List<Player> result = new();
		foreach (Player player in _players)
		{
			if (player.IsInsideRadius3D(location, radius))
				result.Add(player);
		}

		return result;
	}
	
	/**
	 * Spawn doors inside instance world.
	 */
	private void spawnDoors()
	{
		ICollection<DoorTemplate> doorTemplates = _template.getDoors().Values;
		Map<int, bool> doorStates = _template.getDoorStates();
		foreach (DoorTemplate template in doorTemplates)
		{
			bool? isOpenedByDefault = null;
			if (doorStates.TryGetValue(template.getId(), out bool isOpened))
				isOpenedByDefault = isOpened;
			
			// Create new door instance
			Door door = DoorData.getInstance().spawnDoor(template, this, isOpenedByDefault);
			_doors.put(template.getId(), door);
		}
	}
	
	/**
	 * Get all doors spawned inside instance world.
	 * @return collection of spawned doors
	 */
	public ICollection<Door> getDoors()
	{
		return _doors.values();
	}
	
	/**
	 * Get spawned door by template ID.
	 * @param id template ID of door
	 * @return instance of door if found, otherwise {@code null}
	 */
	public Door getDoor(int id)
	{
		return _doors.get(id);
	}
	
	/**
	 * Handle open/close status of instance doors.
	 * @param id ID of doors
	 * @param open {@code true} means open door, {@code false} means close door
	 */
	public void openCloseDoor(int id, bool open)
	{
		Door door = _doors.get(id);
		if (door != null)
		{
			if (open)
			{
				if (!door.isOpen())
				{
					door.openMe();
				}
			}
			else if (door.isOpen())
			{
				door.closeMe();
			}
		}
	}
	
	/**
	 * Check if spawn group with name {@code name} exists.
	 * @param name name of group to be checked
	 * @return {@code true} if group exist, otherwise {@code false}
	 */
	public bool isSpawnGroupExist(string name)
	{
		foreach (SpawnTemplate spawnTemplate in _spawns)
		{
			foreach (SpawnGroup group in spawnTemplate.getGroups())
			{
				if (name.equalsIgnoreCase(group.getName()))
				{
					return true;
				}
			}
		}
		return false;
	}
	
	/**
	 * Get spawn group by group name.
	 * @param name name of group
	 * @return list which contains spawn data from spawn group
	 */
	public List<SpawnGroup> getSpawnGroup(string name)
	{
		List<SpawnGroup> spawns = new();
		foreach (SpawnTemplate spawnTemplate in _spawns)
		{
			spawns.AddRange(spawnTemplate.getGroupsByName(name));
		}
		return spawns;
	}
	
	/**
	 * @param name
	 * @return {@code List} of NPCs that are part of specified group
	 */
	public List<Npc> getNpcsOfGroup(string name)
	{
		return getNpcsOfGroup(name, null);
	}
	
	/**
	 * @param groupName
	 * @param filterValue
	 * @return {@code List} of NPCs that are part of specified group and matches filter specified
	 */
	public List<Npc> getNpcsOfGroup(string groupName, Predicate<Npc> filterValue)
	{
		Predicate<Npc> filter = filterValue;
		if (filter == null)
		{
			filter = x => x is not null;
		}
		
		List<Npc> npcs = new();
		foreach (SpawnTemplate spawnTemplate in _spawns)
		{
			foreach (SpawnGroup group in spawnTemplate.getGroupsByName(groupName))
			{
				foreach (NpcSpawnTemplate npcTemplate in group.getSpawns())
				{
					foreach (Npc npc in npcTemplate.getSpawnedNpcs())
					{
						if (filter(npc))
						{
							npcs.Add(npc);
						}
					}
				}
			}
		}
		return npcs;
	}
	
	/**
	 * @param groupName
	 * @param filterValue
	 * @return {@code Npc} instance of an NPC that is part of a group and matches filter specified
	 */
	public Npc getNpcOfGroup(string groupName, Predicate<Npc> filterValue)
	{
		Predicate<Npc> filter = filterValue;
		if (filter == null)
		{
			filter = x => x is not null;
		}
		
		foreach (SpawnTemplate spawnTemplate in _spawns)
		{
			foreach (SpawnGroup group in spawnTemplate.getGroupsByName(groupName))
			{
				foreach (NpcSpawnTemplate npcTemplate in group.getSpawns())
				{
					foreach (Npc npc in npcTemplate.getSpawnedNpcs())
					{
						if (filter(npc))
						{
							return npc;
						}
					}
				}
			}
		}
		return null;
	}
	
	/**
	 * Spawn NPCs from group (defined in XML template) into instance world.
	 * @param name name of group which should be spawned
	 * @return list that contains NPCs spawned by this method
	 */
	public List<Npc> spawnGroup(string name)
	{
		List<SpawnGroup> spawns = getSpawnGroup(name);
		if (spawns == null)
		{
			LOGGER.Warn("Spawn group " + name + " doesn't exist for instance " + _template.getName() + " (" + _id + ")!");
			return new();
		}
		
		List<Npc> npcs = new();
		try
		{
			foreach (SpawnGroup holder in spawns)
			{
				holder.spawnAll(this);
				holder.getSpawns().ForEach(spawn => npcs.AddRange(spawn.getSpawnedNpcs()));
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Unable to spawn group " + name + " inside instance " + _template.getName() + " (" + _id + ")");
		}
		return npcs;
	}
	
	/**
	 * De-spawns NPCs from group (defined in XML template) from the instance world.
	 * @param name of group which should be de-spawned
	 */
	public void despawnGroup(string name)
	{
		List<SpawnGroup> spawns = getSpawnGroup(name);
		if (spawns == null)
		{
			LOGGER.Warn("Spawn group " + name + " doesn't exist for instance " + _template.getName() + " (" + _id + ")!");
			return;
		}
		
		try
		{
			spawns.ForEach(x => x.despawnAll());
		}
		catch (Exception e)
		{
			LOGGER.Warn("Unable to spawn group " + name + " inside instance " + _template.getName() + " (" + _id + ")");
		}
	}
	
	/**
	 * Get spawned NPCs from instance.
	 * @return set of NPCs from instance
	 */
	public Set<Npc> getNpcs()
	{
		return _npcs;
	}
	
	/**
	 * Get spawned NPCs from instance with specific IDs.
	 * @param id IDs of NPCs which should be found
	 * @return list of filtered NPCs from instance
	 */
	public List<Npc> getNpcs(params int[] id)
	{
		List<Npc> result = new();
		foreach (Npc npc in _npcs)
		{
			if (Array.IndexOf(id, npc.getId()) >= 0)
			{
				result.Add(npc);
			}
		}
		return result;
	}
	
	/**
	 * Get spawned NPCs from instance with specific IDs and class type.
	 * @param <T>
	 * @param clazz
	 * @param ids IDs of NPCs which should be found
	 * @return list of filtered NPCs from instance
	 */
	public List<T> getNpcs<T>(params int[] ids)
		where T: Npc
	{
		List<T> result = new();
		foreach (Npc npc in _npcs)
		{
			if ((ids.Length == 0 || Array.IndexOf(ids, npc.getId()) >= 0) && npc is T npc1)
			{
				result.Add(npc1);
			}
		}
		return result;
	}
	
	/**
	 * Get alive NPCs from instance.
	 * @return set of NPCs from instance
	 */
	public List<Npc> getAliveNpcs()
	{
		List<Npc> result = new();
		foreach (Npc npc in _npcs)
		{
			if (npc.getCurrentHp() > 0)
			{
				result.Add(npc);
			}
		}
		return result;
	}
	
	/**
	 * Get alive NPCs from instance with specific IDs.
	 * @param id IDs of NPCs which should be found
	 * @return list of filtered NPCs from instance
	 */
	public List<Npc> getAliveNpcs(params int[] id)
	{
		List<Npc> result = new();
		foreach (Npc npc in _npcs)
		{
			if ((npc.getCurrentHp() > 0) && Array.IndexOf(id, npc.getId()) >= 0)
			{
				result.Add(npc);
			}
		}
		return result;
	}
	
	/**
	 * Get spawned and alive NPCs from instance with specific IDs and class type.
	 * @param <T>
	 * @param clazz
	 * @param ids IDs of NPCs which should be found
	 * @return list of filtered NPCs from instance
	 */
	public List<T> getAliveNpcs<T>(params int[] ids)
		where T: Npc
	{
		List<T> result = new();
		foreach (Npc npc in _npcs)
		{
			if ((ids.Length == 0 || Array.IndexOf(ids, npc.getId()) >= 0) && npc.getCurrentHp() > 0 && npc is T npc1)
			{
				result.Add(npc1);
			}
		}
		return result;
	}
	
	/**
	 * Get alive NPC count from instance.
	 * @return count of filtered NPCs from instance
	 */
	public int getAliveNpcCount()
	{
		int count = 0;
		foreach (Npc npc in _npcs)
		{
			if (npc.getCurrentHp() > 0)
			{
				count++;
			}
		}
		return count;
	}
	
	/**
	 * Get alive NPC count from instance with specific IDs.
	 * @param id IDs of NPCs which should be counted
	 * @return count of filtered NPCs from instance
	 */
	public int getAliveNpcCount(params int[] id)
	{
		int count = 0;
		foreach (Npc npc in _npcs)
		{
			if ((npc.getCurrentHp() > 0) && Array.IndexOf(id, npc.getId()) >= 0)
			{
				count++;
			}
		}
		return count;
	}
	
	/**
	 * Get first found spawned NPC with specific ID.
	 * @param id ID of NPC to be found
	 * @return first found NPC with specified ID, otherwise {@code null}
	 */
	public Npc getNpc(int id)
	{
		foreach (Npc npc in _npcs)
		{
			if (npc.getId() == id)
			{
				return npc;
			}
		}
		return null;
	}
	
	public void addNpc(Npc npc)
	{
		_npcs.add(npc);
	}
	
	public void removeNpc(Npc npc)
	{
		_npcs.remove(npc);
	}
	
	/**
	 * Remove all players from instance world.
	 */
	private void removePlayers()
	{
		_players.ForEach(ejectPlayer);
		_players.clear();
	}
	
	/**
	 * Despawn doors inside instance world.
	 */
	private void removeDoors()
	{
		foreach (Door door in _doors.values())
		{
			if (door != null)
			{
				door.decayMe();
			}
		}
		_doors.clear();
	}
	
	/**
	 * Despawn NPCs inside instance world.
	 */
	public void removeNpcs()
	{
		_spawns.ForEach(x => x.despawnAll());
		_npcs.ForEach(x => x.deleteMe());
		_npcs.clear();
	}
	
	/**
	 * Change instance duration.
	 * @param minutes remaining time to destroy instance
	 */
	public void setDuration(TimeSpan? duration)
	{
		// Instance never ends
		if (duration is null)
		{
			_endTime = null;
			return;
		}
		
		// Stop running tasks
		if (_cleanUpTask != null)
		{
			_cleanUpTask.cancel(true);
			_cleanUpTask = null;
		}
		
		if ((_emptyDestroyTask != null) && (duration.Value < _emptyDestroyTask.getDelay()))
		{
			_emptyDestroyTask.cancel(true);
			_emptyDestroyTask = null;
		}
		
		// Set new cleanup task
		_endTime = DateTime.UtcNow + duration.Value;
		if (duration.Value <= TimeSpan.Zero) // Destroy instance
		{
			destroy();
		}
		else
		{
			sendWorldDestroyMessage(duration.Value);
			if (duration.Value <= TimeSpan.FromMinutes(5)) // Message 1 minute before destroy
			{
				_cleanUpTask = ThreadPool.schedule(cleanUp, duration.Value - TimeSpan.FromMinutes(1));
			}
			else // Message 5 minutes before destroy
			{
				_cleanUpTask = ThreadPool.schedule(cleanUp, duration.Value - TimeSpan.FromMinutes(5));
			}
		}
	}
	
	/**
	 * Destroy current instance world.<br>
	 * <b><font color=red>Use this method to destroy instance world properly.</font></b>
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void destroy()
	{
		if (_cleanUpTask != null)
		{
			_cleanUpTask.cancel(false);
			_cleanUpTask = null;
		}
		
		if (_emptyDestroyTask != null)
		{
			_emptyDestroyTask.cancel(false);
			_emptyDestroyTask = null;
		}
		
		_ejectDeadTasks.Values.ForEach(t => t.cancel(true));
		_ejectDeadTasks.Clear();
		
		// Notify DP scripts
		if (!isDynamic() && _template.Events.HasSubscribers<OnInstanceDestroy>())
		{
			_template.Events.Notify(new OnInstanceDestroy(this));
		}
		
		removePlayers();
		removeDoors();
		removeNpcs();
		
		InstanceManager.getInstance().unregister(getId());
	}
	
	/**
	 * Teleport player out of instance.
	 * @param player player that should be moved out
	 */
	public void ejectPlayer(Player player)
	{
		Instance world = player.getInstanceWorld();
		if ((world != null) && world == this)
		{
			Location3D? loc = _template.getExitLocation(player);
			if (loc != null)
			{
				player.teleToLocation(new Location(loc.Value, 0), null);
			}
			else
			{
				player.teleToLocation(TeleportWhereType.TOWN, null);
			}
		}
	}
	
	/**
	 * Send packet to each player from instance world.
	 * @param packets packets to be send
	 */
	public PacketSendUtil broadcastPacket()
	{
		return new PacketSendUtil(_players);
	}
	
	/**
	 * Get instance creation time.
	 * @return creation time in milliseconds
	 */
	public DateTime getStartTime()
	{
		return _startTime;
	}
	
	/**
	 * Get elapsed time since instance create.
	 * @return elapsed time in milliseconds
	 */
	public TimeSpan getElapsedTime()
	{
		return DateTime.UtcNow - _startTime;
	}
	
	/**
	 * Get remaining time before instance will be destroyed.
	 * @return remaining time in milliseconds if duration is not equal to -1, otherwise -1
	 */
	public TimeSpan? getRemainingTime()
	{
		return _endTime is null ? null : (_endTime.Value - DateTime.UtcNow);
	}
	
	/**
	 * Get instance destroy time.
	 * @return destroy time in milliseconds if duration is not equal to -1, otherwise -1
	 */
	public DateTime? getEndTime()
	{
		return _endTime;
	}
	
	/**
	 * Set reenter penalty for players associated with current instance.<br>
	 * Penalty time is calculated from XML reenter data.
	 */
	public void setReenterTime()
	{
		setReenterTime(_template.calculateReenterTime());
	}
	
	/**
	 * Set reenter penalty for players associated with current instance.
	 * @param time penalty time in milliseconds since January 1, 1970
	 */
	public void setReenterTime(DateTime time)
	{
		// Cannot store reenter data for instance without template id.
		if (_template.getId() == -1)
		{
			return;
		}
		
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			
			// Save to database
			foreach (int playerId in _allowed)
			{
				ctx.CharacterInstances.Add(new CharacterInstance()
				{
					CharacterId = playerId,
					InstanceId = _template.getId(),
					Time = time
				});
			}

			ctx.SaveChanges();
			
			// Save to memory and send message to player
			SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId
				.INSTANCE_ZONE_S1_S_ENTRY_HAS_BEEN_RESTRICTED_YOU_CAN_CHECK_THE_NEXT_POSSIBLE_ENTRY_TIME_WITH_INSTANCEZONE);
			
			if (InstanceManager.getInstance().getInstanceName(getTemplateId()) != null)
			{
				msg.Params.addInstanceName(_template.getId());
			}
			else
			{
				msg.Params.addString(_template.getName());
			}
			_allowed.ForEach(playerId =>
			{
				InstanceManager.getInstance().setReenterPenalty(playerId, getTemplateId(), time);
				Player player = World.getInstance().getPlayer(playerId);
				if ((player != null) && player.isOnline())
				{
					player.sendPacket(msg);
				}
			});
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not insert character instance reenter data: " + e);
		}
	}
	
	/**
	 * Set instance world to finish state.<br>
	 * Calls method {@link Instance#finishInstance(int)} with {@link Config#INSTANCE_FINISH_TIME} as argument.
	 */
	public void finishInstance()
	{
		finishInstance(TimeSpan.FromMinutes(Config.INSTANCE_FINISH_TIME));
	}
	
	/**
	 * Set instance world to finish state.<br>
	 * Set re-enter for allowed players if required data are defined in template.<br>
	 * Change duration of instance and set empty destroy time to 0 (instant effect).
	 * @param delay delay in minutes
	 */
	public void finishInstance(TimeSpan delay)
	{
		// Set re-enter for players
		if (_template.getReenterType() == InstanceReenterType.ON_FINISH)
		{
			setReenterTime();
		}
		// Change instance duration
		setDuration(delay);
	}
	
	// ---------------------------------------------
	// Listeners
	// ---------------------------------------------
	/**
	 * This method is called when player dies inside instance.
	 * @param player
	 */
	public void onDeath(Player player)
	{
		if (!player.isOnEvent() && (_template.getEjectTime() > TimeSpan.Zero))
		{
			// Send message
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.IF_YOU_ARE_NOT_RESURRECTED_IN_S1_MIN_YOU_WILL_BE_TELEPORTED_OUT_OF_THE_INSTANCE_ZONE);
			sm.Params.addInt((int)_template.getEjectTime().TotalMinutes);
			player.sendPacket(sm);
			
			// Start eject task
			_ejectDeadTasks.put(player.getObjectId(), ThreadPool.schedule(() =>
			{
				if (player.isDead())
				{
					ejectPlayer(player.getActingPlayer());
				}
			}, _template.getEjectTime())); // minutes to milliseconds
		}
	}
	
	/**
	 * This method is called when player was resurrected inside instance.
	 * @param player resurrected player
	 */
	public void doRevive(Player player)
	{
		ScheduledFuture task = _ejectDeadTasks.remove(player.getObjectId());
		if (task != null)
		{
			task.cancel(true);
		}
	}
	
	/**
	 * This method is called when object enter or leave this instance.
	 * @param object instance of object which enters/leaves instance
	 * @param enter {@code true} when object enter, {@code false} when object leave
	 */
	public void onInstanceChange(WorldObject @object, bool enter)
	{
		if (@object.isPlayer())
		{
			Player player = @object.getActingPlayer();
			if (enter)
			{
				addPlayer(player);
				
				// Set origin return location if enabled
				if (_template.getExitLocationType() == InstanceTeleportType.ORIGIN)
				{
					player.getVariables().set(PlayerVariables.INSTANCE_ORIGIN, player.getX() + ";" + player.getY() + ";" + player.getZ());
				}
				
				// Remove player buffs
				if (_template.isRemoveBuffEnabled())
				{
					_template.removePlayerBuff(player);
				}
				
				// Notify DP scripts
				if (!isDynamic() && _template.Events.HasSubscribers<OnInstanceEnter>())
				{
					_template.Events.NotifyAsync(new OnInstanceEnter(player, this));
				}
			}
			else
			{
				removePlayer(player);
				
				// Notify DP scripts
				if (!isDynamic() && _template.Events.HasSubscribers<OnInstanceLeave>())
				{
					_template.Events.NotifyAsync(new OnInstanceLeave(player, this));
				}
			}
		}
		else if (@object.isNpc())
		{
			Npc npc = (Npc) @object;
			if (enter)
			{
				addNpc(npc);
			}
			else
			{
				if (npc.getSpawn() != null)
				{
					npc.getSpawn().stopRespawn();
				}
				removeNpc(npc);
			}
		}
	}
	
	/**
	 * This method is called when player logout inside instance world.
	 * @param player player who logout
	 */
	public void onPlayerLogout(Player player)
	{
		removePlayer(player);
		if (Config.RESTORE_PLAYER_INSTANCE)
		{
			player.getVariables().set(PlayerVariables.INSTANCE_RESTORE, _id);
		}
		else
		{
			Location3D? loc = getExitLocation(player);
			if (loc != null)
			{
				player.setLocationInvisible(loc.Value);
				// If player has death pet, put him out of instance world
				Summon pet = player.getPet();
				if (pet != null)
				{
					pet.teleToLocation(new Location(loc.Value, 0), true);
				}
			}
		}
	}
	
	// ----------------------------------------------
	// Template methods
	// ----------------------------------------------
	/**
	 * Get parameters from instance template.
	 * @return template parameters
	 */
	public StatSet getTemplateParameters()
	{
		return _template.getParameters();
	}
	
	/**
	 * Get template ID of instance world.
	 * @return instance template ID
	 */
	public int getTemplateId()
	{
		return _template.getId();
	}
	
	/**
	 * Get type of re-enter data.
	 * @return type of re-enter (see {@link InstanceReenterType} for possible values)
	 */
	public InstanceReenterType getReenterType()
	{
		return _template.getReenterType();
	}
	
	/**
	 * Check if instance world is PvP zone.
	 * @return {@code true} when instance is PvP zone, otherwise {@code false}
	 */
	public bool isPvP()
	{
		return _template.isPvP();
	}
	
	/**
	 * Check if summoning players to instance world is allowed.
	 * @return {@code true} when summon is allowed, otherwise {@code false}
	 */
	public bool isPlayerSummonAllowed()
	{
		return _template.isPlayerSummonAllowed();
	}
	
	/**
	 * Get enter location for instance world.
	 * @return {@link Location} object if instance has enter location defined, otherwise {@code null}
	 */
	public Location? getEnterLocation()
	{
		return _template.getEnterLocation();
	}
	
	/**
	 * Get all enter locations defined in XML template.
	 * @return list of enter locations
	 */
	public ImmutableArray<Location> getEnterLocations()
	{
		return _template.getEnterLocations();
	}
	
	/**
	 * Get exit location for player from instance world.
	 * @param player instance of player who wants to leave instance world
	 * @return {@link Location} object if instance has exit location defined, otherwise {@code null}
	 */
	public Location3D? getExitLocation(Player player)
	{
		return _template.getExitLocation(player);
	}
	
	/**
	 * @return the exp rate of the instance
	 */
	public float getExpRate()
	{
		return _template.getExpRate();
	}
	
	/**
	 * @return the sp rate of the instance
	 */
	public float getSPRate()
	{
		return _template.getSPRate();
	}
	
	/**
	 * @return the party exp rate of the instance
	 */
	public float getExpPartyRate()
	{
		return _template.getExpPartyRate();
	}
	
	/**
	 * @return the party sp rate of the instance
	 */
	public float getSPPartyRate()
	{
		return _template.getSPPartyRate();
	}
	
	// ----------------------------------------------
	// Tasks
	// ----------------------------------------------
	/**
	 * Clean up instance.
	 */
	private void cleanUp()
	{
		if (getRemainingTime() <= TimeSpan.FromMinutes(1))
		{
			sendWorldDestroyMessage(TimeSpan.FromMinutes(1));
			_cleanUpTask = ThreadPool.schedule(destroy, TimeSpan.FromMinutes(1)); // 1 minute
		}
		else
		{
			sendWorldDestroyMessage(TimeSpan.FromMinutes(5));
			_cleanUpTask = ThreadPool.schedule(cleanUp, TimeSpan.FromMinutes(5)); // 5 minutes
		}
	}
	
	/**
	 * Show instance destroy messages to players inside instance world.
	 * @param delay time in minutes
	 */
	private void sendWorldDestroyMessage(TimeSpan delay)
	{
		// Dimensional wrap does not show timer after 5 minutes.
		if (delay > TimeSpan.FromMinutes(5))
		{
			return;
		}
		
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_INSTANCE_ZONE_EXPIRES_IN_S1_MIN_AFTER_THAT_YOU_WILL_BE_TELEPORTED_OUTSIDE_2);
		sm.Params.addInt((int)delay.TotalMinutes);
		broadcastPacket().SendPackets(sm);
	}
	
	public override bool Equals(object? obj)
	{
		return (obj is Instance) && (((Instance) obj).getId() == getId());
	}
	
	public override string ToString()
	{
		return _template.getName() + "(" + _id + ")";
	}
}