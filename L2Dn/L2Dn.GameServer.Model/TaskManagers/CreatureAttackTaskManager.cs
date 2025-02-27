using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.TaskManagers;

/**
 * Creature Attack task manager class.
 * @author Mobius
 */
public sealed class CreatureAttackTaskManager
{
	private static readonly Set<Map<Creature, ScheduledAttack>> ATTACK_POOLS = new();
	private static readonly Set<Map<Creature, ScheduledFinish>> FINISH_POOLS = new();
	private const int POOL_SIZE = 300;
	private const int TASK_DELAY = 10;

	private CreatureAttackTaskManager()
	{
	}

	private class ScheduleAttackTask(Map<Creature, ScheduledAttack> creatureAttackData): Runnable
	{
		public void run()
		{
			if (creatureAttackData.Count == 0)
				return;

			DateTime currentTime = DateTime.UtcNow;
			foreach (var entry in creatureAttackData)
			{
				ScheduledAttack scheduledAttack = entry.Value;
				if (currentTime >= scheduledAttack.endTime)
				{
					Creature creature = entry.Key;
					creatureAttackData.TryRemove(creature, out _);
					switch (scheduledAttack.type)
					{
						case ScheduledAttackType.NORMAL:
						{
							creature.onHitTimeNotDual(scheduledAttack.weapon, scheduledAttack.attack, scheduledAttack.hitTime, scheduledAttack.attackTime);
							break;
						}
						case ScheduledAttackType.DUAL_FIRST:
						{
							creature.onFirstHitTimeForDual(scheduledAttack.weapon, scheduledAttack.attack, scheduledAttack.hitTime, scheduledAttack.attackTime, scheduledAttack.delayForSecondAttack);
							break;
						}
						case ScheduledAttackType.DUAL_SECOND:
						{
							creature.onSecondHitTimeForDual(scheduledAttack.weapon, scheduledAttack.attack, scheduledAttack.hitTime, scheduledAttack.delayForSecondAttack, scheduledAttack.attackTime);
							break;
						}
					}
				}
			}
		}
	}

	private class ScheduleAbortTask(Map<Creature, ScheduledFinish> creatureFinishData): Runnable
	{
		public void run()
		{
			if (creatureFinishData.Count == 0)
				return;

			DateTime currentTime = DateTime.UtcNow;
			foreach (var entry in creatureFinishData)
			{
				ScheduledFinish scheduledFinish = entry.Value;

				if (currentTime >= scheduledFinish.endTime)
				{
					Creature creature = entry.Key;
					creatureFinishData.TryRemove(creature, out _);
					creature.onAttackFinish(scheduledFinish.attack);
				}
			}
		}
	}

	public void onHitTimeNotDual(Creature creature, Weapon? weapon, AttackPacket attack, int hitTime, int attackTime)
	{
		scheduleAttack(ScheduledAttackType.NORMAL, creature, weapon, attack, hitTime, attackTime, 0, hitTime);
	}

	public void onFirstHitTimeForDual(Creature creature, Weapon? weapon, AttackPacket attack, int hitTime, int attackTime, int delayForSecondAttack)
	{
		scheduleAttack(ScheduledAttackType.DUAL_FIRST, creature, weapon, attack, hitTime, attackTime, delayForSecondAttack, hitTime);
	}

	public void onSecondHitTimeForDual(Creature creature, Weapon? weapon, AttackPacket attack, int hitTime, int attackTime, int delayForSecondAttack)
	{
		scheduleAttack(ScheduledAttackType.DUAL_SECOND, creature, weapon, attack, hitTime, attackTime, delayForSecondAttack, delayForSecondAttack);
	}

	private void scheduleAttack(ScheduledAttackType type, Creature creature, Weapon? weapon, AttackPacket attack, int hitTime, int attackTime, int delayForSecondAttack, int taskDelay)
	{
		ScheduledAttack scheduledAttack = new ScheduledAttack(type, weapon, attack, hitTime, attackTime, delayForSecondAttack,
			DateTime.UtcNow.AddMilliseconds(taskDelay));

		foreach (Map<Creature, ScheduledAttack> pool in ATTACK_POOLS)
		{
			if (pool.Count < POOL_SIZE)
			{
				pool.put(creature, scheduledAttack);
				return;
			}
		}

		Map<Creature, ScheduledAttack> pool1 = new();
		pool1.put(creature, scheduledAttack);
		ThreadPool.scheduleAtFixedRate(new ScheduleAttackTask(pool1), TASK_DELAY, TASK_DELAY); // TODO: high priority task
		ATTACK_POOLS.add(pool1);
	}

	public void onAttackFinish(Creature creature, AttackPacket attack, int taskDelay)
	{
		ScheduledFinish scheduledFinish = new ScheduledFinish(attack, DateTime.UtcNow.AddMilliseconds(taskDelay));

		foreach (Map<Creature, ScheduledFinish> pool in FINISH_POOLS)
		{
			if (pool.Count < POOL_SIZE)
			{
				pool.put(creature, scheduledFinish);
				return;
			}
		}

		Map<Creature, ScheduledFinish> pool1 = new();
		pool1.put(creature, scheduledFinish);
		ThreadPool.scheduleAtFixedRate(new ScheduleAbortTask(pool1), TASK_DELAY, TASK_DELAY);
		FINISH_POOLS.add(pool1);
	}

	public void abortAttack(Creature creature)
	{
		foreach (Map<Creature, ScheduledAttack> pool in ATTACK_POOLS)
		{
			if (pool.remove(creature) != null)
			{
				break;
			}
		}

		foreach (Map<Creature, ScheduledFinish> pool in FINISH_POOLS)
		{
			if (pool.remove(creature) != null)
			{
				return;
			}
		}
	}

	private class ScheduledAttack
	{
		public readonly ScheduledAttackType type;
		public readonly Weapon? weapon;
		public readonly AttackPacket attack;
		public readonly int hitTime;
		public readonly int attackTime;
		public readonly int delayForSecondAttack;
		public readonly DateTime endTime;

		public ScheduledAttack(ScheduledAttackType type, Weapon? weapon, AttackPacket attack, int hitTime, int attackTime, int delayForSecondAttack, DateTime endTime)
		{
			this.type = type;
			this.weapon = weapon;
			this.attack = attack;
			this.hitTime = hitTime;
			this.attackTime = attackTime;
			this.delayForSecondAttack = delayForSecondAttack;
			this.endTime = endTime;
		}
	}

	private class ScheduledFinish
	{
		public readonly AttackPacket attack;
		public readonly DateTime endTime;

		public ScheduledFinish(AttackPacket attack, DateTime endTime)
		{
			this.attack = attack;
			this.endTime = endTime;
		}
	}

	public static CreatureAttackTaskManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly CreatureAttackTaskManager INSTANCE = new CreatureAttackTaskManager();
	}
}