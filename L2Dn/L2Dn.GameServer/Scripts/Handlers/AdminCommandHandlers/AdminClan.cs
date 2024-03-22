using System.Text;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
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
	{
		"admin_clan_info",
		"admin_clan_changeleader",
		"admin_clan_show_pending",
		"admin_clan_force_pending"
	};
	
	public bool useAdminCommand(String command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command);
		String cmd = st.nextToken();
		switch (cmd)
		{
			case "admin_clan_info":
			{
				Player player = getPlayer(activeChar, st);
				if (player == null)
				{
					break;
				}
				
				Clan clan = player.getClan();
				if (clan == null)
				{
					activeChar.sendPacket(SystemMessageId.THE_TARGET_MUST_BE_A_CLAN_MEMBER);
					return false;
				}

				HtmlPacketHelper helper = new HtmlPacketHelper(HtmCache.getInstance().getHtm(activeChar, "html/admin/claninfo.htm"));
				NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(0, 1, helper);
				helper.Replace("%clan_name%", clan.getName());
				helper.Replace("%clan_leader%", clan.getLeaderName());
				helper.Replace("%clan_level%", clan.getLevel().ToString());
				helper.Replace("%clan_has_castle%", clan.getCastleId() > 0 ? CastleManager.getInstance().getCastleById(clan.getCastleId() ?? 0).getName() : "No");
				helper.Replace("%clan_has_clanhall%", clan.getHideoutId() > 0 ? ClanHallData.getInstance().getClanHallById(clan.getHideoutId()).getName() : "No");
				helper.Replace("%clan_has_fortress%", clan.getFortId() > 0 ? FortManager.getInstance().getFortById(clan.getFortId() ?? 0).getName() : "No");
				helper.Replace("%clan_points%", clan.getReputationScore().ToString());
				helper.Replace("%clan_players_count%", clan.getMembersCount().ToString());
				helper.Replace("%clan_ally%", clan.getAllyId() > 0 ? clan.getAllyName() : "Not in ally");
				helper.Replace("%current_player_objectId%", player.getObjectId().ToString());
				helper.Replace("%current_player_name%", player.getName());
				activeChar.sendPacket(html);
				break;
			}
			case "admin_clan_changeleader":
			{
				Player player = getPlayer(activeChar, st);
				if (player == null)
				{
					break;
				}
				
				Clan clan = player.getClan();
				if (clan == null)
				{
					activeChar.sendPacket(SystemMessageId.THE_TARGET_MUST_BE_A_CLAN_MEMBER);
					return false;
				}
				
				ClanMember member = clan.getClanMember(player.getObjectId());
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
				HtmlPacketHelper helper = new HtmlPacketHelper(HtmCache.getInstance().getHtm(activeChar, "html/admin/clanchanges.htm"));
				NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(0, 1, helper);
				StringBuilder sb = new StringBuilder();
				foreach (Clan clan in ClanTable.getInstance().getClans())
				{
					if (clan.getNewLeaderId() != 0)
					{
						sb.Append("<tr>");
						sb.Append("<td>" + clan.getName() + "</td>");
						sb.Append("<td>" + clan.getNewLeaderName() + "</td>");
						sb.Append("<td><a action=\"bypass -h admin_clan_force_pending " + clan.getId() + "\">Force</a></td>");
						sb.Append("</tr>");
					}
				}
				helper.Replace("%data%", sb.ToString());
				activeChar.sendPacket(html);
				break;
			}
			case "admin_clan_force_pending":
			{
				if (st.hasMoreElements())
				{
					String token = st.nextToken();
					if (!Util.isDigit(token))
					{
						break;
					}
					int clanId = int.Parse(token);
					Clan clan = ClanTable.getInstance().getClan(clanId);
					if (clan == null)
					{
						break;
					}
					
					ClanMember member = clan.getClanMember(clan.getNewLeaderId() ?? 0);
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
	private Player getPlayer(Player activeChar, StringTokenizer st)
	{
		String val;
		Player player = null;
		if (st.hasMoreTokens())
		{
			val = st.nextToken();
			// From the HTML we receive player's object Id.
			if (Util.isDigit(val))
			{
				player = World.getInstance().getPlayer(int.Parse(val));
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
			WorldObject targetObj = activeChar.getTarget();
			if ((targetObj == null) || !targetObj.isPlayer())
			{
				activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				return null;
			}
			player = targetObj.getActingPlayer();
		}
		return player;
	}
	
	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
