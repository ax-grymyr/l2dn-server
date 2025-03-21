using L2Dn.GameServer.AI;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Scripts.Handlers.PlayerActions;

/**
 * Servitor Attack player action handler.
 * @author Mobius
 */
public class ServitorAttack: IPlayerActionHandler
{
    public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
    {
        if (!player.hasServitors())
        {
            player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_A_SERVITOR);
            return;
        }

        WorldObject? target = player.getTarget();
        if (target == null)
        {
            return;
        }

        bool targetOutOfRange = player.Distance3D(target) > 3000;
        foreach (Summon summon in player.getServitors().Values)
        {
            if (targetOutOfRange)
            {
                summon.getAI().setIntention(CtrlIntention.AI_INTENTION_FOLLOW, player);
            }
            else if (summon.canAttack(target, ctrlPressed))
            {
                summon.doAttack(target);
            }
        }
    }

    public bool isPetAction()
    {
        return true;
    }
}