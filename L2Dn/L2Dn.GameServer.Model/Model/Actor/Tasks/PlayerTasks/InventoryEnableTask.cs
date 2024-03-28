using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;

/**
 * Task dedicated to enable player's inventory.
 * @author UnAfraid
 */
public class InventoryEnableTask: Runnable
{
	private readonly Player _player;
	
	public InventoryEnableTask(Player player)
	{
		_player = player;
	}
	
	public void run()
	{
		if (_player != null)
		{
			_player.setInventoryBlockingStatus(false);
		}
	}
}