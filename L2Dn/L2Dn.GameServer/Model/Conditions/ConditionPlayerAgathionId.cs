using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerAgathionId.
 */
public class ConditionPlayerAgathionId: Condition
{
	private readonly int _agathionId;
	
	/**
	 * Instantiates a new condition player agathion id.
	 * @param agathionId the agathion id
	 */
	public ConditionPlayerAgathionId(int agathionId)
	{
		_agathionId = agathionId;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return (effector.getActingPlayer() != null) && (effector.getActingPlayer().getAgathionId() == _agathionId);
	}
}
