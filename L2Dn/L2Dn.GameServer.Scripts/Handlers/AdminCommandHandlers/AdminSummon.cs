using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author poltomb
 */
public class AdminSummon: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_summon",
    ];
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		int id;
		long count = 1;
		string[] data = command.Split(" ");
		try
		{
			id = int.Parse(data[1]);
			if (data.Length > 2)
			{
				count = long.Parse(data[2]);
			}
		}
		catch (FormatException nfe)
		{
			BuilderUtil.sendSysMessage(activeChar, "Incorrect format for command 'summon'");
			return false;
		}
		
		string subCommand;
		if (id < 1000000)
		{
			subCommand = "admin_create_item";
		}
		else
		{
			subCommand = "admin_spawn_once";
			BuilderUtil.sendSysMessage(activeChar, "This is only a temporary spawn.  The mob(s) will NOT respawn.");
			id -= 1000000;
		}
		
		if ((id > 0) && (count > 0))
		{
			AdminCommandHandler.getInstance().useAdminCommand(activeChar, subCommand + " " + id + " " + count, true);
		}
		
		return true;
	}
}