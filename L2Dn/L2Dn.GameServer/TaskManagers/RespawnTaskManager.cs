using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.TaskManagers;

/**
 * @author Mobius
 */
public class RespawnTaskManager: Runnable
{
	private static readonly Map<Npc, DateTime> PENDING_RESPAWNS = new();
	private static bool _working = false;
	
	protected RespawnTaskManager()
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

		if (!PENDING_RESPAWNS.isEmpty())
		{
			DateTime currentTime = DateTime.UtcNow;
			List<Npc> npcs = PENDING_RESPAWNS.Where(x => currentTime > x.Value).Select(x => x.Key).ToList();
			foreach (Npc npc in npcs)
			{
				PENDING_RESPAWNS.remove(npc);

				Spawn spawn = npc.getSpawn();
				if (spawn != null)
				{
					spawn.respawnNpc(npc);
					spawn._scheduledCount--;
				}
			}
		}

		_working = false;
	}
	
	public void add(Npc npc, DateTime time)
	{
		PENDING_RESPAWNS.put(npc, time);
	}
	
	public static RespawnTaskManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly RespawnTaskManager INSTANCE = new RespawnTaskManager();
	}
}