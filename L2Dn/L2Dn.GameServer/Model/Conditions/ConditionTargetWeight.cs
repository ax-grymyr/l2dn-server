using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetWeight.
 * @author Zoey76
 */
public class ConditionTargetWeight : Condition
{
	private readonly int _weight;
	
	/**
	 * Instantiates a new condition player weight.
	 * @param weight the weight
	 */
	public ConditionTargetWeight(int weight)
	{
		_weight = weight;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if ((effected != null) && effected.isPlayer())
		{
			Player target = effected.getActingPlayer();
			if (!target.getDietMode() && (target.getMaxLoad() > 0))
			{
				int weightproc = (((target.getCurrentLoad() - target.getBonusWeightPenalty()) * 100) / target.getMaxLoad());
				return (weightproc < _weight);
			}
		}
		return false;
	}
}
