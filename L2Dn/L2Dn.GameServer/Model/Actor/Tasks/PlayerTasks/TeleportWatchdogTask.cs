using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;

/**
 * Task dedicated watch for player teleportation.
 * @author UnAfraid
 */
public class TeleportWatchdogTask: Runnable
{
	private readonly Player _player;
	
	public TeleportWatchdogTask(Player player)
	{
		_player = player;
	}
	
	public override void run()
	{
		if ((_player == null) || !_player.isTeleporting())
		{
			return;
		}
		_player.onTeleported();
	}
}
