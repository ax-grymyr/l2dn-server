using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
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
	
	public void run()
	{
		if (_player != null)
		{
			if (_player.isOnline())
			{
				int hours = (int)(_player.getUptime() + TimeSpan.FromMinutes(1)).TotalHours;
				SystemMessagePacket sm =
					new SystemMessagePacket(SystemMessageId.YOU_HAVE_PLAYED_FOR_S1_H_TAKE_A_BREAK_PLEASE);
				
				sm.Params.addLong(hours);
				_player.sendPacket(sm);
			}
			else
			{
				_player.stopWarnUserTakeBreak();
			}
		}
	}
}