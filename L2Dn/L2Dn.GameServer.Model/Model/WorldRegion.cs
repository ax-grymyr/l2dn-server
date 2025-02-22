using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model;

public class WorldRegion
{
	/** Set containing visible objects in this world region. */
	private readonly Set<WorldObject> _visibleObjects = [];

	/** List containing doors in this world region. */
	private readonly List<Door> _doors = [];

	/** List containing fences in this world region. */
	private readonly List<Fence> _fences = [];

	/** Array containing nearby regions forming this world region's effective area. */
	private WorldRegion[] _surroundingRegions = [];

	private readonly int _regionX;
	private readonly int _regionY;
	private bool _active = Config.GRIDS_ALWAYS_ON;
	private ScheduledFuture? _neighborsTask;
	private readonly AtomicInteger _activeNeighbors = new();

	public WorldRegion(int regionX, int regionY)
	{
		_regionX = regionX;
		_regionY = regionY;
	}

	private void switchAI(bool isOn)
	{
		if (_visibleObjects.Count == 0)
		{
			return;
		}

		if (!isOn)
		{
			foreach (WorldObject wo in _visibleObjects)
			{
				if (wo.isAttackable())
				{
					Attackable mob = (Attackable)wo;

					// Set target to null and cancel attack or cast.
					mob.setTarget(null);

					// Stop movement.
					mob.stopMove(null);

					// Stop all active skills effects in progress on the Creature.
					mob.stopAllEffects();

					mob.clearAggroList();
					mob.getAttackByList().clear();

					// Teleport to spawn when too far away.
					Spawn spawn = mob.getSpawn();
					if (spawn != null && mob.Distance2D(spawn.Location.Location2D) > Config.MAX_DRIFT_RANGE)
					{
						mob.teleToLocation(spawn.Location);
					}

					// Stop the AI tasks.
					if (mob.hasAI())
					{
						mob.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
						mob.getAI().stopAITask();
					}

					// Stop attack task.
					mob.abortAttack();

					RandomAnimationTaskManager.getInstance().remove(mob);
				}
				else if (wo is Npc)
				{
					RandomAnimationTaskManager.getInstance().remove((Npc)wo);
				}
			}
		}
		else
		{
			foreach (WorldObject wo in _visibleObjects)
			{
				if (wo.isAttackable())
				{
					// Start HP/MP/CP regeneration task.
					((Attackable)wo).getStatus().startHpMpRegeneration();
					RandomAnimationTaskManager.getInstance().add((Npc)wo);
				}
				else if (wo.isNpc())
				{
					RandomAnimationTaskManager.getInstance().add((Npc)wo);
				}
			}
		}
	}

	public bool isActive()
	{
		return _active;
	}

	public void incrementActiveNeighbors()
	{
		_activeNeighbors.incrementAndGet();
	}

	public void decrementActiveNeighbors()
	{
		_activeNeighbors.decrementAndGet();
	}

	public bool areNeighborsActive()
	{
		return Config.GRIDS_ALWAYS_ON || _activeNeighbors.get() > 0;
	}

	public bool areNeighborsEmpty()
	{
		for (int i = 0; i < _surroundingRegions.Length; i++)
		{
			WorldRegion worldRegion = _surroundingRegions[i];
			if (worldRegion.isActive())
			{
				ICollection<WorldObject> regionObjects = worldRegion.getVisibleObjects();
				if (regionObjects.Count == 0)
				{
					continue;
				}

				foreach (WorldObject wo in regionObjects)
				{
					if (wo != null && wo.isPlayable())
					{
						return false;
					}
				}
			}
		}

		return true;
	}

	/**
	 * this function turns this region's AI and geodata on or off
	 * @param value
	 */
	public void setActive(bool value)
	{
		if (_active == value)
		{
			return;
		}

		_active = value;

		if (value)
		{
			for (int i = 0; i < _surroundingRegions.Length; i++)
			{
				_surroundingRegions[i].incrementActiveNeighbors();
			}
		}
		else
		{
			for (int i = 0; i < _surroundingRegions.Length; i++)
			{
				_surroundingRegions[i].decrementActiveNeighbors();
			}
		}

		// Turn the AI on or off to match the region's activation.
		switchAI(value);
	}

	/**
	 * Immediately sets self as active and starts a timer to set neighbors as active this timer is to avoid turning on neighbors in the case when a person just teleported into a region and then teleported out immediately...there is no reason to activate all the neighbors in that case.
	 */
	private void startActivation()
	{
		// First set self to active and do self-tasks...
		setActive(true);

		// If the timer to deactivate neighbors is running, cancel it.
		lock (this)
		{
			if (_neighborsTask != null)
			{
				_neighborsTask.cancel(true);
				_neighborsTask = null;
			}

			// Then, set a timer to activate the neighbors.
			_neighborsTask = ThreadPool.schedule(() =>
			{
				for (int i = 0; i < _surroundingRegions.Length; i++)
				{
					_surroundingRegions[i].setActive(true);
				}
			}, 1000 * Config.GRID_NEIGHBOR_TURNON_TIME);
		}
	}

	/**
	 * starts a timer to set neighbors (including self) as inactive this timer is to avoid turning off neighbors in the case when a person just moved out of a region that he may very soon return to. There is no reason to turn self & neighbors off in that case.
	 */
	private void startDeactivation()
	{
		// If the timer to activate neighbors is running, cancel it.
		lock (this)
		{
			if (_neighborsTask != null)
			{
				_neighborsTask.cancel(true);
				_neighborsTask = null;
			}

			// Start a timer to "suggest" a deactivate to self and neighbors.
			// Suggest means: first check if a neighbor has Players in it. If not, deactivate.
			_neighborsTask = ThreadPool.schedule(() =>
			{
				for (int i = 0; i < _surroundingRegions.Length; i++)
				{
					WorldRegion worldRegion = _surroundingRegions[i];
					if (worldRegion.areNeighborsEmpty())
					{
						worldRegion.setActive(false);
					}
				}
			}, 1000 * Config.GRID_NEIGHBOR_TURNOFF_TIME);
		}
	}

	/**
	 * Add the WorldObject in the WorldObjectHashSet(WorldObject) _visibleObjects containing WorldObject visible in this WorldRegion<br>
	 * If WorldObject is a Player, Add the Player in the WorldObjectHashSet(Player) _allPlayable containing Player of all player in game in this WorldRegion
	 * @param object
	 */
	public void addVisibleObject(WorldObject @object)
	{
		if (@object == null)
		{
			return;
		}

		_visibleObjects.Add(@object);

		if (@object.isDoor())
		{
			for (int i = 0; i < _surroundingRegions.Length; i++)
			{
				_surroundingRegions[i].addDoor((Door)@object);
			}
		}
		else if (@object.isFence())
		{
			for (int i = 0; i < _surroundingRegions.Length; i++)
			{
				_surroundingRegions[i].addFence((Fence)@object);
			}
		}

		// If this is the first player to enter the region, activate self and neighbors.
		if (@object.isPlayable() && !_active && !Config.GRIDS_ALWAYS_ON)
		{
			startActivation();
		}
	}

	/**
	 * Remove the WorldObject from the WorldObjectHashSet(WorldObject) _visibleObjects in this WorldRegion. If WorldObject is a Player, remove it from the WorldObjectHashSet(Player) _allPlayable of this WorldRegion
	 * @param object
	 */
	public void removeVisibleObject(WorldObject @object)
	{
		if (@object == null)
		{
			return;
		}

		if (_visibleObjects.Count == 0)
		{
			return;
		}

		_visibleObjects.Remove(@object);

		if (@object.isDoor())
		{
			for (int i = 0; i < _surroundingRegions.Length; i++)
			{
				_surroundingRegions[i].removeDoor((Door)@object);
			}
		}
		else if (@object.isFence())
		{
			for (int i = 0; i < _surroundingRegions.Length; i++)
			{
				_surroundingRegions[i].removeFence((Fence)@object);
			}
		}

		if (@object.isPlayable() && areNeighborsEmpty() && !Config.GRIDS_ALWAYS_ON)
		{
			startDeactivation();
		}
	}

	public ICollection<WorldObject> getVisibleObjects()
	{
		return _visibleObjects;
	}

	public void addDoor(Door door)
	{
		lock (_doors)
		{
			if (!_doors.Contains(door))
			{
				_doors.Add(door);
			}
		}
	}

	private void removeDoor(Door door)
	{
		lock (_doors)
		{
			_doors.Remove(door);
		}
	}

	public List<Door> getDoors()
	{
		return _doors;
	}

	public void addFence(Fence fence)
	{
		lock (_fences)
		{
			if (!_fences.Contains(fence))
			{
				_fences.Add(fence);
			}
		}
	}

	private void removeFence(Fence fence)
	{
		lock (_fences)
		{
			_fences.Remove(fence);
		}
	}

	public List<Fence> getFences()
	{
		return _fences;
	}

	public void setSurroundingRegions(WorldRegion[] regions)
	{
		_surroundingRegions = regions;

		// Make sure that this region is always the first region to improve bulk operations when this region should be updated first.
		for (int i = 0; i < _surroundingRegions.Length; i++)
		{
			if (_surroundingRegions[i] == this)
			{
				WorldRegion first = _surroundingRegions[0];
				_surroundingRegions[0] = this;
				_surroundingRegions[i] = first;
			}
		}
	}

	public WorldRegion[] getSurroundingRegions()
	{
		return _surroundingRegions;
	}

	public bool isSurroundingRegion(WorldRegion region)
	{
		return region != null && _regionX >= region.getRegionX() - 1 && _regionX <= region.getRegionX() + 1 &&
			_regionY >= region.getRegionY() - 1 && _regionY <= region.getRegionY() + 1;
	}

	public int getRegionX()
	{
		return _regionX;
	}

	public int getRegionY()
	{
		return _regionY;
	}

	public override string ToString()
	{
		return "(" + _regionX + ", " + _regionY + ")";
	}
}