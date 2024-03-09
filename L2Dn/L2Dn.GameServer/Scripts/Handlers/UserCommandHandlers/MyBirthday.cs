using System.Globalization;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Handlers.UserCommandHandlers;

/**
 * My Birthday user command.
 * @author JIV
 */
public class MyBirthday: IUserCommandHandler
{
	private static readonly int[] COMMAND_IDS =
	{
		126
	};
	
	public bool useUserCommand(int id, Player player)
	{
		if (id != COMMAND_IDS[0])
		{
			return false;
		}
		
		DateTime date = player.getCreateDate();
		
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_S_BIRTHDAY_IS_S3_S4_S2);
		sm.Params.addPcName(player);
		sm.Params.addString(date.Year.ToString());
		sm.Params.addString(date.Month.ToString());
		sm.Params.addString(date.Day.ToString());
		
		player.sendPacket(sm);
		return true;
	}

	public int[] getUserCommandList()
	{
		return COMMAND_IDS;
	}
}