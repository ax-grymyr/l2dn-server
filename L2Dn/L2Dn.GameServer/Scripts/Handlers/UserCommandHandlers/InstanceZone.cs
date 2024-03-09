using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Handlers.UserCommandHandlers;

/**
 * Instance Zone user command.
 * @author nille02, UnAfraid
 */
public class InstanceZone: IUserCommandHandler
{
	private static readonly int[] COMMAND_IDS =
	{
		90
	};
	
	public int[] getUserCommandList()
	{
		return COMMAND_IDS;
	}
	
	public bool useUserCommand(int id, Player player)
	{
		if (id != COMMAND_IDS[0])
		{
			return false;
		}
		player.sendPacket(new ExInzoneWaitingPacket(player, false));
		return true;
	}
}