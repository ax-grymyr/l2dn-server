using System.Globalization;
using System.Net;
using System.Text;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Punishment;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author UnAfraid
 */
public class AdminPunishment: IAdminCommandHandler
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AdminPunishment));
	
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_punishment",
		"admin_punishment_add",
		"admin_punishment_remove",
		"admin_ban_acc",
		"admin_unban_acc",
		"admin_ban_hwid",
		"admin_unban_hwid",
		"admin_ban_chat",
		"admin_unban_chat",
		"admin_ban_char",
		"admin_unban_char",
		"admin_jail",
		"admin_unjail",
    ];
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command, " ");
		if (!st.hasMoreTokens())
		{
			return false;
		}
		string cmd = st.nextToken();
		switch (cmd)
		{
			case "admin_punishment":
			{
				if (!st.hasMoreTokens())
				{
					HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/punishment.htm", activeChar);
					htmlContent.Replace("%punishments%", string.Join(";", EnumUtil.GetValues<PunishmentType>()));
					htmlContent.Replace("%affects%", string.Join(";", EnumUtil.GetValues<PunishmentAffect>()));
					activeChar.sendPacket(new NpcHtmlMessagePacket(null, 1, htmlContent));
				}
				else
				{
					string subcmd = st.nextToken();
					switch (subcmd)
					{
						case "info":
						{
							string key = st.hasMoreTokens() ? st.nextToken() : null;
							string af = st.hasMoreTokens() ? st.nextToken() : null;
							string name = key;
							if ((key == null) || (af == null))
							{
								BuilderUtil.sendSysMessage(activeChar, "Not enough data specified!");
								break;
							}
							
							PunishmentAffect affect = Enum.Parse<PunishmentAffect>(af);
							if (Enum.IsDefined(affect))
							{
								BuilderUtil.sendSysMessage(activeChar, "Incorrect value specified for affect type!");
								break;
							}
							
							// Swap the name of the character with it's id.
							if (affect == PunishmentAffect.CHARACTER)
							{
								key = findCharId(key);
							}
							
							HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/punishment-info.htm", activeChar);
							StringBuilder sb = new StringBuilder();
							foreach (PunishmentType type in EnumUtil.GetValues<PunishmentType>())
							{
								if (PunishmentManager.getInstance().hasPunishment(key, affect, type))
								{
									DateTime? expiration = PunishmentManager.getInstance().getPunishmentExpiration(key, affect, type);
									string expire = "never";
									if (expiration != null)
									{
										expire = expiration.Value.ToString("yyyy.MM.dd HH:mm:ss");
									}

									sb.Append("<tr><td><font color=\"LEVEL\">" + type + "</font></td><td>" + expire + "</td><td><a action=\"bypass -h admin_punishment_remove " + name + " " + affect + " " + type + "\">Remove</a></td></tr>");
								}
							}
								
							htmlContent.Replace("%player_name%", name);
							htmlContent.Replace("%punishments%", sb.ToString());
							htmlContent.Replace("%affects%", string.Join(";", EnumUtil.GetValues<PunishmentAffect>()));
							htmlContent.Replace("%affect_type%", affect.ToString());
							activeChar.sendPacket(new NpcHtmlMessagePacket(null, 1, htmlContent));

							break;
						}
						case "player":
						{
							Player target = null;
							if (st.hasMoreTokens())
							{
								string playerName = st.nextToken();
								if (string.IsNullOrEmpty(playerName) && ((activeChar.getTarget() == null) || !activeChar.getTarget().isPlayer()))
								{
									return useAdminCommand("admin_punishment", activeChar);
								}
								target = World.getInstance().getPlayer(playerName);
							}
							if ((target == null) && ((activeChar.getTarget() == null) || !activeChar.getTarget().isPlayer()))
							{
								BuilderUtil.sendSysMessage(activeChar, "You must target player!");
								break;
							}
							if (target == null)
							{
								target = activeChar.getTarget().getActingPlayer();
							}
							
							HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/punishment-player.htm", activeChar);
							htmlContent.Replace("%player_name%", target.getName());
							htmlContent.Replace("%punishments%", string.Join(";", EnumUtil.GetValues<PunishmentType>()));
							htmlContent.Replace("%acc%", target.getAccountName());
							htmlContent.Replace("%char%", target.getName());
							htmlContent.Replace("%ip%", target.getClient()?.IpAddress.ToString() ?? "-");
							htmlContent.Replace("%hwid%", target.getClient()?.HardwareInfo?.getMacAddress() ?? "-");
							activeChar.sendPacket(new NpcHtmlMessagePacket(null, 1, htmlContent));

							break;
						}
					}
				}
				break;
			}
			case "admin_punishment_add":
			{
				// Add new punishment
				string key = st.hasMoreTokens() ? st.nextToken() : null;
				string af = st.hasMoreTokens() ? st.nextToken() : null;
				string t = st.hasMoreTokens() ? st.nextToken() : null;
				string exp = st.hasMoreTokens() ? st.nextToken() : null;
				string reason = st.hasMoreTokens() ? st.nextToken() : null;
				
				// Let's grab the other part of the reason if there is..
				if (reason != null)
				{
					while (st.hasMoreTokens())
					{
						reason += " " + st.nextToken();
					}
					if (!string.IsNullOrEmpty(reason))
					{
						reason = reason.replaceAll("\\$", "\\\\\\$");
						reason = reason.replaceAll("\r\n", "<br1>");
						reason = reason.Replace("<", "&lt;");
						reason = reason.Replace(">", "&gt;");
					}
				}
				
				string name = key;
				if ((key == null) || (af == null) || (t == null) || (exp == null) || (reason == null))
				{
					BuilderUtil.sendSysMessage(activeChar, "Please fill all the fields!");
					break;
				}
				if (!int.TryParse(exp, CultureInfo.InvariantCulture, out int expirationTime))
				{
					BuilderUtil.sendSysMessage(activeChar, "Incorrect value specified for expiration time!");
					break;
				}
				
				DateTime? expTime = null;
				if (expirationTime > 0)
				{
					expTime = DateTime.UtcNow + TimeSpan.FromMinutes(expirationTime);
				}
				
				PunishmentAffect affect = Enum.Parse<PunishmentAffect>(af);
				PunishmentType type = Enum.Parse<PunishmentType>(t);
				if (Enum.IsDefined(affect) || Enum.IsDefined(type))
				{
					BuilderUtil.sendSysMessage(activeChar, "Incorrect value specified for affect/punishment type!");
					break;
				}
				
				// Swap the name of the character with it's id.
				if (affect == PunishmentAffect.CHARACTER)
				{
					key = findCharId(key);
				}
				else if (affect == PunishmentAffect.IP)
				{
					try
					{
						IPAddress addr = IPAddress.Parse(key);
						// if (addr.) // TODO
						// {
						// 	throw new UnknownHostException("You cannot ban any local address!");
						// }
						// else if (Config.GAME_SERVER_HOSTS.Contains(addr.getHostAddress()))
						// {
						// 	throw new UnknownHostException("You cannot ban your gameserver's address!");
						// }
					}
					catch (Exception e)
					{
						BuilderUtil.sendSysMessage(activeChar, "You've entered an incorrect IP address!");
						activeChar.sendMessage("Error: " + e.Message);
						break;
					}
				}
				
				// Check if we already put the same punishment on that guy ^^
				if (PunishmentManager.getInstance().hasPunishment(key, affect, type))
				{
					BuilderUtil.sendSysMessage(activeChar, "Target is already affected by that punishment.");
					break;
				}
				
				// Punish him!
				PunishmentManager.getInstance().startPunishment(new PunishmentTask(key, affect, type, expTime, reason, activeChar.getName()));
				BuilderUtil.sendSysMessage(activeChar, "Punishment " + type + " have been applied to: " + affect + " " + name + "!");
				//GMAudit.auditGMAction(activeChar.getName() + " [" + activeChar.getObjectId() + "]", cmd, affect.name(), name);// TODO
				return useAdminCommand("admin_punishment info " + name + " " + affect, activeChar);
			}
			case "admin_punishment_remove":
			{
				// Remove punishment.
				string key = st.hasMoreTokens() ? st.nextToken() : null;
				string af = st.hasMoreTokens() ? st.nextToken() : null;
				string t = st.hasMoreTokens() ? st.nextToken() : null;
				string name = key;
				if ((key == null) || (af == null) || (t == null))
				{
					BuilderUtil.sendSysMessage(activeChar, "Not enough data specified!");
					break;
				}
				
				PunishmentAffect affect = Enum.Parse<PunishmentAffect>(af);
				PunishmentType type = Enum.Parse<PunishmentType>(t);
				if ((!Enum.IsDefined(affect)) || (!Enum.IsDefined(type)))
				{
					BuilderUtil.sendSysMessage(activeChar, "Incorrect value specified for affect/punishment type!");
					break;
				}
				
				// Swap the name of the character with it's id.
				if (affect == PunishmentAffect.CHARACTER)
				{
					key = findCharId(key);
				}
				
				if (!PunishmentManager.getInstance().hasPunishment(key, affect, type))
				{
					BuilderUtil.sendSysMessage(activeChar, "Target is not affected by that punishment!");
					break;
				}
				
				PunishmentManager.getInstance().stopPunishment(key, affect, type);
				BuilderUtil.sendSysMessage(activeChar, "Punishment " + type + " have been stopped to: " + affect + " " + name + "!");
				// GMAudit.auditGMAction(activeChar.getName() + " [" + activeChar.getObjectId() + "]", cmd, affect.name(), name); // TODO
				return useAdminCommand("admin_punishment info " + name + " " + affect, activeChar);
			}
			case "admin_ban_char":
			{
				if (st.hasMoreTokens())
				{
					return useAdminCommand(string.Format("admin_punishment_add {0} {1} {2} {3} {4}", st.nextToken(), PunishmentAffect.CHARACTER, PunishmentType.BAN, 0, "Banned by admin"), activeChar);
				}
				break;
			}
			case "admin_unban_char":
			{
				if (st.hasMoreTokens())
				{
					return useAdminCommand(string.Format("admin_punishment_remove {0} {1} {2}", st.nextToken(), PunishmentAffect.CHARACTER, PunishmentType.BAN), activeChar);
				}
				break;
			}
			case "admin_ban_acc":
			{
				if (st.hasMoreTokens())
				{
					return useAdminCommand(string.Format("admin_punishment_add {0} {1} {2} {3} {4}", st.nextToken(), PunishmentAffect.ACCOUNT, PunishmentType.BAN, 0, "Banned by admin"), activeChar);
				}
				break;
			}
			case "admin_unban_acc":
			{
				if (st.hasMoreTokens())
				{
					return useAdminCommand(string.Format("admin_punishment_remove {0} {1} {2}", st.nextToken(), PunishmentAffect.ACCOUNT, PunishmentType.BAN), activeChar);
				}
				break;
			}
			case "admin_ban_hwid":
			{
				if (st.hasMoreTokens())
				{
					return useAdminCommand(string.Format("admin_punishment_add {0} {1} {2} {3} {4}", st.nextToken(), PunishmentAffect.HWID, PunishmentType.BAN, 0, "Banned by admin"), activeChar);
				}
				break;
			}
			case "admin_unban_hwid":
			{
				if (st.hasMoreTokens())
				{
					return useAdminCommand(string.Format("admin_punishment_remove {0} {1} {2}", st.nextToken(), PunishmentAffect.HWID, PunishmentType.BAN), activeChar);
				}
				break;
			}
			case "admin_ban_chat":
			{
				if (st.hasMoreTokens())
				{
					return useAdminCommand(string.Format("admin_punishment_add {0} {1} {2} {3} {4}", st.nextToken(), PunishmentAffect.CHARACTER, PunishmentType.CHAT_BAN, 0, "Chat banned by admin"), activeChar);
				}
				break;
			}
			case "admin_unban_chat":
			{
				if (st.hasMoreTokens())
				{
					return useAdminCommand(string.Format("admin_punishment_remove {0} {1} {2}", st.nextToken(), PunishmentAffect.CHARACTER, PunishmentType.CHAT_BAN), activeChar);
				}
				break;
			}
			case "admin_jail":
			{
				if (st.hasMoreTokens())
				{
					return useAdminCommand(string.Format("admin_punishment_add {0} {1} {2} {3} {4}", st.nextToken(), PunishmentAffect.CHARACTER, PunishmentType.JAIL, 0, "Jailed by admin"), activeChar);
				}
				break;
			}
			case "admin_unjail":
			{
				if (st.hasMoreTokens())
				{
					return useAdminCommand(string.Format("admin_punishment_remove {0} {1} {2}", st.nextToken(), PunishmentAffect.CHARACTER, PunishmentType.JAIL), activeChar);
				}
				break;
			}
		}
		return true;
	}
	
	private static string findCharId(string key)
	{
		int charId = CharInfoTable.getInstance().getIdByName(key);
		if (charId > 0) // Yeah its a char name!
		{
			return charId.ToString();
		}
		return key;
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}