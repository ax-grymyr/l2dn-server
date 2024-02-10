using L2Dn.GameServer.AI;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;

/**
 * Task dedicated to put player to stand up.
 * @author UnAfraid
 */
public class StandUpTask: Runnable
{
	private readonly Player _player;
	
	public StandUpTask(Player player)
	{
		_player = player;
	}
	
	public override void run()
	{
		if (_player != null)
		{
			_player.setSitting(false);
			_player.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
		}
	}
}
