using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * Camera commands.
 * @author Zoey76
 */
public class AdminCamera: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_cam",
		"admin_camex",
		"admin_cam3"
	};
	
	public bool useAdminCommand(String command, Player activeChar)
	{
		if ((activeChar.getTarget() == null) || !activeChar.getTarget().isCreature())
		{
			activeChar.sendPacket(SystemMessageId.YOUR_TARGET_CANNOT_BE_FOUND);
			return false;
		}
		
		Creature target = (Creature) activeChar.getTarget();
		String[] com = command.Split(" ");
		switch (com[0])
		{
			case "admin_cam":
			{
				if (com.Length != 12)
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: //cam force angle1 angle2 time range duration relYaw relPitch isWide relAngle");
					return false;
				}
				AbstractScript.specialCamera(activeChar, target, int.Parse(com[1]), int.Parse(com[2]), int.Parse(com[3]), int.Parse(com[4]), int.Parse(com[5]), int.Parse(com[6]), int.Parse(com[7]), int.Parse(com[8]), int.Parse(com[9]), int.Parse(com[10]));
				break;
			}
			case "admin_camex":
			{
				if (com.Length != 10)
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: //camex force angle1 angle2 time duration relYaw relPitch isWide relAngle");
					return false;
				}
				AbstractScript.specialCameraEx(activeChar, target, int.Parse(com[1]), int.Parse(com[2]), int.Parse(com[3]), int.Parse(com[4]), int.Parse(com[5]), int.Parse(com[6]), int.Parse(com[7]), int.Parse(com[8]), int.Parse(com[9]));
				break;
			}
			case "admin_cam3":
			{
				if (com.Length != 12)
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: //cam3 force angle1 angle2 time range duration relYaw relPitch isWide relAngle unk");
					return false;
				}
				AbstractScript.specialCamera3(activeChar, target, int.Parse(com[1]), int.Parse(com[2]), int.Parse(com[3]), int.Parse(com[4]), int.Parse(com[5]), int.Parse(com[6]), int.Parse(com[7]), int.Parse(com[8]), int.Parse(com[9]), int.Parse(com[10]), int.Parse(com[11]));
				break;
			}
		}
		return true;
	}
	
	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}