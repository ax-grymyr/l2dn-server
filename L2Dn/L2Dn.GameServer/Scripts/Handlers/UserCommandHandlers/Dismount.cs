using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Handlers.UserCommandHandlers;

/**
 * Dismount user command.
 * @author Micht
 */
public class Dismount: IUserCommandHandler
{
	private static readonly int[] COMMAND_IDS =
	{
		62
	};
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public bool useUserCommand(int id, Player player)
	{
		if (id != COMMAND_IDS[0])
		{
			return false;
		}
		
		if (player.isRentedPet())
		{
			player.stopRentPet();
		}
		else if (player.isMounted())
		{
			player.dismount();
		}
		return true;
	}
	
	public int[] getUserCommandList()
	{
		return COMMAND_IDS;
	}
}