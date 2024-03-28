using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;

/**
* Task dedicated to dismount player from pet.
* @author UnAfraid
*/
public class DismountTask: Runnable
{
	private readonly Player _player;

	public DismountTask(Player player)
	{
		_player = player;
	}

	public void run()
	{
		if (_player != null)
		{
			_player.dismount();
		}
	}
}