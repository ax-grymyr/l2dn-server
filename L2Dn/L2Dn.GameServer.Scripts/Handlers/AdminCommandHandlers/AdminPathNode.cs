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
	
	public bool useAdminCommand(string command, Player activeChar)
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
				List<AbstractNodeLoc>? path = PathFinding.getInstance().findPath(activeChar.Location.Location3D,
					activeChar.getTarget().Location.Location3D, activeChar.getInstanceWorld(), true);

				if (path == null)
				{
					BuilderUtil.sendSysMessage(activeChar, "No Route!");
					return true;
				}
				foreach (AbstractNodeLoc a in path)
				{
					BuilderUtil.sendSysMessage(activeChar, a.Location.ToString());
				}
			}
			else
			{
				BuilderUtil.sendSysMessage(activeChar, "No Target!");
			}
		}
		return true;
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
