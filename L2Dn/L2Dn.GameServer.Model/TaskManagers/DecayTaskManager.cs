using System.Text;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.TaskManagers;

/**
 * @author Mobius
 */
public class DecayTaskManager: Runnable
{
	private static readonly Map<Creature, DateTime> DECAY_SCHEDULES = new();
	private static bool _working = false;

	protected DecayTaskManager()
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

		if (DECAY_SCHEDULES.Count != 0)
		{
			DateTime currentTime = DateTime.UtcNow;
			List<Creature> creatures = DECAY_SCHEDULES.Where(c => currentTime > c.Value).Select(x => x.Key).ToList();
			foreach (Creature creature in creatures)
			{
				creature.onDecay();
				DECAY_SCHEDULES.remove(creature);
			}
		}

		_working = false;
	}

	/**
	 * Adds a decay task for the specified character.<br>
	 * If the decay task already exists it cancels it and re-adds it.
	 * @param creature the creature
	 */
	public void add(Creature creature)
	{
		if (creature == null)
		{
			return;
		}

		long delay;

		// Pet related - Removed on Essence.
		// if (creature.isPet())
		// {
		// delay = 86400;
		// }
		// else

		if (creature.getTemplate() is NpcTemplate)
		{
			delay = ((NpcTemplate) creature.getTemplate()).getCorpseTime();
		}
		else
		{
			delay = Config.Npc.DEFAULT_CORPSE_TIME;
		}

		if (creature.isAttackable() && (((Attackable) creature).isSpoiled() || ((Attackable) creature).isSeeded()))
		{
			delay += Config.Npc.SPOILED_CORPSE_EXTEND_TIME;
		}

		if (creature.isPlayer() && creature.getActingPlayer() is {} player)
		{
			if (player.isOfflinePlay() && Config.OfflinePlay.OFFLINE_PLAY_LOGOUT_ON_DEATH)
			{
				delay = 10; // 10 seconds
			}
			else if (player.isInTimedHuntingZone())
			{
				delay = 600; // 10 minutes
			}
			else if (Config.Character.DISCONNECT_AFTER_DEATH)
			{
				delay = 3600; // 1 hour
			}
		}

		// Add to decay schedules.
		DECAY_SCHEDULES.put(creature, DateTime.UtcNow.AddMilliseconds(delay * 1000));
	}

	/**
	 * Cancels the decay task of the specified character.
	 * @param creature the creature
	 */
	public void cancel(Creature creature)
	{
		DECAY_SCHEDULES.remove(creature);
	}

	/**
	 * Gets the remaining time of the specified character's decay task.
	 * @param creature the creature
	 * @return if a decay task exists the remaining time, {@code Long.MAX_VALUE} otherwise
	 */
	public TimeSpan getRemainingTime(Creature creature)
	{
		if (DECAY_SCHEDULES.TryGetValue(creature, out DateTime time))
			return time - DateTime.UtcNow;

		return TimeSpan.MaxValue;
	}

	public override string ToString()
	{
		StringBuilder ret = new StringBuilder();
		ret.Append("============= DecayTask Manager Report ============");
		ret.Append(Environment.NewLine);
		ret.Append("Tasks count: ");
		ret.Append(DECAY_SCHEDULES.Count);
		ret.Append(Environment.NewLine);
		ret.Append("Tasks dump:");
		ret.Append(Environment.NewLine);

		DateTime time = DateTime.UtcNow;
		foreach (var entry in DECAY_SCHEDULES)
		{
			ret.Append("Class/Name: ");
			ret.Append(entry.Key.GetType().Name);
			ret.Append('/');
			ret.Append(entry.Key.getName());
			ret.Append(" decay timer: ");
			ret.Append(entry.Value - time);
			ret.Append(Environment.NewLine);
		}

		return ret.ToString();
	}

	public static DecayTaskManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static DecayTaskManager INSTANCE = new DecayTaskManager();
	}
}