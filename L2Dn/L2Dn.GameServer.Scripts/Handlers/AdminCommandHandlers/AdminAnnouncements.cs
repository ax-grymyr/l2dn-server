using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Announcements;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author UnAfraid
 */
public class AdminAnnouncements: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_announce",
		"admin_announce_crit",
		"admin_announce_screen",
		"admin_announces",
	};
	
	public bool useAdminCommand(String command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command);
		String cmd = st.hasMoreTokens() ? st.nextToken() : "";
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
				String announce = st.nextToken();
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
				String subCmd = st.hasMoreTokens() ? st.nextToken() : "";
				switch (subCmd)
				{
					case "add":
					{
						if (!st.hasMoreTokens())
						{
							String content2 = HtmCache.getInstance().getHtm("html/admin/announces-add.htm", activeChar.getLang());
							Util.sendCBHtml(activeChar, content2);
							break;
						}
						String annType = st.nextToken();
						AnnouncementType type = Enum.Parse<AnnouncementType>(annType);
						// ************************************
						if (!st.hasMoreTokens())
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces add <type> <delay> <repeat> <text>");
							break;
						}
						String annInitDelay = st.nextToken();
						if (!Util.isDigit(annInitDelay))
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces add <type> <delay> <repeat> <text>");
							break;
						}
						int initDelay = int.Parse(annInitDelay) * 1000;
						// ************************************
						if (!st.hasMoreTokens())
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces add <type> <delay> <repeat> <text>");
							break;
						}
						String annDelay = st.nextToken();
						if (!Util.isDigit(annDelay))
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces add <type> <delay> <repeat> <text>");
							break;
						}
						int delay = int.Parse(annDelay) * 1000;
						if ((delay < (10 * 1000)) && ((type == AnnouncementType.AUTO_NORMAL) || (type == AnnouncementType.AUTO_CRITICAL)))
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
						String annRepeat = st.nextToken();
						if (!Util.isDigit(annRepeat))
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces add <type> <delay> <repeat> <text>");
							break;
						}
						int repeat = int.Parse(annRepeat);
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
						String content = st.nextToken();
						while (st.hasMoreTokens())
						{
							content += " " + st.nextToken();
						}
						// ************************************
						IAnnouncement announce;
						if ((type == AnnouncementType.AUTO_CRITICAL) || (type == AnnouncementType.AUTO_NORMAL))
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
						String annId = st.nextToken();
						if (!Util.isDigit(annId))
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces edit <id>");
							break;
						}
						int id = int.Parse(annId);
						IAnnouncement announce = AnnouncementsTable.getInstance().getAnnounce(id);
						if (announce == null)
						{
							BuilderUtil.sendSysMessage(activeChar, "Announcement does not exist!");
							break;
						}
						if (!st.hasMoreTokens())
						{
							String content = HtmCache.getInstance().getHtm("html/admin/announces-edit.htm", activeChar.getLang());
							String announcementId = announce.getId().ToString();
							String announcementType = announce.getType().ToString();
							String announcementInital = "0";
							String announcementDelay = "0";
							String announcementRepeat = "0";
							String announcementAuthor = announce.getAuthor();
							String announcementContent = announce.getContent();
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
						String annType = st.nextToken();
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
						String annInitDelay = st.nextToken();
						if (!Util.isDigit(annInitDelay))
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces add <type> <delay> <repeat> <text>");
							break;
						}
						int initDelay = int.Parse(annInitDelay);
						// ************************************
						if (!st.hasMoreTokens())
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces add <type> <delay> <repeat> <text>");
							break;
						}
						String annDelay = st.nextToken();
						if (!Util.isDigit(annDelay))
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces add <type> <delay> <repeat> <text>");
							break;
						}
						int delay = int.Parse(annDelay);
						if ((delay < 10) && ((type == AnnouncementType.AUTO_NORMAL) || (type == AnnouncementType.AUTO_CRITICAL)))
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
						String annRepeat = st.nextToken();
						if (!Util.isDigit(annRepeat))
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces add <type> <delay> <repeat> <text>");
							break;
						}
						int repeat = int.Parse(annRepeat);
						if (repeat == 0)
						{
							repeat = -1;
						}
						// ************************************
						String content1 = "";
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
						if (announce is AutoAnnouncement)
						{
							AutoAnnouncement autoAnnounce = (AutoAnnouncement) announce;
							autoAnnounce.setInitial(TimeSpan.FromMilliseconds(initDelay * 1000));
							autoAnnounce.setDelay(TimeSpan.FromMilliseconds(delay * 1000));
							autoAnnounce.setRepeat(repeat);
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
						String token = st.nextToken();
						if (!Util.isDigit(token))
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces remove <announcement id>");
							break;
						}
						int id = int.Parse(token);
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
						String token = st.nextToken();
						if (!Util.isDigit(token))
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces show <announcement id>");
							break;
						}
						int id = int.Parse(token);
						IAnnouncement announce1 = AnnouncementsTable.getInstance().getAnnounce(id);
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
						String token = st.nextToken();
						if (!Util.isDigit(token))
						{
							BuilderUtil.sendSysMessage(activeChar, "Syntax: //announces show <announcement id>");
							break;
						}
						int id = int.Parse(token);
						IAnnouncement announce = AnnouncementsTable.getInstance().getAnnounce(id);
						if (announce != null)
						{
							String content = HtmCache.getInstance().getHtm("html/admin/announces-show.htm", activeChar.getLang());
							String announcementId = announce.getId().ToString();
							String announcementType = announce.getType().ToString();
							String announcementInital = "0";
							String announcementDelay = "0";
							String announcementRepeat = "0";
							String announcementAuthor = announce.getAuthor();
							String announcementContent = announce.getContent();
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
							String token = st.nextToken();
							if (Util.isDigit(token))
							{
								page = int.Parse(token);
							}
						}
						
						String content = HtmCache.getInstance().getHtm("html/admin/announces-list.htm", activeChar.getLang());
						PageResult result = PageBuilder.newBuilder(AnnouncementsTable.getInstance().getAllAnnouncements().ToList(), 8, "bypass admin_announces list").
							currentPage(page).bodyHandler((pages, announcement, sb) =>
						{
							sb.Append("<tr>");
							sb.Append("<td width=5></td>");
							sb.Append("<td width=80>" + announcement.getId() + "</td>");
							sb.Append("<td width=100>" + announcement.getType() + "</td>");
							sb.Append("<td width=100>" + announcement.getAuthor() + "</td>");
							if ((announcement.getType() == AnnouncementType.AUTO_NORMAL) || (announcement.getType() == AnnouncementType.AUTO_CRITICAL))
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
	
	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}