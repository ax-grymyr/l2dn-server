using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Handlers.UserCommandHandlers;

/**
 * Mount user command.
 * @author Tempy
 */
public class Mount: IUserCommandHandler
{
	private static readonly int[] COMMAND_IDS =
	{
		61
	};
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public bool useUserCommand(int id, Player player)
	{
		if (id != COMMAND_IDS[0])
		{
			return false;
		}
		
		return player.mountPlayer(player.getPet());
	}
	
	public int[] getUserCommandList()
	{
		return COMMAND_IDS;
	}
}