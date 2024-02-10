using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetClassIdRestriction.
 */
public class ConditionTargetClassIdRestriction: Condition
{
	private readonly Set<int> _classIds;
	
	/**
	 * Instantiates a new condition target class id restriction.
	 * @param classId the class id
	 */
	public ConditionTargetClassIdRestriction(Set<int> classId)
	{
		_classIds = classId;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return effected.isPlayer() && (_classIds.contains(effected.getActingPlayer().getClassId().getId()));
	}
}
