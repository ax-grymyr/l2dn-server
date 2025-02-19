using System.Text;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.CommunityBoard;

/**
 * Clan board.
 * @author Zoey76
 */
public class ClanBoard: IWriteBoardHandler
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(ClanBoard));

	private static readonly string[] COMMANDS =
    [
        "_bbsclan",
		"_bbsclan_list",
		"_bbsclan_clanhome",
    ];

	public string[] getCommunityBoardCommands()
	{
		return COMMANDS;
	}

	public bool parseCommunityBoardCommand(string command, Player player)
	{
		if (command.equals("_bbsclan"))
        {
            Clan? clan = player.getClan();
			CommunityBoardHandler.getInstance().addBypass(player, "Clan", command);
			if (clan == null || clan.getLevel() < 2)
			{
				clanList(player, 1);
			}
			else
			{
				clanHome(player);
			}
		}
		else if (command.startsWith("_bbsclan_clanlist"))
		{
			CommunityBoardHandler.getInstance().addBypass(player, "Clan List", command);
			if (command.equals("_bbsclan_clanlist"))
			{
				clanList(player, 1);
			}
			else if (command.startsWith("_bbsclan_clanlist;"))
			{
				try
				{
					clanList(player, int.Parse(command.Split(";")[1]));
				}
				catch (Exception e)
				{
					clanList(player, 1);
					_logger.Warn(nameof(ClanBoard) + ": " + player + " send invalid clan list bypass " + command + "! " + e);
				}
			}
		}
		else if (command.startsWith("_bbsclan_clanhome"))
		{
			CommunityBoardHandler.getInstance().addBypass(player, "Clan Home", command);
			if (command.equals("_bbsclan_clanhome"))
			{
				clanHome(player);
			}
			else if (command.startsWith("_bbsclan_clanhome;"))
			{
				try
				{
					clanHome(player, int.Parse(command.Split(";")[1]));
				}
				catch (Exception e)
				{
					clanHome(player);
					_logger.Warn(nameof(ClanBoard) + ": " + player + " send invalid clan home bypass " + command + "! " + e);
				}
			}
		}
		else if (command.startsWith("_bbsclan_clannotice_edit;"))
		{
			CommunityBoardHandler.getInstance().addBypass(player, "Clan Edit", command);
			clanNotice(player, player.getClanId());
		}
		else if (command.startsWith("_bbsclan_clannotice_enable"))
		{
			CommunityBoardHandler.getInstance().addBypass(player, "Clan Notice Enable", command);
            Clan? clan = player.getClan();
			if (clan != null)
			{
				clan.setNoticeEnabled(true);
			}
			clanNotice(player, player.getClanId());
		}
		else if (command.startsWith("_bbsclan_clannotice_disable"))
		{
			CommunityBoardHandler.getInstance().addBypass(player, "Clan Notice Disable", command);
            Clan? clan = player.getClan();
			if (clan != null)
			{
				clan.setNoticeEnabled(false);
			}
			clanNotice(player, player.getClanId());
		}
		else
		{
			CommunityBoardHandler.separateAndSend("<html><body><br><br><center>Command " + command + " need development.</center><br><br></body></html>", player);
		}
		return true;
	}

	private void clanNotice(Player player, int? clanId)
	{
        if (clanId is null)
            return;

		Clan? cl = ClanTable.getInstance().getClan(clanId.Value);
        if (cl == null)
            return;

        if (cl.getLevel() < 2)
        {
            player.sendPacket(SystemMessageId.THERE_ARE_NO_COMMUNITIES_IN_MY_CLAN_CLAN_COMMUNITIES_ARE_ALLOWED_FOR_CLANS_WITH_SKILL_LEVELS_OF_2_AND_HIGHER);
            parseCommunityBoardCommand("_bbsclan_clanlist", player);
        }
        else
        {
            Clan? clan = player.getClan(); // TODO: mess with player clan and clanId
            StringBuilder html = new StringBuilder(2048);
            html.Append("<html><body><br><br><table border=0 width=610><tr><td width=10></td><td width=600 align=left><a action=\"bypass _bbshome\">HOME</a> &gt; <a action=\"bypass _bbsclan_clanlist\"> CLAN COMMUNITY </a>  &gt; <a action=\"bypass _bbsclan_clanhome;");
            html.Append(clanId);
            html.Append("\"> &amp;$802; </a></td></tr></table>");
            if (player.isClanLeader() && clan != null)
            {
                html.Append("<br><br><center><table width=610 border=0 cellspacing=0 cellpadding=0><tr><td fixwidth=610><font color=\"AAAAAA\">The Clan Notice function allows the clan leader to send messages through a pop-up window to clan members at login.</font> </td></tr><tr><td height=20></td></tr>");
                if (clan.isNoticeEnabled())
                {
                    html.Append("<tr><td fixwidth=610> Clan Notice Function:&nbsp;&nbsp;&nbsp;on&nbsp;&nbsp;&nbsp;/&nbsp;&nbsp;&nbsp;<a action=\"bypass _bbsclan_clannotice_disable\">off</a>");
                }
                else
                {
                    html.Append("<tr><td fixwidth=610> Clan Notice Function:&nbsp;&nbsp;&nbsp;<a action=\"bypass _bbsclan_clannotice_enable\">on</a>&nbsp;&nbsp;&nbsp;/&nbsp;&nbsp;&nbsp;off");
                }

                html.Append("</td></tr></table><img src=\"L2UI.Squaregray\" width=\"610\" height=\"1\"><br> <br><table width=610 border=0 cellspacing=2 cellpadding=0><tr><td>Edit Notice: </td></tr><tr><td height=5></td></tr><tr><td><MultiEdit var =\"Content\" width=610 height=100></td></tr></table><br><table width=610 border=0 cellspacing=0 cellpadding=0><tr><td height=5></td></tr><tr><td align=center FIXWIDTH=65><button value=\"&$140;\" action=\"Write Notice Set _ Content Content Content\" back=\"l2ui_ch3.smallbutton2_down\" width=65 height=20 fore=\"l2ui_ch3.smallbutton2\" ></td><td align=center FIXWIDTH=45></td><td align=center FIXWIDTH=500></td></tr></table></center></body></html>");
                Util.sendCBHtml(player, html.ToString(), clan.getNotice());
            }
            else if (clan != null)
            {
                html.Append("<img src=\"L2UI.squareblank\" width=\"1\" height=\"10\"><center><table border=0 cellspacing=0 cellpadding=0><tr><td>You are not your clan's leader, and therefore cannot change the clan notice</td></tr></table>");
                if (clan.isNoticeEnabled())
                {
                    html.Append("<table border=0 cellspacing=0 cellpadding=0><tr><td>The current clan notice:</td></tr><tr><td fixwidth=5></td><td FIXWIDTH=600 align=left>" + clan.getNotice() + "</td><td fixqqwidth=5></td></tr></table>");
                }
                html.Append("</center></body></html>");
                CommunityBoardHandler.separateAndSend(html.ToString(), player);
            }
        }
    }

	private void clanList(Player player, int indexValue)
	{
		int index = indexValue;
		if (index < 1)
		{
			index = 1;
		}

        Clan? clan = player.getClan();

		// header
		StringBuilder html = new StringBuilder(2048);
		html.Append("<html><body><br><br><center><br1><br1><table border=0 cellspacing=0 cellpadding=0><tr><td FIXWIDTH=15>&nbsp;</td><td width=610 height=30 align=left><a action=\"bypass _bbsclan_clanlist\"> CLAN COMMUNITY </a></td></tr></table><table border=0 cellspacing=0 cellpadding=0 width=610 bgcolor=434343><tr><td height=10></td></tr><tr><td fixWIDTH=5></td><td fixWIDTH=600><a action=\"bypass _bbsclan_clanhome;");
		html.Append(clan != null ? clan.getId() : 0);
		html.Append("\">[GO TO MY CLAN]</a>&nbsp;&nbsp;</td><td fixWIDTH=5></td></tr><tr><td height=10></td></tr></table><br><table border=0 cellspacing=0 cellpadding=2 bgcolor=5A5A5A width=610><tr><td FIXWIDTH=5></td><td FIXWIDTH=200 align=center>CLAN NAME</td><td FIXWIDTH=200 align=center>CLAN LEADER</td><td FIXWIDTH=100 align=center>CLAN LEVEL</td><td FIXWIDTH=100 align=center>CLAN MEMBERS</td><td FIXWIDTH=5></td></tr></table><img src=\"L2UI.Squareblank\" width=\"1\" height=\"5\">");
		int i = 0;
		foreach (Clan cl in ClanTable.getInstance().getClans())
		{
			if (i > (index + 1) * 7)
			{
				break;
			}

			if (i++ >= (index - 1) * 7)
			{
				html.Append("<img src=\"L2UI.SquareBlank\" width=\"610\" height=\"3\"><table border=0 cellspacing=0 cellpadding=0 width=610><tr> <td FIXWIDTH=5></td><td FIXWIDTH=200 align=center><a action=\"bypass _bbsclan_clanhome;");
				html.Append(cl.getId());
				html.Append("\">");
				html.Append(cl.getName());
				html.Append("</a></td><td FIXWIDTH=200 align=center>");
				html.Append(cl.getLeaderName());
				html.Append("</td><td FIXWIDTH=100 align=center>");
				html.Append(cl.getLevel());
				html.Append("</td><td FIXWIDTH=100 align=center>");
				html.Append(cl.getMembersCount());
				html.Append("</td><td FIXWIDTH=5></td></tr><tr><td height=5></td></tr></table><img src=\"L2UI.SquareBlank\" width=\"610\" height=\"3\"><img src=\"L2UI.SquareGray\" width=\"610\" height=\"1\">");
			}
		}

		html.Append("<img src=\"L2UI.SquareBlank\" width=\"610\" height=\"2\"><table cellpadding=0 cellspacing=2 border=0><tr>");
		if (index == 1)
		{
			html.Append("<td><button action=\"\" back=\"l2ui_ch3.prev1_down\" fore=\"l2ui_ch3.prev1\" width=16 height=16 ></td>");
		}
		else
		{
			html.Append("<td><button action=\"_bbsclan_clanlist;");
			html.Append(index - 1);
			html.Append("\" back=\"l2ui_ch3.prev1_down\" fore=\"l2ui_ch3.prev1\" width=16 height=16 ></td>");
		}

		int nbp = ClanTable.getInstance().getClanCount() / 8;
		if (nbp * 8 != ClanTable.getInstance().getClanCount())
		{
			nbp++;
		}
		for (i = 1; i <= nbp; i++)
		{
			if (i == index)
			{
				html.Append("<td> ");
				html.Append(i);
				html.Append(" </td>");
			}
			else
			{
				html.Append("<td><a action=\"bypass _bbsclan_clanlist;");
				html.Append(i);
				html.Append("\"> ");
				html.Append(i);
				html.Append(" </a></td>");
			}
		}
		if (index == nbp)
		{
			html.Append("<td><button action=\"\" back=\"l2ui_ch3.next1_down\" fore=\"l2ui_ch3.next1\" width=16 height=16 ></td>");
		}
		else
		{
			html.Append("<td><button action=\"bypass _bbsclan_clanlist;");
			html.Append(index + 1);
			html.Append("\" back=\"l2ui_ch3.next1_down\" fore=\"l2ui_ch3.next1\" width=16 height=16 ></td>");
		}
		html.Append("</tr></table><table border=0 cellspacing=0 cellpadding=0><tr><td width=610><img src=\"sek.cbui141\" width=\"610\" height=\"1\"></td></tr></table><table border=0><tr><td><combobox width=65 var=keyword list=\"Name;Ruler\"></td><td><edit var = \"Search\" width=130 height=11 length=\"16\"></td>" +
		// TODO: search (Write in BBS)
			"<td><button value=\"&$420;\" action=\"Write 5 -1 0 Search keyword keyword\" back=\"l2ui_ch3.smallbutton2_down\" width=65 height=20 fore=\"l2ui_ch3.smallbutton2\"> </td> </tr></table><br><br></center></body></html>");
		CommunityBoardHandler.separateAndSend(html.ToString(), player);
	}

	private void clanHome(Player player)
	{
		clanHome(player, player.getClan()?.getId() ?? 0);
	}

	private void clanHome(Player player, int clanId) // TODO: second argument must be Clan object
	{
		Clan? cl = ClanTable.getInstance().getClan(clanId);
		if (cl != null)
		{
			if (cl.getLevel() < 2)
			{
				player.sendPacket(SystemMessageId.THERE_ARE_NO_COMMUNITIES_IN_MY_CLAN_CLAN_COMMUNITIES_ARE_ALLOWED_FOR_CLANS_WITH_SKILL_LEVELS_OF_2_AND_HIGHER);
				parseCommunityBoardCommand("_bbsclan_clanlist", player);
			}
            else
            {
                string html = string.Concat(
                    "<html><body><center><br><br><br1><br1><table border=0 cellspacing=0 cellpadding=0><tr><td FIXWIDTH=15>&nbsp;</td><td width=610 height=30 align=left><a action=\"bypass _bbshome\">HOME</a> &gt; <a action=\"bypass _bbsclan_clanlist\"> CLAN COMMUNITY </a>  &gt; <a action=\"bypass _bbsclan_clanhome;",
                    clanId.ToString(),
                    "\"> &amp;$802; </a></td></tr></table><table border=0 cellspacing=0 cellpadding=0 width=610 bgcolor=434343><tr><td height=10></td></tr><tr><td fixWIDTH=5></td><td fixwidth=600><a action=\"bypass _bbsclan_clanhome;",
                    clanId.ToString(), ";announce\">[CLAN ANNOUNCEMENT]</a> <a action=\"bypass _bbsclan_clanhome;",
                    clanId.ToString(), ";cbb\">[CLAN BULLETIN BOARD]</a><a action=\"bypass _bbsclan_clanhome;",
                    clanId.ToString(),
                    ";cmail\">[CLAN MAIL]</a>&nbsp;&nbsp;<a action=\"bypass _bbsclan_clannotice_edit;",
                    clanId.ToString(),
                    ";cnotice\">[CLAN NOTICE]</a>&nbsp;&nbsp;</td><td fixWIDTH=5></td></tr><tr><td height=10></td></tr></table><table border=0 cellspacing=0 cellpadding=0 width=610><tr><td height=10></td></tr><tr><td fixWIDTH=5></td><td fixwidth=290 valign=top></td><td fixWIDTH=5></td><td fixWIDTH=5 align=center valign=top><img src=\"l2ui.squaregray\" width=2  height=128></td><td fixWIDTH=5></td><td fixwidth=295><table border=0 cellspacing=0 cellpadding=0 width=295><tr><td fixWIDTH=100 align=left>CLAN NAME</td><td fixWIDTH=195 align=left>",
                    cl.getName(),
                    "</td></tr><tr><td height=7></td></tr><tr><td fixWIDTH=100 align=left>CLAN LEVEL</td><td fixWIDTH=195 align=left height=16>",
                    cl.getLevel().ToString(),
                    "</td></tr><tr><td height=7></td></tr><tr><td fixWIDTH=100 align=left>CLAN MEMBERS</td><td fixWIDTH=195 align=left height=16>",
                    cl.getMembersCount().ToString(),
                    "</td></tr><tr><td height=7></td></tr><tr><td fixWIDTH=100 align=left>CLAN LEADER</td><td fixWIDTH=195 align=left height=16>",
                    cl.getLeaderName(), "</td></tr><tr><td height=7></td></tr>" +
                    // ADMINISTRATOR ??
                    /*
                     * html.Append("<tr>"); html.Append("<td fixWIDTH=100 align=left>ADMINISTRATOR</td>"); html.Append("<td fixWIDTH=195 align=left height=16>"+cl.getLeaderName()+"</td>"); html.Append("</tr>");
                     */
                    "<tr><td height=7></td></tr><tr><td fixWIDTH=100 align=left>ALLIANCE</td><td fixWIDTH=195 align=left height=16>",
                    cl.getAllyName() != null ? cl.getAllyName() : "",
                    "</td></tr></table></td><td fixWIDTH=5></td></tr><tr><td height=10></td></tr></table>" +
                    // TODO: the BB for clan :)
                    // html.Append("<table border=0 cellspacing=0 cellpadding=0 width=610 bgcolor=333333>");
                    "<img src=\"L2UI.squareblank\" width=\"1\" height=\"5\"><img src=\"L2UI.squaregray\" width=\"610\" height=\"1\"><br></center><br> <br></body></html>");
                CommunityBoardHandler.separateAndSend(html, player);
            }
        }
	}

	public bool writeCommunityBoardCommand(Player player, string arg1, string arg2, string arg3, string arg4, string arg5)
	{
		// the only Write bypass that comes to this handler is "Write Notice Set _ Content Content Content";
		// arg1 = Set, arg2 = _
		Clan? clan = player.getClan();
		if (clan != null && player.isClanLeader())
		{
			clan.setNotice(arg3);
		}
		return true;
	}
}