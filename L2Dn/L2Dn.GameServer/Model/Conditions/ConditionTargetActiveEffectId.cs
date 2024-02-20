using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetActiveEffectId.
 */
public class ConditionTargetActiveEffectId : Condition
{
	private readonly int _effectId;
	private readonly int _effectLvl;
	
	/**
	 * Instantiates a new condition target active effect id.
	 * @param effectId the effect id
	 */
	public ConditionTargetActiveEffectId(int effectId)
	{
		_effectId = effectId;
		_effectLvl = -1;
	}
	
	/**
	 * Instantiates a new condition target active effect id.
	 * @param effectId the effect id
	 * @param effectLevel the effect level
	 */
	public ConditionTargetActiveEffectId(int effectId, int effectLevel)
	{
		_effectId = effectId;
		_effectLvl = effectLevel;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		BuffInfo info = effected.getEffectList().getBuffInfoBySkillId(_effectId);
		return (info != null) && ((_effectLvl == -1) || (_effectLvl <= info.getSkill().getLevel()));
	}
}
