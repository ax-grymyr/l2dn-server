using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerClassIdRestriction.
 */
public class ConditionPlayerClassIdRestriction: Condition
{
	private readonly Set<int> _classIds;
	
	/**
	 * Instantiates a new condition player class id restriction.
	 * @param classId the class id
	 */
	public ConditionPlayerClassIdRestriction(Set<int> classId)
	{
		_classIds = classId;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return (effector.getActingPlayer() != null) && (_classIds.contains(effector.getActingPlayer().getClassId().getId()));
	}
}
