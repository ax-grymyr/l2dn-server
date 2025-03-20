using System.Globalization;
using System.Text;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author UnAfraid, Zoey76
 */
public class AdminClan: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_clan_info",
		"admin_clan_changeleader",
		"admin_clan_show_pending",
		"admin_clan_force_pending",
    ];

	public bool useAdminCommand(string command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command);
		string cmd = st.nextToken();
		switch (cmd)
		{
			case "admin_clan_info":
			{
				Player? player = getPlayer(activeChar, st);
				if (player == null)
				{
					break;
				}

				Clan? clan = player.getClan();
				if (clan == null)
				{
					activeChar.sendPacket(SystemMessageId.THE_TARGET_MUST_BE_A_CLAN_MEMBER);
					return false;
				}

                Castle? castle = CastleManager.getInstance().getCastleById(clan.getCastleId() ?? 0);
                ClanHall? clanHall = ClanHallData.getInstance().getClanHallById(clan.getHideoutId());
                Fort? fort = FortManager.getInstance().getFortById(clan.getFortId() ?? 0);

				HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/claninfo.htm", activeChar);
				NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);
				htmlContent.Replace("%clan_name%", clan.getName());
				htmlContent.Replace("%clan_leader%", clan.getLeaderName());
				htmlContent.Replace("%clan_level%", clan.getLevel().ToString());
				htmlContent.Replace("%clan_has_castle%", castle != null ? castle.getName() : "No");
				htmlContent.Replace("%clan_has_clanhall%", clanHall != null ? clanHall.getName() : "No");
				htmlContent.Replace("%clan_has_fortress%", fort != null ? fort.getName() : "No");
				htmlContent.Replace("%clan_points%", clan.getReputationScore().ToString());
				htmlContent.Replace("%clan_players_count%", clan.getMembersCount().ToString());
				htmlContent.Replace("%clan_ally%", clan.getAllyId() > 0 ? clan.getAllyName() : "Not in ally");
				htmlContent.Replace("%current_player_objectId%", player.ObjectId.ToString());
				htmlContent.Replace("%current_player_name%", player.getName());
				activeChar.sendPacket(html);
				break;
			}
			case "admin_clan_changeleader":
			{
				Player? player = getPlayer(activeChar, st);
				if (player == null)
				{
					break;
				}

				Clan? clan = player.getClan();
				if (clan == null)
				{
					activeChar.sendPacket(SystemMessageId.THE_TARGET_MUST_BE_A_CLAN_MEMBER);
					return false;
				}

				ClanMember? member = clan.getClanMember(player.ObjectId);
				if (member != null)
				{
					if (player.isAcademyMember())
					{
						player.sendPacket(SystemMessageId.THAT_PRIVILEGE_CANNOT_BE_GRANTED_TO_A_CLAN_ACADEMY_MEMBER);
					}
					else
					{
						clan.setNewLeader(member);
					}
				}
				break;
			}
			case "admin_clan_show_pending":
			{
				HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/clanchanges.htm", activeChar);
				NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);
				StringBuilder sb = new StringBuilder();
				foreach (Clan clan in ClanTable.getInstance().getClans())
				{
					if (clan.getNewLeaderId() != 0)
					{
						sb.Append("<tr>");
						sb.Append("<td>" + clan.getName() + "</td>");
						sb.Append("<td>" + clan.getNewLeaderName() + "</td>");
						sb.Append("<td><a action=\"bypass -h admin_clan_force_pending " + clan.Id + "\">Force</a></td>");
						sb.Append("</tr>");
					}
				}
				htmlContent.Replace("%data%", sb.ToString());
				activeChar.sendPacket(html);
				break;
			}
			case "admin_clan_force_pending":
			{
				if (st.hasMoreElements())
				{
					string token = st.nextToken();
					if (!int.TryParse(token, CultureInfo.InvariantCulture, out int clanId))
					{
						break;
					}
					Clan? clan = ClanTable.getInstance().getClan(clanId);
					if (clan == null)
					{
						break;
					}

					ClanMember? member = clan.getClanMember(clan.getNewLeaderId() ?? 0);
					if (member == null)
					{
						break;
					}

					clan.setNewLeader(member);
					BuilderUtil.sendSysMessage(activeChar, "Task have been forcely executed.");
				}

				break;
			}
		}
		return true;
	}

	/**
	 * @param activeChar
	 * @param st
	 * @return
	 */
	private Player? getPlayer(Player activeChar, StringTokenizer st)
	{
		string val;
		Player? player;
		if (st.hasMoreTokens())
		{
			val = st.nextToken();
			// From the HTML we receive player's object Id.
			if (int.TryParse(val, CultureInfo.InvariantCulture, out int value))
			{
				player = World.getInstance().getPlayer(value);
				if (player == null)
				{
					activeChar.sendPacket(SystemMessageId.THAT_PLAYER_IS_NOT_ONLINE);
					return null;
				}
			}
			else
			{
				player = World.getInstance().getPlayer(val);
				if (player == null)
				{
					activeChar.sendPacket(SystemMessageId.INCORRECT_NAME_PLEASE_TRY_AGAIN);
					return null;
				}
			}
		}
		else
		{
			WorldObject? targetObj = activeChar.getTarget();
			if (targetObj == null || !targetObj.isPlayer())
			{
				activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				return null;
			}
			player = targetObj.getActingPlayer();
		}
		return player;
	}

	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}