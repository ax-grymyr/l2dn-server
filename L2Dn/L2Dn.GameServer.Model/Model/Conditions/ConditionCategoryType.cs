using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Condition Category Type implementation.
 * @author Adry_85
 */
public class ConditionCategoryType(Set<CategoryType> categoryTypes): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        foreach (CategoryType type in categoryTypes)
        {
            if (effector.isInCategory(type))
                return true;
        }

        return false;
    }
}