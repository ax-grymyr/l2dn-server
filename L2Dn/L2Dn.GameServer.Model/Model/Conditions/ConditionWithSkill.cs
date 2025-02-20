using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionWithSkill.
 * @author Steuf
 */
public sealed class ConditionWithSkill(bool withSkill): Condition
{
    protected override bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item)
    {
        return skill != null == withSkill;
    }
}