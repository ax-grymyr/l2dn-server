using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.ActionHandlers;

public class DecoyAction: IActionHandler
{
	public bool action(Player player, WorldObject target, bool interact)
	{
		// Aggression target lock effect
		if (player.isLockedTarget() && player.getLockedTarget() != target)
		{
			player.sendPacket(SystemMessageId.FAILED_TO_CHANGE_ENMITY);
			return false;
		}
		
		player.setTarget(target);
		return true;
	}
	
	public InstanceType getInstanceType()
	{
		return InstanceType.Decoy;
	}
}