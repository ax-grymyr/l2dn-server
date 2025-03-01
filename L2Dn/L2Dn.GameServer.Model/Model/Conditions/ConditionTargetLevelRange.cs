using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author UnAfraid
 */
public sealed class ConditionTargetLevelRange(int minLevel, int maxLevel): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        if (effected is null)
            return false;

        int level = effected.getLevel();
        return level >= minLevel && level <= maxLevel;
    }
}