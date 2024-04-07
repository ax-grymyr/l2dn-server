using System.Runtime.CompilerServices;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.TaskManagers;

/**
 * Movement task manager class.
 * @author Mobius
 */
public class MovementTaskManager
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(MovementTaskManager));
	
	private static readonly Set<Set<Creature>> POOLS = new();
	private const int POOL_SIZE = 1000;
	private const int TASK_DELAY = 100;
	
	protected MovementTaskManager()
	{
	}
	
	private class Movement: Runnable
	{
		private readonly Set<Creature> _creatures;
		
		public Movement(Set<Creature> creatures)
		{
			_creatures = creatures;
		}
		
		public void run()
		{
			if (_creatures.isEmpty())
			{
				return;
			}

			foreach (Creature creature in _creatures)
			{
				try
				{
					if (creature.updatePosition())
					{
						_creatures.Remove(creature);
						creature.getAI().notifyEvent(CtrlEvent.EVT_ARRIVED);
					}
				}
				catch (Exception e)
				{
					_creatures.Remove(creature);
					LOGGER.Warn("MovementTaskManager: Problem updating position of " + creature + ": " + e);
				}
			}
		}
	}
	
	/**
	 * Add a Creature to moving objects of MovementTaskManager.
	 * @param creature The Creature to add to moving objects of MovementTaskManager.
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void registerMovingObject(Creature creature)
	{
		foreach (Set<Creature> pool in POOLS)
		{
			if (pool.Contains(creature))
			{
				return;
			}
		}
		
		foreach (Set<Creature> pool in POOLS)
		{
			if (pool.Count < POOL_SIZE)
			{
				pool.add(creature);
				return;
			}
		}
		
		Set<Creature> pool1 = new();
		pool1.add(creature);
		ThreadPool.scheduleAtFixedRate(new Movement(pool1), TASK_DELAY, TASK_DELAY); // TODO: high priority task
		POOLS.add(pool1);
	}
	
	public static MovementTaskManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly MovementTaskManager INSTANCE = new MovementTaskManager();
	}
}