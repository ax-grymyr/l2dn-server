using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author lord_rex
 */
public class AdminHide: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_hide"
	};
	
	public bool useAdminCommand(string command, Player player)
	{
		StringTokenizer st = new StringTokenizer(command);
		st.nextToken();
		
		try
		{
			string param = st.nextToken();
			switch (param)
			{
				case "on":
				{
					if (!BuilderUtil.setHiding(player, true))
					{
						BuilderUtil.sendSysMessage(player, "Currently, you cannot be seen.");
						return true;
					}
					
					BuilderUtil.sendSysMessage(player, "Now, you cannot be seen.");
					return true;
				}
				case "off":
				{
					if (!BuilderUtil.setHiding(player, false))
					{
						BuilderUtil.sendSysMessage(player, "Currently, you can be seen.");
						return true;
					}
					
					BuilderUtil.sendSysMessage(player, "Now, you can be seen.");
					return true;
				}
				default:
				{
					BuilderUtil.sendSysMessage(player, "//hide [on|off]");
					return false;
				}
			}
		}
		catch (Exception e)
		{
			BuilderUtil.sendSysMessage(player, "//hide [on|off]");
			return false;
		}
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
