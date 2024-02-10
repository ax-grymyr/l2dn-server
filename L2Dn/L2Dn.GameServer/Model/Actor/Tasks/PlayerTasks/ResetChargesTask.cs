using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;

/**
 * Task dedicated to reset player's current charges.
 * @author UnAfraid
 */
public class ResetChargesTask: Runnable
{
	private readonly Player _player;
	
	public ResetChargesTask(Player player)
	{
		_player = player;
	}
	
	public override void run()
	{
		if (_player != null)
		{
			_player.clearCharges();
		}
	}
}
