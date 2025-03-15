using System.Globalization;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model.Enums;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands: - show_moves - show_teleport - teleport_to_character - move_to - teleport_character
 * @version $Revision: 1.3.2.6.2.4 $ $Date: 2005/04/11 10:06:06 $ con.close() change and small typo fix by Zoey76 24/02/2011
 */
public class AdminTeleport: IAdminCommandHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(AdminTeleport));

	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_show_moves",
		"admin_show_moves_other",
		"admin_show_teleport",
		"admin_teleport_to_character",
		"admin_teleportto",
		"admin_teleport",
		"admin_move_to",
		"admin_teleport_character",
		"admin_recall",
		"admin_walk",
		"teleportto",
		"recall",
		"admin_recall_npc",
		"admin_gonorth",
		"admin_gosouth",
		"admin_goeast",
		"admin_gowest",
		"admin_goup",
		"admin_godown",
		"admin_tele",
		"admin_teleto",
		"admin_instant_move",
		"admin_sendhome",
    ];

	public bool useAdminCommand(string command, Player activeChar)
	{
		if (command.equals("admin_instant_move"))
		{
			BuilderUtil.sendSysMessage(activeChar, "Instant move ready. Click where you want to go.");
			activeChar.setTeleMode(AdminTeleportType.DEMONIC);
		}
		else if (command.equals("admin_teleto sayune"))
		{
			BuilderUtil.sendSysMessage(activeChar, "Sayune move ready. Click where you want to go.");
			activeChar.setTeleMode(AdminTeleportType.SAYUNE);
		}
		else if (command.equals("admin_teleto charge"))
		{
			BuilderUtil.sendSysMessage(activeChar, "Charge move ready. Click where you want to go.");
			activeChar.setTeleMode(AdminTeleportType.CHARGE);
		}
		else if (command.equals("admin_teleto end"))
		{
			activeChar.setTeleMode(AdminTeleportType.NORMAL);
		}
		else if (command.equals("admin_show_moves"))
		{
			AdminHtml.showAdminHtml(activeChar, "teleports.htm");
		}
		else if (command.equals("admin_show_moves_other"))
		{
			AdminHtml.showAdminHtml(activeChar, "tele/other.html");
		}
		else if (command.equals("admin_show_teleport"))
		{
			showTeleportCharWindow(activeChar);
		}
		else if (command.equals("admin_recall_npc"))
		{
			RecallNpc(activeChar);
		}
		else if (command.equals("admin_teleport_to_character"))
		{
			TeleportToCharacter(activeChar, activeChar.getTarget());
		}
		else if (command.startsWith("admin_walk"))
		{
			try
			{
				string val = command.Substring(11);
				StringTokenizer st = new StringTokenizer(val);
				int x = int.Parse(st.nextToken());
				int y = int.Parse(st.nextToken());
				int z = int.Parse(st.nextToken());
				activeChar.getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, new Location3D(x, y, z));
			}
			catch (Exception e)
			{
                _logger.Error(e);
			}
		}
		else if (command.startsWith("admin_move_to"))
		{
			try
			{
				string val = command.Substring(14);
				teleportTo(activeChar, val);
			}
			catch (IndexOutOfRangeException e)
			{
                _logger.Error(e);
				// Case of empty or missing coordinates
				AdminHtml.showAdminHtml(activeChar, "teleports.htm");
			}
			catch (FormatException nfe)
			{
                _logger.Error(nfe);
				BuilderUtil.sendSysMessage(activeChar, "Usage: //move_to <x> <y> <z>");
				AdminHtml.showAdminHtml(activeChar, "teleports.htm");
			}
		}
		else if (command.startsWith("admin_teleport_character"))
		{
			try
			{
				string val = command.Substring(25);
				TeleportCharacter(activeChar, val);
			}
			catch (IndexOutOfRangeException e)
			{
                _logger.Error(e);
				// Case of empty coordinates
				BuilderUtil.sendSysMessage(activeChar, "Wrong or no Coordinates given.");
				showTeleportCharWindow(activeChar); // back to character teleport
			}
		}
		else if (command.startsWith("admin_teleportto "))
		{
			try
			{
				string targetName = command.Substring(17);
				Player? player = World.getInstance().getPlayer(targetName);
				TeleportToCharacter(activeChar, player);
			}
			catch (IndexOutOfRangeException e)
			{
                _logger.Error(e);
			}
		}
		else if (command.startsWith("admin_teleport"))
		{
			try
			{
				StringTokenizer st = new StringTokenizer(command, " ");
				st.nextToken();
				int x = (int) float.Parse(st.nextToken(), CultureInfo.InvariantCulture);
				int y = (int) float.Parse(st.nextToken(), CultureInfo.InvariantCulture);
				int z = st.hasMoreTokens() ? (int) float.Parse(st.nextToken(), CultureInfo.InvariantCulture) : GeoEngine.getInstance().getHeight(new Location3D(x, y, 10000));
				activeChar.teleToLocation(new Location3D(x, y, z));
			}
			catch (Exception e)
			{
                _logger.Error(e);
				BuilderUtil.sendSysMessage(activeChar, "Wrong coordinates!");
			}
		}
		else if (command.startsWith("admin_recall "))
		{
			try
			{
				string[] param = command.Split(" ");
				if (param.Length != 2)
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: //recall <playername>");
					return false;
				}
				string targetName = param[1];
				Player? player = World.getInstance().getPlayer(targetName);
				if (player != null)
				{
					teleportCharacter(player, activeChar.Location.Location3D, activeChar);
				}
				else
				{
					changeCharacterPosition(activeChar, targetName);
				}
			}
			catch (IndexOutOfRangeException e)
			{
                _logger.Error(e);
			}
		}
		else if (command.equals("admin_tele"))
		{
			showTeleportWindow(activeChar);
		}
		else if (command.startsWith("admin_go"))
		{
			int intVal = 150;
			int x = activeChar.getX();
			int y = activeChar.getY();
			int z = activeChar.getZ();
			try
			{
				string val = command.Substring(8);
				StringTokenizer st = new StringTokenizer(val);
				string dir = st.nextToken();
				if (st.hasMoreTokens())
				{
					intVal = int.Parse(st.nextToken());
				}
				if (dir.equals("east"))
				{
					x += intVal;
				}
				else if (dir.equals("west"))
				{
					x -= intVal;
				}
				else if (dir.equals("north"))
				{
					y -= intVal;
				}
				else if (dir.equals("south"))
				{
					y += intVal;
				}
				else if (dir.equals("up"))
				{
					z += intVal;
				}
				else if (dir.equals("down"))
				{
					z -= intVal;
				}
				activeChar.teleToLocation(new Location(x, y, z, 0));
				showTeleportWindow(activeChar);
			}
			catch (Exception e)
			{
                _logger.Error(e);
				BuilderUtil.sendSysMessage(activeChar, "Usage: //go<north|south|east|west|up|down> [offset] (default 150)");
			}
		}
		else if (command.startsWith("admin_sendhome"))
		{
			StringTokenizer st = new StringTokenizer(command, " ");
			st.nextToken(); // Skip command.
			if (st.countTokens() > 1)
			{
				BuilderUtil.sendSysMessage(activeChar, "Usage: //sendhome <playername>");
			}
			else if (st.countTokens() == 1)
			{
				string name = st.nextToken();
				Player? player = World.getInstance().getPlayer(name);
				if (player == null)
				{
					activeChar.sendPacket(SystemMessageId.THAT_PLAYER_IS_NOT_ONLINE);
					return false;
				}
				teleportHome(player);
			}
			else
			{
				WorldObject? target = activeChar.getTarget();
                Player? targetPlayer = target?.getActingPlayer();
				if (target != null && target.isPlayer() && targetPlayer != null)
				{
					teleportHome(targetPlayer);
				}
				else
				{
					activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				}
			}
		}
		return true;
	}

	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}

	/**
	 * This method sends a player to it's home town.
	 * @param player the player to teleport.
	 */
	private void teleportHome(Player player)
	{
		string regionName;
		switch (player.getRace())
		{
			case Race.ELF:
			{
				regionName = "elf_town";
				break;
			}
			case Race.DARK_ELF:
			{
				regionName = "darkelf_town";
				break;
			}
			case Race.ORC:
			{
				regionName = "orc_town";
				break;
			}
			case Race.DWARF:
			{
				regionName = "dwarf_town";
				break;
			}
			case Race.KAMAEL:
			{
				regionName = "kamael_town";
				break;
			}
			case Race.HUMAN:
			default:
			{
				regionName = "talking_island_town";
				break;
			}
		}

        MapRegion? region = MapRegionData.Instance.GetMapRegionByName(regionName);
        if (region == null)
        {
            _logger.Warn("Region " + regionName + " not found.");
            return;
        }

		player.teleToLocation(region.GetSpawnLocation(), true);
	}

	private void teleportTo(Player activeChar, string coords)
	{
		try
		{
			StringTokenizer st = new StringTokenizer(coords);
			int x = int.Parse(st.nextToken());
			int y = int.Parse(st.nextToken());
			int z = int.Parse(st.nextToken());
			activeChar.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
			activeChar.teleToLocation(new Location(x, y, z, 0), null);
			BuilderUtil.sendSysMessage(activeChar, "You have been teleported to " + coords);
		}
		catch (Exception nsee)
		{
            _logger.Error(nsee);
			BuilderUtil.sendSysMessage(activeChar, "Wrong or no Coordinates given.");
		}
	}

	private void showTeleportWindow(Player activeChar)
	{
		AdminHtml.showAdminHtml(activeChar, "move.htm");
	}

	private void showTeleportCharWindow(Player activeChar)
	{
		WorldObject? target = activeChar.getTarget();
		Player? player = null;
		if (target != null && target.isPlayer())
		{
			player = (Player) target;
		}
		else
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			return;
		}

		string replyMSG =
			"<html><title>Teleport Character</title><body>The character you will teleport is " + player.getName() +
			".<br>Co-ordinate x<edit var=\"char_cord_x\" width=110>Co-ordinate y<edit var=\"char_cord_y\" width=110>Co-ordinate z<edit var=\"char_cord_z\" width=110><button value=\"Teleport\" action=\"bypass -h admin_teleport_character $char_cord_x $char_cord_y $char_cord_z\" width=60 height=15 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"><button value=\"Teleport near you\" action=\"bypass -h admin_teleport_character " +
			activeChar.getX() + " " + activeChar.getY() + " " + activeChar.getZ() +
			"\" width=115 height=15 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"><center><button value=\"Back\" action=\"bypass -h admin_current_player\" width=40 height=15 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></center></body></html>";

		HtmlContent htmlContent = HtmlContent.LoadFromText(replyMSG, activeChar);
		NpcHtmlMessagePacket adminReply = new NpcHtmlMessagePacket(null, 1, htmlContent);
		activeChar.sendPacket(adminReply);
	}

	private void TeleportCharacter(Player activeChar, string coords)
	{
		WorldObject? target = activeChar.getTarget();
		Player? player = null;
		if (target != null && target.isPlayer())
		{
			player = (Player) target;
		}
		else
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			return;
		}

		if (player.ObjectId == activeChar.ObjectId)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_USE_THIS_ON_YOURSELF);
		}
		else
		{
			try
			{
				StringTokenizer st = new StringTokenizer(coords);
				string x1 = st.nextToken();
				int x = int.Parse(x1);
				string y1 = st.nextToken();
				int y = int.Parse(y1);
				string z1 = st.nextToken();
				int z = int.Parse(z1);
				teleportCharacter(player, new Location3D(x, y, z), activeChar);
			}
			catch (Exception nsee)
			{
                _logger.Error(nsee);
			}
		}
	}

	/**
	 * @param player
	 * @param loc
	 * @param activeChar
	 */
	private static void teleportCharacter(Player player, Location3D loc, Player activeChar)
	{
		if (player != null)
		{
			// Check for jail
			if (player.isJailed())
			{
				BuilderUtil.sendSysMessage(activeChar, "Sorry, player " + player.getName() + " is in Jail.");
			}
			else
			{
				BuilderUtil.sendSysMessage(activeChar, "You have recalled " + player.getName());
				player.sendMessage("Admin is teleporting you.");
				player.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
				player.teleToLocation(loc, activeChar.getInstanceWorld(), true);
			}
		}
	}

	private static void TeleportToCharacter(Player activeChar, WorldObject? target)
	{
        Player? player = target?.getActingPlayer();
		if (target == null || !target.isPlayer() || player == null)
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			return;
		}

		if (player.ObjectId == activeChar.ObjectId)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_USE_THIS_ON_YOURSELF);
		}
		else
		{
			activeChar.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
			activeChar.teleToLocation(player.Location, player.getInstanceWorld(), true);
			BuilderUtil.sendSysMessage(activeChar, "You have teleported to character " + player.getName() + ".");
		}
	}

	private void changeCharacterPosition(Player activeChar, string name)
	{
		int x = activeChar.getX();
		int y = activeChar.getY();
		int z = activeChar.getZ();
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int count = ctx.Characters.Where(c => c.Name == name).ExecuteUpdate(s =>
				s.SetProperty(r => r.X, x).SetProperty(r => r.Y, y).SetProperty(r => r.Z, z));

			if (count == 0)
			{
				BuilderUtil.sendSysMessage(activeChar, "Character not found or position unaltered.");
			}
			else
			{
				BuilderUtil.sendSysMessage(activeChar, "Player's [" + name + "] position is now set to (" + x + "," + y + "," + z + ").");
			}
		}
		catch (Exception se)
		{
            _logger.Error(se);
			BuilderUtil.sendSysMessage(activeChar, "SQLException while changing offline character's position");
		}
	}

	private static void RecallNpc(Player activeChar)
	{
		WorldObject? obj = activeChar.getTarget();
		if (obj is Npc && !((Npc) obj).isMinion() && !(obj is RaidBoss) && !(obj is GrandBoss))
		{
			Npc target = (Npc) obj;
			int monsterTemplate = target.getTemplate().getId();
			NpcTemplate? template1 = NpcData.getInstance().getTemplate(monsterTemplate);
			if (template1 == null)
			{
				BuilderUtil.sendSysMessage(activeChar, "Incorrect monster template.");
				_logger.Warn("ERROR: NPC " + target.ObjectId + " has a 'null' template.");
				return;
			}

			Spawn? spawn = target.getSpawn();
			if (spawn == null)
			{
				BuilderUtil.sendSysMessage(activeChar, "Incorrect monster spawn.");
				_logger.Warn("ERROR: NPC " + target.ObjectId + " has a 'null' spawn.");
				return;
			}

			TimeSpan respawnTime = spawn.getRespawnDelay();
			target.deleteMe();
			spawn.stopRespawn();
			SpawnTable.getInstance().deleteSpawn(spawn, true);

			try
			{
				spawn = new Spawn(template1);
				spawn.Location = activeChar.Location;
				spawn.setAmount(1);
				spawn.setRespawnDelay(respawnTime);
				if (activeChar.isInInstance())
				{
					spawn.setInstanceId(activeChar.getInstanceId());
				}
				SpawnTable.getInstance().addNewSpawn(spawn, true);
				spawn.init();
				if (respawnTime <= TimeSpan.Zero)
				{
					spawn.stopRespawn();
				}

				BuilderUtil.sendSysMessage(activeChar, "Created " + template1.getName() + " on " + target.ObjectId + ".");
			}
			catch (Exception e)
			{
                _logger.Error(e);
				BuilderUtil.sendSysMessage(activeChar, "Target is not in game.");
			}
		}
		else if (obj is RaidBoss)
		{
			RaidBoss target = (RaidBoss) obj;
			Spawn? spawn = target.getSpawn();
			double curHP = target.getCurrentHp();
			double curMP = target.getCurrentMp();
			if (spawn == null)
			{
				BuilderUtil.sendSysMessage(activeChar, "Incorrect raid spawn.");
				_logger.Warn("ERROR: NPC Id" + target.getId() + " has a 'null' spawn.");
				return;
			}
			DbSpawnManager.getInstance().deleteSpawn(spawn, true);
			try
			{
				Spawn spawnDat = new Spawn(target.getId());
				spawnDat.Location = activeChar.Location;
				spawnDat.setAmount(1);
				spawnDat.setRespawnMinDelay(TimeSpan.FromSeconds(43200));
				spawnDat.setRespawnMaxDelay(TimeSpan.FromSeconds(129600));

				DbSpawnManager.getInstance().addNewSpawn(spawnDat, null, curHP, curMP, true);
			}
			catch (Exception e)
			{
                _logger.Error(e);
				activeChar.sendPacket(SystemMessageId.YOUR_TARGET_CANNOT_BE_FOUND);
			}
		}
		else
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
		}
	}
}