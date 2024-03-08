using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Magical Abnormal-depending dispel Attack effect implementation.
 * @author Nik
 */
public class MagicalAbnormalDispelAttack: AbstractEffect
{
	private readonly double _power;
	private readonly AbnormalType _abnormalType;
	
	public MagicalAbnormalDispelAttack(StatSet @params)
	{
		_power = @params.getDouble("power", 0);
		string abnormalType = @params.getString("abnormalType", null);
		if (Enum.TryParse<AbnormalType>(abnormalType, out var val))
			_abnormalType = val;
	}
	
	public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
	{
		return !Formulas.calcSkillEvasion(effector, effected, skill);
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.MAGICAL_ATTACK;
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		// First dispells the effect, then does damage. Sometimes the damage is evaded, but debuff is still dispelled.
		if (effector.isAlikeDead() || (_abnormalType == AbnormalType.NONE) || !effected.getEffectList().stopEffects(_abnormalType))
		{
			return;
		}
		
		if (effected.isPlayer() && effected.getActingPlayer().isFakeDeath() && Config.FAKE_DEATH_DAMAGE_STAND)
		{
			effected.stopFakeDeath(true);
		}
		
		bool sps = skill.useSpiritShot() && effector.isChargedShot(ShotType.SPIRITSHOTS);
		bool bss = skill.useSpiritShot() && effector.isChargedShot(ShotType.BLESSED_SPIRITSHOTS);
		bool mcrit = Formulas.calcCrit(skill.getMagicCriticalRate(), effector, effected, skill);
		double damage = Formulas.calcMagicDam(effector, effected, skill, effector.getMAtk(), _power, effected.getMDef(), sps, bss, mcrit);
		
		effector.doAttack(damage, effected, skill, false, false, mcrit, false);
	}
}