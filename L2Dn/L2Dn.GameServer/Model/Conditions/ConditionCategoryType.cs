using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Condition Category Type implementation.
 * @author Adry_85
 */
public class ConditionCategoryType: Condition
{
	private readonly Set<CategoryType> _categoryTypes;
	
	public ConditionCategoryType(Set<CategoryType> categoryTypes)
	{
		_categoryTypes = categoryTypes;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		foreach (CategoryType type in _categoryTypes.Keys)
		{
			if (effector.isInCategory(type))
			{
				return true;
			}
		}
		return false;
	}
}
