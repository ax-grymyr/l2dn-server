using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands:
 * <li>add_exp_sp_to_character <i>shows menu for add or remove</i>
 * <li>add_exp_sp exp sp <i>Adds exp & sp to target, displays menu if a parameter is missing</i>
 * <li>remove_exp_sp exp sp <i>Removes exp & sp from target, displays menu if a parameter is missing</i>
 * @version $Revision: 1.2.4.6 $ $Date: 2005/04/11 10:06:06 $
 */
public class AdminExpSp: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_add_exp_sp_to_character",
		"admin_add_exp_sp",
		"admin_remove_exp_sp",
    ];
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		if (command.startsWith("admin_add_exp_sp"))
		{
			try
			{
				string val = command.Substring(16);
				if (!adminAddExpSp(activeChar, val))
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: //add_exp_sp exp sp");
				}
			}
			catch (IndexOutOfRangeException e)
			{ // Case of missing parameter
				BuilderUtil.sendSysMessage(activeChar, "Usage: //add_exp_sp exp sp");
			}
		}
		else if (command.startsWith("admin_remove_exp_sp"))
		{
			try
			{
				string val = command.Substring(19);
				if (!adminRemoveExpSP(activeChar, val))
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: //remove_exp_sp exp sp");
				}
			}
			catch (IndexOutOfRangeException e)
			{ // Case of missing parameter
				BuilderUtil.sendSysMessage(activeChar, "Usage: //remove_exp_sp exp sp");
			}
		}
		addExpSp(activeChar);
		return true;
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
	
	private void addExpSp(Player activeChar)
	{
		WorldObject target = activeChar.getTarget();
		Player player = null;
		if ((target != null) && target.isPlayer())
		{
			player = (Player) target;
		}
		else
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			return;
		}

		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/expsp.htm", activeChar);
		NpcHtmlMessagePacket adminReply = new NpcHtmlMessagePacket(null, 1, htmlContent);
		htmlContent.Replace("%name%", player.getName());
		htmlContent.Replace("%level%", player.getLevel().ToString());
		htmlContent.Replace("%xp%", player.getExp().ToString());
		htmlContent.Replace("%sp%", player.getSp().ToString());
		htmlContent.Replace("%class%", ClassListData.getInstance().getClass(player.getClassId()).getClientCode());
		activeChar.sendPacket(adminReply);
	}
	
	private bool adminAddExpSp(Player activeChar, string expSp)
	{
		WorldObject target = activeChar.getTarget();
		Player player = null;
		if ((target != null) && target.isPlayer())
		{
			player = (Player) target;
		}
		else
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			return false;
		}
		StringTokenizer st = new StringTokenizer(expSp);
		if (st.countTokens() != 2)
		{
			return false;
		}
		
		string exp = st.nextToken();
		string sp = st.nextToken();
		long expval = 0;
		long spval = 0;
		try
		{
			expval = long.Parse(exp);
			spval = long.Parse(sp);
		}
		catch (Exception e)
		{
			return false;
		}
		if ((expval != 0) || (spval != 0))
		{
			// Common character information
			player.sendMessage("Admin is adding you " + expval + " xp and " + spval + " sp.");
			player.addExpAndSp(expval, spval);
			// Admin information
			BuilderUtil.sendSysMessage(activeChar, "Added " + expval + " xp and " + spval + " sp to " + player.getName() + ".");
		}
		return true;
	}
	
	private bool adminRemoveExpSP(Player activeChar, string expSp)
	{
		WorldObject target = activeChar.getTarget();
		Player player = null;
		if ((target != null) && target.isPlayer())
		{
			player = (Player) target;
		}
		else
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			return false;
		}
		StringTokenizer st = new StringTokenizer(expSp);
		if (st.countTokens() != 2)
		{
			return false;
		}
		
		string exp = st.nextToken();
		string sp = st.nextToken();
		long expval = 0;
		int spval = 0;
		try
		{
			expval = long.Parse(exp);
			spval = int.Parse(sp);
		}
		catch (Exception e)
		{
			return false;
		}
		if ((expval != 0) || (spval != 0))
		{
			// Common character information
			player.sendMessage("Admin is removing you " + expval + " xp and " + spval + " sp.");
			player.removeExpAndSp(expval, spval);
			// Admin information
			BuilderUtil.sendSysMessage(activeChar, "Removed " + expval + " xp and " + spval + " sp from " + player.getName() + ".");
		}
		return true;
	}
}
