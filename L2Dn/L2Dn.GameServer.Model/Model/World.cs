using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Network;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using NLog;

namespace L2Dn.GameServer.Model;

public sealed class World
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(World));

	public static volatile int MAX_CONNECTED_COUNT = 0;
	public static volatile int OFFLINE_TRADE_COUNT = 0;

	/** Map containing all the players in game. */
	private static readonly Map<int, Player> _allPlayers = new();

	/** Map containing all the Good players in game. */
	private static readonly Map<int, Player> _allGoodPlayers = new();

	/** Map containing all the Evil players in game. */
	private static readonly Map<int, Player> _allEvilPlayers = new();

	/** Map containing all the players in Store Buy or Sell mode. */
	private static readonly Map<int, Player> _allStoreModeBuySellPlayers = new();

	/** Map containing all visible objects. */
	private static readonly Map<int, WorldObject> _allObjects = new();

	/** Map with the pets instances and their owner ID. */
	private static readonly Map<int, Pet> _petsInstance = new();

	private static int _partyNumber;
	private static int _memberInPartyNumber;

    private readonly WorldRegionCollection _worldRegions = new();

	private static DateTime _nextPrivateStoreUpdate;

	private World()
	{
	}

	/**
	 * Adds an object to the world.<br>
	 * <br>
	 * <b><u>Example of use</u>:</b>
	 * <ul>
	 * <li>Withdraw an item from the warehouse, create an item</li>
	 * <li>Spawn a Creature (PC, NPC, Pet)</li>
	 * </ul>
	 * @param object
	 */
	public void addObject(WorldObject @object)
	{
		_allObjects.TryAdd(@object.ObjectId, @object);

		if (@object.isPlayer())
		{
			Player newPlayer = (Player)@object;
			if (newPlayer.isTeleporting()) // TODO: Drop when we stop removing player from the world while teleporting.
			{
				return;
			}

			Player existingPlayer = _allPlayers.GetOrAdd(@object.ObjectId, newPlayer);
			if (existingPlayer != newPlayer)
			{
				LeaveWorldPacket packet = LeaveWorldPacket.STATIC_PACKET;
				Disconnection.of(existingPlayer).defaultSequence(ref packet);
				Disconnection.of(newPlayer).defaultSequence(ref packet);
				LOGGER.Warn(GetType().Name + ": Duplicate character!? Disconnected both characters (" + newPlayer.getName() + ")");
			}
			else if (Config.FactionSystem.FACTION_SYSTEM_ENABLED)
			{
				addFactionPlayerToWorld(newPlayer);
			}
		}
	}

	/**
	 * Removes an object from the world.<br>
	 * <br>
	 * <b><u>Example of use</u>:</b>
	 * <ul>
	 * <li>Delete item from inventory, transfer Item from inventory to warehouse</li>
	 * <li>Crystallize item</li>
	 * <li>Remove NPC/PC/Pet from the world</li>
	 * </ul>
	 * @param object the object to remove
	 */
	public void removeObject(WorldObject @object)
	{
		_allObjects.TryRemove(@object.ObjectId, out _);
		if (@object.isPlayer())
		{
			Player player = (Player) @object;
			if (player.isTeleporting()) // TODO: Drop when we stop removing player from the world while teleporting.
			{
				return;
			}
			_allPlayers.TryRemove(@object.ObjectId, out _);

			if (Config.FactionSystem.FACTION_SYSTEM_ENABLED)
			{
				if (player.isGood())
				{
					_allGoodPlayers.TryRemove(player.ObjectId, out _);
				}
				else if (player.isEvil())
				{
					_allEvilPlayers.TryRemove(player.ObjectId, out _);
				}
			}
		}
	}

	/**
	 * <b><u>Example of use</u>:</b>
	 * <ul>
	 * <li>Client packets : Action, AttackRequest, RequestJoinParty, RequestJoinPledge...</li>
	 * </ul>
	 * @param objectId Identifier of the WorldObject
	 * @return the WorldObject object that belongs to an ID or null if no object found.
	 */
	public WorldObject? findObject(int objectId)
	{
		_allObjects.TryGetValue(objectId, out WorldObject? obj);
		return obj;
	}

	public ICollection<WorldObject> getVisibleObjects()
	{
		return _allObjects.Values;
	}

	/**
	 * Get the count of all visible objects in world.
	 * @return count off all World objects
	 */
	public int getVisibleObjectsCount()
	{
		return _allObjects.Count;
	}

	public ICollection<Player> getPlayers()
	{
		return _allPlayers.Values;
	}

	public ICollection<Player> getAllGoodPlayers()
	{
		return _allGoodPlayers.Values;
	}

	public ICollection<Player> getAllEvilPlayers()
	{
		return _allEvilPlayers.Values;
	}

	/**
	 * <b>If you have access to player objectId use {@link #getPlayer(int playerObjId)}</b>
	 * @param name Name of the player to get Instance
	 * @return the player instance corresponding to the given name.
	 */
	public Player? getPlayer(string name)
	{
		return getPlayer(CharInfoTable.getInstance().getIdByName(name));
	}

	/**
	 * @param objectId of the player to get Instance
	 * @return the player instance corresponding to the given object ID.
	 */
	public Player? getPlayer(int objectId)
	{
		_allPlayers.TryGetValue(objectId, out Player? obj);
		return obj;
	}

	/**
	 * @param ownerId ID of the owner
	 * @return the pet instance from the given ownerId.
	 */
	public Pet? getPet(int ownerId)
	{
		_petsInstance.TryGetValue(ownerId, out Pet? obj);
		return obj;
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public ICollection<Player> getSellingOrBuyingPlayers()
	{
		lock (_allStoreModeBuySellPlayers)
		{
			DateTime currentTime = DateTime.UtcNow;
			if (currentTime > _nextPrivateStoreUpdate)
			{
				_nextPrivateStoreUpdate = currentTime + TimeSpan.FromMilliseconds(Config.General.STORE_REVIEW_CACHE_TIME);
				_allStoreModeBuySellPlayers.Clear();
				foreach (Player player in _allPlayers.Values)
				{
					if (player.isInStoreSellOrBuyMode())
					{
						_allStoreModeBuySellPlayers[player.ObjectId] = player;
					}
				}
			}
		}

		return _allStoreModeBuySellPlayers.Values;
	}

	/**
	 * Add the given pet instance from the given ownerId.
	 * @param ownerId ID of the owner
	 * @param pet Pet of the pet
	 * @return
	 */
	public Pet addPet(int ownerId, Pet pet)
	{
		return _petsInstance[ownerId] = pet;
	}

	/**
	 * Remove the given pet instance.
	 * @param ownerId ID of the owner
	 */
	public void removePet(int ownerId)
	{
		_petsInstance.TryRemove(ownerId, out _);
	}

	/**
	 * Add a WorldObject in the world. <b><u>Concept</u>:</b> WorldObject (including Player) are identified in <b>_visibleObjects</b> of his current WorldRegion and in <b>_knownObjects</b> of other surrounding Creatures<br>
	 * Player are identified in <b>_allPlayers</b> of World, in <b>_allPlayers</b> of his current WorldRegion and in <b>_knownPlayer</b> of other surrounding Creatures <b><u> Actions</u>:</b>
	 * <li>Add the WorldObject object in _allPlayers* of World</li>
	 * <li>Add the WorldObject object in _gmList** of GmListTable</li>
	 * <li>Add object in _knownObjects and _knownPlayer* of all surrounding WorldRegion Creatures</li>
	 * <li>If object is a Creature, add all surrounding WorldObject in its _knownObjects and all surrounding Player in its _knownPlayer</li><br>
	 * <i>* only if object is a Player</i><br>
	 * <i>** only if object is a GM Player</i> <font color=#FF0000><b><u>Caution</u>: This method DOESN'T ADD the object in _visibleObjects and _allPlayers* of WorldRegion (need synchronisation)</b></font><br>
	 * <font color=#FF0000><b><u>Caution</u>: This method DOESN'T ADD the object to _allObjects and _allPlayers* of World (need synchronisation)</b></font> <b><u> Example of use</u>:</b>
	 * <li>Drop an Item</li>
	 * <li>Spawn a Creature</li>
	 * <li>Apply Death Penalty of a Player</li><br>
	 * @param object L2object to add in the world
	 * @param newRegion WorldRegion in wich the object will be add (not used)
	 */
	public void addVisibleObject(WorldObject @object, WorldRegion newRegion)
	{
		if (!newRegion.Active)
		{
			return;
		}

		forEachVisibleObject<WorldObject>(@object, wo =>
		{
			if (@object.isPlayer() && wo.isVisibleFor((Player) @object))
			{
				wo.sendInfo((Player) @object);
				if (wo.isCreature())
				{
					CreatureAI ai = ((Creature) wo).getAI();
					if (ai != null)
					{
						ai.describeStateToPlayer((Player) @object);
						if (wo.isMonster() && ai.getIntention() == CtrlIntention.AI_INTENTION_IDLE)
						{
							ai.setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
						}
					}
				}
			}

			if (wo.isPlayer() && @object.isVisibleFor((Player) wo))
			{
				@object.sendInfo((Player) wo);
				if (@object.isCreature())
				{
					CreatureAI ai = ((Creature) @object).getAI();
					if (ai != null)
					{
						ai.describeStateToPlayer((Player) wo);
						if (@object.isMonster() && ai.getIntention() == CtrlIntention.AI_INTENTION_IDLE)
						{
							ai.setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
						}
					}
				}
			}
		});
	}

	public static void addFactionPlayerToWorld(Player player)
	{
		if (player.isGood())
		{
			_allGoodPlayers.put(player.ObjectId, player);
		}
		else if (player.isEvil())
		{
			_allEvilPlayers.put(player.ObjectId, player);
		}
	}

	/**
	 * Remove a WorldObject from the world. <b><u>Concept</u>:</b> WorldObject (including Player) are identified in <b>_visibleObjects</b> of his current WorldRegion and in <b>_knownObjects</b> of other surrounding Creatures<br>
	 * Player are identified in <b>_allPlayers</b> of World, in <b>_allPlayers</b> of his current WorldRegion and in <b>_knownPlayer</b> of other surrounding Creatures <b><u> Actions</u>:</b>
	 * <li>Remove the WorldObject object from _allPlayers* of World</li>
	 * <li>Remove the WorldObject object from _visibleObjects and _allPlayers* of WorldRegion</li>
	 * <li>Remove the WorldObject object from _gmList** of GmListTable</li>
	 * <li>Remove object from _knownObjects and _knownPlayer* of all surrounding WorldRegion Creatures</li>
	 * <li>If object is a Creature, remove all WorldObject from its _knownObjects and all Player from its _knownPlayer</li> <font color=#FF0000><b><u>Caution</u>: This method DOESN'T REMOVE the object from _allObjects of World</b></font> <i>* only if object is a Player</i><br>
	 * <i>** only if object is a GM Player</i> <b><u> Example of use</u>:</b>
	 * <li>Pickup an Item</li>
	 * <li>Decay a Creature</li><br>
	 * @param object L2object to remove from the world
	 * @param oldRegion WorldRegion in which the object was before removing
	 */
	public void removeVisibleObject(WorldObject obj, WorldRegion oldRegion)
	{
		if (obj == null || oldRegion == null)
		{
			return;
		}

		oldRegion.RemoveVisibleObject(obj);

		// Go through all surrounding WorldRegion Creatures
		ImmutableArray<WorldRegion> surroundingRegions = oldRegion.SurroundingRegions;
		for (int i = 0; i < surroundingRegions.Length; i++)
		{
            IReadOnlyCollection<WorldObject> visibleObjects = surroundingRegions[i].VisibleObjects;
			if (visibleObjects.Count == 0)
			{
				continue;
			}

			foreach (WorldObject wo in visibleObjects)
			{
				if (wo == obj)
				{
					continue;
				}

				if (obj.isCreature())
				{
					Creature objectCreature = (Creature) obj;
					CreatureAI ai = objectCreature.getAI();
					if (ai != null)
					{
						ai.notifyEvent(CtrlEvent.EVT_FORGET_OBJECT, wo);
					}

					if (objectCreature.getTarget() == wo)
					{
						objectCreature.setTarget(null);
					}

					if (obj.isPlayer())
					{
						obj.sendPacket(new DeleteObjectPacket(wo.ObjectId));
					}
				}

				if (wo.isCreature())
				{
					Creature woCreature = (Creature) wo;
					CreatureAI ai = woCreature.getAI();
					if (ai != null)
					{
						ai.notifyEvent(CtrlEvent.EVT_FORGET_OBJECT, obj);
					}

					if (woCreature.getTarget() == obj)
					{
						woCreature.setTarget(null);
					}

					if (wo.isPlayer())
					{
						wo.sendPacket(new DeleteObjectPacket(obj.ObjectId));
					}
				}
			}
		}
	}

	public void switchRegion(WorldObject obj, WorldRegion newRegion)
	{
		WorldRegion oldRegion = obj.getWorldRegion();
		if (oldRegion == null || oldRegion == newRegion)
		{
			return;
		}

		ImmutableArray<WorldRegion> oldSurroundingRegions = oldRegion.SurroundingRegions;
		for (int i = 0; i < oldSurroundingRegions.Length; i++)
		{
			WorldRegion worldRegion = oldSurroundingRegions[i];
			if (newRegion.IsSurroundingRegion(worldRegion))
			{
				continue;
			}

            IReadOnlyCollection<WorldObject> visibleObjects = worldRegion.VisibleObjects;
			if (visibleObjects.Count == 0)
			{
				continue;
			}

			foreach (WorldObject wo in visibleObjects)
			{
				if (wo == obj)
				{
					continue;
				}

				if (obj.isCreature())
				{
					Creature objectCreature = (Creature) obj;
					CreatureAI ai = objectCreature.getAI();
					if (ai != null)
					{
						ai.notifyEvent(CtrlEvent.EVT_FORGET_OBJECT, wo);
					}

					if (objectCreature.getTarget() == wo)
					{
						objectCreature.setTarget(null);
					}

					if (obj.isPlayer())
					{
						obj.sendPacket(new DeleteObjectPacket(wo.ObjectId));
					}
				}

				if (wo.isCreature())
				{
					Creature woCreature = (Creature) wo;
					CreatureAI ai = woCreature.getAI();
					if (ai != null)
					{
						ai.notifyEvent(CtrlEvent.EVT_FORGET_OBJECT, obj);
					}

					if (woCreature.getTarget() == obj)
					{
						woCreature.setTarget(null);
					}

					if (wo.isPlayer())
					{
						wo.sendPacket(new DeleteObjectPacket(obj.ObjectId));
					}
				}
			}
		}

		ImmutableArray<WorldRegion> newSurroundingRegions = newRegion.SurroundingRegions;
		for (int i = 0; i < newSurroundingRegions.Length; i++)
		{
			WorldRegion worldRegion = newSurroundingRegions[i];
			if (oldRegion.IsSurroundingRegion(worldRegion))
			{
				continue;
			}

            IReadOnlyCollection<WorldObject> visibleObjects = worldRegion.VisibleObjects;
			if (visibleObjects.Count == 0)
			{
				continue;
			}

			foreach (WorldObject wo in visibleObjects)
			{
				if (wo == obj || wo.getInstanceWorld() != obj.getInstanceWorld())
				{
					continue;
				}

				if (obj.isPlayer() && wo.isVisibleFor((Player) obj))
				{
					wo.sendInfo((Player) obj);
					if (wo.isCreature())
					{
						CreatureAI ai = ((Creature) wo).getAI();
						if (ai != null)
						{
							ai.describeStateToPlayer((Player) obj);
							if (wo.isMonster() && ai.getIntention() == CtrlIntention.AI_INTENTION_IDLE)
							{
								ai.setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
							}
						}
					}
				}

				if (wo.isPlayer() && obj.isVisibleFor((Player) wo))
				{
					obj.sendInfo((Player) wo);
					if (obj.isCreature())
					{
						CreatureAI ai = ((Creature) obj).getAI();
						if (ai != null)
						{
							ai.describeStateToPlayer((Player) wo);
							if (obj.isMonster() && ai.getIntention() == CtrlIntention.AI_INTENTION_IDLE)
							{
								ai.setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
							}
						}
					}
				}
			}
		}
	}

	public List<T> getVisibleObjects<T>(WorldObject @object)
		where T: WorldObject
	{
		List<T> result = new List<T>();
		forEachVisibleObject<T>(@object, result.Add);
		return result;
	}

	public List<T> getVisibleObjects<T>(WorldObject @object, Predicate<T> predicate)
		where T: WorldObject
	{
		List<T> result = new List<T>();
		forEachVisibleObject<T>(@object, o =>
		{
			if (predicate(o))
			{
				result.Add(o);
			}
		});

		return result;
	}

	public void forEachVisibleObject<T>(WorldObject? @object, Action<T> c)
		where T:WorldObject
	{
		if (@object == null)
		{
			return;
		}

		WorldRegion? worldRegion = getRegion(@object);
		if (worldRegion == null)
		{
			return;
		}

		ImmutableArray<WorldRegion> surroundingRegions = worldRegion.SurroundingRegions;
		for (int i = 0; i < surroundingRegions.Length; i++)
		{
            IReadOnlyCollection<WorldObject> visibleObjects = surroundingRegions[i].VisibleObjects;
			if (visibleObjects.Count == 0)
			{
				continue;
			}

			foreach (WorldObject wo in visibleObjects)
			{
				if (wo == @object || !(wo is T))
				{
					continue;
				}

				if (wo.getInstanceWorld() != @object.getInstanceWorld())
				{
					continue;
				}

				c((T)wo);
			}
		}
	}

	public List<T> getVisibleObjectsInRange<T>(WorldObject @object, int range)
		where T:WorldObject
	{
		List<T> result = new List<T>();
		forEachVisibleObjectInRange<T>(@object, range, result.Add);
		return result;
	}

	public List<T> getVisibleObjectsInRange<T>(WorldObject @object, int range, Predicate<T> predicate)
		where T: WorldObject
	{
		List<T> result = new List<T>();
		forEachVisibleObjectInRange<T>(@object, range, o =>
		{
			if (predicate(o))
			{
				result.Add(o);
			}
		});
		return result;
	}

	public void forEachVisibleObjectInRange<T>(WorldObject obj, int range, Action<T> c)
		where T: WorldObject
	{
		if (obj == null)
		{
			return;
		}

		WorldRegion? worldRegion = getRegion(obj);
		if (worldRegion == null)
		{
			return;
		}

		ImmutableArray<WorldRegion> surroundingRegions = worldRegion.SurroundingRegions;
		for (int i = 0; i < surroundingRegions.Length; i++)
		{
            IReadOnlyCollection<WorldObject> visibleObjects = surroundingRegions[i].VisibleObjects;
			if (visibleObjects.Count == 0)
			{
				continue;
			}

			foreach (WorldObject wo in visibleObjects)
			{
				if (wo == obj || !(wo is T))
				{
					continue;
				}

				if (wo.getInstanceWorld() != obj.getInstanceWorld())
				{
					continue;
				}

				if (wo.Distance3D(obj) <= range)
				{
					c((T)wo);
				}
			}
		}
	}

	/**
	 * Calculate the current WorldRegions of the object according to its position (x,y). <b><u>Example of use</u>:</b>
	 * <li>Set position of a new WorldObject (drop, spawn...)</li>
	 * <li>Update position of a WorldObject after a movement</li><br>
	 * @param object the object
	 * @return
	 */
	public WorldRegion getRegion(WorldObject @object)
    {
        // TODO: coordinates must be validated during movement or changing location
        return _worldRegions.GetRegion(new Location2D(@object.getX(), @object.getY()));
	}

    public WorldRegion getRegion(int x, int y) => _worldRegions.GetRegion(new Location2D(x, y));

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void disposeOutOfBoundsObject(WorldObject @object)
	{
		if (@object.isPlayer())
		{
			Player player = (Player)@object;
			player.stopMove(new Location(player.getLastServerPosition(), 0));
		}
		else if (@object.isSummon())
		{
			Summon summon = (Summon) @object;
			summon.unSummon(summon.getOwner());
		}
		else if (_allObjects.remove(@object.ObjectId) != null)
		{
			if (@object.isNpc())
			{
				Npc npc = (Npc) @object;
				LOGGER.Warn("Deleting npc " + @object.getName() + " NPCID[" + npc.getId() + "] from invalid location X:" + @object.getX() + " Y:" + @object.getY() + " Z:" + @object.getZ());
				npc.deleteMe();

				Spawn? spawn = npc.getSpawn();
				if (spawn != null)
				{
					LOGGER.Warn("Spawn location: " + spawn.Location);
				}
			}
			else if (@object.isCreature())
			{
				LOGGER.Warn("Deleting object " + @object.getName() + " OID[" + @object.ObjectId + "] from invalid location X:" + @object.getX() + " Y:" + @object.getY() + " Z:" + @object.getZ());
				((Creature) @object).deleteMe();
			}

			@object.getWorldRegion().RemoveVisibleObject(@object);
		}
	}

	public void incrementParty()
	{
		Interlocked.Increment(ref _partyNumber);
	}

	public void decrementParty()
	{
		Interlocked.Decrement(ref _partyNumber);
	}

	public void incrementPartyMember()
	{
		Interlocked.Increment(ref _memberInPartyNumber);
	}

	public void decrementPartyMember()
	{
		Interlocked.Decrement(ref _memberInPartyNumber);
	}

	public int getPartyCount()
	{
		return _partyNumber;
	}

	public int getPartyMemberCount()
	{
		return _memberInPartyNumber;
	}

	public static World getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly World INSTANCE = new();
	}
}