using L2Dn.GameServer.Geo.PathFindings;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

public class AdminPathNode: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_path_find"
	};
	
	public bool useAdminCommand(String command, Player activeChar)
	{
		if (command.equals("admin_path_find"))
		{
			if (Config.PATHFINDING < 1)
			{
				BuilderUtil.sendSysMessage(activeChar, "PathFinding is disabled.");
				return true;
			}
			
			if (activeChar.getTarget() != null)
			{
				List<AbstractNodeLoc> path = PathFinding.getInstance().findPath(activeChar.getX(), activeChar.getY(), (short) activeChar.getZ(), activeChar.getTarget().getX(), activeChar.getTarget().getY(), (short) activeChar.getTarget().getZ(), activeChar.getInstanceWorld(), true);
				if (path == null)
				{
					BuilderUtil.sendSysMessage(activeChar, "No Route!");
					return true;
				}
				foreach (AbstractNodeLoc a in path)
				{
					BuilderUtil.sendSysMessage(activeChar, "x:" + a.getX() + " y:" + a.getY() + " z:" + a.getZ());
				}
			}
			else
			{
				BuilderUtil.sendSysMessage(activeChar, "No Target!");
			}
		}
		return true;
	}
	
	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
