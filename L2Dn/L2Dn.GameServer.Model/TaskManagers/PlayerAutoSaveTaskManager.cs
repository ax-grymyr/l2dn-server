using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.TaskManagers;

/**
 * @author Mobius
 */
public class PlayerAutoSaveTaskManager: Runnable
{
	private static readonly Map<Player, DateTime> PLAYER_TIMES = new();
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

		if (PLAYER_TIMES.Count != 0)
		{
			DateTime currentTime = DateTime.UtcNow;
			List<Player> toRemove = new List<Player>();
			foreach (var entry in PLAYER_TIMES)
			{
				Player player = entry.Key;
				DateTime time = entry.Value;

				if (currentTime > time)
				{
					if (player.isOnline())
					{
						player.autoSave();
						PLAYER_TIMES.put(player, currentTime + TimeSpan.FromMilliseconds(Config.CHAR_DATA_STORE_INTERVAL));
						break; // Prevent SQL flood.
					}

					toRemove.Add(player);
				}
			}

			foreach (Player player in toRemove)
			{
				PLAYER_TIMES.remove(player);
			}
		}

		_working = false;
	}

	public void add(Player player)
	{
		PLAYER_TIMES.put(player, DateTime.UtcNow + TimeSpan.FromMilliseconds(Config.CHAR_DATA_STORE_INTERVAL));
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