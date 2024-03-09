using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Scripts.Handlers.ActionShiftHandlers;

public class SummonActionShift: IActionShiftHandler
{
	public bool action(Player player, WorldObject target, bool interact)
	{
		if (player.isGM())
		{
			if (player.getTarget() != target)
			{
				// Set the target of the Player player
				player.setTarget(target);
			}
			
			AdminCommandHandler.getInstance().useAdminCommand(player, "admin_summon_info", true);
		}
		return true;
	}
	
	public InstanceType getInstanceType()
	{
		return InstanceType.Summon;
	}
}