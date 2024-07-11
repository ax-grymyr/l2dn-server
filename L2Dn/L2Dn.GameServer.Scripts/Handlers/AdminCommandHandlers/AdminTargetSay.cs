using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands: - targetsay <message> = makes talk a Creature
 * @author nonom
 */
public class AdminTargetSay: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_targetsay"
	};
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		if (command.startsWith("admin_targetsay"))
		{
			try
			{
				WorldObject obj = activeChar.getTarget();
				if ((obj is StaticObject) || !obj.isCreature())
				{
					activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
					return false;
				}
				
				string message = command.Substring(16);
				Creature target = (Creature) obj;
				target.broadcastPacket(new CreatureSayPacket(target, target.isPlayer() ? ChatType.GENERAL : ChatType.NPC_GENERAL, target.getName(), message));
			}
			catch (IndexOutOfRangeException e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Usage: //targetsay <text>");
				return false;
			}
		}
		return true;
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
