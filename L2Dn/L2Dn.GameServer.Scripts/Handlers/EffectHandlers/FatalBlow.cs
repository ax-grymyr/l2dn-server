using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Fatal Blow effect implementation.
 * @author Adry_85
 */
public class FatalBlow: AbstractEffect
{
	private readonly double _power;
	private readonly double _chanceBoost;
	private readonly double _criticalChance;
	private readonly Set<AbnormalType> _abnormals;
	private readonly double _abnormalPower;
	private readonly bool _overHit;
	
	public FatalBlow(StatSet @params)
	{
		_power = @params.getDouble("power");
		_chanceBoost = @params.getDouble("chanceBoost");
		_criticalChance = @params.getDouble("criticalChance", 0);
		_overHit = @params.getBoolean("overHit", false);
		
		String abnormals = @params.getString("abnormalType", null);
		if (!string.IsNullOrEmpty(abnormals))
		{
			_abnormals = new();
			foreach (String slot in abnormals.Split(";"))
			{
				_abnormals.add(Enum.Parse<AbnormalType>(slot));
			}
		}
		else
		{
			_abnormals = new();
		}
		_abnormalPower = @params.getDouble("abnormalPower", 1);
	}
	
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
		
		double power = _power;
		
		// Check if we apply an abnormal modifier.
		if (!_abnormals.isEmpty())
		{
			foreach (AbnormalType abnormal in _abnormals)
			{
				if (effected.hasAbnormalType(abnormal))
				{
					power += _abnormalPower;
					break;
				}
			}
		}
		
		bool ss = skill.useSoulShot() && (effector.isChargedShot(ShotType.SOULSHOTS) || effector.isChargedShot(ShotType.BLESSED_SOULSHOTS));
		byte shld = Formulas.calcShldUse(effector, effected);
		double damage = Formulas.calcBlowDamage(effector, effected, skill, false, power, shld, ss);
		bool crit = Formulas.calcCrit(_criticalChance, effector, effected, skill);
		
		if (crit)
		{
			damage *= 2;
		}
		
		effector.doAttack(damage, effected, skill, false, false, true, false);
	}
}