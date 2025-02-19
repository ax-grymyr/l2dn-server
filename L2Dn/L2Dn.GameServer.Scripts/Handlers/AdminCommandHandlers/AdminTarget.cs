using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands: - target name = sets player with respective name as target
 * @version $Revision: 1.2.4.3 $ $Date: 2005/04/11 10:05:56 $
 */
public class AdminTarget: IAdminCommandHandler
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(AdminTarget));
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_target",
    ];

	public bool useAdminCommand(string command, Player activeChar)
	{
		if (command.startsWith("admin_target"))
		{
			handleTarget(command, activeChar);
		}
		return true;
	}

	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}

	private void handleTarget(string command, Player activeChar)
	{
		try
		{
			string targetName = command.Substring(13);
			Player? player = World.getInstance().getPlayer(targetName);
			if (player != null)
			{
				player.onAction(activeChar);
			}
			else
			{
				BuilderUtil.sendSysMessage(activeChar, "Player " + targetName + " not found");
			}
		}
		catch (Exception e)
		{
            _logger.Error(e);
			BuilderUtil.sendSysMessage(activeChar, "Please specify correct name.");
		}
	}
}