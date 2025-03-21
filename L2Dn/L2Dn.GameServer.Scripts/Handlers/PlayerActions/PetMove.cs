using L2Dn.GameServer.AI;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.PlayerActions;

/**
 * Pet move to target player action handler.
 * @author Nik
 */
public class PetMove: IPlayerActionHandler
{
    public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
    {
        WorldObject? target = player.getTarget();
        Pet? pet = player.getPet();
        if (pet == null || !pet.isPet())
        {
            player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_A_PET);
            return;
        }

        if (pet.isUncontrollable())
        {
            player.sendPacket(SystemMessageId.WHEN_YOUR_PET_S_SATIETY_REACHES_0_YOU_CANNOT_CONTROL_IT);
        }
        else if (pet.isBetrayed())
        {
            player.sendPacket(SystemMessageId.YOUR_SERVITOR_IS_UNRESPONSIVE_AND_WILL_NOT_OBEY_ANY_ORDERS);
        }
        else if (target != null && pet != target && !pet.isMovementDisabled())
        {
            pet.setFollowStatus(false);
            pet.getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, target.Location.Location3D);
        }
    }

    public bool isPetAction()
    {
        return true;
    }
}