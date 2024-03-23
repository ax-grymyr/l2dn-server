using System.Text;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles all siege commands: Todo: change the class name, and neaten it up
 */
public class AdminFortSiege: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_fortsiege",
		"admin_add_fortattacker",
		"admin_list_fortsiege_clans",
		"admin_clear_fortsiege_list",
		"admin_spawn_fortdoors",
		"admin_endfortsiege",
		"admin_startfortsiege",
		"admin_setfort",
		"admin_removefort"
	};
	
	public bool useAdminCommand(String commandValue, Player activeChar)
	{
		String command = commandValue;
		StringTokenizer st = new StringTokenizer(command, " ");
		command = st.nextToken(); // Get actual command
		
		// Get fort
		Fort fort = null;
		int fortId = 0;
		if (st.hasMoreTokens())
		{
			fortId = int.Parse(st.nextToken());
			fort = FortManager.getInstance().getFortById(fortId);
		}
		// Get fort
		if (((fort == null) || (fortId == 0)))
		{
			// No fort specified
			showFortSelectPage(activeChar);
		}
		else
		{
			WorldObject target = activeChar.getTarget();
			Player player = null;
			if ((target != null) && target.isPlayer())
			{
				player = (Player) target;
			}
			
			if (command.equalsIgnoreCase("admin_add_fortattacker"))
			{
				if (player == null)
				{
					activeChar.sendPacket(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET);
				}
				else if (fort.getSiege().addAttacker(player, false) == 4)
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOUR_CLAN_HAS_BEEN_REGISTERED_TO_S1_S_FORTRESS_BATTLE);
					sm.Params.addCastleId(fort.getResidenceId());
					player.sendPacket(sm);
				}
				else
				{
					player.sendMessage("During registering error occurred!");
				}
			}
			else if (command.equalsIgnoreCase("admin_clear_fortsiege_list"))
			{
				fort.getSiege().clearSiegeClan();
			}
			else if (command.equalsIgnoreCase("admin_endfortsiege"))
			{
				fort.getSiege().endSiege();
			}
			else if (command.equalsIgnoreCase("admin_list_fortsiege_clans"))
			{
				BuilderUtil.sendSysMessage(activeChar, "Not implemented yet.");
			}
			else if (command.equalsIgnoreCase("admin_setfort"))
			{
				if ((player == null) || (player.getClan() == null))
				{
					activeChar.sendPacket(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET);
				}
				else
				{
					fort.endOfSiege(player.getClan());
				}
			}
			else if (command.equalsIgnoreCase("admin_removefort"))
			{
				Clan clan = fort.getOwnerClan();
				if (clan != null)
				{
					fort.removeOwner(true);
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "Unable to remove fort");
				}
			}
			else if (command.equalsIgnoreCase("admin_spawn_fortdoors"))
			{
				fort.resetDoors();
			}
			else if (command.equalsIgnoreCase("admin_startfortsiege"))
			{
				fort.getSiege().startSiege();
			}
			
			showFortSiegePage(activeChar, fort);
		}
		return true;
	}
	
	private void showFortSelectPage(Player activeChar)
	{
		int i = 0;
		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/forts.htm", activeChar);
		NpcHtmlMessagePacket adminReply = new NpcHtmlMessagePacket(null, 1, htmlContent);
		
		ICollection<Fort> forts = FortManager.getInstance().getForts();
		StringBuilder cList = new StringBuilder(forts.Count * 100);
		foreach (Fort fort in forts)
		{
			if (fort != null)
			{
				cList.Append("<td fixwidth=90><a action=\"bypass -h admin_fortsiege " + fort.getResidenceId() + "\">" + fort.getName() + " id: " + fort.getResidenceId() + "</a></td>");
				i++;
			}
			
			if (i > 2)
			{
				cList.Append("</tr><tr>");
				i = 0;
			}
		}
		
		htmlContent.Replace("%forts%", cList.ToString());
		activeChar.sendPacket(adminReply);
	}
	
	private void showFortSiegePage(Player activeChar, Fort fort)
	{
		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/fort.htm", activeChar);
		NpcHtmlMessagePacket adminReply = new NpcHtmlMessagePacket(null, 1, htmlContent);
		htmlContent.Replace("%fortName%", fort.getName());
		htmlContent.Replace("%fortId%", fort.getResidenceId().ToString());
		activeChar.sendPacket(adminReply);
	}

	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
