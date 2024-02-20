using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * This condition becomes true whether the player is transformed and the transformation Id match the parameter or<br>
 * the parameter is -1 which returns true if player is transformed regardless the transformation Id.
 * @author Zoey76
 */
public class ConditionPlayerTransformationId : Condition
{
	private readonly int _id;
	
	/**
	 * Instantiates a new condition player is transformed.
	 * @param id the transformation Id.
	 */
	public ConditionPlayerTransformationId(int id)
	{
		_id = id;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return _id == -1 ? effector.isTransformed() : effector.getTransformationId() == _id;
	}
}
