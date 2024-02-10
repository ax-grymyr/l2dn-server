using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Condition implementation to verify player's abnormal type and level.
 * @author Zoey76
 */
public class ConditionPlayerCheckAbnormal: Condition
{
	private readonly AbnormalType _type;
	private readonly int _level;
	
	/**
	 * Instantiates a new condition player check abnormal.
	 * @param type the abnormal type
	 */
	public ConditionPlayerCheckAbnormal(AbnormalType type)
	{
		_type = type;
		_level = -1;
	}
	
	/**
	 * Instantiates a new condition player check abnormal.
	 * @param type the abnormal type
	 * @param level the abnormal level
	 */
	public ConditionPlayerCheckAbnormal(AbnormalType type, int level)
	{
		_type = type;
		_level = level;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if (_level == -1)
		{
			return effector.getEffectList().hasAbnormalType(_type);
		}
		return effector.getEffectList().hasAbnormalType(_type, info => _level >= info.getSkill().getAbnormalLevel());
	}
}
