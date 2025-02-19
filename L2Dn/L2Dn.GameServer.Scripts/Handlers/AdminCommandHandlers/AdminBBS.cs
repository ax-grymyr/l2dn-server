using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

public class AdminBBS: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_bbs",
    ];
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		return true;
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}