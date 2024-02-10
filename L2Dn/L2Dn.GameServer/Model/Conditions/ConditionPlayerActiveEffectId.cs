using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerActiveEffectId.
 */
public class ConditionPlayerActiveEffectId: Condition
{
	private readonly int _effectId;
	private readonly int _effectLvl;
	
	/**
	 * Instantiates a new condition player active effect id.
	 * @param effectId the effect id
	 */
	public ConditionPlayerActiveEffectId(int effectId)
	{
		_effectId = effectId;
		_effectLvl = -1;
	}
	
	/**
	 * Instantiates a new condition player active effect id.
	 * @param effectId the effect id
	 * @param effectLevel the effect level
	 */
	public ConditionPlayerActiveEffectId(int effectId, int effectLevel)
	{
		_effectId = effectId;
		_effectLvl = effectLevel;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		BuffInfo info = effector.getEffectList().getBuffInfoBySkillId(_effectId);
		return ((info != null) && ((_effectLvl == -1) || (_effectLvl <= info.getSkill().getLevel())));
	}
}
