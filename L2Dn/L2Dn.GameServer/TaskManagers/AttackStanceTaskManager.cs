using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = System.Threading.ThreadPool;

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
	private static bool _working = false;
	
	protected AttackStanceTaskManager()
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
		
		if (!CREATURE_ATTACK_STANCES.isEmpty())
		{
			try
			{
				DateTime currentTime = DateTime.Now;
				Iterator<Entry<Creature, long>> iterator = CREATURE_ATTACK_STANCES.entrySet().iterator();
				Entry<Creature, long> entry;
				Creature creature;
				
				while (iterator.hasNext())
				{
					entry = iterator.next();
					if ((currentTime - entry.getValue()) > COMBAT_TIME)
					{
						creature = entry.getKey();
						if (creature != null)
						{
							creature.broadcastPacket(new AutoAttackStop(creature.getObjectId()));
							creature.getAI().setAutoAttacking(false);
							if (creature.isPlayer() && creature.hasSummon())
							{
								creature.getActingPlayer().clearDamageTaken();
								Summon pet = creature.getPet();
								if (pet != null)
								{
									pet.broadcastPacket(new AutoAttackStop(pet.getObjectId()));
								}
								creature.getServitors().values().forEach(s => s.broadcastPacket(new AutoAttackStop(s.getObjectId())));
							}
						}
						iterator.remove();
					}
				}
			}
			catch (Exception e)
			{
				// Unless caught here, players remain in attack positions.
				LOGGER.Warn("Error in AttackStanceTaskManager: " + e);
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
		
		CREATURE_ATTACK_STANCES.put(creature, DateTime.Now);
	}
	
	/**
	 * Removes the attack stance task.
	 * @param creature the actor
	 */
	public void removeAttackStanceTask(Creature creature)
	{
		Creature actor = creature;
		if (actor != null)
		{
			if (actor.isSummon())
			{
				actor = actor.getActingPlayer();
			}
			CREATURE_ATTACK_STANCES.remove(actor);
		}
	}
	
	/**
	 * Checks for attack stance task.
	 * @param creature the actor
	 * @return {@code true} if the character has an attack stance task, {@code false} otherwise
	 */
	public bool hasAttackStanceTask(Creature creature)
	{
		Creature actor = creature;
		if (actor != null)
		{
			if (actor.isSummon())
			{
				actor = actor.getActingPlayer();
			}
			return CREATURE_ATTACK_STANCES.containsKey(actor);
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
		public static readonly AttackStanceTaskManager INSTANCE = new AttackStanceTaskManager();
	}
}