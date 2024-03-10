using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.TaskManagers;

/**
 * @author Mobius
 */
public class PvpFlagTaskManager: Runnable
{
	private static readonly Set<Player> PLAYERS = new();
	private static bool _working = false;
	
	protected PvpFlagTaskManager()
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
		
		if (!PLAYERS.isEmpty())
		{
			DateTime currentTime = DateTime.UtcNow;
			foreach (Player player in PLAYERS)
			{
				if (currentTime > player.getPvpFlagLasts())
				{
					player.stopPvPFlag();
				}
				else if (currentTime > (player.getPvpFlagLasts() - TimeSpan.FromMilliseconds(20000)))
				{
					player.updatePvPFlag(2);
				}
				else
				{
					player.updatePvPFlag(1);
				}
			}
		}
		
		_working = false;
	}
	
	public void add(Player player)
	{
		PLAYERS.add(player);
	}
	
	public void remove(Player player)
	{
		PLAYERS.remove(player);
	}
	
	public static PvpFlagTaskManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly PvpFlagTaskManager INSTANCE = new PvpFlagTaskManager();
	}
}