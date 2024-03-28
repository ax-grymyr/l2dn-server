using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;

/**
 * @author Mobius
 */
public class HennaDurationTask: Runnable
{
	private readonly Player _player;
	private readonly int _slot;
	
	public HennaDurationTask(Player player, int slot)
	{
		_player = player;
		_slot = slot;
	}
	
	public void run()
	{
		if (_player != null)
		{
			_player.removeHenna(_slot);
		}
	}
}