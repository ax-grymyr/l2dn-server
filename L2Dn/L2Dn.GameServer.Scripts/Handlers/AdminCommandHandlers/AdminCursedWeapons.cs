using System.Text;
using System.Text.RegularExpressions;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands: - cw_info = displays cursed weapon status - cw_remove = removes a cursed weapon from the world, item id or name must be provided - cw_add = adds a cursed weapon into the world, item id or name must be provided. Target will be the weilder - cw_goto =
 * teleports GM to the specified cursed weapon - cw_reload = reloads instance manager
 * @version $Revision: 1.1.6.3 $ $Date: 2007/07/31 10:06:06 $
 */
public class AdminCursedWeapons: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_cw_info",
		"admin_cw_remove",
		"admin_cw_goto",
		"admin_cw_reload",
		"admin_cw_add",
		"admin_cw_info_menu",
    ];
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		CursedWeaponsManager cwm = CursedWeaponsManager.getInstance();
		int id = 0;
		
		StringTokenizer st = new StringTokenizer(command);
		st.nextToken();
		
		if (command.startsWith("admin_cw_info"))
		{
			if (!command.contains("menu"))
			{
				BuilderUtil.sendSysMessage(activeChar, "====== Cursed Weapons: ======");
				foreach (CursedWeapon cw in cwm.getCursedWeapons())
				{
					BuilderUtil.sendSysMessage(activeChar, "> " + cw.getName() + " (" + cw.getItemId() + ")");
					if (cw.isActivated())
					{
						Player pl = cw.getPlayer();
						BuilderUtil.sendSysMessage(activeChar, "  Player holding: " + (pl == null ? "null" : pl.getName()));
						BuilderUtil.sendSysMessage(activeChar, "    Player Reputation: " + cw.getPlayerReputation());
						BuilderUtil.sendSysMessage(activeChar, "    Time Remaining: " + (cw.getTimeLeft() / 60000) + " min.");
						BuilderUtil.sendSysMessage(activeChar, "    Kills : " + cw.getNbKills());
					}
					else if (cw.isDropped())
					{
						BuilderUtil.sendSysMessage(activeChar, "  Lying on the ground.");
						BuilderUtil.sendSysMessage(activeChar, "    Time Remaining: " + (cw.getTimeLeft() / 60000) + " min.");
						BuilderUtil.sendSysMessage(activeChar, "    Kills : " + cw.getNbKills());
					}
					else
					{
						BuilderUtil.sendSysMessage(activeChar, "  Don't exist in the world.");
					}
					activeChar.sendPacket(SystemMessageId.EMPTY_3);
				}
			}
			else
			{
				ICollection<CursedWeapon> cws = cwm.getCursedWeapons();
				StringBuilder replyMSG = new StringBuilder(cws.Count * 300);
				HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/cwinfo.htm", activeChar);
				NpcHtmlMessagePacket adminReply = new NpcHtmlMessagePacket(null, 1, htmlContent);
				foreach (CursedWeapon cw in cwm.getCursedWeapons())
				{
					int itemId = cw.getItemId();
					replyMSG.Append("<table width=270><tr><td>Name:</td><td>");
					replyMSG.Append(cw.getName());
					replyMSG.Append("</td></tr>");
					
					if (cw.isActivated())
					{
						Player pl = cw.getPlayer();
						replyMSG.Append("<tr><td>Weilder:</td><td>");
						replyMSG.Append(pl == null ? "null" : pl.getName());
						replyMSG.Append("</td></tr>");
						replyMSG.Append("<tr><td>Karma:</td><td>");
						replyMSG.Append(cw.getPlayerReputation());
						replyMSG.Append("</td></tr>");
						replyMSG.Append("<tr><td>Kills:</td><td>");
						replyMSG.Append(cw.getPlayerPkKills());
						replyMSG.Append("/");
						replyMSG.Append(cw.getNbKills());
						replyMSG.Append("</td></tr><tr><td>Time remaining:</td><td>");
						replyMSG.Append(cw.getTimeLeft() / 60000);
						replyMSG.Append(" min.</td></tr>");
						replyMSG.Append("<tr><td><button value=\"Remove\" action=\"bypass -h admin_cw_remove ");
						replyMSG.Append(itemId);
						replyMSG.Append("\" width=73 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td>");
						replyMSG.Append("<td><button value=\"Go\" action=\"bypass -h admin_cw_goto ");
						replyMSG.Append(itemId);
						replyMSG.Append("\" width=73 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr>");
					}
					else if (cw.isDropped())
					{
						replyMSG.Append("<tr><td>Position:</td><td>Lying on the ground</td></tr><tr><td>Time remaining:</td><td>");
						replyMSG.Append(cw.getTimeLeft() / 60000);
						replyMSG.Append(" min.</td></tr><tr><td>Kills:</td><td>");
						replyMSG.Append(cw.getNbKills());
						replyMSG.Append("</td></tr><tr><td><button value=\"Remove\" action=\"bypass -h admin_cw_remove ");
						replyMSG.Append(itemId);
						replyMSG.Append("\" width=73 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td>");
						replyMSG.Append("<td><button value=\"Go\" action=\"bypass -h admin_cw_goto ");
						replyMSG.Append(itemId);
						replyMSG.Append("\" width=73 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr>");
					}
					else
					{
						replyMSG.Append("<tr><td>Position:</td><td>Doesn't exist.</td></tr><tr><td><button value=\"Give to Target\" action=\"bypass -h admin_cw_add ");
						replyMSG.Append(itemId);
						replyMSG.Append("\" width=130 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td><td></td></tr>");
					}
					
					replyMSG.Append("</table><br>");
				}
				
				htmlContent.Replace("%cwinfo%", replyMSG.ToString());
				activeChar.sendPacket(adminReply);
			}
		}
		else if (command.startsWith("admin_cw_reload"))
		{
			cwm.load();
		}
		else
		{
			CursedWeapon cw = null;
			try
			{
				string parameter = st.nextToken();
				if (Regex.IsMatch(parameter, "[0-9]+"))
				{
					id = int.Parse(parameter);
				}
				else
				{
					parameter = parameter.Replace('_', ' ');
					foreach (CursedWeapon cwp in cwm.getCursedWeapons())
					{
						if (cwp.getName().toLowerCase().contains(parameter.toLowerCase()))
						{
							id = cwp.getItemId();
							break;
						}
					}
				}
				cw = cwm.getCursedWeapon(id);
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Usage: //cw_remove|//cw_goto|//cw_add <itemid|name>");
			}
			
			if (cw == null)
			{
				BuilderUtil.sendSysMessage(activeChar, "Unknown cursed weapon ID.");
				return false;
			}
			
			if (command.startsWith("admin_cw_remove "))
			{
				cw.endOfLife();
			}
			else if (command.startsWith("admin_cw_goto "))
			{
				cw.goTo(activeChar);
			}
			else if (command.startsWith("admin_cw_add"))
			{
				if (cw.isActive())
				{
					BuilderUtil.sendSysMessage(activeChar, "This cursed weapon is already active.");
				}
				else
				{
					WorldObject target = activeChar.getTarget();
					if ((target != null) && target.isPlayer())
					{
						((Player) target).addItem("AdminCursedWeaponAdd", id, 1, target, true);
					}
					else
					{
						activeChar.addItem("AdminCursedWeaponAdd", id, 1, activeChar, true);
					}
					cw.setEndTime(DateTime.UtcNow.AddMilliseconds(cw.getDuration() * 60000));
					cw.reActivate();
				}
			}
			else
			{
				BuilderUtil.sendSysMessage(activeChar, "Unknown command.");
			}
		}
		return true;
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
