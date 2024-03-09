using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Magical Soul Attack effect implementation.
 * @author Adry_85
 */
public class MagicalSoulAttack: AbstractEffect
{
	private readonly double _power;
	
	public MagicalSoulAttack(StatSet @params)
	{
		_power = @params.getDouble("power", 0);
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
		if (effector.isAlikeDead())
		{
			return;
		}
		
		if (effected.isPlayer() && effected.getActingPlayer().isFakeDeath() && Config.FAKE_DEATH_DAMAGE_STAND)
		{
			effected.stopFakeDeath(true);
		}
		
		int chargedLightSouls = Math.Min(skill.getMaxLightSoulConsumeCount(), effector.getActingPlayer().getCharges());
		if ((chargedLightSouls > 0) && !effector.getActingPlayer().decreaseCharges(chargedLightSouls))
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
			sm.Params.addSkillName(skill);
			effector.sendPacket(sm);
			return;
		}
		
		int chargedShadowSouls = Math.Min(skill.getMaxShadowSoulConsumeCount(), effector.getActingPlayer().getCharges());
		if ((chargedShadowSouls > 0) && !effector.getActingPlayer().decreaseCharges(chargedShadowSouls))
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
			sm.Params.addSkillName(skill);
			effector.sendPacket(sm);
			return;
		}
		
		int chargedSouls = chargedLightSouls + chargedShadowSouls;
		if (chargedSouls < 1)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
			sm.Params.addSkillName(skill);
			effector.sendPacket(sm);
			return;
		}
		
		bool sps = skill.useSpiritShot() && effector.isChargedShot(ShotType.SPIRITSHOTS);
		bool bss = skill.useSpiritShot() && effector.isChargedShot(ShotType.BLESSED_SPIRITSHOTS);
		bool mcrit = Formulas.calcCrit(skill.getMagicCriticalRate(), effector, effected, skill);
		double mAtk = effector.getMAtk() * (chargedSouls > 0 ? (1.3 + (chargedSouls * 0.05)) : 1);
		double damage = Formulas.calcMagicDam(effector, effected, skill, mAtk, _power, effected.getMDef(), sps, bss, mcrit);
		
		effector.doAttack(damage, effected, skill, false, false, mcrit, false);
	}
}