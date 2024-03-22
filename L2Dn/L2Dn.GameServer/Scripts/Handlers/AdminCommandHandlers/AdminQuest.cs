// using System.Text;
// using L2Dn.GameServer.Data;
// using L2Dn.GameServer.Handlers;
// using L2Dn.GameServer.InstanceManagers;
// using L2Dn.GameServer.Model.Actor;
// using L2Dn.GameServer.Model.Quests;
// using L2Dn.GameServer.Network.Enums;
// using L2Dn.GameServer.Network.OutgoingPackets;
// using L2Dn.GameServer.Scripting;
// using L2Dn.GameServer.Utilities;
// using L2Dn.Utilities;
// using NLog;
//
// namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;
//
// public class AdminQuest: IAdminCommandHandler
// {
// 	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AdminQuest));
// 	
// 	private static readonly string[] ADMIN_COMMANDS =
// 	{
// 		"admin_quest_reload",
// 		"admin_script_load",
// 		"admin_script_unload",
// 		"admin_script_dir",
// 		"admin_show_quests",
// 		"admin_quest_info"
// 	};
// 	
// 	private static Quest findScript(String script)
// 	{
// 		if (Util.isDigit(script))
// 		{
// 			return QuestManager.getInstance().getQuest(int.Parse(script));
// 		}
// 		return QuestManager.getInstance().getQuest(script);
// 	}
// 	
// 	public bool useAdminCommand(String command, Player activeChar)
// 	{
// 		if (command.startsWith("admin_quest_reload"))
// 		{
// 			StringTokenizer st = new StringTokenizer(command);
// 			st.nextToken(); // skip command token
// 			if (!st.hasMoreTokens())
// 			{
// 				BuilderUtil.sendSysMessage(activeChar, "Usage: //quest_reload <questName> or <questId>");
// 				return false;
// 			}
// 			
// 			String script = st.nextToken();
// 			Quest quest = findScript(script);
// 			if (quest == null)
// 			{
// 				BuilderUtil.sendSysMessage(activeChar, "The script " + script + " couldn't be found!");
// 				return false;
// 			}
// 			
// 			if (!quest.reload())
// 			{
// 				BuilderUtil.sendSysMessage(activeChar, "Failed to reload " + script + "!");
// 				return false;
// 			}
// 			
// 			BuilderUtil.sendSysMessage(activeChar, "Script successful reloaded.");
// 		}
// 		else if (command.startsWith("admin_script_load"))
// 		{
// 			StringTokenizer st = new StringTokenizer(command);
// 			st.nextToken(); // skip command token
// 			if (!st.hasMoreTokens())
// 			{
// 				BuilderUtil.sendSysMessage(activeChar, "Usage: //script_load path/to/script.cs");
// 				return false;
// 			}
// 			
// 			String script = st.nextToken();
// 			try
// 			{
// 				ScriptEngineManager.getInstance().executeScript(Paths.get(script));
// 				BuilderUtil.sendSysMessage(activeChar, "Script loaded seccessful!");
// 			}
// 			catch (Exception e)
// 			{
// 				BuilderUtil.sendSysMessage(activeChar, "Failed to load script!");
// 				LOGGER.Warn("Failed to load script " + script + "!", e);
// 			}
// 		}
// 		else if (command.startsWith("admin_script_unload"))
// 		{
// 			StringTokenizer st = new StringTokenizer(command);
// 			st.nextToken(); // skip command token
// 			if (!st.hasMoreTokens())
// 			{
// 				BuilderUtil.sendSysMessage(activeChar, "Usage: //script_load path/to/script.java");
// 				return false;
// 			}
// 			
// 			String script = st.nextToken();
// 			Quest quest = findScript(script);
// 			if (quest == null)
// 			{
// 				BuilderUtil.sendSysMessage(activeChar, "The script " + script + " couldn't be found!");
// 				return false;
// 			}
// 			
// 			quest.unload();
// 			BuilderUtil.sendSysMessage(activeChar, "Script successful unloaded!");
// 		}
// 		else if (command.startsWith("admin_script_dir"))
// 		{
// 			String[] parts = command.Split(" ");
// 			if (parts.Length == 1)
// 			{
// 				showDir(null, activeChar);
// 			}
// 			else
// 			{
// 				showDir(parts[1], activeChar);
// 			}
// 		}
// 		else if (command.startsWith("admin_show_quests"))
// 		{
// 			if (activeChar.getTarget() == null)
// 			{
// 				BuilderUtil.sendSysMessage(activeChar, "Get a target first.");
// 			}
// 			else if (!activeChar.getTarget().isCreature())
// 			{
// 				BuilderUtil.sendSysMessage(activeChar, "Invalid Target.");
// 			}
// 			else
// 			{
// 				Creature creature = (Creature) activeChar.getTarget();
// 				StringBuilder sb = new StringBuilder();
// 				Set<String> questNames = new();
// 				foreach (EventType type in EnumUtil.GetValues<EventType>())
// 				{
// 					foreach (AbstractEventListener listener in creature.getListeners(type))
// 					{
// 						if (listener.getOwner() is Quest)
// 						{
// 							Quest quest = (Quest) listener.getOwner();
// 							if (!questNames.add(quest.getName()))
// 							{
// 								continue;
// 							}
// 							sb.Append("<tr><td colspan=\"4\"><font color=\"LEVEL\"><a action=\"bypass -h admin_quest_info " + quest.getName() + "\">" + quest.getName() + "</a></font></td></tr>");
// 						}
// 					}
// 				}
//
// 				HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, "html/admin/npc-quests.htm");
// 				NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(0, 1, helper);
// 				helper.Replace("%quests%", sb.ToString());
// 				helper.Replace("%objid%", creature.getObjectId().ToString());
// 				helper.Replace("%questName%", "");
// 				activeChar.sendPacket(msg);
// 			}
// 		}
// 		else if (command.startsWith("admin_quest_info "))
// 		{
// 			String questName = command.Substring("admin_quest_info ".Length);
// 			Quest quest = QuestManager.getInstance().getQuest(questName);
// 			String events = "";
// 			String npcs = "";
// 			String items = "";
// 			String timers = "";
// 			int counter = 0;
// 			if (quest == null)
// 			{
// 				BuilderUtil.sendSysMessage(activeChar, "Couldn't find quest or script with name " + questName + " !");
// 				return false;
// 			}
// 			
// 			Set<EventType> listenerTypes = new();
// 			foreach (AbstractEventListener listener in quest.getListeners())
// 			{
// 				if (listenerTypes.add(listener.getType()))
// 				{
// 					events += ", " + listener.getType().name();
// 					counter++;
// 				}
// 				if (counter > 10)
// 				{
// 					counter = 0;
// 					break;
// 				}
// 			}
// 			
// 			Set<Integer> npcIds = new(quest.getRegisteredIds(ListenerRegisterType.NPC));
// 			for (int npcId : npcIds)
// 			{
// 				npcs += ", " + npcId;
// 				counter++;
// 				if (counter > 50)
// 				{
// 					counter = 0;
// 					break;
// 				}
// 			}
// 			
// 			if (!events.isEmpty())
// 			{
// 				events = listenerTypes.size() + ": " + events.Substring(2);
// 			}
// 			
// 			if (!npcs.isEmpty())
// 			{
// 				npcs = npcIds.size() + ": " + npcs.Substring(2);
// 			}
// 			
// 			if (quest.getRegisteredItemIds() != null)
// 			{
// 				for (int itemId : quest.getRegisteredItemIds())
// 				{
// 					items += ", " + itemId;
// 					counter++;
// 					if (counter > 20)
// 					{
// 						counter = 0;
// 						break;
// 					}
// 				}
// 				items = quest.getRegisteredItemIds().Length + ":" + items.Substring(2);
// 			}
// 			
// 			for (List<QuestTimer> list : quest.getQuestTimers().values())
// 			{
// 				for (QuestTimer timer : list)
// 				{
// 					timers += "<tr><td colspan=\"4\"><table width=270 border=0 bgcolor=131210><tr><td width=270><font color=\"LEVEL\">" + timer.toString() + ":</font> <font color=00FF00>Active: " + timer.isActive() + " Repeatable: " + timer.isRepeating() + " Player: " + timer.getPlayer() + " Npc: " + timer.getNpc() + "</font></td></tr></table></td></tr>";
// 					counter++;
// 					if (counter > 10)
// 					{
// 						break;
// 					}
// 				}
// 			}
// 			
// 			StringBuilder sb = new StringBuilder();
// 			sb.Append("<tr><td colspan=\"4\"><table width=270 border=0 bgcolor=131210><tr><td width=270><font color=\"LEVEL\">ID:</font> <font color=00FF00>" + quest.getId() + "</font></td></tr></table></td></tr>");
// 			sb.Append("<tr><td colspan=\"4\"><table width=270 border=0 bgcolor=131210><tr><td width=270><font color=\"LEVEL\">Name:</font> <font color=00FF00>" + quest.getName() + "</font></td></tr></table></td></tr>");
// 			sb.Append("<tr><td colspan=\"4\"><table width=270 border=0 bgcolor=131210><tr><td width=270><font color=\"LEVEL\">Path:</font> <font color=00FF00>" + quest.getPath() + "</font></td></tr></table></td></tr>");
// 			sb.Append("<tr><td colspan=\"4\"><table width=270 border=0 bgcolor=131210><tr><td width=270><font color=\"LEVEL\">Events:</font> <font color=00FF00>" + events + "</font></td></tr></table></td></tr>");
// 			if (!npcs.isEmpty())
// 			{
// 				sb.Append("<tr><td colspan=\"4\"><table width=270 border=0 bgcolor=131210><tr><td width=270><font color=\"LEVEL\">NPCs:</font> <font color=00FF00>" + npcs + "</font></td></tr></table></td></tr>");
// 			}
// 			if (!items.isEmpty())
// 			{
// 				sb.Append("<tr><td colspan=\"4\"><table width=270 border=0 bgcolor=131210><tr><td width=270><font color=\"LEVEL\">Items:</font> <font color=00FF00>" + items + "</font></td></tr></table></td></tr>");
// 			}
// 			if (!timers.isEmpty())
// 			{
// 				sb.Append("<tr><td colspan=\"4\"><table width=270 border=0 bgcolor=131210><tr><td width=270><font color=\"LEVEL\">Timers:</font> <font color=00FF00></font></td></tr></table></td></tr>");
// 				sb.Append(timers);
// 			}
// 			
// 			NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(0, 1);
// 			msg.setFile(activeChar, "data/html/admin/npc-quests.htm");
// 			msg.replace("%quests%", sb.toString());
// 			msg.replace("%questName%", "<table><tr><td width=\"50\" align=\"left\"><a action=\"bypass -h admin_quest_reload " + quest.getName() + "\">Reload</a></td> <td width=\"150\"  align=\"center\"><a action=\"bypass -h admin_quest_info " + quest.getName() + "\">" + quest.getName() + "</a></td> <td width=\"50\" align=\"right\"><a action=\"bypass -h admin_script_unload " + quest.getName() + "\">Unload</a></td></tr></table>");
// 			activeChar.sendPacket(msg);
// 		}
// 		return true;
// 	}
// 	
// 	private void showDir(String dir, Player activeChar)
// 	{
// 		String replace = null;
// 		File path;
// 		String currentPath = "/";
// 		if ((dir == null) || dir.Trim().isEmpty() || dir.contains(".."))
// 		{
// 			StringBuilder sb = new StringBuilder(200);
// 			path = ScriptEngineManager.SCRIPT_FOLDER.toFile();
// 			String[] children = path.list();
// 			Arrays.sort(children);
// 			for (String c : children)
// 			{
// 				File n = new File(path, c);
// 				if (n.isHidden() || n.getName().startsWith("."))
// 				{
// 					continue;
// 				}
// 				if (n.isDirectory())
// 				{
// 					sb.Append("<a action=\"bypass -h admin_script_dir " + c + "\">" + c + "</a><br1>");
// 				}
// 				else if (c.endsWith(".java"))
// 				{
// 					sb.Append("<a action=\"bypass -h admin_script_load " + c + "\"><font color=\"LEVEL\">" + c + "</font></a><br1>");
// 				}
// 			}
// 			replace = sb.toString();
// 		}
// 		else
// 		{
// 			path = new File(ScriptEngineManager.SCRIPT_FOLDER.toFile(), dir);
// 			if (!path.isDirectory())
// 			{
// 				BuilderUtil.sendSysMessage(activeChar, "Wrong path.");
// 				return;
// 			}
// 			currentPath = dir;
// 			bool questReducedNames = currentPath.equalsIgnoreCase("quests");
// 			StringBuilder sb = new StringBuilder(200);
// 			sb.Append("<a action=\"bypass -h admin_script_dir " + getUpPath(currentPath) + "\">..</a><br1>");
// 			String[] children = path.list();
// 			Arrays.sort(children);
// 			for (String c : children)
// 			{
// 				File n = new File(path, c);
// 				if (n.isHidden() || n.getName().startsWith("."))
// 				{
// 					continue;
// 				}
// 				if (n.isDirectory())
// 				{
// 					sb.Append("<a action=\"bypass -h admin_script_dir " + currentPath + "/" + c + "\">" + (questReducedNames ? getQuestName(c) : c) + "</a><br1>");
// 				}
// 				else if (c.endsWith(".java"))
// 				{
// 					sb.Append("<a action=\"bypass -h admin_script_load " + currentPath + "/" + c + "\"><font color=\"LEVEL\">" + c + "</font></a><br1>");
// 				}
// 			}
// 			replace = sb.toString();
// 			if (questReducedNames)
// 			{
// 				currentPath += " (limited list - HTML too long)";
// 			}
// 		}
// 		
// 		if (replace.Length() > 17200)
// 		{
// 			replace = replace.substring(0, 17200); // packetlimit
// 		}
// 		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(0, 1);
// 		html.setFile(activeChar, "data/html/admin/scriptdirectory.htm");
// 		html.replace("%path%", currentPath);
// 		html.replace("%list%", replace);
// 		activeChar.sendPacket(html);
// 	}
// 	
// 	private String getUpPath(String full)
// 	{
// 		int index = full.lastIndexOf('/');
// 		if (index == -1)
// 		{
// 			return "";
// 		}
// 		return full.substring(0, index);
// 	}
// 	
// 	private String getQuestName(String full)
// 	{
// 		return full.Split("_")[0];
// 	}
// 	
// 	@Override
// 	public String[] getAdminCommandList()
// 	{
// 		return ADMIN_COMMANDS;
// 	}
// }