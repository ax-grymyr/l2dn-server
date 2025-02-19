using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands: - gmchat text = sends text to all online GM's - gmchat_menu text = same as gmchat, displays the admin panel after chat
 * @version $Revision: 1.2.4.3 $ $Date: 2005/04/11 10:06:06 $
 */
public class AdminGmChat: IAdminCommandHandler
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(AdminGmChat));
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_gmchat",
		"admin_snoop",
		"admin_gmchat_menu",
    ];

	public bool useAdminCommand(string command, Player activeChar)
	{
		if (command.startsWith("admin_gmchat"))
		{
			handleGmChat(command, activeChar);
		}
		else if (command.startsWith("admin_snoop"))
		{
			snoop(command, activeChar);
		}
		if (command.startsWith("admin_gmchat_menu"))
		{
			AdminHtml.showAdminHtml(activeChar, "gm_menu.htm");
		}
		return true;
	}

	/**
	 * @param command
	 * @param activeChar
	 */
	private void snoop(string command, Player activeChar)
	{
		WorldObject? target = null;
		if (command.Length > 12)
		{
			target = World.getInstance().getPlayer(command.Substring(12));
		}
		if (target == null)
		{
			target = activeChar.getTarget();
		}

		if (target == null)
		{
			activeChar.sendPacket(SystemMessageId.SELECT_TARGET);
			return;
		}
		if (!target.isPlayer())
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			return;
		}
		Player player = (Player) target;
		player.addSnooper(activeChar);
		activeChar.addSnooped(player);
	}

	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}

	/**
	 * @param command
	 * @param activeChar
	 */
	private void handleGmChat(string command, Player activeChar)
	{
		try
		{
			int offset = 0;
			string text;
			if (command.startsWith("admin_gmchat_menu"))
			{
				offset = 18;
			}
			else
			{
				offset = 13;
			}
			text = command.Substring(offset);
			AdminData.getInstance().broadcastToGMs(new CreatureSayPacket(null, ChatType.ALLIANCE, activeChar.getName(), text));
		}
		catch (IndexOutOfRangeException e)
		{
            _logger.Error(e);
			// Who cares?
		}
	}
}