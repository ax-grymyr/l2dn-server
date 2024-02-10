using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;

/**
 * Task dedicated to end player's recommendation bonus.
 * @author UnAfraid
 */
public class RecoBonusTaskEnd: Runnable
{
	private readonly Player _player;
	
	public RecoBonusTaskEnd(Player player)
	{
		_player = player;
	}
	
	public override void run()
	{
		if (_player != null)
		{
			_player.sendPacket(new ExVoteSystemInfo(_player));
		}
	}
}
