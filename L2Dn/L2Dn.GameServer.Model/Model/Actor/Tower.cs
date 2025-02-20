using L2Dn.GameServer.AI;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Model.Actor;

/**
 * This class is a super-class for ControlTower and FlameTower.
 * @author Zoey76
 */
public abstract class Tower: Npc
{
    public Tower(NpcTemplate template): base(template)
    {
        setInvul(false);
    }

    public override bool canBeAttacked()
    {
        // Attackable during siege by attacker only
        return getCastle() != null && getCastle().getResidenceId() > 0 && getCastle().getSiege().isInProgress();
    }

    public override bool isAutoAttackable(Creature attacker)
    {
        // Attackable during siege by attacker only
        return attacker != null && attacker.isPlayer() && getCastle() != null &&
               getCastle().getResidenceId() > 0 && getCastle().getSiege().isInProgress() &&
               getCastle().getSiege().checkIsAttacker(((Player)attacker).getClan());
    }

    public override void onAction(Player player, bool interact)
    {
        if (!canTarget(player))
        {
            return;
        }

        if (this != player.getTarget())
        {
            // Set the target of the Player player
            player.setTarget(this);
        }
        else if (interact && isAutoAttackable(player) && Math.Abs(player.getZ() - getZ()) < 100 &&
                 GeoEngine.getInstance().canSeeTarget(player, this))
        {
            // Notify the Player AI with AI_INTENTION_INTERACT
            player.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, this);
        }

        // Send a Server->Client ActionFailed to the Player in order to avoid that the client wait another packet
        player.sendPacket(ActionFailedPacket.STATIC_PACKET);
    }

    public override void onForcedAttack(Player player)
    {
        onAction(player);
    }
}
