using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetClassIdRestriction.
 */
public class ConditionTargetClassIdRestriction: Condition
{
	private readonly Set<CharacterClass> _classIds;
	
	/**
	 * Instantiates a new condition target class id restriction.
	 * @param classId the class id
	 */
	public ConditionTargetClassIdRestriction(Set<CharacterClass> classId)
	{
		_classIds = classId;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return effected.isPlayer() && (_classIds.Contains(effected.getActingPlayer().getClassId()));
	}
}
