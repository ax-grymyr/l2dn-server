using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerCp.
 */
public sealed class ConditionPlayerCp(int cp): Condition
{
    protected override bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item)
    {
        return effector != null && effector.getCurrentCp() * 100 / effector.getMaxCp() >= cp;
    }
}