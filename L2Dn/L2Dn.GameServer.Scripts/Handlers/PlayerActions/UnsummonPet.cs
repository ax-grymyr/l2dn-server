using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.PlayerActions;

/**
 * Unsummon Pet player action handler.
 * @author St3eT
 */
public class UnsummonPet: IPlayerActionHandler
{
    public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
    {
        Summon? pet = player.getPet();
        if (pet == null)
        {
            player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_A_PET);
        }
        else if (((Pet)pet).isUncontrollable())
        {
            player.sendPacket(SystemMessageId.WHEN_YOUR_PET_S_SATIETY_REACHES_0_YOU_CANNOT_CONTROL_IT);
        }
        else if (pet.isBetrayed())
        {
            player.sendPacket(SystemMessageId.WHEN_YOUR_PET_S_SATIETY_REACHES_0_YOU_CANNOT_CONTROL_IT);
        }
        else if (pet.isDead())
        {
            player.sendPacket(SystemMessageId.DEAD_PETS_CANNOT_BE_RETURNED_TO_THEIR_SUMMONING_ITEM);
        }
        else if (pet.isAttackingNow() || pet.isInCombat() || pet.isMovementDisabled())
        {
            player.sendPacket(SystemMessageId.A_PET_CANNOT_BE_RECALLED_IN_COMBAT);
        }
        else if (pet.isHungry())
        {
            player.sendPacket(SystemMessageId.YOU_MAY_NOT_RESTORE_A_HUNGRY_PET);
        }
        else
        {
            pet.unSummon(player);
        }
    }

    public bool isPetAction()
    {
        return true;
    }
}