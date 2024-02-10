using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;

/**
 * Task dedicated to warn user to take a break.
 * @author UnAfraid
 */
public class WarnUserTakeBreakTask: Runnable
{
	private readonly Player _player;
	
	public WarnUserTakeBreakTask(Player player)
	{
		_player = player;
	}
	
	public override void run()
	{
		if (_player != null)
		{
			if (_player.isOnline())
			{
				long hours = TimeUnit.MILLISECONDS.toHours(_player.getUptime() + 60000);
				_player.sendPacket(new SystemMessage(SystemMessageId.YOU_HAVE_PLAYED_FOR_S1_H_TAKE_A_BREAK_PLEASE).addLong(hours));
			}
			else
			{
				_player.stopWarnUserTakeBreak();
			}
		}
	}
}