using System.Globalization;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Announcements;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author UnAfraid
 */
public class AdminAnnouncements: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_announce",
		"admin_announce_crit",
		"admin_announce_screen",
		"admin_announces",
    ];

	public bool useAdminCommand(string command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command);
		string cmd = st.hasMoreTokens() ? st.nextToken() : "";
		switch (cmd)
		{
			case "admin_announce":
			case "admin_announce_crit":
			case "admin_announce_screen":
			{
				if (!st.hasMoreTokens())
				{
					BuilderUtil.sendSysMessage(activeChar, "Syntax: //announce <text to announce here>");
					return false;
				}
				string announce = st.nextToken();
				while (st.hasMoreTokens())
				{
					announce += " " + st.nextToken();
				}
				if (cmd.equals("admin_announce_screen"))
				{
					Broadcast.toAllOnlinePlayersOnScreen(announce);
				}
				else
				{
					if (Config.GM_ANNOUNCER_NAME)
					{
						announce = announce + " [" + activeChar.getName() + "]";
					}
					Broadcast.toAllOnlinePlayers(announce, cmd.equals("admin_announce_crit"));
				}
				AdminHtml.showAdminHtml(activeChar, "gm_menu.htm");
				break;
			}
			case "admin_announces":
			{
				string subCmd = st.hasMoreTokens() ? st.nextToken() : "";
				switch (subCmd)
				{
					case "add":
					{
						if (!st.hasMoreTokens())
						{
							string content2 = HtmCache.getInstance().getHtm("html/admin/announces-add.htm", activeChar.getLang());
							Util.sendCBHtml(activeChar, content2);
							break;
						}
						string annType = st.nextToken();
						AnnouncementType type = Enum.Parse<AnnouncementType>(annType);
						// ************************************
						if (!st.hasMoreTokens())
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces add <type> <delay> <repeat> <text>");
							break;
						}
						string annInitDelay = st.nextToken();
						if (!int.TryParse(annInitDelay, CultureInfo.InvariantCulture, out int initDelay))
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces add <type> <delay> <repeat> <text>");
							break;
						}
						initDelay *= 1000;
						// ************************************
						if (!st.hasMoreTokens())
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces add <type> <delay> <repeat> <text>");
							break;
						}
						string annDelay = st.nextToken();
						if (!int.TryParse(annDelay, CultureInfo.InvariantCulture, out int delay))
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces add <type> <delay> <repeat> <text>");
							break;
						}
						delay *= 1000;
						if (delay < 10 * 1000 && (type == AnnouncementType.AUTO_NORMAL || type == AnnouncementType.AUTO_CRITICAL))
						{
							BuilderUtil.sendSysMessage(activeChar, "Delay cannot be less then 10 seconds!");
							break;
						}
						// ************************************
						if (!st.hasMoreTokens())
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces add <type> <delay> <repeat> <text>");
							break;
						}
						string annRepeat = st.nextToken();
						if (!int.TryParse(annRepeat, CultureInfo.InvariantCulture, out int repeat))
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces add <type> <delay> <repeat> <text>");
							break;
						}
						if (repeat == 0)
						{
							repeat = -1;
						}
						// ************************************
						if (!st.hasMoreTokens())
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces add <type> <delay> <repeat> <text>");
							break;
						}
						string content = st.nextToken();
						while (st.hasMoreTokens())
						{
							content += " " + st.nextToken();
						}
						// ************************************
						IAnnouncement announce;
						if (type == AnnouncementType.AUTO_CRITICAL || type == AnnouncementType.AUTO_NORMAL)
						{
							announce = new AutoAnnouncement(type, content, activeChar.getName(), TimeSpan.FromMilliseconds(initDelay), TimeSpan.FromMilliseconds(delay), repeat);
						}
						else
						{
							announce = new Announcement(type, content, activeChar.getName());
						}

						AnnouncementsTable.getInstance().addAnnouncement(announce);
						BuilderUtil.sendSysMessage(activeChar, "Announcement has been successfully added!");
						return useAdminCommand("admin_announces list", activeChar);
					}
					case "edit":
					{
						if (!st.hasMoreTokens())
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces edit <id>");
							break;
						}
						string annId = st.nextToken();
						if (!int.TryParse(annId, CultureInfo.InvariantCulture, out int id))
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces edit <id>");
							break;
						}

						IAnnouncement? announce = AnnouncementsTable.getInstance().getAnnounce(id);
						if (announce == null)
						{
							BuilderUtil.sendSysMessage(activeChar, "Announcement does not exist!");
							break;
						}
						if (!st.hasMoreTokens())
						{
							string content = HtmCache.getInstance().getHtm("html/admin/announces-edit.htm", activeChar.getLang());
							string announcementId = announce.getId().ToString();
							string announcementType = announce.getType().ToString();
							string announcementInital = "0";
							string announcementDelay = "0";
							string announcementRepeat = "0";
							string announcementAuthor = announce.getAuthor();
							string announcementContent = announce.getContent();
							if (announce is AutoAnnouncement)
							{
								AutoAnnouncement autoAnnounce = (AutoAnnouncement) announce;
								announcementInital = (autoAnnounce.getInitial() / 1000).ToString();
								announcementDelay = (autoAnnounce.getDelay() / 1000).ToString();
								announcementRepeat = autoAnnounce.getRepeat().ToString();
							}

							content = content.Replace("%id%", announcementId);
							content = content.Replace("%type%", announcementType);
							content = content.Replace("%initial%", announcementInital);
							content = content.Replace("%delay%", announcementDelay);
							content = content.Replace("%repeat%", announcementRepeat);
							content = content.Replace("%author%", announcementAuthor);
							content = content.Replace("%content%", announcementContent);
							Util.sendCBHtml(activeChar, content);
							break;
						}
						string annType = st.nextToken();
						AnnouncementType type = Enum.Parse<AnnouncementType>(annType);
						switch (announce.getType())
						{
							case AnnouncementType.AUTO_CRITICAL:
							case AnnouncementType.AUTO_NORMAL:
							{
								switch (type)
								{
									case AnnouncementType.AUTO_CRITICAL:
									case AnnouncementType.AUTO_NORMAL:
									{
										break;
									}
									default:
									{
										BuilderUtil.sendSysMessage(activeChar, "Announce type can be changed only to AUTO_NORMAL or AUTO_CRITICAL!");
										return false;
									}
								}
								break;
							}
							case AnnouncementType.NORMAL:
							case AnnouncementType.CRITICAL:
							{
								switch (type)
								{
									case AnnouncementType.NORMAL:
									case AnnouncementType.CRITICAL:
									{
										break;
									}
									default:
									{
										BuilderUtil.sendSysMessage(activeChar, "Announce type can be changed only to NORMAL or CRITICAL!");
										return false;
									}
								}
								break;
							}
						}
						// ************************************
						if (!st.hasMoreTokens())
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces add <type> <delay> <repeat> <text>");
							break;
						}
						string annInitDelay = st.nextToken();
						if (!int.TryParse(annInitDelay, CultureInfo.InvariantCulture, out int initDelay))
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces add <type> <delay> <repeat> <text>");
							break;
						}
						// ************************************
						if (!st.hasMoreTokens())
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces add <type> <delay> <repeat> <text>");
							break;
						}
						string annDelay = st.nextToken();
						if (!int.TryParse(annDelay, CultureInfo.InvariantCulture, out int delay))
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces add <type> <delay> <repeat> <text>");
							break;
						}
						if (delay < 10 && (type == AnnouncementType.AUTO_NORMAL || type == AnnouncementType.AUTO_CRITICAL))
						{
							BuilderUtil.sendSysMessage(activeChar, "Delay cannot be less then 10 seconds!");
							break;
						}
						// ************************************
						if (!st.hasMoreTokens())
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces add <type> <delay> <repeat> <text>");
							break;
						}
						string annRepeat = st.nextToken();
						if (!int.TryParse(annRepeat, CultureInfo.InvariantCulture, out int repeat))
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces add <type> <delay> <repeat> <text>");
							break;
						}
						if (repeat == 0)
						{
							repeat = -1;
						}
						// ************************************
						string content1 = "";
						if (st.hasMoreTokens())
						{
							content1 = st.nextToken();
							while (st.hasMoreTokens())
							{
								content1 += " " + st.nextToken();
							}
						}
						if (string.IsNullOrEmpty(content1))
						{
							content1 = announce.getContent();
						}
						// ************************************
						announce.setType(type);
						announce.setContent(content1);
						announce.setAuthor(activeChar.getName());
						if (announce is AutoAnnouncement announcement)
						{
							announcement.setInitial(TimeSpan.FromMilliseconds(initDelay * 1000));
							announcement.setDelay(TimeSpan.FromMilliseconds(delay * 1000));
							announcement.setRepeat(repeat);
						}
						announce.updateMe();
						BuilderUtil.sendSysMessage(activeChar, "Announcement has been successfully edited!");
						return useAdminCommand("admin_announces list", activeChar);
					}
					case "remove":
					{
						if (!st.hasMoreTokens())
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces remove <announcement id>");
							break;
						}
						string token = st.nextToken();
						if (!int.TryParse(token, CultureInfo.InvariantCulture, out int id))
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces remove <announcement id>");
							break;
						}
						if (AnnouncementsTable.getInstance().deleteAnnouncement(id))
						{
							BuilderUtil.sendSysMessage(activeChar, "Announcement has been successfully removed!");
						}
						else
						{
							BuilderUtil.sendSysMessage(activeChar, "Announcement does not exist!");
						}
						return useAdminCommand("admin_announces list", activeChar);
					}
					case "restart":
					{
						if (!st.hasMoreTokens())
						{
							foreach (IAnnouncement announce in AnnouncementsTable.getInstance().getAllAnnouncements())
							{
								if (announce is AutoAnnouncement)
								{
									AutoAnnouncement autoAnnounce = (AutoAnnouncement) announce;
									autoAnnounce.restartMe();
								}
							}
							BuilderUtil.sendSysMessage(activeChar, "Auto announcements has been successfully restarted.");
							break;
						}
						string token = st.nextToken();
						if (!int.TryParse(token, CultureInfo.InvariantCulture, out int id))
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces show <announcement id>");
							break;
						}

						IAnnouncement? announce1 = AnnouncementsTable.getInstance().getAnnounce(id);
						if (announce1 != null)
						{
							if (announce1 is AutoAnnouncement)
							{
								AutoAnnouncement autoAnnounce = (AutoAnnouncement) announce1;
								autoAnnounce.restartMe();
								BuilderUtil.sendSysMessage(activeChar, "Auto announcement has been successfully restarted.");
							}
							else
							{
								BuilderUtil.sendSysMessage(activeChar, "This option has effect only on auto announcements!");
							}
						}
						else
						{
							BuilderUtil.sendSysMessage(activeChar, "Announcement does not exist!");
						}
						break;
					}
					case "show":
					{
						if (!st.hasMoreTokens())
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces show <announcement id>");
							break;
						}
						string token = st.nextToken();
						if (!int.TryParse(token, CultureInfo.InvariantCulture, out int id))
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces show <announcement id>");
							break;
						}

						IAnnouncement? announce = AnnouncementsTable.getInstance().getAnnounce(id);
						if (announce != null)
						{
							string content = HtmCache.getInstance().getHtm("html/admin/announces-show.htm", activeChar.getLang());
							string announcementId = announce.getId().ToString();
							string announcementType = announce.getType().ToString();
							string announcementInital = "0";
							string announcementDelay = "0";
							string announcementRepeat = "0";
							string announcementAuthor = announce.getAuthor();
							string announcementContent = announce.getContent();
							if (announce is AutoAnnouncement)
							{
								AutoAnnouncement autoAnnounce = (AutoAnnouncement) announce;
								announcementInital = (autoAnnounce.getInitial() / 1000).ToString();
								announcementDelay = (autoAnnounce.getDelay() / 1000).ToString();
								announcementRepeat = autoAnnounce.getRepeat().ToString();
							}
							content = content.Replace("%id%", announcementId);
							content = content.Replace("%type%", announcementType);
							content = content.Replace("%initial%", announcementInital);
							content = content.Replace("%delay%", announcementDelay);
							content = content.Replace("%repeat%", announcementRepeat);
							content = content.Replace("%author%", announcementAuthor);
							content = content.Replace("%content%", announcementContent);
							Util.sendCBHtml(activeChar, content);
							break;
						}
						BuilderUtil.sendSysMessage(activeChar, "Announcement does not exist!");
						return useAdminCommand("admin_announces list", activeChar);
					}
					case "list":
					{
						int page = 0;
						if (st.hasMoreTokens())
						{
							string token = st.nextToken();
							if (int.TryParse(token, CultureInfo.InvariantCulture, out int value))
							{
								page = value;
							}
						}

						string content = HtmCache.getInstance().getHtm("html/admin/announces-list.htm", activeChar.getLang());
						PageResult result = PageBuilder.newBuilder(AnnouncementsTable.getInstance().getAllAnnouncements().ToList(), 8, "bypass admin_announces list").
							currentPage(page).bodyHandler((pages, announcement, sb) =>
						{
							sb.Append("<tr>");
							sb.Append("<td width=5></td>");
							sb.Append("<td width=80>" + announcement.getId() + "</td>");
							sb.Append("<td width=100>" + announcement.getType() + "</td>");
							sb.Append("<td width=100>" + announcement.getAuthor() + "</td>");
							if (announcement.getType() == AnnouncementType.AUTO_NORMAL || announcement.getType() == AnnouncementType.AUTO_CRITICAL)
							{
								sb.Append("<td width=60><button action=\"bypass -h admin_announces restart " + announcement.getId() + "\" value=\"Restart\" width=\"60\" height=\"21\" back=\"L2UI_CT1.Button_DF_Down\" fore=\"L2UI_CT1.Button_DF\"></td>");
							}
							else
							{
								sb.Append("<td width=60><button action=\"\" value=\"\" width=\"60\" height=\"21\" back=\"L2UI_CT1.Button_DF_Down\" fore=\"L2UI_CT1.Button_DF\"></td>");
							}
							if (announcement.getType() == AnnouncementType.EVENT)
							{
								sb.Append("<td width=60><button action=\"bypass -h admin_announces show " + announcement.getId() + "\" value=\"Show\" width=\"60\" height=\"21\" back=\"L2UI_CT1.Button_DF_Down\" fore=\"L2UI_CT1.Button_DF\"></td>");
								sb.Append("<td width=60></td>");
							}
							else
							{
								sb.Append("<td width=60><button action=\"bypass -h admin_announces show " + announcement.getId() + "\" value=\"Show\" width=\"60\" height=\"21\" back=\"L2UI_CT1.Button_DF_Down\" fore=\"L2UI_CT1.Button_DF\"></td>");
								sb.Append("<td width=60><button action=\"bypass -h admin_announces edit " + announcement.getId() + "\" value=\"Edit\" width=\"60\" height=\"21\" back=\"L2UI_CT1.Button_DF_Down\" fore=\"L2UI_CT1.Button_DF\"></td>");
							}
							sb.Append("<td width=60><button action=\"bypass -h admin_announces remove " + announcement.getId() + "\" value=\"Remove\" width=\"60\" height=\"21\" back=\"L2UI_CT1.Button_DF_Down\" fore=\"L2UI_CT1.Button_DF\"></td>");
							sb.Append("<td width=5></td>");
							sb.Append("</tr>");
						}).build();

						content = content.Replace("%pages%", result.getPagerTemplate().ToString());
						content = content.Replace("%announcements%", result.getBodyTemplate().ToString());
						Util.sendCBHtml(activeChar, content);
						break;
					}
				}

				break;
			}
		}
		return false;
	}

	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}