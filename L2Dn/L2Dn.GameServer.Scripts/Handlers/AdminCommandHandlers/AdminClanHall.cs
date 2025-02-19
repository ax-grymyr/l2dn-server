using System.Text;
using L2Dn.Extensions;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Html.Formatters;
using L2Dn.GameServer.Model.Html.PageHandlers;
using L2Dn.GameServer.Model.Html.Styles;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * Clan Hall admin commands.
 * @author St3eT
 */
public class AdminClanHall: IAdminCommandHandler
{
    private static readonly string[] ADMIN_COMMANDS = ["admin_clanhall"];

	public bool useAdminCommand(string command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command, " ");
		string actualCommand = st.nextToken();
		if (actualCommand.equalsIgnoreCase("admin_clanhall"))
		{
			ProcessBypass(activeChar, new BypassParser(command));
		}

		return true;
	}

	private void DoAction(Player player, int clanHallId, string action, string actionVal)
	{
		ClanHall? clanHall = ClanHallData.getInstance().getClanHallById(clanHallId);
		if (clanHall != null)
		{
			switch (action)
			{
				case "openCloseDoors":
				{
					if (actionVal != null)
					{
						clanHall.openCloseDoors(bool.Parse(actionVal));
					}
					break;
				}
				case "teleport":
				{
					if (actionVal != null)
					{
						switch (actionVal)
						{
							case "inside":
							{
								player.teleToLocation(new Location(clanHall.getOwnerLocation(), 0));
								break;
							}
							case "outside":
							{
								player.teleToLocation(new Location(clanHall.getBanishLocation(), 0));
								break;
							}
						}
					}
					break;
				}
				case "give":
                {
                    WorldObject? playerTarget = player.getTarget();
					if (playerTarget != null && playerTarget.getActingPlayer() != null)
					{
						Clan? targetClan = playerTarget.getActingPlayer()?.getClan();
						if (targetClan == null || targetClan.getHideoutId() != 0)
						{
							player.sendPacket(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET);
						}

						clanHall.setOwner(targetClan);
					}
					else
					{
						player.sendPacket(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET);
					}
					break;
				}
				case "take":
				{
					Clan clan = clanHall.getOwner();
					if (clan != null)
					{
						clanHall.setOwner(null);
					}
					else
					{
						player.sendMessage("You cannot take Clan Hall which don't have any owner.");
					}
					break;
				}
				case "cancelFunc":
				{
					ResidenceFunction? function = clanHall.getFunction(int.Parse(actionVal));
					if (function != null)
					{
						clanHall.removeFunction(function);
						SendClanHallDetails(player, clanHallId);
					}
					break;
				}
			}
		}
		else
		{
			player.sendMessage("Clan Hall with id " + clanHallId + " does not exist!");
		}
		useAdminCommand("admin_clanhall id=" + clanHallId, player);
	}

	private void SendClanHallList(Player player, int page)
	{
		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/clanhall_list.htm", player);
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);
		List<ClanHall> clanHallList = ClanHallData.getInstance().getClanHalls().OrderBy(x => x.getResidenceId()).ToList();

		//@formatter:off
		PageResult result = PageBuilder.newBuilder(clanHallList, 4, "bypass -h admin_clanhall")
			.currentPage(page)
			.pageHandler(NextPrevPageHandler.INSTANCE)
			.formatter(BypassParserFormatter.INSTANCE)
			.style(ButtonsStyle.INSTANCE)
			.bodyHandler((pages, clanHall, sb) =>
		{
			sb.Append("<table border=0 cellpadding=0 cellspacing=0 bgcolor=\"363636\">");
			sb.Append("<tr><td align=center fixwidth=\"250\"><font color=\"LEVEL\">&%" + clanHall.getResidenceId() + "; (" + clanHall.getResidenceId() + ")</font></td></tr>");
			sb.Append("</table>");

			sb.Append("<table border=0 cellpadding=0 cellspacing=0 bgcolor=\"363636\">");
			sb.Append("<tr>");
			sb.Append("<td align=center fixwidth=\"83\">Status:</td>");
			sb.Append("<td align=center fixwidth=\"83\"></td>");
			sb.Append("<td align=center fixwidth=\"83\">" + (clanHall.getOwner() == null ? "<font color=\"00FF00\">Free</font>" : "<font color=\"FF9900\">Owned</font>") + "</td>");
			sb.Append("</tr>");

			sb.Append("<tr>");
			sb.Append("<td align=center fixwidth=\"83\">Location:</td>");
			sb.Append("<td align=center fixwidth=\"83\"></td>");
			sb.Append("<td align=center fixwidth=\"83\">&^" + clanHall.getResidenceId() + ";</td>");
			sb.Append("</tr>");

			sb.Append("<tr>");
			sb.Append("<td align=center fixwidth=\"83\">Detailed Info:</td>");
			sb.Append("<td align=center fixwidth=\"83\"></td>");
			sb.Append("<td align=center fixwidth=\"83\"><button value=\"Show me!\" action=\"bypass -h admin_clanhall id=" + clanHall.getResidenceId() + "\" width=\"85\" height=\"20\" back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td>");
			sb.Append("</tr>");


			sb.Append("</table>");
			sb.Append("<br>");
		}).build();
		//@formatter:on

		htmlContent.Replace("%pages%", result.getPages() > 0 ? "<center><table width=\"100%\" cellspacing=0><tr>" + result.getPagerTemplate() + "</tr></table></center>" : "");
		htmlContent.Replace("%data%", result.getBodyTemplate().ToString());
		player.sendPacket(html);
	}

	private void SendClanHallDetails(Player player, int clanHallId)
	{
		ClanHall? clanHall = ClanHallData.getInstance().getClanHallById(clanHallId);
		if (clanHall != null)
		{
			HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/clanhall_detail.htm", player);
			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);
			StringBuilder sb = new StringBuilder();
			htmlContent.Replace("%clanHallId%", clanHall.getResidenceId().ToString());
			htmlContent.Replace("%clanHallOwner%", clanHall.getOwner() == null ? "<font color=\"00FF00\">Free</font>" : "<font color=\"FF9900\">" + clanHall.getOwner().getName() + "</font>");
			string grade = clanHall.getGrade().ToString().Replace("GRADE_", "") + " Grade";
			htmlContent.Replace("%clanHallGrade%", grade);
			htmlContent.Replace("%clanHallSize%", clanHall.getGrade().ToString());
			if (clanHall.getFunctions().Count != 0)
			{
				sb.Append("<table border=0 cellpadding=0 cellspacing=0 bgcolor=\"363636\">");
				sb.Append("<tr>");
				sb.Append("<td align=center fixwidth=\"40\"><font color=\"LEVEL\">ID</font></td>");
				sb.Append("<td align=center fixwidth=\"200\"><font color=\"LEVEL\">Type</font></td>");
				sb.Append("<td align=center fixwidth=\"40\"><font color=\"LEVEL\">Lvl</font></td>");
				sb.Append("<td align=center fixwidth=\"200\"><font color=\"LEVEL\">End date</font></td>");
				sb.Append("<td align=center fixwidth=\"100\"><font color=\"LEVEL\">Action</font></td>");
				sb.Append("</tr>");
				sb.Append("</table>");
				sb.Append("<table border=0 cellpadding=0 cellspacing=0 bgcolor=\"363636\">");
				clanHall.getFunctions().ForEach(function =>
				{
					sb.Append("<tr>");
					sb.Append("<td align=center fixwidth=\"40\">" + function.getId() + "</td>");
					sb.Append("<td align=center fixwidth=\"200\">" + function.getType().ToString() + "</td>");
					sb.Append("<td align=center fixwidth=\"40\">" + function.getLevel() + "</td>");
					sb.Append("<td align=center fixwidth=\"200\">" + function.getExpiration().ToString("dd/MM HH:mm") + "</td>");
					sb.Append("<td align=center fixwidth=\"100\"><button value=\"Remove\" action=\"bypass -h admin_clanhall id=" + clanHallId + " action=cancelFunc actionVal=" + function.getId() + "\" width=50 height=21 back=\"L2UI_CT1.Button_DF_Down\" fore=\"L2UI_CT1.Button_DF\"></td>");
					sb.Append("</tr>");
				});
				sb.Append("</table>");
			}
			else
			{
				sb.Append("This Clan Hall doesn't have any Function yet.");
			}

			htmlContent.Replace("%functionList%", sb.ToString());
			player.sendPacket(html);
		}
		else
		{
			player.sendMessage("Clan Hall with id " + clanHallId + " does not exist!");
			useAdminCommand("admin_clanhall", player);
		}
	}

	private void ProcessBypass(Player player, BypassParser parser)
	{
		int page = parser.getInt("page", 0);
		int clanHallId = parser.getInt("id", 0);
		string action = parser.getString("action", string.Empty);
		string actionVal = parser.getString("actionVal", string.Empty);
		if (clanHallId > 0 && !string.IsNullOrEmpty(action))
		{
			DoAction(player, clanHallId, action, actionVal);
		}
		else if (clanHallId > 0)
		{
			SendClanHallDetails(player, clanHallId);
		}
		else
		{
			SendClanHallList(player, page);
		}
	}

	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}