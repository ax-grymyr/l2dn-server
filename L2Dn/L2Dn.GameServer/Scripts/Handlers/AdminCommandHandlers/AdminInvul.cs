using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands: - invul = turns invulnerability on/off
 * @version $Revision: 1.2.4.4 $ $Date: 2007/07/31 10:06:02 $
 */
public class AdminInvul: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_invul",
		"admin_setinvul",
		"admin_undying",
		"admin_setundying"
	};
	
	public bool useAdminCommand(String command, Player activeChar)
	{
		if (command.equals("admin_invul"))
		{
			handleInvul(activeChar);
			AdminHtml.showAdminHtml(activeChar, "gm_menu.htm");
		}
		else if (command.equals("admin_undying"))
		{
			handleUndying(activeChar, activeChar);
			AdminHtml.showAdminHtml(activeChar, "gm_menu.htm");
		}
		
		else if (command.equals("admin_setinvul"))
		{
			WorldObject target = activeChar.getTarget();
			if ((target != null) && target.isPlayer())
			{
				handleInvul((Player) target);
			}
		}
		else if (command.equals("admin_setundying"))
		{
			WorldObject target = activeChar.getTarget();
			if (target.isCreature())
			{
				handleUndying(activeChar, (Creature) target);
			}
		}
		return true;
	}
	
	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
	
	private void handleInvul(Player activeChar)
	{
		String text;
		if (activeChar.isInvul())
		{
			activeChar.setInvul(false);
			text = activeChar.getName() + " is now mortal.";
		}
		else
		{
			activeChar.setInvul(true);
			text = activeChar.getName() + " is now invulnerable.";
		}
		BuilderUtil.sendSysMessage(activeChar, text);
	}
	
	private void handleUndying(Player activeChar, Creature target)
	{
		String text;
		if (target.isUndying())
		{
			target.setUndying(false);
			text = target.getName() + " is now mortal.";
		}
		else
		{
			target.setUndying(true);
			text = target.getName() + " is now undying.";
		}
		BuilderUtil.sendSysMessage(activeChar, text);
	}
}
