using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerState.
 * @author mkizub
 */
public sealed class ConditionPlayerState(PlayerState check, bool required): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Player? player = effector.getActingPlayer();
        switch (check)
        {
            case PlayerState.RESTING:
            {
                if (player != null)
                    return player.isSitting() == required;

                return !required;
            }
            case PlayerState.MOVING:
            {
                return effector.isMoving() == required;
            }
            case PlayerState.RUNNING:
            {
                return effector.isRunning() == required;
            }
            case PlayerState.STANDING:
            {
                if (player != null)
                {
                    return required != (player.isSitting() || player.isMoving());
                }

                return required != effector.isMoving();
            }
            case PlayerState.FLYING:
            {
                return effector.isFlying() == required;
            }
            case PlayerState.BEHIND:
            {
                return effected != null && effector.IsBehindOf(effected) == required;
            }
            case PlayerState.FRONT:
            {
                return effected != null && effector.IsInFrontOf(effected) == required;
            }
            case PlayerState.CHAOTIC:
            {
                if (player != null)
                    return player.getReputation() < 0 == required;

                return !required;
            }
            case PlayerState.OLYMPIAD:
            {
                if (player != null)
                    return player.isInOlympiadMode() == required;

                return !required;
            }
        }

        return !required;
    }
}