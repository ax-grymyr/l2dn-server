using System.Collections.Immutable;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model;

public sealed class WorldRegion
{
    private readonly WorldRegionCollection _collection;
    private readonly int _regionX;
    private readonly int _regionY;

    // Array containing nearby regions forming this world region's effective area.
    private ImmutableArray<WorldRegion> _surroundingRegions;

    // Set containing visible objects in this world region.
    private readonly Set<WorldObject> _visibleObjects = [];

    // Set containing doors in this world region.
    private readonly Set<Door> _doors = [];

    // Set containing fences in this world region.
    private readonly Set<Fence> _fences = [];

    private bool _active = Config.GRIDS_ALWAYS_ON;
    private ScheduledFuture? _neighborsTask;
    private int _activeNeighbors;

    public WorldRegion(WorldRegionCollection collection, int regionX, int regionY)
    {
        _collection = collection;
        _regionX = regionX;
        _regionY = regionY;
    }

    public int RegionX => _regionX;
    public int RegionY => _regionY;
    public ImmutableArray<WorldRegion> SurroundingRegions => _surroundingRegions;
    public bool Active => _active;
    public bool AreNeighborsActive => Config.GRIDS_ALWAYS_ON || _activeNeighbors > 0;
    public IReadOnlyCollection<Door> Doors => _doors;
    public IReadOnlyCollection<Fence> Fences => _fences;
    public IReadOnlyCollection<WorldObject> VisibleObjects => _visibleObjects;

    /// <summary>
    /// Adds the WorldObject to the visible objects.
    /// </summary>
    public void AddVisibleObject(WorldObject worldObject)
    {
        _visibleObjects.Add(worldObject);

        if (worldObject is Door door)
        {
            foreach (WorldRegion region in _surroundingRegions)
                region.AddDoor(door);
        }
        else if (worldObject is Fence fence)
        {
            foreach (WorldRegion region in _surroundingRegions)
                region.AddFence(fence);
        }

        // If this is the first player to enter the region, activate self and neighbors.
        if (worldObject.isPlayable() && !_active && !Config.GRIDS_ALWAYS_ON)
            StartActivation();
    }


    /// <summary>
    /// Removes the WorldObject from the visible objects.
    /// </summary>
    public void RemoveVisibleObject(WorldObject worldObject)
    {
        if (_visibleObjects.Count == 0)
            return;

        _visibleObjects.Remove(worldObject);

        if (worldObject is Door door)
        {
            foreach (WorldRegion region in _surroundingRegions)
                region.RemoveDoor(door);
        }
        else if (worldObject is Fence fence)
        {
            foreach (WorldRegion region in _surroundingRegions)
                region.RemoveFence(fence);
        }

        if (worldObject.isPlayable() && AreNeighborsEmpty() && !Config.GRIDS_ALWAYS_ON)
            StartDeactivation();
    }

    public bool IsSurroundingRegion(WorldRegion region) =>
        _regionX >= region.RegionX - 1 && _regionX <= region.RegionX + 1 &&
        _regionY >= region.RegionY - 1 && _regionY <= region.RegionY + 1;

    public override string ToString() => $"({_regionX}, {_regionY})";

    internal void CalculateSurroundingRegions()
    {
        ImmutableArray<WorldRegion>.Builder arrayBuilder = ImmutableArray.CreateBuilder<WorldRegion>();
        arrayBuilder.Add(this);

        int xMin = Math.Max(0, _regionX - 1);
        int xMax = Math.Min(WorldMap.RegionCountX - 1, _regionX + 1);
        int yMin = Math.Max(0, _regionY - 1);
        int yMax = Math.Min(WorldMap.RegionCountY - 1, _regionY + 1);

        for (int rx = xMin; rx <= xMax; rx++)
        for (int ry = yMin; ry <= yMax; ry++)
            if (rx != _regionX || ry != _regionY)
                arrayBuilder.Add(_collection[rx, ry]);

        _surroundingRegions = arrayBuilder.ToImmutable();
    }

    private bool AreNeighborsEmpty()
    {
        foreach (WorldRegion worldRegion in _surroundingRegions)
        {
            if (!worldRegion.Active)
                continue;

            IReadOnlyCollection<WorldObject> regionObjects = worldRegion.VisibleObjects;
            if (regionObjects.Count == 0)
                continue;

            if (regionObjects.Any(static worldObject => worldObject.isPlayable()))
                return false;
        }

        return true;
    }

    private void AddDoor(Door door)
    {
        _doors.Add(door);
    }

    private void RemoveDoor(Door door)
    {
        _doors.Remove(door);
    }

    private void AddFence(Fence fence)
    {
        _fences.Add(fence);
    }

    private void RemoveFence(Fence fence)
    {
        _fences.Remove(fence);
    }

    /// <summary>
    /// This function turns this region's AI and geodata on or off.
    /// </summary>
    /// <param name="value"></param>
    private void SetActive(bool value)
    {
        if (_active == value)
            return;

        _active = value;

        if (value)
        {
            foreach (WorldRegion region in _surroundingRegions)
                Interlocked.Increment(ref region._activeNeighbors);
        }
        else
        {
            foreach (WorldRegion region in _surroundingRegions)
                Interlocked.Decrement(ref region._activeNeighbors);
        }

        // Turn the AI on or off to match the region's activation.
        SwitchAi(value);
    }

    /// <summary>
    /// Immediately sets self as active and starts a timer to set neighbors as active this timer is to avoid turning
    /// on neighbors in the case when a person just teleported into a region and then teleported out immediately.
    /// There is no reason to activate all the neighbors in that case.
    /// </summary>
    private void StartActivation()
    {
        // First set self to active and do self-tasks...
        SetActive(true);

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
                foreach (WorldRegion region in _surroundingRegions)
                    region.SetActive(true);
            }, 1000 * Config.GRID_NEIGHBOR_TURNON_TIME);
        }
    }

    /// <summary>
    /// Starts a timer to set neighbors (including self) as inactive.
    /// This timer is to avoid turning off neighbors in the case when a person just moved out of a region
    /// that he may very soon return to. There is no reason to turn self & neighbors off in that case.
    /// </summary>
    private void StartDeactivation()
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
                foreach (WorldRegion worldRegion in _surroundingRegions)
                {
                    if (worldRegion.AreNeighborsEmpty())
                        worldRegion.SetActive(false);
                }
            }, 1000 * Config.GRID_NEIGHBOR_TURNOFF_TIME);
        }
    }

    private void SwitchAi(bool isOn)
    {
        if (_visibleObjects.Count == 0)
            return;

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
                    Spawn? spawn = mob.getSpawn();
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
}