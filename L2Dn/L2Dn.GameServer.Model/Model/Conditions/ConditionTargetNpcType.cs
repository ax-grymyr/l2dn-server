using System.Collections.Frozen;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetNpcType.
 */
public sealed class ConditionTargetNpcType(FrozenSet<InstanceType> types): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        if (effected == null)
            return false;

        return types.Contains(effected.InstanceType);
    }
}