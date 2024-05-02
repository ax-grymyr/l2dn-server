using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.TaskManagers;

/**
 * @author Mobius
 */
public class CreatureFollowTaskManager
{
	protected static readonly Map<Creature, int> NORMAL_FOLLOW_CREATURES = new();
	protected static readonly Map<Creature, int> ATTACK_FOLLOW_CREATURES = new();
	protected static bool _workingNormal = false;
	protected static bool _workingAttack = false;
	
	protected CreatureFollowTaskManager()
	{
		ThreadPool.scheduleAtFixedRate(new CreatureFollowNormalTask(this), 1000, 1000); // TODO: high priority task
		ThreadPool.scheduleAtFixedRate(new CreatureFollowAttackTask(this), 500, 500); // TODO: high priority task
	}
	
	protected class CreatureFollowNormalTask: Runnable
	{
		private readonly CreatureFollowTaskManager _manager;

		public CreatureFollowNormalTask(CreatureFollowTaskManager manager)
		{
			_manager = manager;
		}
		
		public void run()
		{
			if (_workingNormal)
			{
				return;
			}
			_workingNormal = true;
			
			if (!NORMAL_FOLLOW_CREATURES.isEmpty())
			{
				foreach (var entry in NORMAL_FOLLOW_CREATURES)
				{
					_manager.follow(entry.Key, entry.Value);
				}
			}
			
			_workingNormal = false;
		}
	}
	
	protected class CreatureFollowAttackTask: Runnable
	{
		private readonly CreatureFollowTaskManager _manager;

		public CreatureFollowAttackTask(CreatureFollowTaskManager manager)
		{
			_manager = manager;
		}
		
		public void run()
		{
			if (_workingAttack)
			{
				return;
			}
			_workingAttack = true;
			
			if (!ATTACK_FOLLOW_CREATURES.isEmpty())
			{
				foreach (var entry in ATTACK_FOLLOW_CREATURES)
				{
					_manager.follow(entry.Key, entry.Value);
				}
			}
			
			_workingAttack = false;
		}
	}
	
	protected void follow(Creature creature, int range)
	{
		try
		{
			if (creature.hasAI())
			{
				CreatureAI ai = creature.getAI();
				if (ai != null)
				{
					WorldObject followTarget = ai.getTarget();
					if (followTarget == null)
					{
						if (creature.isSummon())
						{
							((Summon) creature).setFollowStatus(false);
						}
						ai.setIntention(CtrlIntention.AI_INTENTION_IDLE);
						return;
					}
					
					int followRange = range == -1 ? Rnd.get(50, 100) : range;
					if (!creature.IsInsideRadius3D(followTarget, followRange))
					{
						if (!creature.IsInsideRadius3D(followTarget, 3000))
						{
							// If the target is too far (maybe also teleported).
							if (creature.isSummon())
							{
								((Summon) creature).setFollowStatus(false);
							}
							ai.setIntention(CtrlIntention.AI_INTENTION_IDLE);
							return;
						}
						ai.moveToPawn(followTarget, followRange);
					}
				}
				else
				{
					remove(creature);
				}
			}
			else
			{
				remove(creature);
			}
		}
		catch (Exception e)
		{
			// Ignore.
		}
	}
	
	public bool isFollowing(Creature creature)
	{
		return NORMAL_FOLLOW_CREATURES.containsKey(creature) || ATTACK_FOLLOW_CREATURES.containsKey(creature);
	}
	
	public void addNormalFollow(Creature creature, int range)
	{
		NORMAL_FOLLOW_CREATURES.TryAdd(creature, range);
		follow(creature, range);
	}
	
	public void addAttackFollow(Creature creature, int range)
	{
		ATTACK_FOLLOW_CREATURES.TryAdd(creature, range);
		follow(creature, range);
	}
	
	public void remove(Creature creature)
	{
		NORMAL_FOLLOW_CREATURES.remove(creature);
		ATTACK_FOLLOW_CREATURES.remove(creature);
	}
	
	public static CreatureFollowTaskManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static CreatureFollowTaskManager INSTANCE = new CreatureFollowTaskManager();
	}
}