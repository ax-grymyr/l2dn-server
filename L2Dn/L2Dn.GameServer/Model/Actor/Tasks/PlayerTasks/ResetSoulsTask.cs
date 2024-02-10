using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;

/**
 * Task dedicated to reset player's current souls.
 * @author UnAfraid
 */
public class ResetSoulsTask: Runnable
{
	private readonly Player _player;
	
	public ResetSoulsTask(Player player)
	{
		_player = player;
	}
	
	public override void run()
	{
		if (_player != null)
		{
			_player.clearSouls();
		}
	}
}
