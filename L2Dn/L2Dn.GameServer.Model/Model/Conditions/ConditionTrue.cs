using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Conditions;

public sealed class ConditionTrue: Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        return true;
    }
}