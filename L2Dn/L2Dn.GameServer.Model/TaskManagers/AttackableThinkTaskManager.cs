using System.Runtime.CompilerServices;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.TaskManagers;

/**
 * @author Mobius
 */
public class AttackableThinkTaskManager
{
	private static readonly Set<Set<Attackable>> POOLS = new();
	private const int POOL_SIZE = 1000;
	private const int TASK_DELAY = 1000;
	
	protected AttackableThinkTaskManager()
	{
	}
	
	private class AttackableThink: Runnable
	{
		private readonly Set<Attackable> _attackables;
		
		public AttackableThink(Set<Attackable> attackables)
		{
			_attackables = attackables;
		}
		
		public void run()
		{
			if (_attackables.isEmpty())
			{
				return;
			}
			
			CreatureAI ai;
			foreach (Attackable attackable in _attackables)
			{
				if (attackable.hasAI())
				{
					ai = attackable.getAI();
					if (ai != null)
					{
						ai.onEvtThink();
					}
					else
					{
						_attackables.remove(attackable);
					}
				}
				else
				{
					_attackables.remove(attackable);
				}
			}
		}
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void add(Attackable attackable)
	{
		foreach (Set<Attackable> pool in POOLS)
		{
			if (pool.Contains(attackable))
			{
				return;
			}
		}
		
		foreach (Set<Attackable> pool in POOLS)
		{
			if (pool.Count < POOL_SIZE)
			{
				pool.add(attackable);
				return;
			}
		}
		
		Set<Attackable> pool1 = new();
		pool1.add(attackable);
		ThreadPool.scheduleAtFixedRate(new AttackableThink(pool1), TASK_DELAY, TASK_DELAY);
		POOLS.add(pool1);
	}
	
	public void remove(Attackable attackable)
	{
		foreach (Set<Attackable> pool in POOLS)
		{
			if (pool.remove(attackable))
			{
				return;
			}
		}
	}
	
	public static AttackableThinkTaskManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly AttackableThinkTaskManager INSTANCE = new AttackableThinkTaskManager();
	}
}