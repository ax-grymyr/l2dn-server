using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author Mobius
 */
public class AdminTransform: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_transform",
		"admin_untransform",
		"admin_transform_menu",
	};
	
	public bool useAdminCommand(String command, Player activeChar)
	{
		if (command.equals("admin_transform_menu"))
		{
			AdminHtml.showAdminHtml(activeChar, "transform.htm");
			return true;
		}
		else if (command.startsWith("admin_untransform"))
		{
			WorldObject obj = activeChar.getTarget() == null ? activeChar : activeChar.getTarget();
			if (!obj.isCreature() || !((Creature) obj).isTransformed())
			{
				activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				return false;
			}
			
			((Creature) obj).stopTransformation(true);
		}
		else if (command.startsWith("admin_transform"))
		{
			WorldObject obj = activeChar.getTarget();
			if ((obj == null) || !obj.isPlayer())
			{
				activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				return false;
			}
			
			Player player = obj.getActingPlayer();
			if (activeChar.isSitting())
			{
				activeChar.sendPacket(SystemMessageId.YOU_CANNOT_TRANSFORM_WHILE_SITTING);
				return false;
			}
			
			if (player.isTransformed())
			{
				if (!command.contains(" "))
				{
					player.untransform();
					return true;
				}
				activeChar.sendPacket(SystemMessageId.YOU_ALREADY_POLYMORPHED_AND_CANNOT_POLYMORPH_AGAIN);
				return false;
			}
			
			if (player.isInWater())
			{
				activeChar.sendPacket(SystemMessageId.YOU_CANNOT_POLYMORPH_INTO_THE_DESIRED_FORM_IN_WATER);
				return false;
			}
			
			if (player.isFlyingMounted() || player.isMounted())
			{
				activeChar.sendPacket(SystemMessageId.YOU_CANNOT_TRANSFORM_WHILE_RIDING_A_PET);
				return false;
			}
			
			String[] parts = command.Split(" ");
			if ((parts.Length != 2) || !Util.isDigit(parts[1]))
			{
				BuilderUtil.sendSysMessage(activeChar, "Usage: //transform <id>");
				return false;
			}
			
			int id = int.Parse(parts[1]);
			if (!player.transform(id, true))
			{
				player.sendMessage("Unknown transformation ID: " + id);
				return false;
			}
		}
		
		return true;
	}
	
	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
