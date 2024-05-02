using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;

/**
 * @author UnAfraid
 */
public class TeleportTask: Runnable
{
	private readonly Player _player;
	private readonly LocationHeading _loc;
	
	public TeleportTask(Player player, LocationHeading loc)
	{
		_player = player;
		_loc = loc;
	}
	
	public void run()
	{
		if ((_player != null) && _player.isOnline())
		{
			_player.teleToLocation(_loc, true);
		}
	}
}