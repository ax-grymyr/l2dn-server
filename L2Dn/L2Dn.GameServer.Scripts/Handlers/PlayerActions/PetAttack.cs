using L2Dn.GameServer.AI;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.PlayerActions;

/**
 * Pet attack player action handler.
 * @author Mobius
 */
public class PetAttack: IPlayerActionHandler
{
	public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
	{
		Pet pet = player.getPet();
		if ((pet == null) || !pet.isPet())
		{
			player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_A_PET);
			return;
		}
		
		if (pet.isUncontrollable())
		{
			player.sendPacket(SystemMessageId.WHEN_YOUR_PET_S_SATIETY_REACHES_0_YOU_CANNOT_CONTROL_IT);
			return;
		}
		
		if (pet.isBetrayed())
		{
			player.sendPacket(SystemMessageId.YOUR_SERVITOR_IS_UNRESPONSIVE_AND_WILL_NOT_OBEY_ANY_ORDERS);
			return;
		}
		
		WorldObject target = player.getTarget();
		if (target == null)
		{
			return;
		}
		
		if (player.calculateDistance3D(target) > 3000)
		{
			pet.getAI().setIntention(CtrlIntention.AI_INTENTION_FOLLOW, player);
		}
		else if (pet.canAttack(target, ctrlPressed))
		{
			pet.doAttack(target);
		}
	}
	
	public bool isPetAction()
	{
		return true;
	}
}