using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.TaskManagers;

namespace L2Dn.GameServer.Scripts.Handlers.UserCommandHandlers;

/**
 * Time user command.
 */
public class Time: IUserCommandHandler
{
	private static readonly int[] COMMAND_IDS =
	{
		77
	};
	
	public bool useUserCommand(int id, Player player)
	{
		if (COMMAND_IDS[0] != id)
		{
			return false;
		}
		
		int t = GameTimeTaskManager.getInstance().getGameTime();
		string h = ((t / 60) % 24).ToString();
		string m;
		if ((t % 60) < 10)
		{
			m = "0" + (t % 60);
		}
		else
		{
			m = (t % 60).ToString();
		}
		
		SystemMessagePacket sm;
		if (GameTimeTaskManager.getInstance().isNight())
		{
			sm = new SystemMessagePacket(SystemMessageId.THE_CURRENT_TIME_IS_S1_S2_2);
			sm.Params.addString(h);
			sm.Params.addString(m);
		}
		else
		{
			sm = new SystemMessagePacket(SystemMessageId.THE_CURRENT_TIME_IS_S1_S2);
			sm.Params.addString(h);
			sm.Params.addString(m);
		}
		
		player.sendPacket(sm);
		if (Config.DISPLAY_SERVER_TIME)
		{
			player.sendMessage("Server time is " + DateTime.Today.ToShortTimeString());
		}
		
		return true;
	}
	
	public int[] getUserCommandList()
	{
		return COMMAND_IDS;
	}
}