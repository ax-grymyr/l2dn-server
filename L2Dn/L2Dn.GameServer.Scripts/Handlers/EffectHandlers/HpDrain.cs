using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * HP Drain effect implementation.
 * @author Adry_85
 */
public class HpDrain: AbstractEffect
{
	private readonly double _power;
	private readonly double _percentage;
	
	public HpDrain(StatSet @params)
	{
		_power = @params.getDouble("power", 0);
		_percentage = @params.getDouble("percentage", 0);
	}
	
	public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
	{
		return !Formulas.calcSkillEvasion(effector, effected, skill);
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.HP_DRAIN;
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
		
		bool sps = skill.useSpiritShot() && effector.isChargedShot(ShotType.SPIRITSHOTS);
		bool bss = skill.useSpiritShot() && effector.isChargedShot(ShotType.BLESSED_SPIRITSHOTS);
		bool mcrit = Formulas.calcCrit(skill.getMagicCriticalRate(), effector, effected, skill);
		double damage = Formulas.calcMagicDam(effector, effected, skill, effector.getMAtk(), _power, effected.getMDef(), sps, bss, mcrit);
		
		double drain = 0;
		int cp = (int) effected.getCurrentCp();
		int hp = (int) effected.getCurrentHp();
		
		if (cp > 0)
		{
			drain = damage < cp ? 0 : damage - cp;
		}
		else if (damage > hp)
		{
			drain = hp;
		}
		else
		{
			drain = damage;
		}
		
		double hpAdd = _percentage / 100 * drain;
		double hpFinal = effector.getCurrentHp() + hpAdd > effector.getMaxHp() ? effector.getMaxHp() : effector.getCurrentHp() + hpAdd;
		effector.setCurrentHp(hpFinal);
		
		effector.doAttack(damage, effected, skill, false, false, mcrit, false);
	}
}