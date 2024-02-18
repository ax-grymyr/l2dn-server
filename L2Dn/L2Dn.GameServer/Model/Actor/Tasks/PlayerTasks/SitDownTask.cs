using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;

/**
 * Task dedicated to put player to sit down.
 * @author UnAfraid
 */
public class SitDownTask: Runnable
{
	private readonly Player _player;
	
	public SitDownTask(Player player)
	{
		_player = player;
	}
	
	public void run()
	{
		if (_player != null)
		{
			_player.setBlockActions(false);
		}
	}
}