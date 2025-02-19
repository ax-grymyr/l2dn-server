using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Magical Attack effect implementation.
 * @author Adry_85, Mobius
 */
public class MagicalAttack: AbstractEffect
{
	private readonly double _power;
	private readonly bool _overHit;
	private readonly double _debuffModifier;
	private readonly double _raceModifier;
	private readonly Set<Race> _races = [];

	public MagicalAttack(StatSet @params)
	{
		_power = @params.getDouble("power", 0);
		_overHit = @params.getBoolean("overHit", false);
		_debuffModifier = @params.getDouble("debuffModifier", 1);
		_raceModifier = @params.getDouble("raceModifier", 1);
		if (@params.contains("races"))
		{
			foreach (string race in @params.getString("races", "").Split(";"))
			{
				_races.add(Enum.Parse<Race>(race));
			}
		}
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

        Player? effectedPlayer = effected.getActingPlayer();
		if (effected.isPlayer() && effectedPlayer != null && effectedPlayer.isFakeDeath() && Config.FAKE_DEATH_DAMAGE_STAND)
		{
			effected.stopFakeDeath(true);
		}

		if (_overHit && effected.isAttackable())
		{
			((Attackable) effected).overhitEnabled(true);
		}

		bool sps = skill.useSpiritShot() && effector.isChargedShot(ShotType.SPIRITSHOTS);
		bool bss = skill.useSpiritShot() && effector.isChargedShot(ShotType.BLESSED_SPIRITSHOTS);
		bool mcrit = Formulas.calcCrit(skill.getMagicCriticalRate(), effector, effected, skill);
		double damage = Formulas.calcMagicDam(effector, effected, skill, effector.getMAtk(), _power, effected.getMDef(), sps, bss, mcrit);

		// Apply debuff modifier.
		if (effected.getEffectList().getDebuffCount() > 0)
		{
			damage *= _debuffModifier;
		}

		// Apply race modifier.
		if (_races.Contains(effected.getRace()))
		{
			damage *= _raceModifier;
		}

		effector.doAttack(damage, effected, skill, false, false, mcrit, false);
	}
}