using System.Text;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Quests;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

public class AdminEvents: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_event_menu",
		"admin_event_start",
		"admin_event_stop",
		"admin_event_start_menu",
		"admin_event_stop_menu",
		"admin_event_bypass"
	};
	
	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
	
	public bool useAdminCommand(String command, Player activeChar)
	{
		if (activeChar == null)
		{
			return false;
		}
		
		String eventName = "";
		String eventBypass = "";
		StringTokenizer st = new StringTokenizer(command, " ");
		st.nextToken();
		if (st.hasMoreTokens())
		{
			eventName = st.nextToken();
		}
		if (st.hasMoreTokens())
		{
			eventBypass = st.nextToken();
		}
		
		if (command.contains("_menu"))
		{
			showMenu(activeChar);
		}
		
		if (command.startsWith("admin_event_start"))
		{
			try
			{
				if (eventName != null)
				{
					Event ev = (Event)QuestManager.getInstance().getQuest(eventName);
					if (ev != null)
					{
						if (ev.eventStart(activeChar))
						{
							BuilderUtil.sendSysMessage(activeChar, "Event " + eventName + " started.");
							return true;
						}
						
						BuilderUtil.sendSysMessage(activeChar, "There is problem starting " + eventName + " event.");
						return true;
					}
				}
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Usage: //event_start <eventname>");
				return false;
			}
		}
		else if (command.startsWith("admin_event_stop"))
		{
			try
			{
				if (eventName != null)
				{
					Event ev = (Event) QuestManager.getInstance().getQuest(eventName);
					if (ev != null)
					{
						if (ev.eventStop())
						{
							BuilderUtil.sendSysMessage(activeChar, "Event " + eventName + " stopped.");
							return true;
						}
						
						BuilderUtil.sendSysMessage(activeChar, "There is problem with stoping " + eventName + " event.");
						return true;
					}
				}
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Usage: //event_start <eventname>");
				return false;
			}
		}
		else if (command.startsWith("admin_event_bypass"))
		{
			try
			{
				if (eventName != null)
				{
					Event ev = (Event) QuestManager.getInstance().getQuest(eventName);
					if (ev != null)
					{
						ev.eventBypass(activeChar, eventBypass);
					}
				}
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Usage: //event_bypass <eventname> <bypass>");
				return false;
			}
		}
		return false;
	}
	
	private void showMenu(Player activeChar)
	{
		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/gm_events.htm", activeChar);
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);
		StringBuilder cList = new StringBuilder(500);
		foreach (AbstractScript ev in ScriptManager.GetScripts())
		{
			if (ev is Event)
			{
				cList.Append("<tr><td><font color=\"LEVEL\">" + ev.Name +
				             ":</font></td><br><td><button value=\"Start\" action=\"bypass -h admin_event_start_menu " +
				             ev.Name +
				             "\" width=80 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td><td><button value=\"Stop\" action=\"bypass -h admin_event_stop_menu " +
				             ev.Name +
				             "\" width=80 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr>");
			}
		}

		htmlContent.Replace("%LIST%", cList.ToString());
		activeChar.sendPacket(html);
	}
}