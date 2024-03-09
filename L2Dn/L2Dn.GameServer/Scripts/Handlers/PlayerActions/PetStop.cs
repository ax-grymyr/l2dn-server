using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.PlayerActions;

/**
 * Pet stop action player action handler.
 * @author Nik
 */
public class PetStop: IPlayerActionHandler
{
	public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
	{
		if ((player.getPet() == null) || !player.getPet().isPet())
		{
			player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_A_PET);
			return;
		}
		
		Pet pet = player.getPet();
		if (pet.isUncontrollable())
		{
			player.sendPacket(SystemMessageId.WHEN_YOUR_PET_S_SATIETY_REACHES_0_YOU_CANNOT_CONTROL_IT);
		}
		else if (pet.isBetrayed())
		{
			player.sendPacket(SystemMessageId.YOUR_SERVITOR_IS_UNRESPONSIVE_AND_WILL_NOT_OBEY_ANY_ORDERS);
		}
		else
		{
			pet.cancelAction();
		}
	}
	
	public bool isPetAction()
	{
		return true;
	}
}