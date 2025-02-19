using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands: - GM = turns GM mode off
 * @version $Revision: 1.2.4.4 $ $Date: 2005/04/11 10:06:06 $
 */
public class AdminGm: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_gm",
    ];
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		if (command.equals("admin_gm") && activeChar.isGM())
		{
			AdminData.getInstance().deleteGm(activeChar);
			activeChar.setAccessLevel(0, true, false);
			BuilderUtil.sendSysMessage(activeChar, "You deactivated your GM access for this session, if you login again you will be GM again, in order to remove your access completely please shift yourself and set your accesslevel to 0.");
		}
		return true;
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
