using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Zoey76
 */
public sealed class ConditionPlayerLevelRange(int minLevel, int maxLevel): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        int level = effector.getLevel();
        return level >= minLevel && level <= maxLevel;
    }
}