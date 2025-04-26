using System.Collections.Frozen;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Conditions;

public sealed class ConditionTargetCategoryType(FrozenSet<CategoryType> categoryTypes): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        if (effected is null)
            return false;

        foreach (CategoryType type in categoryTypes)
        {
            if (effected.isInCategory(type))
                return true;
        }

        return false;
    }
}