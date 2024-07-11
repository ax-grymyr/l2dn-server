using System.Text;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

public class AdminInstanceZone: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_instancezone",
		"admin_instancezone_clear"
	};
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		string target = (activeChar.getTarget() != null) ? activeChar.getTarget().getName() : "no-target";
		// GMAudit.auditGMAction(activeChar.getName(), command, target, ""); // TODO
		if (command.startsWith("admin_instancezone_clear"))
		{
			try
			{
				StringTokenizer st = new StringTokenizer(command, " ");
				st.nextToken();
				Player player = World.getInstance().getPlayer(st.nextToken());
				int instanceId = int.Parse(st.nextToken());
				string name = InstanceManager.getInstance().getInstanceName(instanceId);
				InstanceManager.getInstance().deleteInstanceTime(player, instanceId);
				BuilderUtil.sendSysMessage(activeChar, "Instance zone " + name + " cleared for player " + player.getName());
				player.sendMessage("Admin cleared instance zone " + name + " for you");
				display(activeChar, activeChar); // for refreshing instance window
				return true;
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Failed clearing instance time: " + e);
				BuilderUtil.sendSysMessage(activeChar, "Usage: //instancezone_clear <playername> [instanceId]");
				return false;
			}
		}
		else if (command.startsWith("admin_instancezone"))
		{
			StringTokenizer st = new StringTokenizer(command, " ");
			st.nextToken();
			
			if (st.hasMoreTokens())
			{
				Player player = null;
				string playername = st.nextToken();
				
				try
				{
					player = World.getInstance().getPlayer(playername);
				}
				catch (Exception e)
				{
				}
				
				if (player != null)
				{
					display(player, activeChar);
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "The player " + playername + " is not online");
					BuilderUtil.sendSysMessage(activeChar, "Usage: //instancezone [playername]");
					return false;
				}
			}
			else if (activeChar.getTarget() != null)
			{
				if (activeChar.getTarget().isPlayer())
				{
					display((Player) activeChar.getTarget(), activeChar);
				}
			}
			else
			{
				display(activeChar, activeChar);
			}
		}
		return true;
	}
	
	private void display(Player player, Player activeChar)
	{
		Map<int, DateTime> instanceTimes = InstanceManager.getInstance().getAllInstanceTimes(player);
		StringBuilder html = new StringBuilder(500 + (instanceTimes.size() * 200));
		html.Append("<html><center><table width=260><tr><td width=40><button value=\"Main\" action=\"bypass admin_admin\" width=40 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td><td width=180><center>Character Instances</center></td><td width=40><button value=\"Back\" action=\"bypass -h admin_current_player\" width=40 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr></table><br><font color=\"LEVEL\">Instances for " + player.getName() + "</font><center><br><table><tr><td width=150>Name</td><td width=50>Time</td><td width=70>Action</td></tr>");
		foreach (var entry in instanceTimes)
		{
			int hours = 0;
			int minutes = 0;
			int id = entry.Key;
			TimeSpan remainingTime = entry.Value - DateTime.UtcNow;
			if (remainingTime > TimeSpan.Zero)
			{
				hours = remainingTime.Hours;
				minutes = remainingTime.Minutes;
			}
			
			html.Append("<tr><td>" + InstanceManager.getInstance().getInstanceName(id) + "</td><td>" + hours + ":" + minutes + "</td><td><button value=\"Clear\" action=\"bypass -h admin_instancezone_clear " + player.getName() + " " + id + "\" width=60 height=15 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr>");
		}
		
		html.Append("</table></html>");
		
		HtmlContent htmlContent = HtmlContent.LoadFromText(html.ToString(), activeChar);
		NpcHtmlMessagePacket ms = new NpcHtmlMessagePacket(null, 1, htmlContent);
		activeChar.sendPacket(ms);
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}