using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Npcs;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model;

/**
 * Holds info about current walk progress.
 * @author GKR, UnAfraid
 */
public class WalkInfo
{
	private readonly WalkRoute _route;
	private readonly string _routeName;
	private ScheduledFuture? _walkCheckTask;
	private bool _blocked;
	private bool _suspended;
	private bool _stoppedByAttack;
	private int _currentNode;
	private bool _forward = true; // Determines first --> last or first <-- last direction
	private long _lastActionTime; // Debug field

	public WalkInfo(WalkRoute route)
	{
		_route = route;
		_routeName = route.getName();
	}

	/**
	 * @return name of route of this WalkInfo.
	 */
	public WalkRoute getRoute()
    {
        return _route;
    }

	/**
	 * @return current node of this WalkInfo.
	 */
	public NpcWalkerNode getCurrentNode()
	{
		return getRoute().getNodeList()[Math.Min(Math.Max(0, _currentNode), getRoute().getNodeList().Count - 1)];
	}

	/**
	 * Calculate next node for this WalkInfo and send debug message from given npc
	 * @param npc NPC to debug message to be sent from
	 */
	public void calculateNextNode(Npc npc)
	{
		lock (this)
		{
			// Check this first, within the bounds of random moving, we have no conception of "first" or "last" node
			if (getRoute().getRepeatType() == WalkingManager.REPEAT_RANDOM)
			{
				int newNode = _currentNode;

				while (newNode == _currentNode)
				{
					newNode = Rnd.get(getRoute().getNodesCount());
				}

				_currentNode = newNode;
			}
			else
			{
				if (_forward)
				{
					_currentNode++;
				}
				else
				{
					_currentNode--;
				}

				if (_currentNode == getRoute().getNodesCount()) // Last node arrived
				{
					// Notify quest
					if (npc.Events.HasSubscribers<OnNpcMoveRouteFinished>())
					{
						npc.Events.NotifyAsync(new OnNpcMoveRouteFinished(npc));
					}

					if (!getRoute().repeatWalk())
					{
						WalkingManager.getInstance().cancelMoving(npc);
						return;
					}

					switch (getRoute().getRepeatType())
					{
						case WalkingManager.REPEAT_GO_BACK:
						{
							_forward = false;
							_currentNode -= 2;
							break;
						}
						case WalkingManager.REPEAT_GO_FIRST:
						{
							_currentNode = 0;
							break;
						}
						case WalkingManager.REPEAT_TELE_FIRST:
						{
                            Spawn? spawn = npc.getSpawn();
                            if (spawn != null)
							    npc.teleToLocation(spawn.Location);

							_currentNode = 0;
							break;
						}
					}
				}
				else if (_currentNode == WalkingManager.NO_REPEAT) // First node arrived, when direction is first <-- last
				{
					_currentNode = 1;
					_forward = true;
				}
			}
		}
	}

	/**
	 * @return {@code true} if walking task is blocked, {@code false} otherwise,
	 */
	public bool isBlocked()
	{
		return _blocked;
	}

	/**
	 * @param value
	 */
	public void setBlocked(bool value)
	{
		_blocked = value;
	}

	/**
	 * @return {@code true} if walking task is suspended, {@code false} otherwise,
	 */
	public bool isSuspended()
	{
		return _suspended;
	}

	/**
	 * @param value
	 */
	public void setSuspended(bool value)
	{
		_suspended = value;
	}

	/**
	 * @return {@code true} if walking task shall be stopped by attack, {@code false} otherwise,
	 */
	public bool isStoppedByAttack()
	{
		return _stoppedByAttack;
	}

	/**
	 * @param value
	 */
	public void setStoppedByAttack(bool value)
	{
		_stoppedByAttack = value;
	}

	/**
	 * @return the id of the current node in this walking task.
	 */
	public int getCurrentNodeId()
	{
		return _currentNode;
	}

	/**
	 * @return {@code long} last action time used only for debugging.
	 */
	public long getLastAction()
	{
		return _lastActionTime;
	}

	/**
	 * @param value
	 */
	public void setLastAction(long value)
	{
		_lastActionTime = value;
	}

	/**
	 * @return walking check task.
	 */
	public ScheduledFuture? getWalkCheckTask()
	{
		return _walkCheckTask;
	}

	/**
	 * @param task walking check task.
	 */
	public void setWalkCheckTask(ScheduledFuture task)
	{
		_walkCheckTask = task;
	}

	public override string ToString() => $"WalkInfo [_routeName={_routeName}, _walkCheckTask={_walkCheckTask}, " +
        $"_blocked={_blocked}, _suspended={_suspended}, _stoppedByAttack={_stoppedByAttack}, " +
        $"_currentNode={_currentNode}, _forward={_forward}, _lastActionTime={_lastActionTime}]";
}