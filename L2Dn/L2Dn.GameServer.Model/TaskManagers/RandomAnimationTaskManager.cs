using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.TaskManagers;

/**
 * @author Mobius
 */
public class RandomAnimationTaskManager: Runnable
{
	private static readonly Map<Npc, DateTime> PENDING_ANIMATIONS = new();
	private static bool _working = false;

	protected RandomAnimationTaskManager()
	{
		ThreadPool.scheduleAtFixedRate(this, 0, 1000);
	}

	public void run()
	{
		if (_working)
		{
			return;
		}
		_working = true;

		DateTime currentTime = DateTime.UtcNow;
		List<Npc> npcs = PENDING_ANIMATIONS.Where(x => currentTime > x.Value).Select(x => x.Key).ToList();

		foreach (Npc npc in npcs)
		{
			if (npc.isInActiveRegion() && !npc.isDead() && !npc.isInCombat() && !npc.isMoving() &&
			    !npc.hasBlockActions())
			{
				npc.onRandomAnimation(Rnd.get(2, 3));
			}

			PENDING_ANIMATIONS.put(npc,
				currentTime.AddMilliseconds(Rnd.get(npc.isAttackable() ? Config.MIN_MONSTER_ANIMATION : Config.MIN_NPC_ANIMATION,
					npc.isAttackable() ? Config.MAX_MONSTER_ANIMATION : Config.MAX_NPC_ANIMATION) * 1000));
		}

		_working = false;
	}

	public void add(Npc npc)
	{
		if (npc.hasRandomAnimation())
		{
			PENDING_ANIMATIONS.TryAdd(npc,
				DateTime.UtcNow.AddMilliseconds(
					Rnd.get(npc.isAttackable() ? Config.MIN_MONSTER_ANIMATION : Config.MIN_NPC_ANIMATION,
						npc.isAttackable() ? Config.MAX_MONSTER_ANIMATION : Config.MAX_NPC_ANIMATION) * 1000));
		}
	}

	public void remove(Npc npc)
	{
		PENDING_ANIMATIONS.remove(npc);
	}

	public static RandomAnimationTaskManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly RandomAnimationTaskManager INSTANCE = new RandomAnimationTaskManager();
	}
}