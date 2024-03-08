using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Physical Attack effect implementation.<br>
 * <b>Note</b>: Initial formula taken from PhysicalAttack.
 * @author Adry_85, Nik
 */
public class PhysicalAttackSaveHp: AbstractEffect
{
	private readonly double _power;
	private readonly double _criticalChance;
	private readonly bool _ignoreShieldDefence;
	private readonly bool _overHit;
	private readonly double _saveHp;
	
	public PhysicalAttackSaveHp(StatSet @params)
	{
		_power = @params.getDouble("power", 0);
		_criticalChance = @params.getDouble("criticalChance", 0);
		_ignoreShieldDefence = @params.getBoolean("ignoreShieldDefence", false);
		_overHit = @params.getBoolean("overHit", false);
		_saveHp = @params.getDouble("saveHp", 0);
	}
	
	public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
	{
		return !Formulas.calcSkillEvasion(effector, effected, skill);
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
		
		if (effected.isPlayer() && effected.getActingPlayer().isFakeDeath() && Config.FAKE_DEATH_DAMAGE_STAND)
		{
			effected.stopFakeDeath(true);
		}
		
		if (_overHit && effected.isAttackable())
		{
			((Attackable) effected).overhitEnabled(true);
		}
		
		double attack = effector.getPAtk();
		double defence = effected.getPDef();
		
		double shieldDefenceIgnoreRemoval = effected.getStat().getValue(Stat.SHIELD_DEFENCE_IGNORE_REMOVAL, 1);
		double shieldDefenceIgnoreRemovalAdd = effected.getStat().getValue(Stat.SHIELD_DEFENCE_IGNORE_REMOVAL_ADD, 0);
		if (!_ignoreShieldDefence || (shieldDefenceIgnoreRemoval > 1) || (shieldDefenceIgnoreRemovalAdd > 0))
		{
			byte shield = Formulas.calcShldUse(effector, effected);
			switch (shield)
			{
				case Formulas.SHIELD_DEFENSE_SUCCEED:
				{
					int shieldDef = effected.getShldDef();
					if (_ignoreShieldDefence)
					{
						double shieldDefMod = Math.Max(0, shieldDefenceIgnoreRemoval - 1);
						double ignoredShieldDef = shieldDef - (shieldDef * shieldDefMod);
						if (ignoredShieldDef > 0)
						{
							ignoredShieldDef = Math.Max(0, ignoredShieldDef - shieldDefenceIgnoreRemovalAdd);
						}
						defence += shieldDef - ignoredShieldDef;
					}
					else
					{
						defence += effected.getShldDef();
					}
					break;
				}
				case Formulas.SHIELD_DEFENSE_PERFECT_BLOCK:
				{
					defence = -1;
					break;
				}
			}
		}
		
		double damage = 1;
		bool critical = Formulas.calcCrit(_criticalChance, effector, effected, skill);
		
		if (defence != -1)
		{
			// Trait, elements
			double weaponTraitMod = Formulas.calcWeaponTraitBonus(effector, effected);
			double generalTraitMod = Formulas.calcGeneralTraitBonus(effector, effected, skill.getTraitType(), true);
			double weaknessMod = Formulas.calcWeaknessBonus(effector, effected, skill.getTraitType());
			double attributeMod = Formulas.calcAttributeBonus(effector, effected, skill);
			double pvpPveMod = Formulas.calculatePvpPveBonus(effector, effected, skill, true);
			double randomMod = effector.getRandomDamageMultiplier();
			
			// Skill specific mods.
			double weaponMod = effector.getAttackType().isRanged() ? 70 : 77;
			double power = _power + effector.getStat().getValue(Stat.SKILL_POWER_ADD, 0);
			double rangedBonus = effector.getAttackType().isRanged() ? attack + power : 0;
			double critMod = critical ? Formulas.calcCritDamage(effector, effected, skill) : 1;
			double ssmod = 1;
			if (skill.useSoulShot())
			{
				if (effector.isChargedShot(ShotType.SOULSHOTS))
				{
					ssmod = 2 * effector.getStat().getValue(Stat.SHOTS_BONUS) * effected.getStat().getValue(Stat.SOULSHOT_RESISTANCE, 1); // 2.04 for dual weapon?
				}
				else if (effector.isChargedShot(ShotType.BLESSED_SOULSHOTS))
				{
					ssmod = 4 * effector.getStat().getValue(Stat.SHOTS_BONUS) * effected.getStat().getValue(Stat.SOULSHOT_RESISTANCE, 1);
				}
			}
			
			// ...................____________Melee Damage_____________......................................___________________Ranged Damage____________________
			// ATTACK CALCULATION 77 * ((pAtk * lvlMod) + power) / pdef            RANGED ATTACK CALCULATION 70 * ((pAtk * lvlMod) + power + patk + power) / pdef
			// ```````````````````^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^``````````````````````````````````````^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
			double baseMod = (weaponMod * ((attack * effector.getLevelMod()) + power + rangedBonus)) / defence;
			damage = baseMod * ssmod * critMod * weaponTraitMod * generalTraitMod * weaknessMod * attributeMod * pvpPveMod * randomMod;
			damage *= effector.getStat().getValue(Stat.PHYSICAL_SKILL_POWER, 1);
		}
		
		double minHp = (effected.getMaxHp() * _saveHp) / 100;
		
		if ((effected.getCurrentHp() - damage) < minHp)
		{
			damage = effected.getCurrentHp() - minHp;
		}
		
		effector.doAttack(damage, effected, skill, false, false, critical, false);
	}
}