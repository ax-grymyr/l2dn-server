using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetAbnormal.
 * @author janiii
 */
public class ConditionTargetAbnormalType: Condition
{
	private readonly AbnormalType _abnormalType;
	
	/**
	 * Instantiates a new condition target abnormal type.
	 * @param abnormalType the abnormal type
	 */
	public ConditionTargetAbnormalType(AbnormalType abnormalType)
	{
		_abnormalType = abnormalType;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return effected.hasAbnormalType(_abnormalType);
	}
}
