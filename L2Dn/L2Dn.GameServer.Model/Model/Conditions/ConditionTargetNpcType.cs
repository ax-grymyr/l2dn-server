using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetNpcType.
 */
public sealed class ConditionTargetNpcType(InstanceType[] type): Condition
{
    protected override bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item)
    {
        if (effected == null)
            return false;

        return Array.IndexOf(type, effected.InstanceType) >= 0;
    }
}