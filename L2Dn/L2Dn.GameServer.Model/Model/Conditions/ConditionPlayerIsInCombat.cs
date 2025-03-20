using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Sdw
 */
public sealed class ConditionPlayerIsInCombat(bool value): Condition
{
    private bool Value => value;

    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        bool isInCombat = !AttackStanceTaskManager.getInstance().hasAttackStanceTask(effector);
        return value == isInCombat;
    }

    public override int GetHashCode() => HashCode.Combine(value);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x.Value);
}