using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands: - character_disconnect = disconnects target player
 * @version $Revision: 1.2.4.4 $ $Date: 2005/04/11 10:06:00 $
 */
public class AdminDisconnect: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_character_disconnect"
	};
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		if (command.equals("admin_character_disconnect"))
		{
			disconnectCharacter(activeChar);
		}
		return true;
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
	
	private void disconnectCharacter(Player activeChar)
	{
		WorldObject target = activeChar.getTarget();
		Player player = null;
		if ((target != null) && target.isPlayer())
		{
			player = (Player) target;
		}
		else
		{
			return;
		}
		
		if (player == activeChar)
		{
			BuilderUtil.sendSysMessage(activeChar, "You cannot logout your own character.");
		}
		else
		{
			BuilderUtil.sendSysMessage(activeChar, "Character " + player.getName() + " disconnected from server.");
			Disconnection.of(player).defaultSequence(LeaveWorldPacket.STATIC_PACKET);
		}
	}
}
