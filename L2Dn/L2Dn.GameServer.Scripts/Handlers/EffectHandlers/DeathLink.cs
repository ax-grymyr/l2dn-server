using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Death Link effect implementation.
 * @author Adry_85
 */
public class DeathLink: AbstractEffect
{
	private readonly double _power;
	
	public DeathLink(StatSet @params)
	{
		_power = @params.getDouble("power", 0);
	}
	
	public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
	{
		return !Formulas.calcSkillEvasion(effector, effected, skill);
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.DEATH_LINK;
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
		
		if (effected.isPlayer() && effected.getActingPlayer().isFakeDeath() && Config.FAKE_DEATH_DAMAGE_STAND)
		{
			effected.stopFakeDeath(true);
		}
		
		bool mcrit = Formulas.calcCrit(skill.getMagicCriticalRate(), effector, effected, skill);
		double damage = Formulas.calcMagicDam(effector, effected, skill, effector.getMAtk(), _power * (-((effector.getCurrentHp() * 2) / effector.getMaxHp()) + 2), effected.getMDef(), sps, bss, mcrit);
		effector.doAttack(damage, effected, skill, false, false, mcrit, false);
	}
}