using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Soul Blow effect implementation.
 * @author Adry_85
 */
public class SoulBlow: AbstractEffect
{
	private readonly double _power;
	private readonly double _chanceBoost;
	private readonly bool _overHit;
	
	public SoulBlow(StatSet @params)
	{
		_power = @params.getDouble("power");
		_chanceBoost = @params.getDouble("chanceBoost");
		_overHit = @params.getBoolean("overHit", false);
	}
	
	/**
	 * If is not evaded and blow lands.
	 * @param effector
	 * @param effected
	 * @param skill
	 */
	public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
	{
		return !Formulas.calcSkillEvasion(effector, effected, skill) && Formulas.calcBlowSuccess(effector, effected, skill, _chanceBoost);
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.PHYSICAL_ATTACK;
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effector.isAlikeDead())
		{
			return;
		}
		
		if (_overHit && effected.isAttackable())
		{
			((Attackable) effected).overhitEnabled(true);
		}
		
		bool ss = skill.useSoulShot() && (effector.isChargedShot(ShotType.SOULSHOTS) || effector.isChargedShot(ShotType.BLESSED_SOULSHOTS));
		byte shld = Formulas.calcShldUse(effector, effected);
		double damage = Formulas.calcBlowDamage(effector, effected, skill, false, _power, shld, ss);
		
		if (effector.isPlayer())
		{
			if (skill.getMaxLightSoulConsumeCount() > 0)
			{
				// Souls Formula (each soul increase +4%)
				int chargedSouls = (effector.getActingPlayer().getChargedSouls(SoulType.LIGHT) <= skill.getMaxLightSoulConsumeCount()) ? effector.getActingPlayer().getChargedSouls(SoulType.LIGHT) : skill.getMaxLightSoulConsumeCount();
				damage *= 1 + (chargedSouls * 0.04);
			}
			if (skill.getMaxShadowSoulConsumeCount() > 0)
			{
				// Souls Formula (each soul increase +4%)
				int chargedSouls = (effector.getActingPlayer().getChargedSouls(SoulType.SHADOW) <= skill.getMaxShadowSoulConsumeCount()) ? effector.getActingPlayer().getChargedSouls(SoulType.SHADOW) : skill.getMaxShadowSoulConsumeCount();
				damage *= 1 + (chargedSouls * 0.04);
			}
		}
		
		effector.doAttack(damage, effected, skill, false, false, true, false);
	}
}