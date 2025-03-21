using L2Dn.Extensions;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.TaskManagers;

/**
 * Attack stance task manager.
 * @author Luca Baldi
 */
public class AttackStanceTaskManager: Runnable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AttackStanceTaskManager));

	public static readonly TimeSpan COMBAT_TIME = TimeSpan.FromSeconds(15);

	private static readonly Map<Creature, DateTime> CREATURE_ATTACK_STANCES = new();
	private static bool _working;

    private AttackStanceTaskManager()
	{
		ThreadPool.scheduleAtFixedRate(this, 0, 1000); // TODO: high priority task
	}

	public void run()
	{
		if (_working)
		{
			return;
		}

		_working = true;

		if (CREATURE_ATTACK_STANCES.Count != 0)
		{
			try
			{
				DateTime currentTime = DateTime.UtcNow;
				List<Creature> toRemove = new List<Creature>();
				foreach (var entry in CREATURE_ATTACK_STANCES)
				{
					if (currentTime - entry.Value > COMBAT_TIME)
					{
						Creature creature = entry.Key;
						if (creature != null)
						{
							creature.broadcastPacket(new AutoAttackStopPacket(creature.ObjectId));
							creature.getAI().setAutoAttacking(false);
                            if (creature.isPlayer() && creature.hasSummon() &&
                                creature.getActingPlayer() is { } actingPlayer)
                            {
                                actingPlayer.clearDamageTaken();
                                Summon? pet = creature.getPet();
                                if (pet != null)
                                {
                                    pet.broadcastPacket(new AutoAttackStopPacket(pet.ObjectId));
                                }

                                creature.getServitors().Values.ForEach(s =>
                                    s.broadcastPacket(new AutoAttackStopPacket(s.ObjectId)));
                            }

                            toRemove.Add(creature);
						}
					}
				}

				foreach (Creature creature in toRemove)
				{
					CREATURE_ATTACK_STANCES.remove(creature);
				}
			}
			catch (Exception e)
			{
				// Unless caught here, players remain in attack positions.
				LOGGER.Error("Error in AttackStanceTaskManager: " + e);
			}
		}

		_working = false;
	}

	/**
	 * Adds the attack stance task.
	 * @param creature the actor
	 */
	public void addAttackStanceTask(Creature creature)
	{
		if (creature == null)
		{
			return;
		}

		CREATURE_ATTACK_STANCES.put(creature, DateTime.UtcNow);
	}

	/**
	 * Removes the attack stance task.
	 * @param creature the actor
	 */
	public void removeAttackStanceTask(Creature? creature)
	{
		Creature? actor = creature;
		if (actor != null)
		{
			if (actor.isSummon())
			{
				actor = actor.getActingPlayer();
			}

            if (actor != null)
			    CREATURE_ATTACK_STANCES.remove(actor);
		}
	}

	/**
	 * Checks for attack stance task.
	 * @param creature the actor
	 * @return {@code true} if the character has an attack stance task, {@code false} otherwise
	 */
	public bool hasAttackStanceTask(Creature? creature)
	{
		Creature? actor = creature;
		if (actor != null)
		{
			if (actor.isSummon())
			{
				actor = actor.getActingPlayer();
			}

			return actor != null && CREATURE_ATTACK_STANCES.ContainsKey(actor);
		}

		return false;
	}

	/**
	 * Gets the single instance of AttackStanceTaskManager.
	 * @return single instance of AttackStanceTaskManager
	 */
	public static AttackStanceTaskManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly AttackStanceTaskManager INSTANCE = new();
	}
}