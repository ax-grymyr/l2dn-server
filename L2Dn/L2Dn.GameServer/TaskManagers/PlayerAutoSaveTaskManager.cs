using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.TaskManagers;

/**
 * @author Mobius
 */
public class PlayerAutoSaveTaskManager: Runnable
{
	private static readonly Map<Player, long> PLAYER_TIMES = new();
	private static bool _working = false;
	
	protected PlayerAutoSaveTaskManager()
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
		
		if (!PLAYER_TIMES.isEmpty())
		{
			DateTime currentTime = DateTime.Now;
			Iterator<Entry<Player, Long>> iterator = PLAYER_TIMES.entrySet().iterator();
			Entry<Player, long> entry;
			Player player;
			long time;
			
			while (iterator.hasNext())
			{
				entry = iterator.next();
				player = entry.getKey();
				time = entry.getValue();
				
				if (currentTime > time)
				{
					if ((player != null) && player.isOnline())
					{
						player.autoSave();
						PLAYER_TIMES.put(player, currentTime + Config.CHAR_DATA_STORE_INTERVAL);
						break; // Prevent SQL flood.
					}
					
					iterator.remove();
				}
			}
		}
		
		_working = false;
	}
	
	public void add(Player player)
	{
		PLAYER_TIMES.put(player, System.currentTimeMillis() + Config.CHAR_DATA_STORE_INTERVAL);
	}
	
	public void remove(Player player)
	{
		PLAYER_TIMES.remove(player);
	}
	
	public static PlayerAutoSaveTaskManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly PlayerAutoSaveTaskManager INSTANCE = new PlayerAutoSaveTaskManager();
	}
}