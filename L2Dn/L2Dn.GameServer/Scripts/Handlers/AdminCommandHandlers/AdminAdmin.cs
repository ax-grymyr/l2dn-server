using System.Globalization;
using System.Text;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands: - admin|admin1/admin2/admin3/admin4/admin5 = slots for the 5 starting admin menus - gmliston/gmlistoff = includes/excludes active character from /gmlist results - silence = toggles private messages acceptance mode - diet = toggles weight penalty mode -
 * tradeoff = toggles trade acceptance mode - reload = reloads specified component from multisell|skill|npc|htm|item - set/set_menu/set_mod = alters specified server setting - saveolymp = saves olympiad state manually - manualhero = cycles olympiad and calculate new heroes.
 * @version $Revision: 1.3.2.1.2.4 $ $Date: 2007/07/28 10:06:06 $
 */
public class AdminAdmin: IAdminCommandHandler
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AdminAdmin));
	
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_admin",
		"admin_admin1",
		"admin_admin2",
		"admin_admin3",
		"admin_admin4",
		"admin_admin5",
		"admin_admin6",
		"admin_admin7",
		"admin_gmliston",
		"admin_gmlistoff",
		"admin_silence",
		"admin_diet",
		"admin_tradeoff",
		"admin_set",
		"admin_set_mod",
		"admin_saveolymp",
		"admin_sethero",
		"admin_settruehero",
		"admin_givehero",
		"admin_endolympiad",
		"admin_setconfig",
		"admin_config_server",
		"admin_gmon",
		"admin_worldchat",
	};
	
	public bool useAdminCommand(String command, Player activeChar)
	{
		if (command.startsWith("admin_admin"))
		{
			showMainPage(activeChar, command);
		}
		else if (command.equals("admin_config_server"))
		{
			showConfigPage(activeChar);
		}
		else if (command.startsWith("admin_gmliston"))
		{
			AdminData.getInstance().addGm(activeChar, false);
			BuilderUtil.sendSysMessage(activeChar, "Registered into GM list.");
			AdminHtml.showAdminHtml(activeChar, "gm_menu.htm");
		}
		else if (command.startsWith("admin_gmlistoff"))
		{
			AdminData.getInstance().addGm(activeChar, true);
			BuilderUtil.sendSysMessage(activeChar, "Removed from GM list.");
			AdminHtml.showAdminHtml(activeChar, "gm_menu.htm");
		}
		else if (command.startsWith("admin_silence"))
		{
			if (activeChar.isSilenceMode()) // already in message refusal mode
			{
				activeChar.setSilenceMode(false);
				activeChar.sendPacket(SystemMessageId.MESSAGE_ACCEPTANCE_MODE);
			}
			else
			{
				activeChar.setSilenceMode(true);
				activeChar.sendPacket(SystemMessageId.MESSAGE_REFUSAL_MODE);
			}
			AdminHtml.showAdminHtml(activeChar, "gm_menu.htm");
		}
		else if (command.startsWith("admin_saveolymp"))
		{
			Olympiad.getInstance().saveOlympiadStatus();
			BuilderUtil.sendSysMessage(activeChar, "olympiad system saved.");
		}
		else if (command.startsWith("admin_endolympiad"))
		{
			try
			{
				Olympiad.getInstance().manualSelectHeroes();
			}
			catch (Exception e)
			{
				LOGGER.Warn("An error occured while ending olympiad: " + e);
			}
			BuilderUtil.sendSysMessage(activeChar, "Heroes formed.");
		}
		else if (command.startsWith("admin_sethero"))
		{
			if (activeChar.getTarget() == null)
			{
				activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				return false;
			}
			
			Player target = activeChar.getTarget().isPlayer() ? activeChar.getTarget().getActingPlayer() : activeChar;
			target.setHero(!target.isHero());
			target.broadcastUserInfo();
		}
		else if (command.startsWith("admin_settruehero"))
		{
			if (activeChar.getTarget() == null)
			{
				activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				return false;
			}
			
			Player target = activeChar.getTarget().isPlayer() ? activeChar.getTarget().getActingPlayer() : activeChar;
			target.setTrueHero(!target.isTrueHero());
			target.broadcastUserInfo();
		}
		else if (command.startsWith("admin_givehero"))
		{
			if (activeChar.getTarget() == null)
			{
				activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				return false;
			}
			
			Player target = activeChar.getTarget().isPlayer() ? activeChar.getTarget().getActingPlayer() : activeChar;
			if (Hero.getInstance().isHero(target.getObjectId()))
			{
				BuilderUtil.sendSysMessage(activeChar, "This player has already claimed the hero status.");
				return false;
			}
			
			if (!Hero.getInstance().isUnclaimedHero(target.getObjectId()))
			{
				BuilderUtil.sendSysMessage(activeChar, "This player cannot claim the hero status.");
				return false;
			}
			Hero.getInstance().claimHero(target);
		}
		else if (command.startsWith("admin_diet"))
		{
			try
			{
				StringTokenizer st = new StringTokenizer(command);
				st.nextToken();
				if (st.nextToken().equalsIgnoreCase("on"))
				{
					activeChar.setDietMode(true);
					BuilderUtil.sendSysMessage(activeChar, "Diet mode on.");
				}
				else if (st.nextToken().equalsIgnoreCase("off"))
				{
					activeChar.setDietMode(false);
					BuilderUtil.sendSysMessage(activeChar, "Diet mode off.");
				}
			}
			catch (Exception ex)
			{
				if (activeChar.getDietMode())
				{
					activeChar.setDietMode(false);
					BuilderUtil.sendSysMessage(activeChar, "Diet mode off.");
				}
				else
				{
					activeChar.setDietMode(true);
					BuilderUtil.sendSysMessage(activeChar, "Diet mode on.");
				}
			}
			finally
			{
				activeChar.refreshOverloaded(true);
			}
			AdminHtml.showAdminHtml(activeChar, "gm_menu.htm");
		}
		else if (command.startsWith("admin_tradeoff"))
		{
			try
			{
				String mode = command.Substring(15);
				if (mode.equalsIgnoreCase("on"))
				{
					activeChar.setTradeRefusal(true);
					BuilderUtil.sendSysMessage(activeChar, "Trade refusal enabled.");
				}
				else if (mode.equalsIgnoreCase("off"))
				{
					activeChar.setTradeRefusal(false);
					BuilderUtil.sendSysMessage(activeChar, "Trade refusal disabled.");
				}
			}
			catch (Exception ex)
			{
				if (activeChar.getTradeRefusal())
				{
					activeChar.setTradeRefusal(false);
					BuilderUtil.sendSysMessage(activeChar, "Trade refusal disabled.");
				}
				else
				{
					activeChar.setTradeRefusal(true);
					BuilderUtil.sendSysMessage(activeChar, "Trade refusal enabled.");
				}
			}
			AdminHtml.showAdminHtml(activeChar, "gm_menu.htm");
		}
		else if (command.startsWith("admin_setconfig"))
		{
			StringTokenizer st = new StringTokenizer(command);
			st.nextToken();
			try
			{
				String pName = st.nextToken();
				String pValue = st.nextToken();
				if (!float.TryParse(pValue, CultureInfo.InvariantCulture, out float pVal))
				{
					BuilderUtil.sendSysMessage(activeChar, "Invalid parameter!");
					return false;
				}
				
				switch (pName)
				{
					case "RateXp":
					{
						Config.RATE_XP = pVal;
						break;
					}
					case "RateSp":
					{
						Config.RATE_SP = pVal;
						break;
					}
					case "RateDropSpoil":
					{
						Config.RATE_SPOIL_DROP_CHANCE_MULTIPLIER = pVal;
						break;
					}
				}
				BuilderUtil.sendSysMessage(activeChar, "Config parameter " + pName + " set to " + pValue);
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Usage: //setconfig <parameter> <value>");
			}
			finally
			{
				showConfigPage(activeChar);
			}
		}
		else if (command.startsWith("admin_worldchat"))
		{
			StringTokenizer st = new StringTokenizer(command);
			st.nextToken(); // admin_worldchat
			switch (st.hasMoreTokens() ? st.nextToken() : "")
			{
				case "shout":
				{
					StringBuilder sb = new StringBuilder();
					while (st.hasMoreTokens())
					{
						sb.Append(st.nextToken());
						sb.Append(" ");
					}
					
					CreatureSayPacket cs = new CreatureSayPacket(activeChar, ChatType.WORLD, activeChar.getName(), sb.ToString());
					foreach (Player player in World.getInstance().getPlayers())
					{
						if (player.isNotBlocked(activeChar))
						{
							player.sendPacket(cs);
						}
					}
					break;
				}
				case "see":
				{
					WorldObject target = activeChar.getTarget();
					if ((target == null) || !target.isPlayer())
					{
						activeChar.sendPacket(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET);
						break;
					}
					Player targetPlayer = target.getActingPlayer();
					if (targetPlayer.getLevel() < Config.WORLD_CHAT_MIN_LEVEL)
					{
						BuilderUtil.sendSysMessage(activeChar, "Your target's level is below the minimum: " + Config.WORLD_CHAT_MIN_LEVEL);
						break;
					}
					BuilderUtil.sendSysMessage(activeChar, targetPlayer.getName() + ": has used world chat " + targetPlayer.getWorldChatUsed() + " times out of maximum " + targetPlayer.getWorldChatPoints() + " times.");
					break;
				}
				case "set":
				{
					WorldObject target = activeChar.getTarget();
					if ((target == null) || !target.isPlayer())
					{
						activeChar.sendPacket(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET);
						break;
					}
					
					Player targetPlayer = target.getActingPlayer();
					if (targetPlayer.getLevel() < Config.WORLD_CHAT_MIN_LEVEL)
					{
						BuilderUtil.sendSysMessage(activeChar, "Your target's level is below the minimum: " + Config.WORLD_CHAT_MIN_LEVEL);
						break;
					}
					
					if (!st.hasMoreTokens())
					{
						BuilderUtil.sendSysMessage(activeChar, "Incorrect syntax, use: //worldchat set <times used>");
						break;
					}
					
					String valueToken = st.nextToken();
					if (!Util.isDigit(valueToken))
					{
						BuilderUtil.sendSysMessage(activeChar, "Incorrect syntax, use: //worldchat set <times used>");
						break;
					}
					
					BuilderUtil.sendSysMessage(activeChar, targetPlayer.getName() + ": times used changed from " + targetPlayer.getWorldChatPoints() + " to " + valueToken);
					targetPlayer.setWorldChatUsed(int.Parse(valueToken));
					if (Config.ENABLE_WORLD_CHAT)
					{
						targetPlayer.sendPacket(new ExWorldCharCntPacket(targetPlayer));
					}
					break;
				}
				default:
				{
					BuilderUtil.sendSysMessage(activeChar, "Possible commands:");
					BuilderUtil.sendSysMessage(activeChar, " - Send message: //worldchat shout <text>");
					BuilderUtil.sendSysMessage(activeChar, " - See your target's points: //worldchat see");
					BuilderUtil.sendSysMessage(activeChar, " - Change your target's points: //worldchat set <points>");
					break;
				}
			}
		}
		else if (command.startsWith("admin_gmon"))
		{
			// TODO why is this empty?
		}
		return true;
	}
	
	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
	
	private void showMainPage(Player activeChar, String command)
	{
		int mode = 0;
		String filename = null;
		try
		{
			mode = int.Parse(command.Substring(11));
		}
		catch (Exception e)
		{
			// Not important.
		}
		switch (mode)
		{
			case 1:
			{
				filename = "main";
				break;
			}
			case 2:
			{
				filename = "game";
				break;
			}
			case 3:
			{
				filename = "effects";
				break;
			}
			case 4:
			{
				filename = "server";
				break;
			}
			case 5:
			{
				filename = "mods";
				break;
			}
			case 6:
			{
				filename = "char";
				break;
			}
			case 7:
			{
				filename = "gm";
				break;
			}
			default:
			{
				filename = "main";
				break;
			}
		}
		
		AdminHtml.showAdminHtml(activeChar, filename + "_menu.htm");
	}
	
	private void showConfigPage(Player activeChar)
	{
		StringBuilder replyMSG = new StringBuilder("<html><title>L2J :: Config</title><body>");
		replyMSG.Append("<center><table width=270><tr><td width=60><button value=\"Main\" action=\"bypass -h admin_admin\" width=60 height=25 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td><td width=150>Config Server Panel</td><td width=60><button value=\"Back\" action=\"bypass -h admin_admin4\" width=60 height=25 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr></table></center><br>");
		replyMSG.Append("<center><table width=260><tr><td width=140></td><td width=40></td><td width=40></td></tr>");
		replyMSG.Append("<tr><td><font color=\"00AA00\">Drop:</font></td><td></td><td></td></tr>");
		replyMSG.Append("<tr><td><font color=\"LEVEL\">Rate EXP</font> = " + Config.RATE_XP + "</td><td><edit var=\"param1\" width=40 height=15></td><td><button value=\"Set\" action=\"bypass -h admin_setconfig RateXp $param1\" width=40 height=25 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr>");
		replyMSG.Append("<tr><td><font color=\"LEVEL\">Rate SP</font> = " + Config.RATE_SP + "</td><td><edit var=\"param2\" width=40 height=15></td><td><button value=\"Set\" action=\"bypass -h admin_setconfig RateSp $param2\" width=40 height=25 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr>");
		replyMSG.Append("<tr><td><font color=\"LEVEL\">Rate Drop Spoil</font> = " + Config.RATE_SPOIL_DROP_CHANCE_MULTIPLIER + "</td><td><edit var=\"param4\" width=40 height=15></td><td><button value=\"Set\" action=\"bypass -h admin_setconfig RateDropSpoil $param4\" width=40 height=25 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr>");
		replyMSG.Append("<tr><td width=140></td><td width=40></td><td width=40></td></tr>");
		replyMSG.Append("</table></body></html>");
		
		NpcHtmlMessagePacket adminReply = new NpcHtmlMessagePacket(replyMSG.ToString());
		activeChar.sendPacket(adminReply);
	}
}
