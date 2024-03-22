// using L2Dn.GameServer.Handlers;
// using L2Dn.GameServer.Model.Actor;
// using L2Dn.GameServer.Utilities;
//
// namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;
//
// /**
//  * This class handles the admin commands that acts on the login
//  * @version $Revision: 1.2.2.1.2.4 $ $Date: 2007/07/31 10:05:56 $
//  */
// public class AdminLogin: IAdminCommandHandler
// {
// 	private static readonly string[] ADMIN_COMMANDS =
// 	{
// 		"admin_server_gm_only",
// 		"admin_server_all",
// 		"admin_server_max_player",
// 		"admin_server_list_type",
// 		"admin_server_list_age",
// 		"admin_server_login"
// 	};
// 	
// 	public bool useAdminCommand(String command, Player activeChar)
// 	{
// 		if (command.equals("admin_server_gm_only"))
// 		{
// 			gmOnly();
// 			BuilderUtil.sendSysMessage(activeChar, "Server is now GM only");
// 			showMainPage(activeChar);
// 		}
// 		else if (command.equals("admin_server_all"))
// 		{
// 			allowToAll();
// 			BuilderUtil.sendSysMessage(activeChar, "Server is not GM only anymore");
// 			showMainPage(activeChar);
// 		}
// 		else if (command.startsWith("admin_server_max_player"))
// 		{
// 			StringTokenizer st = new StringTokenizer(command);
// 			if (st.countTokens() > 1)
// 			{
// 				st.nextToken();
// 				String number = st.nextToken();
// 				try
// 				{
// 					LoginServerThread.getInstance().setMaxPlayer(int.Parse(number));
// 					BuilderUtil.sendSysMessage(activeChar, "maxPlayer set to " + number);
// 					showMainPage(activeChar);
// 				}
// 				catch (FormatException e)
// 				{
// 					BuilderUtil.sendSysMessage(activeChar, "Max players must be a number.");
// 				}
// 			}
// 			else
// 			{
// 				BuilderUtil.sendSysMessage(activeChar, "Format is server_max_player <max>");
// 			}
// 		}
// 		else if (command.startsWith("admin_server_list_type"))
// 		{
// 			StringTokenizer st = new StringTokenizer(command);
// 			int tokens = st.countTokens();
// 			if (tokens > 1)
// 			{
// 				st.nextToken();
// 				String[] modes = new String[tokens - 1];
// 				for (int i = 0; i < (tokens - 1); i++)
// 				{
// 					modes[i] = st.nextToken().Trim();
// 				}
// 				int newType = 0;
// 				try
// 				{
// 					newType = int.Parse(modes[0]);
// 				}
// 				catch (FormatException e)
// 				{
// 					newType = Config.getServerTypeId(modes);
// 				}
// 				
// 				if (Config.SERVER_LIST_TYPE != newType)
// 				{
// 					Config.SERVER_LIST_TYPE = newType;
// 					LoginServerThread.getInstance().sendServerType();
// 					BuilderUtil.sendSysMessage(activeChar, "Server Type changed to " + getServerTypeName(newType));
// 					showMainPage(activeChar);
// 				}
// 				else
// 				{
// 					BuilderUtil.sendSysMessage(activeChar, "Server Type is already " + getServerTypeName(newType));
// 					showMainPage(activeChar);
// 				}
// 			}
// 			else
// 			{
// 				BuilderUtil.sendSysMessage(activeChar, "Format is server_list_type <normal/relax/test/nolabel/restricted/event/free>");
// 			}
// 		}
// 		else if (command.startsWith("admin_server_list_age"))
// 		{
// 			StringTokenizer st = new StringTokenizer(command);
// 			if (st.countTokens() > 1)
// 			{
// 				st.nextToken();
// 				String mode = st.nextToken();
// 				int age = 0;
// 				try
// 				{
// 					age = int.Parse(mode);
// 					if (Config.SERVER_LIST_AGE != age)
// 					{
// 						Config.SERVER_LIST_TYPE = age;
// 						LoginServerThread.getInstance().sendServerStatus(ServerStatus.SERVER_AGE, age);
// 						BuilderUtil.sendSysMessage(activeChar, "Server Age changed to " + age);
// 						showMainPage(activeChar);
// 					}
// 					else
// 					{
// 						BuilderUtil.sendSysMessage(activeChar, "Server Age is already " + age);
// 						showMainPage(activeChar);
// 					}
// 				}
// 				catch (FormatException e)
// 				{
// 					BuilderUtil.sendSysMessage(activeChar, "Age must be a number");
// 				}
// 			}
// 			else
// 			{
// 				BuilderUtil.sendSysMessage(activeChar, "Format is server_list_age <number>");
// 			}
// 		}
// 		else if (command.equals("admin_server_login"))
// 		{
// 			showMainPage(activeChar);
// 		}
// 		return true;
// 	}
// 	
// 	/**
// 	 * @param activeChar
// 	 */
// 	private void showMainPage(Player activeChar)
// 	{
// 		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(0, 1);
// 		html.setFile(activeChar, "data/html/admin/login.htm");
// 		html.replace("%server_name%", LoginServerThread.getInstance().getServerName());
// 		html.replace("%status%", LoginServerThread.getInstance().getStatusString());
// 		html.replace("%clock%", getServerTypeName(Config.SERVER_LIST_TYPE));
// 		html.replace("%brackets%", String.valueOf(Config.SERVER_LIST_BRACKET));
// 		html.replace("%max_players%", String.valueOf(LoginServerThread.getInstance().getMaxPlayer()));
// 		activeChar.sendPacket(html);
// 	}
// 	
// 	private String getServerTypeName(int serverType)
// 	{
// 		String nameType = "";
// 		for (int i = 0; i < 7; i++)
// 		{
// 			int currentType = serverType & (int) Math.Pow(2, i);
// 			if (currentType > 0)
// 			{
// 				if (!nameType.isEmpty())
// 				{
// 					nameType += "+";
// 				}
// 				
// 				switch (currentType)
// 				{
// 					case 0x01:
// 					{
// 						nameType += "Normal";
// 						break;
// 					}
// 					case 0x02:
// 					{
// 						nameType += "Relax";
// 						break;
// 					}
// 					case 0x04:
// 					{
// 						nameType += "Test";
// 						break;
// 					}
// 					case 0x08:
// 					{
// 						nameType += "NoLabel";
// 						break;
// 					}
// 					case 0x10:
// 					{
// 						nameType += "Restricted";
// 						break;
// 					}
// 					case 0x20:
// 					{
// 						nameType += "Event";
// 						break;
// 					}
// 					case 0x40:
// 					{
// 						nameType += "Free";
// 						break;
// 					}
// 				}
// 			}
// 		}
// 		return nameType;
// 	}
// 	
// 	private void allowToAll()
// 	{
// 		LoginServerThread.getInstance().setServerStatus(ServerStatus.STATUS_AUTO);
// 		Config.SERVER_GMONLY = false;
// 	}
// 	
// 	private void gmOnly()
// 	{
// 		LoginServerThread.getInstance().setServerStatus(ServerStatus.STATUS_GM_ONLY);
// 		Config.SERVER_GMONLY = true;
// 	}
// 	
// 	public String[] getAdminCommandList()
// 	{
// 		return ADMIN_COMMANDS;
// 	}
// }
