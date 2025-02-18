using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Sdw
 */
public sealed class ConditionPlayerImmobile(bool value): Condition
{
    protected override bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item)
    {
        bool isImmobile = !effector.isMovementDisabled();
        return value == isImmobile;
    }
}