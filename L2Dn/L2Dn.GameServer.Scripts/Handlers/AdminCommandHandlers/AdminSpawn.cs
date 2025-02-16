using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands: - show_spawns = shows menu - spawn_index lvl = shows menu for monsters with respective level - spawn_monster id = spawns monster id on target
 * @version $Revision: 1.2.2.5.2.5 $ $Date: 2005/04/11 10:06:06 $
 */
public class AdminSpawn: IAdminCommandHandler
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AdminSpawn));
	
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_show_spawns",
		"admin_spawnat",
		"admin_spawn",
		"admin_spawn_monster",
		"admin_spawn_index",
		"admin_unspawnall",
		"admin_respawnall",
		"admin_spawn_reload",
		"admin_npc_index",
		"admin_spawn_once",
		"admin_show_npcs",
		"admin_instance_spawns",
		"admin_list_spawns",
		"admin_list_positions",
		"admin_spawn_debug_menu",
		"admin_spawn_debug_print",
		"admin_spawn_debug_print_menu",
		"admin_topspawncount",
		"admin_top_spawn_count"
	};
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		if (command.equals("admin_show_spawns"))
		{
			AdminHtml.showAdminHtml(activeChar, "spawns.htm");
		}
		else if (command.equalsIgnoreCase("admin_spawn_debug_menu"))
		{
			AdminHtml.showAdminHtml(activeChar, "spawns_debug.htm");
		}
		else if (command.startsWith("admin_spawn_debug_print"))
		{
			StringTokenizer st = new StringTokenizer(command, " ");
			WorldObject target = activeChar.getTarget();
			if (target is Npc)
			{
				try
				{
					st.nextToken();
					int type = int.Parse(st.nextToken());
					printSpawn((Npc) target, type);
					if (command.contains("_menu"))
					{
						AdminHtml.showAdminHtml(activeChar, "spawns_debug.htm");
					}
				}
				catch (Exception e)
				{
				}
			}
			else
			{
				activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			}
		}
		else if (command.startsWith("admin_spawn_index"))
		{
			StringTokenizer st = new StringTokenizer(command, " ");
			try
			{
				st.nextToken();
				int level = int.Parse(st.nextToken());
				int from = 0;
				try
				{
					from = int.Parse(st.nextToken());
				}
				catch (Exception nsee)
				{
				}
				showMonsters(activeChar, level, from);
			}
			catch (Exception e)
			{
				AdminHtml.showAdminHtml(activeChar, "spawns.htm");
			}
		}
		else if (command.equals("admin_show_npcs"))
		{
			AdminHtml.showAdminHtml(activeChar, "npcs.htm");
		}
		else if (command.startsWith("admin_npc_index"))
		{
			StringTokenizer st = new StringTokenizer(command, " ");
			try
			{
				st.nextToken();
				string letter = st.nextToken();
				int from = 0;
				try
				{
					from = int.Parse(st.nextToken());
				}
				catch (Exception nsee)
				{
				}
				showNpcs(activeChar, letter, from);
			}
			catch (Exception e)
			{
				AdminHtml.showAdminHtml(activeChar, "npcs.htm");
			}
		}
		else if (command.startsWith("admin_instance_spawns"))
		{
			StringTokenizer st = new StringTokenizer(command, " ");
			try
			{
				st.nextToken();
				int instance = int.Parse(st.nextToken());
				if (instance >= 300000)
				{
					StringBuilder html = new StringBuilder(1500);
					html.Append("<html><table width=\"100%\"><tr><td width=45><button value=\"Main\" action=\"bypass admin_admin\" width=45 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td><td width=180><center><font color=\"LEVEL\">Spawns for " + instance + "</font></td><td width=45><button value=\"Back\" action=\"bypass -h admin_current_player\" width=45 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr></table><br><table width=\"100%\"><tr><td width=200>NpcName</td><td width=70>Action</td></tr>");
					int counter = 0;
					int skiped = 0;
					Instance inst = InstanceManager.getInstance().getInstance(instance);
					if (inst != null)
					{
						foreach (Npc npc in inst.getNpcs())
						{
							if (!npc.isDead())
							{
								// Only 50 because of client html limitation
								if (counter < 50)
								{
									html.Append("<tr><td>" + npc.getName() + "</td><td><a action=\"bypass -h admin_move_to " + npc.getX() + " " + npc.getY() + " " + npc.getZ() + "\">Go</a></td></tr>");
									counter++;
								}
								else
								{
									skiped++;
								}
							}
						}
						html.Append("<tr><td>Skipped:</td><td>" + skiped + "</td></tr></table></body></html>");
						
						HtmlContent htmlContent = HtmlContent.LoadFromText(html.ToString(), activeChar);
						NpcHtmlMessagePacket ms = new NpcHtmlMessagePacket(null, 1, htmlContent);
						activeChar.sendPacket(ms);
					}
					else
					{
						BuilderUtil.sendSysMessage(activeChar, "Cannot find instance " + instance);
					}
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "Invalid instance number.");
				}
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Usage //instance_spawns <instance_number>");
			}
		}
		else if (command.startsWith("admin_unspawnall"))
		{
			Broadcast.toAllOnlinePlayers(new SystemMessagePacket(SystemMessageId.THE_NPC_SERVER_IS_NOT_OPERATING_AT_THIS_TIME));
			// Unload all scripts.
			QuestManager.getInstance().unloadAllScripts();
			// Unload all zones.
			ZoneManager.getInstance().unload();
			// Delete all spawns.
			foreach (Npc npc in DbSpawnManager.getInstance().getNpcs())
			{
				if (npc != null)
				{
					DbSpawnManager.getInstance().deleteSpawn(npc.getSpawn(), true);
					npc.deleteMe();
				}
			}
			DbSpawnManager.getInstance().cleanUp();
			foreach (WorldObject obj in World.getInstance().getVisibleObjects())
			{
				if ((obj != null) && obj.isNpc())
				{
					Npc target = (Npc) obj;
					target.deleteMe();
					Spawn spawn = target.getSpawn();
					if (spawn != null)
					{
						spawn.stopRespawn();
						SpawnTable.getInstance().deleteSpawn(spawn, false);
					}
				}
			}
			// Reload.
			ZoneManager.getInstance().reload();
			QuestManager.getInstance().reloadAllScripts();
			AdminData.getInstance().broadcastMessageToGMs("NPC unspawn completed!");
		}
		else if (command.startsWith("admin_respawnall") || command.startsWith("admin_spawn_reload"))
		{
			// Unload all scripts.
			QuestManager.getInstance().unloadAllScripts();
			// Unload all zones.
			ZoneManager.getInstance().unload();
			// Delete all spawns.
			foreach (Npc npc in DbSpawnManager.getInstance().getNpcs())
			{
				if (npc != null)
				{
					DbSpawnManager.getInstance().deleteSpawn(npc.getSpawn(), true);
					npc.deleteMe();
				}
			}
			DbSpawnManager.getInstance().cleanUp();
			foreach (WorldObject obj in World.getInstance().getVisibleObjects())
			{
				if ((obj != null) && obj.isNpc())
				{
					Npc target = (Npc) obj;
					target.deleteMe();
					Spawn spawn = target.getSpawn();
					if (spawn != null)
					{
						spawn.stopRespawn();
						SpawnTable.getInstance().deleteSpawn(spawn, false);
					}
				}
			}
			// Reload.
			SpawnData.getInstance().init();
			DbSpawnManager.getInstance().load();
			ZoneManager.getInstance().reload();
			QuestManager.getInstance().reloadAllScripts();
			AdminData.getInstance().broadcastMessageToGMs("NPC respawn completed!");
		}
		else if (command.startsWith("admin_spawnat"))
		{
			StringTokenizer st = new StringTokenizer(command, " ");
			try
			{
				st.nextToken();
				string id = st.nextToken();
				string x = st.nextToken();
				string y = st.nextToken();
				string z = st.nextToken();
				int h = activeChar.getHeading();
				if (st.hasMoreTokens())
				{
					h = int.Parse(st.nextToken());
				}
				spawnMonster(activeChar, int.Parse(id), int.Parse(x), int.Parse(y), int.Parse(z), h);
			}
			catch (Exception e)
			{
				// Case of wrong or missing monster data.
				AdminHtml.showAdminHtml(activeChar, "spawns.htm");
			}
		}
		else if (command.startsWith("admin_spawn_monster") || command.startsWith("admin_spawn"))
		{
			try
			{
				// Create a StringTokenizer to Split the command by spaces.
				StringTokenizer st = new StringTokenizer(command, " ");
				
				// Get the first token (the command itself).
				string cmd = st.nextToken();
				
				// Get the second token (the NPC ID or name).
				string npcId = st.nextToken();
				
				// If the second token is not a digit, search for the NPC template by name.
				if (!int.TryParse(npcId, CultureInfo.InvariantCulture, out int _))
				{
					// Initialize the variables.
					StringBuilder searchParam = new StringBuilder();
					string[] pars = command.Split(" ");
					NpcTemplate searchTemplate = null;
					NpcTemplate template = null;
					int pos = 1;
					
					// Iterate through the command parameters, starting from the second one.
					for (int i = 1; i < pars.Length; i++)
					{
						// Add the current parameter to the search parameter string.
						searchParam.Append(pars[i]);
						searchParam.Append(" ");
						
						// Try to get the NPC template using the search parameter string.
						searchTemplate = NpcData.getInstance().getTemplateByName(searchParam.ToString().Trim());
						
						// If the template is found, update the position and the template.
						if (searchTemplate != null)
						{
							template = searchTemplate;
							pos = i;
						}
					}
					
					// Check if an NPC template was found.
					if (template != null)
					{
						// Skip tokens that contain the name.
						for (int i = 1; i < pos; i++)
						{
							st.nextToken();
						}
						
						// Set the npcId based on template found.
						npcId = template.getId().ToString();
					}
				}
				
				// Initialize mobCount to 1.
				int mobCount = 1;
				
				// If next token exists, set the mobCount value.
				if (st.hasMoreTokens())
				{
					mobCount = int.Parse(st.nextToken());
				}
				
				// Initialize respawnTime to 60.
				int respawnTime = 60;
				
				// If next token exists, set the respawnTime value.
				if (st.hasMoreTokens())
				{
					respawnTime = int.Parse(st.nextToken());
				}
				
				// Call the spawnMonster method with the appropriate parameters.
				spawnMonster(activeChar, npcId, respawnTime, mobCount, !cmd.equalsIgnoreCase("admin_spawn_once"));
			}
			catch (Exception e)
			{
				// Case of wrong or missing monster data.
				AdminHtml.showAdminHtml(activeChar, "spawns.htm");
			}
		}
		else if (command.startsWith("admin_list_spawns") || command.startsWith("admin_list_positions"))
		{
			int npcId = 0;
			int teleportIndex = -1;
			
			try
			{
				// Split the command into an array of words.
				string[] pars = command.Split(" ");
				StringBuilder searchParam = new StringBuilder();
				int pos = -1;
				
				// Concatenate all words in the command except the first and last word.
				foreach (string param in pars)
				{
					pos++;
					if ((pos > 0) && (pos < (pars.Length - 1)))
					{
						searchParam.Append(param);
						searchParam.Append(" ");
					}
				}
				
				string searchString = searchParam.ToString().Trim();
				// If the search string is a number, use it as the NPC ID.
				if (!int.TryParse(searchString, CultureInfo.InvariantCulture, out npcId))
				{
					// Otherwise, use it as the NPC name and look up the NPC ID.
					npcId = NpcData.getInstance().getTemplateByName(searchString).getId();
				}
				
				// If there are more than two words in the command, try to parse the last word as the teleport index.
				if (pars.Length > 2)
				{
					string lastParam = pars[^1];
					if (int.TryParse(lastParam, CultureInfo.InvariantCulture, out int value))
					{
						teleportIndex = value;
					}
				}
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Command format is //list_spawns <npcId|npc_name> [tele_index]");
			}
			
			// Call the findNpcs method with the parsed NPC ID and teleport index.
			findNpcs(activeChar, npcId, teleportIndex, command.startsWith("admin_list_positions"));
		}
		else if (command.startsWith("admin_topspawncount") || command.startsWith("admin_top_spawn_count"))
		{
			StringTokenizer st = new StringTokenizer(command, " ");
			st.nextToken();
			int count = 5;
			if (st.hasMoreTokens())
			{
				string nextToken = st.nextToken();
				if (int.TryParse(nextToken, CultureInfo.InvariantCulture, out int v))
				{
					count = v;
				}
				if (count <= 0)
				{
					return true;
				}
			}
			Map<int, int> npcsFound = new();
			foreach (WorldObject obj in World.getInstance().getVisibleObjects())
			{
				if (!obj.isNpc())
				{
					continue;
				}
				int npcId = obj.getId();
				if (npcsFound.ContainsKey(npcId))
				{
					npcsFound.put(npcId, npcsFound.get(npcId) + 1);
				}
				else
				{
					npcsFound.put(npcId, 1);
				}
			}
			BuilderUtil.sendSysMessage(activeChar, "Top " + count + " spawn count.");
			foreach (var entry in npcsFound.OrderByDescending(r => r.Value))
			{
				count--;
				if (count < 0)
				{
					break;
				}
				int npcId = entry.Key;
				BuilderUtil.sendSysMessage(activeChar, NpcData.getInstance().getTemplate(npcId).getName() + " (" + npcId + "): " + entry.Value);
			}
		}
		return true;
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
	
	/**
	 * Get all the spawn of a NPC.
	 * @param activeChar
	 * @param npcId
	 * @param teleportIndex
	 * @param showposition
	 */
	private void findNpcs(Player activeChar, int npcId, int teleportIndex, bool showposition)
	{
		int index = 0;
		foreach (Spawn spawn in SpawnTable.getInstance().getSpawns(npcId))
		{
			index++;
			Npc npc = spawn.getLastSpawn();
			if (teleportIndex > -1)
			{
				if (teleportIndex == index)
				{
					if (showposition && (npc != null))
					{
						activeChar.teleToLocation(npc.Location, true);
					}
					else
					{
						activeChar.teleToLocation(spawn.Location, true);
					}
				}
			}
			else if (showposition && (npc != null))
			{
				activeChar.sendMessage(index + " - " + spawn.getTemplate().getName() + " (" + spawn + "): " + npc.getX() + " " + npc.getY() + " " + npc.getZ());
			}
			else
			{
				activeChar.sendMessage(index + " - " + spawn.getTemplate().getName() + " (" + spawn + "): " +
					spawn.Location.X + " " + spawn.Location.Y + " " + spawn.Location.Z);
			}
		}
		
		if (index == 0)
		{
			activeChar.sendMessage(GetType().Name + ": No current spawns found.");
		}
	}
	
	private void printSpawn(Npc target, int type)
	{
		int i = target.getId();
		int x = target.getSpawn().Location.X;
		int y = target.getSpawn().Location.Y;
		int z = target.getSpawn().Location.Z;
		int h = target.getSpawn().Location.Heading;
		switch (type)
		{
			default:
			case 0:
			{
				LOGGER.Info("('',1," + i + "," + x + "," + y + "," + z + ",0,0," + h + ",60,0,0),");
				break;
			}
			case 1:
			{
				LOGGER.Info("<spawn npcId=\"" + i + "\" x=\"" + x + "\" y=\"" + y + "\" z=\"" + z + "\" heading=\"" + h + "\" respawn=\"0\" />");
				break;
			}
			case 2:
			{
				LOGGER.Info("{ " + i + ", " + x + ", " + y + ", " + z + ", " + h + " },");
				break;
			}
		}
	}
	
	private void spawnMonster(Player activeChar, string monsterIdValue, int respawnTime, int mobCount, bool permanentValue)
	{
		WorldObject target = activeChar.getTarget();
		if (target == null)
		{
			target = activeChar;
		}
		
		NpcTemplate template1;
		string monsterId = monsterIdValue;
		if (Regex.IsMatch(monsterId, "[0-9]+"))
		{
			// First parameter was an ID number
			int monsterTemplate = int.Parse(monsterId);
			template1 = NpcData.getInstance().getTemplate(monsterTemplate);
		}
		else
		{
			// First parameter wasn't just numbers so go by name not ID
			monsterId = monsterId.Replace('_', ' ');
			template1 = NpcData.getInstance().getTemplateByName(monsterId);
		}
		
		if (!Config.FAKE_PLAYERS_ENABLED && template1.isFakePlayer())
		{
			activeChar.sendPacket(SystemMessageId.YOUR_TARGET_CANNOT_BE_FOUND);
			return;
		}
		
		try
		{
			Spawn spawn = new Spawn(template1);
			spawn.Location = target.Location;
			spawn.setAmount(mobCount);
			spawn.setRespawnDelay(TimeSpan.FromSeconds(respawnTime));
			
			bool permanent = permanentValue;
			if (activeChar.isInInstance())
			{
				spawn.setInstanceId(activeChar.getInstanceId());
				permanent = false;
			}
			
			SpawnTable.getInstance().addNewSpawn(spawn, permanent);
			spawn.init();
			
			if (!permanent || (respawnTime <= 0))
			{
				spawn.stopRespawn();
			}
			
			spawn.getLastSpawn().broadcastInfo();
			BuilderUtil.sendSysMessage(activeChar, "Created " + template1.getName() + " on " + target.ObjectId);
		}
		catch (Exception e)
		{
			activeChar.sendPacket(SystemMessageId.YOUR_TARGET_CANNOT_BE_FOUND);
		}
	}
	
	private void spawnMonster(Player activeChar, int id, int x, int y, int z, int h)
	{
		WorldObject target = activeChar.getTarget();
		if (target == null)
		{
			target = activeChar;
		}
		
		NpcTemplate template1 = NpcData.getInstance().getTemplate(id);
		if (!Config.FAKE_PLAYERS_ENABLED && template1.isFakePlayer())
		{
			activeChar.sendPacket(SystemMessageId.YOUR_TARGET_CANNOT_BE_FOUND);
			return;
		}
		
		try
		{
			Spawn spawn = new Spawn(template1);
			spawn.Location = new Location(x, y, z, h);
			spawn.setAmount(1);
			spawn.setRespawnDelay(TimeSpan.FromSeconds(60));
			if (activeChar.isInInstance())
			{
				spawn.setInstanceId(activeChar.getInstanceId());
			}
			
			SpawnTable.getInstance().addNewSpawn(spawn, true);
			spawn.init();
			
			if (activeChar.isInInstance())
			{
				spawn.stopRespawn();
			}
			spawn.getLastSpawn().broadcastInfo();
			BuilderUtil.sendSysMessage(activeChar, "Created " + template1.getName() + " on " + target.ObjectId);
		}
		catch (Exception e)
		{
			activeChar.sendPacket(SystemMessageId.YOUR_TARGET_CANNOT_BE_FOUND);
		}
	}
	
	private void showMonsters(Player activeChar, int level, int from)
	{
		List<NpcTemplate> mobs = NpcData.getInstance().getAllMonstersOfLevel(level);
		int mobsCount = mobs.Count;
		StringBuilder tb = new StringBuilder(500 + (mobsCount * 80));
		tb.Append("<html><title>Spawn Monster:</title><body><p> Level : " + level + "<br>Total NPCs : " + mobsCount + "<br>");
		
		// Loop
		int i = from;
		for (int j = 0; (i < mobsCount) && (j < 50); i++, j++)
		{
			tb.Append("<a action=\"bypass -h admin_spawn_monster " + mobs[i].getId() + "\">" + mobs[i].getName() + "</a><br1>");
		}
		
		if (i == mobsCount)
		{
			tb.Append("<br><center><button value=\"Back\" action=\"bypass -h admin_show_spawns\" width=40 height=15 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></center></body></html>");
		}
		else
		{
			tb.Append("<br><center><button value=\"Next\" action=\"bypass -h admin_spawn_index " + level + " " + i + "\" width=40 height=15 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"><button value=\"Back\" action=\"bypass -h admin_show_spawns\" width=40 height=15 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></center></body></html>");
		}
		
		HtmlContent htmlContent = HtmlContent.LoadFromText(tb.ToString(), activeChar);
		activeChar.sendPacket(new NpcHtmlMessagePacket(null, 1, htmlContent));
	}
	
	private void showNpcs(Player activeChar, string starting, int from)
	{
		List<NpcTemplate> mobs = NpcData.getInstance().getAllNpcStartingWith(starting);
		int mobsCount = mobs.Count;
		StringBuilder tb = new StringBuilder(500 + (mobsCount * 80));
		tb.Append("<html><title>Spawn Monster:</title><body><p> There are " + mobsCount + " Npcs whose name starts with " + starting + ":<br>");
		
		// Loop
		int i = from;
		for (int j = 0; (i < mobsCount) && (j < 50); i++, j++)
		{
			tb.Append("<a action=\"bypass -h admin_spawn_monster " + mobs[i].getId() + "\">" + mobs[i].getName() + "</a><br1>");
		}
		
		if (i == mobsCount)
		{
			tb.Append("<br><center><button value=\"Back\" action=\"bypass -h admin_show_npcs\" width=40 height=15 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></center></body></html>");
		}
		else
		{
			tb.Append("<br><center><button value=\"Next\" action=\"bypass -h admin_npc_index " + starting + " " + i + "\" width=40 height=15 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"><button value=\"Back\" action=\"bypass -h admin_show_npcs\" width=40 height=15 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></center></body></html>");
		}
		
		HtmlContent htmlContent = HtmlContent.LoadFromText(tb.ToString(), activeChar);
		activeChar.sendPacket(new NpcHtmlMessagePacket(null, 1, htmlContent));
	}
}
