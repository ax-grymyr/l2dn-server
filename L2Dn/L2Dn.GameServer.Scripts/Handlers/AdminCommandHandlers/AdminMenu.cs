using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands: - handles every admin menu command
 * @version $Revision: 1.3.2.6.2.4 $ $Date: 2005/04/11 10:06:06 $
 */
public class AdminMenu: IAdminCommandHandler
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AdminMenu));
	
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_char_manage",
		"admin_teleport_character_to_menu",
		"admin_recall_char_menu",
		"admin_recall_party_menu",
		"admin_recall_clan_menu",
		"admin_goto_char_menu",
		"admin_kick_menu",
		"admin_kill_menu",
		"admin_ban_menu",
		"admin_unban_menu"
	};
	
	public bool useAdminCommand(String command, Player activeChar)
	{
		if (command.equals("admin_char_manage"))
		{
			showMainPage(activeChar);
		}
		else if (command.startsWith("admin_teleport_character_to_menu"))
		{
			String[] data = command.Split(" ");
			if (data.Length == 5)
			{
				String playerName = data[1];
				Player player = World.getInstance().getPlayer(playerName);
				if (player != null)
				{
					teleportCharacter(player, new LocationHeading(int.Parse(data[2]), int.Parse(data[3]), int.Parse(data[4]), 0), activeChar, "Admin is teleporting you.");
				}
			}
			showMainPage(activeChar);
		}
		else if (command.startsWith("admin_recall_char_menu"))
		{
			try
			{
				String targetName = command.Substring(23);
				Player player = World.getInstance().getPlayer(targetName);
				teleportCharacter(player, activeChar.getLocation().ToLocationHeading(), activeChar, "Admin is teleporting you.");
			}
			catch (IndexOutOfRangeException e)
			{
				// Not important.
			}
		}
		else if (command.startsWith("admin_recall_party_menu"))
		{
			try
			{
				String targetName = command.Substring(24);
				Player player = World.getInstance().getPlayer(targetName);
				if (player == null)
				{
					activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
					return true;
				}
				if (!player.isInParty())
				{
					BuilderUtil.sendSysMessage(activeChar, "Player is not in party.");
					teleportCharacter(player, activeChar.getLocation().ToLocationHeading(), activeChar, "Admin is teleporting you.");
					return true;
				}
				foreach (Player pm in player.getParty().getMembers())
				{
					teleportCharacter(pm, activeChar.getLocation().ToLocationHeading(), activeChar, "Your party is being teleported by an Admin.");
				}
			}
			catch (Exception e)
			{
				LOGGER.Warn(e);
			}
		}
		else if (command.startsWith("admin_recall_clan_menu"))
		{
			try
			{
				String targetName = command.Substring(23);
				Player player = World.getInstance().getPlayer(targetName);
				if (player == null)
				{
					activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
					return true;
				}
				Clan clan = player.getClan();
				if (clan == null)
				{
					BuilderUtil.sendSysMessage(activeChar, "Player is not in a clan.");
					teleportCharacter(player, activeChar.getLocation().ToLocationHeading(), activeChar, "Admin is teleporting you.");
					return true;
				}
				
				foreach(Player member in clan.getOnlineMembers(0))
				{
					teleportCharacter(member, activeChar.getLocation().ToLocationHeading(), activeChar, "Your clan is being teleported by an Admin.");
				}
			}
			catch (Exception e)
			{
				LOGGER.Warn(e);
			}
		}
		else if (command.startsWith("admin_goto_char_menu"))
		{
			try
			{
				Player player = World.getInstance().getPlayer(command.Substring(21));
				teleportToCharacter(activeChar, player);
			}
			catch (IndexOutOfRangeException e)
			{
				// Not important.
			}
		}
		else if (command.equals("admin_kill_menu"))
		{
			handleKill(activeChar);
		}
		else if (command.startsWith("admin_kick_menu"))
		{
			StringTokenizer st = new StringTokenizer(command);
			if (st.countTokens() > 1)
			{
				st.nextToken();
				String player = st.nextToken();
				Player plyr = World.getInstance().getPlayer(player);
				String text;
				if (plyr != null)
				{
					Disconnection.of(plyr).defaultSequence(LeaveWorldPacket.STATIC_PACKET);
					text = "You kicked " + plyr.getName() + " from the game.";
				}
				else
				{
					text = "Player " + player + " was not found in the game.";
				}
				activeChar.sendMessage(text);
			}
			showMainPage(activeChar);
		}
		else if (command.startsWith("admin_ban_menu"))
		{
			StringTokenizer st = new StringTokenizer(command);
			if (st.countTokens() > 1)
			{
				String subCommand = "admin_ban_char";
				AdminCommandHandler.getInstance().useAdminCommand(activeChar, subCommand + command.Substring(14), true);
			}
			showMainPage(activeChar);
		}
		else if (command.startsWith("admin_unban_menu"))
		{
			StringTokenizer st = new StringTokenizer(command);
			if (st.countTokens() > 1)
			{
				String subCommand = "admin_unban_char";
				AdminCommandHandler.getInstance().useAdminCommand(activeChar, subCommand + command.Substring(16), true);
			}
			showMainPage(activeChar);
		}
		return true;
	}
	
	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
	
	private void handleKill(Player activeChar)
	{
		handleKill(activeChar, null);
	}
	
	private void handleKill(Player activeChar, String player)
	{
		WorldObject obj = activeChar.getTarget();
		Creature target = (Creature) obj;
		String filename = "main_menu.htm";
		if (player != null)
		{
			Player plyr = World.getInstance().getPlayer(player);
			if (plyr != null)
			{
				target = plyr;
				BuilderUtil.sendSysMessage(activeChar, "You killed " + plyr.getName());
			}
		}
		if (target != null)
		{
			if (target.isPlayer())
			{
				target.reduceCurrentHp(target.getMaxHp() + target.getMaxCp() + 1, activeChar, null);
				filename = "charmanage.htm";
			}
			else if (Config.CHAMPION_ENABLE && target.isChampion())
			{
				target.reduceCurrentHp((target.getMaxHp() * Config.CHAMPION_HP) + 1, activeChar, null);
			}
			else
			{
				target.reduceCurrentHp(target.getMaxHp() + 1, activeChar, null);
			}
		}
		else
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
		}
		AdminHtml.showAdminHtml(activeChar, filename);
	}
	
	private void teleportCharacter(Player player, LocationHeading loc, Player activeChar, String message)
	{
		if (player != null)
		{
			player.sendMessage(message);
			player.teleToLocation(loc, true);
		}

		showMainPage(activeChar);
	}
	
	private void teleportToCharacter(Player activeChar, WorldObject target)
	{
		if (!target.isPlayer())
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			return;
		}
		
		Player player = target.getActingPlayer();
		if (player.getObjectId() == activeChar.getObjectId())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_USE_THIS_ON_YOURSELF);
		}
		else
		{
			activeChar.teleToLocation(player.getLocation().ToLocationHeading(), true, player.getInstanceWorld());
			BuilderUtil.sendSysMessage(activeChar, "You're teleporting yourself to character " + player.getName());
		}
		showMainPage(activeChar);
	}
	
	/**
	 * @param activeChar
	 */
	private void showMainPage(Player activeChar)
	{
		AdminHtml.showAdminHtml(activeChar, "charmanage.htm");
	}
}
