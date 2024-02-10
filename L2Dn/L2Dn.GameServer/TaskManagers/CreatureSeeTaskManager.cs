using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.TaskManagers;

/**
 * @author Mobius
 */
public class CreatureSeeTaskManager: Runnable
{
	private static readonly Set<Creature> CREATURES = new();
	private static bool _working = false;
	
	protected CreatureSeeTaskManager()
	{
		ThreadPool.scheduleAtFixedRate(this, 1000, 1000);
	}
	
	public void run()
	{
		if (_working)
		{
			return;
		}
		_working = true;
		
		foreach (Creature creature in CREATURES)
		{
			creature.updateSeenCreatures();
		}
		
		_working = false;
	}
	
	public void add(Creature creature)
	{
		CREATURES.add(creature);
	}
	
	public void remove(Creature creature)
	{
		CREATURES.remove(creature);
	}
	
	public static CreatureSeeTaskManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly CreatureSeeTaskManager INSTANCE = new CreatureSeeTaskManager();
	}
}