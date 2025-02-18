using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetWeight.
 * @author Zoey76
 */
public sealed class ConditionTargetWeight(int weight): Condition
{
    protected override bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item)
    {
        Player? target = effected?.getActingPlayer();
        if (effected != null && effected.isPlayer() && target != null)
        {
            if (!target.getDietMode() && target.getMaxLoad() > 0)
            {
                int weightproc = (target.getCurrentLoad() - target.getBonusWeightPenalty()) * 100 / target.getMaxLoad();
                return weightproc < weight;
            }
        }

        return false;
    }
}