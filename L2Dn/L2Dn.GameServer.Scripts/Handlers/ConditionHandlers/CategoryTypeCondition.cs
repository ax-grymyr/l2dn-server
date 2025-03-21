using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Conditions;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.ConditionHandlers;

/**
 * @author Sdw, Mobius
 */
public class CategoryTypeCondition: ICondition
{
    private readonly Set<CategoryType> _categoryTypes = [];

    public CategoryTypeCondition(StatSet @params)
    {
        List<CategoryType>? items = @params.getEnumList<CategoryType>("category");
        if (items != null)
            _categoryTypes.addAll(items);
    }

    public bool test(Creature creature, WorldObject target)
    {
        foreach (CategoryType type in _categoryTypes)
        {
            if (creature.isInCategory(type))
                return true;
        }

        return false;
    }
}