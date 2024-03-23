using System.Text;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Html.Formatters;
using L2Dn.GameServer.Model.Html.PageHandlers;
using L2Dn.GameServer.Model.Html.Styles;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * Instance admin commands.
 * @author St3eT
 */
public class AdminInstance: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_instance",
		"admin_instances",
		"admin_instancelist",
		"admin_instancecreate",
		"admin_instanceteleport",
		"admin_instancedestroy",
	};
	private static int[] IGNORED_TEMPLATES =
	{
		127, // Chamber of Delusion
		128, // Chamber of Delusion
		129, // Chamber of Delusion
		130, // Chamber of Delusion
		131, // Chamber of Delusion
		132, // Chamber of Delusion
		147, // Grassy Arena
		149, // Heros's Vestiges Arena
		150, // Orbis Arena
		148, // Three Bridges Arena
	};
	
	public bool useAdminCommand(String command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command, " ");
		String actualCommand = st.nextToken();
		
		switch (actualCommand.toLowerCase())
		{
			case "admin_instance":
			case "admin_instances":
			{
				HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/instances.htm", activeChar);
				NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);
				htmlContent.Replace("%instCount%", InstanceManager.getInstance().getInstances().Count.ToString());
				htmlContent.Replace("%tempCount%", InstanceManager.getInstance().getInstanceTemplates().Count.ToString());
				activeChar.sendPacket(html);
				break;
			}
			case "admin_instancelist":
			{
				processBypass(activeChar, new BypassParser(command));
				break;
			}
			case "admin_instancecreate":
			{
				int templateId = CommonUtil.parseNextInt(st, 0);
				InstanceTemplate template = InstanceManager.getInstance().getInstanceTemplate(templateId);
				if (template != null)
				{
					String enterGroup = st.hasMoreTokens() ? st.nextToken() : "Alone";
					List<Player> members = new();
					
					switch (enterGroup)
					{
						case "Alone":
						{
							members.add(activeChar);
							break;
						}
						case "Party":
						{
							if (activeChar.isInParty())
							{
								members.AddRange(activeChar.getParty().getMembers());
							}
							else
							{
								members.add(activeChar);
							}
							break;
						}
						case "CommandChannel":
						{
							if (activeChar.isInCommandChannel())
							{
								members.AddRange(activeChar.getParty().getCommandChannel().getMembers());
							}
							else if (activeChar.isInParty())
							{
								members.AddRange(activeChar.getParty().getMembers());
							}
							else
							{
								members.add(activeChar);
							}
							break;
						}
						default:
						{
							BuilderUtil.sendSysMessage(activeChar, "Wrong enter group usage! Please use those values: Alone, Party or CommandChannel.");
							return true;
						}
					}
					
					Instance instance = InstanceManager.getInstance().createInstance(template, activeChar);
					Location loc = instance.getEnterLocation();
					if (loc != null)
					{
						foreach (Player players in members)
						{
							instance.addAllowed(players);
							players.teleToLocation(loc, instance);
						}
					}
					sendTemplateDetails(activeChar, instance.getTemplateId());
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "Wrong parameters! Please try again.");
					return true;
				}
				break;
			}
			case "admin_instanceteleport":
			{
				Instance instance = InstanceManager.getInstance().getInstance(CommonUtil.parseNextInt(st, -1));
				if (instance != null)
				{
					Location loc = instance.getEnterLocation();
					if (loc != null)
					{
						if (!instance.isAllowed(activeChar))
						{
							instance.addAllowed(activeChar);
						}
						activeChar.teleToLocation(loc, false);
						activeChar.setInstance(instance);
						sendTemplateDetails(activeChar, instance.getTemplateId());
					}
				}
				break;
			}
			case "admin_instancedestroy":
			{
				Instance instance = InstanceManager.getInstance().getInstance(CommonUtil.parseNextInt(st, -1));
				if (instance != null)
				{
					instance.getPlayers().forEach(player => player.sendPacket(new ExShowScreenMessagePacket("Your instance has been destroyed by Game Master!", 10000)));
					BuilderUtil.sendSysMessage(activeChar, "You destroyed Instance " + instance.getId() + " with " + instance.getPlayersCount() + " players inside.");
					instance.destroy();
					sendTemplateDetails(activeChar, instance.getTemplateId());
				}
				break;
			}
		}
		return true;
	}
	
	private void sendTemplateDetails(Player player, int templateId)
	{
		if (InstanceManager.getInstance().getInstanceTemplate(templateId) != null)
		{
			InstanceTemplate template = InstanceManager.getInstance().getInstanceTemplate(templateId);

			HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/instances_detail.htm", player);
			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);
			StringBuilder sb = new StringBuilder();
			htmlContent.Replace("%templateId%", template.getId().ToString());
			htmlContent.Replace("%templateName%", template.getName());
			htmlContent.Replace("%activeWorlds%", template.getWorldCount() + " / " + (template.getMaxWorlds() == -1 ? "Unlimited" : template.getMaxWorlds()));
			htmlContent.Replace("%duration%", template.getDuration() + " minutes");
			htmlContent.Replace("%emptyDuration%", template.getEmptyDestroyTime().TotalMinutes + " minutes");
			htmlContent.Replace("%ejectDuration%", template.getEjectTime().TotalMinutes + " minutes");
			htmlContent.Replace("%removeBuff%", template.isRemoveBuffEnabled().ToString());
			sb.Append("<table border=0 cellpadding=2 cellspacing=0 bgcolor=\"363636\">");
			sb.Append("<tr>");
			sb.Append("<td fixwidth=\"83\"><font color=\"LEVEL\">Instance ID</font></td>");
			sb.Append("<td fixwidth=\"83\"><font color=\"LEVEL\">Teleport</font></td>");
			sb.Append("<td fixwidth=\"83\"><font color=\"LEVEL\">Destroy</font></td>");
			sb.Append("</tr>");
			sb.Append("</table>");
			
			InstanceManager.getInstance().getInstances().Where(inst => (inst.getTemplateId() == templateId)).OrderBy(x => x.getPlayersCount()).forEach(instance =>
			{
				sb.Append("<table border=0 cellpadding=2 cellspacing=0 bgcolor=\"363636\">");
				sb.Append("<tr>");
				sb.Append("<td fixwidth=\"83\">" + instance.getId() + "</td>");
				sb.Append("<td fixwidth=\"83\"><button value=\"Teleport!\" action=\"bypass -h admin_instanceteleport " + instance.getId() + "\" width=75 height=18 back=\"L2UI_CT1.Button_DF_Down\" fore=\"L2UI_CT1.Button_DF\"></td>");
				sb.Append("<td fixwidth=\"83\"><button value=\"Destroy!\" action=\"bypass -h admin_instancedestroy " + instance.getId() + "\" width=75 height=18 back=\"L2UI_CT1.Button_DF_Down\" fore=\"L2UI_CT1.Button_DF\"></td>");
				sb.Append("</tr>");
				sb.Append("</table>");
			});
			
			htmlContent.Replace("%instanceList%", sb.ToString());
			player.sendPacket(html);
		}
		else
		{
			player.sendMessage("Instance template with id " + templateId + " does not exist!");
			useAdminCommand("admin_instance", player);
		}
	}
	
	private void sendTemplateList(Player player, int page)
	{
		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/instances_list.htm", player);
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);
		
		InstanceManager instManager = InstanceManager.getInstance();
		List<InstanceTemplate> templateList = instManager.getInstanceTemplates()
			.OrderByDescending(x => x.getWorldCount())
			.Where(template => !CommonUtil.contains(IGNORED_TEMPLATES, template.getId())).ToList();
		
		//@formatter:off
		PageResult result = PageBuilder.newBuilder(templateList, 4, "bypass -h admin_instancelist")
			.currentPage(page)
			.pageHandler(NextPrevPageHandler.INSTANCE)
			.formatter(BypassParserFormatter.INSTANCE)
			.style(ButtonsStyle.INSTANCE)
			.bodyHandler((pages, template, sb) =>
		{
			sb.Append("<table border=0 cellpadding=0 cellspacing=0 bgcolor=\"363636\">");
			sb.Append("<tr><td align=center fixwidth=\"250\"><font color=\"LEVEL\">" + template.getName() + " (" + template.getId() + ")</font></td></tr>");
			sb.Append("</table>");

			sb.Append("<table border=0 cellpadding=0 cellspacing=0 bgcolor=\"363636\">");
			sb.Append("<tr>");
			sb.Append("<td align=center fixwidth=\"83\">Active worlds:</td>");
			sb.Append("<td align=center fixwidth=\"83\"></td>");
			sb.Append("<td align=center fixwidth=\"83\">" + template.getWorldCount() + " / " + (template.getMaxWorlds() == -1 ? "Unlimited" : template.getMaxWorlds()) + "</td>");
			sb.Append("</tr>");
			
			sb.Append("<tr>");
			sb.Append("<td align=center fixwidth=\"83\">Detailed info:</td>");
			sb.Append("<td align=center fixwidth=\"83\"></td>");
			sb.Append("<td align=center fixwidth=\"83\"><button value=\"Show me!\" action=\"bypass -h admin_instancelist id=" + template.getId() + "\" width=\"85\" height=\"20\" back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td>");
			sb.Append("</tr>");
			
			
			sb.Append("</table>");
			sb.Append("<br>");
		}).build();
		//@formatter:on
		
		htmlContent.Replace("%pages%", result.getPages() > 0 ? "<center><table width=\"100%\" cellspacing=0><tr>" + result.getPagerTemplate() + "</tr></table></center>" : "");
		htmlContent.Replace("%data%", result.getBodyTemplate().ToString());
		player.sendPacket(html);
	}
	
	private void processBypass(Player player, BypassParser parser)
	{
		int page = parser.getInt("page", 0);
		int templateId = parser.getInt("id", 0);
		if (templateId > 0)
		{
			sendTemplateDetails(player, templateId);
		}
		else
		{
			sendTemplateList(player, page);
		}
	}
	
	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}