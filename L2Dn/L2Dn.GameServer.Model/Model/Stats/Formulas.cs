using System.Collections.Immutable;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Transforms;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using CollectionExtensions = System.Collections.Generic.CollectionExtensions;

namespace L2Dn.GameServer.Model.Stats;

/**
 * Global calculations.
 */
public class Formulas
{
	/** Regeneration Task period. */
	private const int HP_REGENERATE_PERIOD = 3000; // 3 secs
	
	public const byte SHIELD_DEFENSE_FAILED = 0; // no shield defense
	public const byte SHIELD_DEFENSE_SUCCEED = 1; // normal shield defense
	public const byte SHIELD_DEFENSE_PERFECT_BLOCK = 2; // perfect block
	
	public const int SKILL_LAUNCH_TIME = 500; // The time to pass after the skill launching until the skill to affect targets. In milliseconds
	private const byte MELEE_ATTACK_RANGE = 40;
	
	/**
	 * Return the period between 2 regeneration task (3s for Creature, 5 Min for Door).
	 * @param creature
	 * @return
	 */
	public static int getRegeneratePeriod(Creature creature)
	{
		return creature.isDoor() ? HP_REGENERATE_PERIOD * 100 : HP_REGENERATE_PERIOD;
	}
	
	public static double calcBlowDamage(Creature attacker, Creature target, Skill skill, bool backstab, double power, byte shld, bool ss)
	{
		double defence = target.getPDef();
		
		switch (shld)
		{
			case SHIELD_DEFENSE_SUCCEED:
			{
				defence += target.getShldDef();
				break;
			}
			case SHIELD_DEFENSE_PERFECT_BLOCK: // perfect block
			{
				return 1;
			}
		}
		
		// Critical
		double criticalMod = attacker.getStat().getValue(Stat.CRITICAL_DAMAGE, 1);
		double criticalPositionMod = attacker.getStat().getPositionTypeValue(Stat.CRITICAL_DAMAGE, PositionUtil.getPosition(attacker, target));
		double criticalVulnMod = target.getStat().getValue(Stat.DEFENCE_CRITICAL_DAMAGE, 1);
		double criticalAddMod = attacker.getStat().getValue(Stat.CRITICAL_DAMAGE_ADD, 0);
		double criticalAddVuln = target.getStat().getValue(Stat.DEFENCE_CRITICAL_DAMAGE_ADD, 0);
		double criticalSkillMod = calcCritDamage(attacker, target, skill) / 2;
		// Trait, elements
		double weaponTraitMod = calcWeaponTraitBonus(attacker, target);
		double generalTraitMod = calcGeneralTraitBonus(attacker, target, skill.getTraitType(), true);
		double weaknessMod = calcWeaknessBonus(attacker, target, skill.getTraitType());
		double attributeMod = calcAttributeBonus(attacker, target, skill);
		double randomMod = attacker.getRandomDamageMultiplier();
		double pvpPveMod = calculatePvpPveBonus(attacker, target, skill, true);
		
		// Initial damage
		double ssmod = ss ? 2 * attacker.getStat().getValue(Stat.SHOTS_BONUS) * target.getStat().getValue(Stat.SOULSHOT_RESISTANCE, 1) : 1; // 2.04 for dual weapon?
		double cdMult = criticalMod * (((criticalPositionMod - 1) / 2) + 1) * (((criticalVulnMod - 1) / 2) + 1);
		double cdPatk = (criticalAddMod + criticalAddVuln) * criticalSkillMod;
		Position position = PositionUtil.getPosition(attacker, target);
		double isPosition = position == Position.BACK ? 0.2 : position == Position.SIDE ? 0.05 : 0;
		
		// Mobius: Manage level difference.
		// if (attacker.getLevel() < target.getLevel())
		// {
		// power *= 1 - (Math.Min(target.getLevel() - attacker.getLevel(), 9) / 10);
		// }
		
		double balanceMod = 1;
		if (attacker.isPlayable())
		{
			balanceMod = target.isPlayable()
				? CollectionExtensions.GetValueOrDefault(Config.PVP_BLOW_SKILL_DAMAGE_MULTIPLIERS, attacker.getActingPlayer().getClassId(), 1)
				: CollectionExtensions.GetValueOrDefault(Config.PVE_BLOW_SKILL_DAMAGE_MULTIPLIERS, attacker.getActingPlayer().getClassId(), 1);
		}

		if (target.isPlayable())
		{
			defence *= attacker.isPlayable()
				? CollectionExtensions.GetValueOrDefault(Config.PVP_BLOW_SKILL_DEFENCE_MULTIPLIERS, target.getActingPlayer().getClassId(), 1)
				: CollectionExtensions.GetValueOrDefault(Config.PVE_BLOW_SKILL_DEFENCE_MULTIPLIERS, target.getActingPlayer().getClassId(), 1);
		}

		double skillPower = power + attacker.getStat().getValue(Stat.SKILL_POWER_ADD, 0);
		
		// ........................_____________________________Initial Damage____________________________...___________Position Additional Damage___________..._CriticalAdd_
		// ATTACK CALCULATION 77 * [(skillpower+patk) * 0.666 * cdbonus * cdPosBonusHalf * cdVulnHalf * ss + isBack0.2Side0.05 * (skillpower+patk*ss) * random + 6 * cd_patk] / pdef
		// ````````````````````````^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^```^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^```^^^^^^^^^^^^
		double baseMod = (77 * (((skillPower + attacker.getPAtk()) * 0.666) + (isPosition * (skillPower + attacker.getPAtk()) * randomMod) + (6 * cdPatk))) / defence;
		double damage = baseMod * ssmod * cdMult * weaponTraitMod * generalTraitMod * weaknessMod * attributeMod * randomMod * pvpPveMod * balanceMod;
		
		return damage;
	}
	
	public static double calcMagicDam(Creature attacker, Creature target, Skill skill, double mAtk, double power, double mDef, bool sps, bool bss, bool mcrit)
	{
		// Bonus Spirit shot
		double shotsBonus = bss
			?
			4 * attacker.getStat().getValue(Stat.SHOTS_BONUS) * target.getStat().getValue(Stat.SPIRITSHOT_RESISTANCE, 1)
			: sps
				? 2 * attacker.getStat().getValue(Stat.SHOTS_BONUS) *
				  target.getStat().getValue(Stat.SPIRITSHOT_RESISTANCE, 1)
				: 1;
		
		double critMod = mcrit ? calcCritDamage(attacker, target, skill) : 1; // TODO not really a proper way... find how it works then implement. // damage += attacker.getStat().getValue(Stats.MAGIC_CRIT_DMG_ADD, 0);
		
		// Trait, elements
		double generalTraitMod = calcGeneralTraitBonus(attacker, target, skill.getTraitType(), true);
		double weaknessMod = calcWeaknessBonus(attacker, target, skill.getTraitType());
		double attributeMod = calcAttributeBonus(attacker, target, skill);
		double randomMod = attacker.getRandomDamageMultiplier();
		double pvpPveMod = calculatePvpPveBonus(attacker, target, skill, mcrit);
		
		// MDAM Formula.
		double damage = ((77 * (power + attacker.getStat().getValue(Stat.SKILL_POWER_ADD, 0)) * Math.Sqrt(mAtk)) / mDef) * shotsBonus;
		
		// Failure calculation
		if (Config.ALT_GAME_MAGICFAILURES && !calcMagicSuccess(attacker, target, skill))
		{
			if (attacker.isPlayer())
			{
				if (calcMagicSuccess(attacker, target, skill))
				{
					if (skill.hasEffectType(EffectType.HP_DRAIN))
					{
						attacker.sendPacket(SystemMessageId.DRAIN_WAS_ONLY_50_SUCCESSFUL);
					}
					else
					{
						attacker.sendPacket(SystemMessageId.YOUR_ATTACK_HAS_FAILED);
					}
					
					damage /= 2;
				}
				else
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_HAS_RESISTED_YOUR_S2);
					sm.Params.addString(target.getName());
					sm.Params.addSkillName(skill);
					attacker.sendPacket(sm);
					damage = 1;
				}
			}
			
			if (target.isPlayer())
			{
				SystemMessagePacket sm = (skill.hasEffectType(EffectType.HP_DRAIN)) ? new SystemMessagePacket(SystemMessageId.YOU_RESISTED_C1_S_DRAIN) : new SystemMessagePacket(SystemMessageId.YOU_RESISTED_C1_S_MAGIC);
				sm.Params.addString(attacker.getName());
				target.sendPacket(sm);
			}
		}
		
		// Nasseka rev. 10196: generalTraitMod == 0 ? 1 : generalTraitMod (no invulnerable traits).
		damage = damage * critMod * (generalTraitMod == 0 ? 1 : generalTraitMod) * weaknessMod * attributeMod * randomMod * pvpPveMod;
		damage *= attacker.getStat().getValue(Stat.MAGICAL_SKILL_POWER, 1);
		
		// AoE modifiers.
		if (skill.isBad() && (skill.getAffectLimit() > 0))
		{
			damage *= Math.Max(
				(attacker.getStat().getMul(Stat.AREA_OF_EFFECT_DAMAGE_MODIFY, 1) -
				 target.getStat().getValue(Stat.AREA_OF_EFFECT_DAMAGE_DEFENCE, 0)), 0.01);
		}
		
		return damage;
	}
	
	/**
	 * Returns true in case of critical hit
	 * @param rateValue
	 * @param skill
	 * @param creature
	 * @param target
	 * @return
	 */
	public static bool calcCrit(double rateValue, Creature creature, Creature target, Skill skill)
	{
		double rate = rateValue;
		double balanceMod;
		
		if (skill != null)
		{
			// Magic Critical Rate.
			if (skill.isMagic())
			{
				rate = creature.getStat().getValue(Stat.MAGIC_CRITICAL_RATE);
				if ((target == null) || !skill.isBad())
				{
					return Math.Min(rate, 320) > Rnd.get(1000);
				}
				
				balanceMod = 1;
				if (creature.isPlayable())
				{
					balanceMod = target.isPlayable()
						? CollectionExtensions.GetValueOrDefault(Config.PVP_MAGICAL_SKILL_CRITICAL_CHANCE_MULTIPLIERS, creature.getActingPlayer().getClassId(), 1)
						: CollectionExtensions.GetValueOrDefault(Config.PVE_MAGICAL_SKILL_CRITICAL_CHANCE_MULTIPLIERS, creature.getActingPlayer().getClassId(), 1);
				}

				double finalRate = target.getStat().getValue(Stat.DEFENCE_MAGIC_CRITICAL_RATE, rate) +
				                   target.getStat().getValue(Stat.DEFENCE_MAGIC_CRITICAL_RATE_ADD, 0);
				
				if ((creature.getLevel() >= 78) && (target.getLevel() >= 78))
				{
					finalRate += Math.Sqrt(creature.getLevel()) + ((creature.getLevel() - target.getLevel()) / 25);
					return Math.Min(finalRate, 320 * balanceMod) > Rnd.get(1000);
				}
				
				return (Math.Min(finalRate, 200) * balanceMod) > Rnd.get(1000);
			}
			
			// Physical skill critical rate.
			double statBonus;
			
			// There is a chance that activeChar has altered base stat for skill critical.
			byte skillCritRateStat = (byte) creature.getStat().getValue(Stat.STAT_BONUS_SKILL_CRITICAL);
			ImmutableArray<BaseStat> baseStats = EnumUtil.GetValues<BaseStat>();
			if ((skillCritRateStat >= 0) && (skillCritRateStat < baseStats.Length))
			{
				// Best tested.
				statBonus = baseStats[skillCritRateStat].calcBonus(creature);
			}
			else
			{
				// Default base stat used for skill critical formula is STR.
				statBonus = BaseStat.STR.calcBonus(creature);
			}
			
			double rateBonus = creature.getStat().getMul(Stat.CRITICAL_RATE_SKILL, 1);
			double rateDefenceBonus = target.getStat().getValue(Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_RATE, 1) +
			                          (target.getStat().getValue(Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_RATE_ADD, 0) /
			                           100);
			
			balanceMod = 1;
			if (creature.isPlayable())
			{
				balanceMod = target.isPlayable()
					? CollectionExtensions.GetValueOrDefault(Config.PVP_PHYSICAL_SKILL_CRITICAL_CHANCE_MULTIPLIERS, creature.getActingPlayer().getClassId(), 1)
					: CollectionExtensions.GetValueOrDefault(Config.PVE_PHYSICAL_SKILL_CRITICAL_CHANCE_MULTIPLIERS, creature.getActingPlayer().getClassId(), 1);
			}
			
			return CommonUtil.constrain(rate * statBonus * rateBonus * rateDefenceBonus * balanceMod, 5, 90) > Rnd.get(100);
		}
		
		// Autoattack critical rate.
		// Even though, visible critical rate is capped to 500, you can reach higher than 50% chance with position and level modifiers.
		// TODO: Find retail-like calculation for criticalRateMod.
		double criticalRateMod = (target.getStat().getValue(Stat.DEFENCE_CRITICAL_RATE, rate) + target.getStat().getValue(Stat.DEFENCE_CRITICAL_RATE_ADD, 0)) / 10;
		double criticalLocBonus = calcCriticalPositionBonus(creature, target);
		double criticalHeightBonus = calcCriticalHeightBonus(creature, target);
		rate = criticalLocBonus * criticalRateMod * criticalHeightBonus;
		
		// Autoattack critical depends on level difference at high levels as well.
		if ((creature.getLevel() >= 78) || (target.getLevel() >= 78))
		{
			rate += (Math.Sqrt(creature.getLevel()) * (creature.getLevel() - target.getLevel()) * 0.125);
		}
		
		// Autoattack critical rate is limited between 3%-97%.
		rate = CommonUtil.constrain(rate, 3, 97);
		
		balanceMod = 1;
		if (creature.isPlayable())
		{
			balanceMod = target.isPlayable()
				? CollectionExtensions.GetValueOrDefault(Config.PVP_PHYSICAL_ATTACK_CRITICAL_CHANCE_MULTIPLIERS, creature.getActingPlayer().getClassId(), 1)
				: CollectionExtensions.GetValueOrDefault(Config.PVE_PHYSICAL_ATTACK_CRITICAL_CHANCE_MULTIPLIERS, creature.getActingPlayer().getClassId(), 1);
		}
		
		return (rate * balanceMod) > Rnd.get(100);
	}
	
	/**
	 * Gets the default (10% for side, 30% for back) positional critical rate bonus and multiplies it by any buffs that give positional critical rate bonus.
	 * @param creature the attacker.
	 * @param target the target.
	 * @return a multiplier representing the positional critical rate bonus. Autoattacks for example get this bonus on top of the already capped critical rate of 500.
	 */
	public static double calcCriticalPositionBonus(Creature creature, Creature target)
	{
		// Position position = activeChar.getStat().has(Stats.ATTACK_BEHIND) ? Position.BACK : Position.getPosition(activeChar, target);
		switch (PositionUtil.getPosition(creature, target))
		{
			case Position.SIDE: // 10% Critical Chance bonus when attacking from side.
			{
				return 1.1 * creature.getStat().getPositionTypeValue(Stat.CRITICAL_RATE, Position.SIDE);
			}
			case Position.BACK: // 30% Critical Chance bonus when attacking from back.
			{
				return 1.3 * creature.getStat().getPositionTypeValue(Stat.CRITICAL_RATE, Position.BACK);
			}
			default: // No Critical Chance bonus when attacking from front.
			{
				return creature.getStat().getPositionTypeValue(Stat.CRITICAL_RATE, Position.FRONT);
			}
		}
	}
	
	public static double calcCriticalHeightBonus(ILocational from, ILocational target)
	{
		return ((((CommonUtil.constrain(from.getZ() - target.getZ(), -25, 25) * 4) / 5) + 10) / 100) + 1;
	}
	
	/**
	 * @param attacker
	 * @param target
	 * @param skill {@code skill} to be used in the calculation, else calculation will result for autoattack.
	 * @return regular critical damage bonus. Positional bonus is excluded!
	 */
	public static double calcCritDamage(Creature attacker, Creature target, Skill skill)
	{
		double criticalDamage;
		double defenceCriticalDamage;
		double balanceMod = 1;
		
		if (skill != null)
		{
			if (skill.isMagic())
			{
				// Magic critical damage.
				criticalDamage = attacker.getStat().getValue(Stat.MAGIC_CRITICAL_DAMAGE, 1);
				defenceCriticalDamage = target.getStat().getValue(Stat.DEFENCE_MAGIC_CRITICAL_DAMAGE, 1);
				if (attacker.isPlayable())
				{
					balanceMod = target.isPlayable()
						? CollectionExtensions.GetValueOrDefault(Config.PVP_MAGICAL_SKILL_CRITICAL_DAMAGE_MULTIPLIERS, attacker.getActingPlayer().getClassId(), 1)
						: CollectionExtensions.GetValueOrDefault(Config.PVE_MAGICAL_SKILL_CRITICAL_DAMAGE_MULTIPLIERS, attacker.getActingPlayer().getClassId(), 1);
				}
			}
			else
			{
				criticalDamage = attacker.getStat().getValue(Stat.PHYSICAL_SKILL_CRITICAL_DAMAGE, 1);
				defenceCriticalDamage = target.getStat().getValue(Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_DAMAGE, 1);
				if (attacker.isPlayable())
				{
					balanceMod = target.isPlayable()
						? CollectionExtensions.GetValueOrDefault(Config.PVP_PHYSICAL_SKILL_CRITICAL_DAMAGE_MULTIPLIERS, attacker.getActingPlayer().getClassId(), 1)
						: CollectionExtensions.GetValueOrDefault(Config.PVE_PHYSICAL_SKILL_CRITICAL_DAMAGE_MULTIPLIERS, attacker.getActingPlayer().getClassId(), 1);
				}
			}
		}
		else
		{
			// Autoattack critical damage.
			criticalDamage = attacker.getStat().getValue(Stat.CRITICAL_DAMAGE, 1) * attacker.getStat().getPositionTypeValue(Stat.CRITICAL_DAMAGE, PositionUtil.getPosition(attacker, target));
			defenceCriticalDamage = target.getStat().getValue(Stat.DEFENCE_CRITICAL_DAMAGE, 1);
			if (attacker.isPlayable())
			{
				balanceMod = target.isPlayable()
					? CollectionExtensions.GetValueOrDefault(Config.PVP_PHYSICAL_ATTACK_CRITICAL_DAMAGE_MULTIPLIERS, attacker.getActingPlayer().getClassId(), 1)
					: CollectionExtensions.GetValueOrDefault(Config.PVE_PHYSICAL_ATTACK_CRITICAL_DAMAGE_MULTIPLIERS, attacker.getActingPlayer().getClassId(), 1);
			}
		}
		
		return 2 * criticalDamage * defenceCriticalDamage * balanceMod;
	}
	
	/**
	 * @param attacker
	 * @param target
	 * @param skill {@code skill} to be used in the calculation, else calculation will result for autoattack.
	 * @return critical damage additional bonus, not multiplier!
	 */
	public static double calcCritDamageAdd(Creature attacker, Creature target, Skill skill)
	{
		double criticalDamageAdd;
		double defenceCriticalDamageAdd;
		
		if (skill != null)
		{
			if (skill.isMagic())
			{
				// Magic critical damage.
				criticalDamageAdd = attacker.getStat().getValue(Stat.MAGIC_CRITICAL_DAMAGE_ADD, 0);
				defenceCriticalDamageAdd = target.getStat().getValue(Stat.DEFENCE_MAGIC_CRITICAL_DAMAGE_ADD, 0);
			}
			else
			{
				criticalDamageAdd = attacker.getStat().getValue(Stat.PHYSICAL_SKILL_CRITICAL_DAMAGE_ADD, 0);
				defenceCriticalDamageAdd = target.getStat().getValue(Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_DAMAGE_ADD, 0);
			}
		}
		else
		{
			// Autoattack critical damage.
			criticalDamageAdd = attacker.getStat().getValue(Stat.CRITICAL_DAMAGE_ADD, 0);
			defenceCriticalDamageAdd = target.getStat().getValue(Stat.DEFENCE_CRITICAL_DAMAGE_ADD, 0);
		}
		
		return criticalDamageAdd + defenceCriticalDamageAdd;
	}
	
	/**
	 * @param target
	 * @param dmg
	 * @return true in case when ATTACK is canceled due to hit
	 */
	public static bool calcAtkBreak(Creature target, double dmg)
	{
		if (target.isChanneling())
		{
			return false;
		}
		
		// Cannot interrupt targets affected by Burst or Superior Burst Casting.
		if (target.hasAbnormalType(AbnormalType.DC_MOD))
		{
			return false;
		}
		
		double init = 0;
		
		if (Config.ALT_GAME_CANCEL_CAST && target.isCastingNow(s => s.canAbortCast()))
		{
			init = 15;
		}
		if (Config.ALT_GAME_CANCEL_BOW && target.isAttackingNow())
		{
			Weapon wpn = target.getActiveWeaponItem();
			if ((wpn != null) && (wpn.getItemType() == WeaponType.BOW))
			{
				init = 15;
			}
		}
		
		if (target.isRaid() || target.isHpBlocked() || (init <= 0))
		{
			return false; // No attack break
		}
		
		// Chance of break is higher with higher dmg
		init += Math.Sqrt(13 * dmg);
		
		// Chance is affected by target MEN
		init -= ((BaseStat.MEN.calcBonus(target) * 100) - 100);
		
		// Calculate all modifiers for ATTACK_CANCEL
		double rate = target.getStat().getValue(Stat.ATTACK_CANCEL, init);
		
		// Adjust the rate to be between 1 and 99
		rate = Math.Max(Math.Min(rate, 99), 1);
		
		return Rnd.get(100) < rate;
	}
	
	/**
	 * Calculate delay (in milliseconds) for skills cast
	 * @param attacker
	 * @param skill
	 * @param skillTime
	 * @return
	 */
	public static TimeSpan calcAtkSpd(Creature attacker, Skill skill, TimeSpan skillTime)
	{
		if (skill.isMagic())
		{
			return skillTime / attacker.getMAtkSpd() * 333;
		}
		
		return skillTime / attacker.getPAtkSpd() * 300;
	}
	
	public static double calcAtkSpdMultiplier(Creature creature)
	{
		double armorBonus = 1; // EquipedArmorSpeedByCrystal TODO: Implement me!
		double dexBonus = BaseStat.DEX.calcBonus(creature);
		double weaponAttackSpeed = calcWeaponBaseValue(creature, Stat.PHYSICAL_ATTACK_SPEED) / armorBonus; // unk868
		double attackSpeedPerBonus = creature.getStat().getMul(Stat.PHYSICAL_ATTACK_SPEED);
		double attackSpeedDiffBonus = creature.getStat().getAdd(Stat.PHYSICAL_ATTACK_SPEED);
		return (dexBonus * (weaponAttackSpeed / 333) * attackSpeedPerBonus) + (attackSpeedDiffBonus / 333);
	}

	public static double calcWeaponBaseValue(Creature creature, Stat stat)
	{
		double baseTemplateValue = creature.getTemplate().getBaseValue(stat, 0);
		double baseValue = creature.getTransformation()?.getStats(creature, stat, baseTemplateValue) ??
		                   baseTemplateValue;
		if (creature.isPet())
		{
			Pet pet = (Pet)creature;
			Item weapon = pet.getActiveWeaponInstance();
			double baseVal = stat == Stat.PHYSICAL_ATTACK ? pet.getPetLevelData().getPetPAtk() :
				stat == Stat.MAGIC_ATTACK ? pet.getPetLevelData().getPetMAtk() : baseTemplateValue;
			baseValue = baseVal + (weapon != null ? weapon.getTemplate().getStats(stat, baseVal) : 0);
		}
		else if (creature.isPlayer() && (!creature.isTransformed() ||
		                                 (creature.getTransformation()?.getType() == TransformType.COMBAT) ||
		                                 (creature.getTransformation()?.getType() == TransformType.MODE_CHANGE)))
		{
			Item weapon = creature.getActiveWeaponInstance();
			baseValue = (weapon != null ? weapon.getTemplate().getStats(stat, baseTemplateValue) : baseTemplateValue);
		}

		return baseValue;
	}
	
	public static double calcMAtkSpdMultiplier(Creature creature)
	{
		double armorBonus = 1; // TODO: Implement me!
		double witBonus = BaseStat.WIT.calcBonus(creature);
		double castingSpeedPerBonus = creature.getStat().getMul(Stat.MAGIC_ATTACK_SPEED);
		double castingSpeedDiffBonus = creature.getStat().getAdd(Stat.MAGIC_ATTACK_SPEED);
		return ((1 / armorBonus) * witBonus * castingSpeedPerBonus) + (castingSpeedDiffBonus / 333);
	}
	
	/**
	 * @param creature
	 * @param skill
	 * @return factor divisor for skill hit time and cancel time.
	 */
	public static double calcSkillTimeFactor(Creature creature, Skill skill)
	{
		if (skill.getOperateType().isChanneling() || (skill.getMagicType() == 2) || (skill.getMagicType() == 4) || (skill.getMagicType() == 21))
		{
			return 1.0d;
		}
		
		double factor = 0.0;
		if (skill.getMagicType() == 1)
		{
			double spiritshotHitTime = (creature.isChargedShot(ShotType.SPIRITSHOTS) || creature.isChargedShot(ShotType.BLESSED_SPIRITSHOTS)) ? 0.4 : 0; // TODO: Implement propper values
			factor = creature.getStat().getMAttackSpeedMultiplier() + (creature.getStat().getMAttackSpeedMultiplier() * spiritshotHitTime); // matkspdmul + (matkspdmul * spiritshot_hit_time)
		}
		else
		{
			factor = creature.getAttackSpeedMultiplier();
		}
		
		if (creature.isNpc())
		{
			double npcFactor = ((Npc) creature).getTemplate().getHitTimeFactorSkill();
			if (npcFactor > 0)
			{
				factor /= npcFactor;
			}
		}
		return Math.Max(0.01, factor);
	}
	
	public static TimeSpan calcSkillCancelTime(Creature creature, Skill skill)
	{
		return Algorithms.Max(skill.getHitCancelTime() / calcSkillTimeFactor(creature, skill), TimeSpan.FromMilliseconds(SKILL_LAUNCH_TIME));
	}
	
	/**
	 * Formula based on http://l2p.l2wh.com/nonskillattacks.html
	 * @param attacker
	 * @param target
	 * @return {@code true} if hit missed (target evaded), {@code false} otherwise.
	 */
	public static bool calcHitMiss(Creature attacker, Creature target)
	{
		double chance = (80 + (2 * (attacker.getAccuracy() - target.getEvasionRate()))) * 10;
		
		// Get additional bonus from the conditions when you are attacking
		chance *= HitConditionBonusData.getInstance().getConditionBonus(attacker, target);
		
		chance = Math.Max(chance, 200);
		chance = Math.Min(chance, 980);
		
		return chance < Rnd.get(1000);
	}
	
	/**
	 * Returns:<br>
	 * 0 = shield defense doesn't succeed<br>
	 * 1 = shield defense succeed<br>
	 * 2 = perfect block
	 * @param attacker
	 * @param target
	 * @param sendSysMsg
	 * @return
	 */
	public static byte calcShldUse(Creature attacker, Creature target, bool sendSysMsg)
	{
		ItemTemplate item = target.getSecondaryWeaponItem();
		if (!(item is Armor) || (((Armor) item).getItemType() == ArmorType.SIGIL))
		{
			return 0;
		}
		
		double shldRate = target.getStat().getValue(Stat.SHIELD_DEFENCE_RATE) * BaseStat.CON.calcBonus(target);
		
		// if attacker use bow and target wear shield, shield block rate is multiplied by 1.3 (30%)
		if (attacker.getAttackType().isRanged())
		{
			shldRate *= 1.3;
		}
		
		int degreeside = target.isAffected(EffectFlag.PHYSICAL_SHIELD_ANGLE_ALL) ? 360 : 120;
		if ((degreeside < 360) && (Math.Abs(target.calculateDirectionTo(attacker) - Util.convertHeadingToDegree(target.getHeading())) > (degreeside / 2)))
		{
			return 0;
		}
		
		byte shldSuccess = SHIELD_DEFENSE_FAILED;
		
		// Check shield success
		if (shldRate > Rnd.get(100))
		{
			// If shield succeed, check perfect block.
			if (((100 - (2 * BaseStat.CON.calcBonus(target))) < Rnd.get(100)))
			{
				shldSuccess = SHIELD_DEFENSE_PERFECT_BLOCK;
			}
			else
			{
				shldSuccess = SHIELD_DEFENSE_SUCCEED;
			}
		}
		
		if (sendSysMsg && target.isPlayer())
		{
			Player enemy = target.getActingPlayer();
			
			switch (shldSuccess)
			{
				case SHIELD_DEFENSE_SUCCEED:
				{
					enemy.sendPacket(SystemMessageId.YOU_VE_BLOCKED_THE_ATTACK);
					break;
				}
				case SHIELD_DEFENSE_PERFECT_BLOCK:
				{
					enemy.sendPacket(SystemMessageId.YOUR_EXCELLENT_SHIELD_DEFENSE_WAS_A_SUCCESS);
					break;
				}
			}
		}
		
		return shldSuccess;
	}
	
	public static byte calcShldUse(Creature attacker, Creature target)
	{
		return calcShldUse(attacker, target, true);
	}
	
	public static bool calcMagicAffected(Creature actor, Creature target, Skill skill)
	{
		double defence = 0;
		if (skill.isActive() && skill.isBad())
		{
			defence = target.getMDef();
		}
		
		if (skill.isDebuff())
		{
			if (target.getAbnormalShieldBlocks() > 0)
			{
				target.decrementAbnormalShieldBlocks();
				return false;
			}
			else if (target.isAffected(EffectFlag.DEBUFF_BLOCK))
			{
				return false;
			}
		}
		
		double attack = 2 * actor.getMAtk() * calcGeneralTraitBonus(actor, target, skill.getTraitType(), false);
		double d = (attack - defence) / (attack + defence);
		d += 0.5 * Rnd.nextDouble(); // TODO nextGaussian
		return d > 0;
	}
	
	public static double calcLvlBonusMod(Creature attacker, Creature target, Skill skill)
	{
		int attackerLvl = skill.getMagicLevel() > 0 ? skill.getMagicLevel() : attacker.getLevel();
		double skillLevelBonusRateMod = 1 + (skill.getLvlBonusRate() / 100.0);
		double lvlMod = 1 + ((attackerLvl - target.getLevel()) / 100.0);
		return skillLevelBonusRateMod * lvlMod;
	}
	
	/**
	 * Calculates the effect landing success.
	 * @param attacker the attacker
	 * @param target the target
	 * @param skill the skill
	 * @return {@code true} if the effect lands
	 */
	public static bool calcEffectSuccess(Creature attacker, Creature target, Skill skill)
	{
		// StaticObjects can not receive continuous effects.
		if (target.isDoor() || (target is SiegeFlag) || (target is StaticObject))
		{
			return false;
		}
		
		if (skill.isDebuff())
		{
			bool resisted = target.isCastingNow(s => s.getSkill().getAbnormalResists().Contains(skill.getAbnormalType()));
			if (!resisted)
			{
				if (target.getAbnormalShieldBlocks() > 0)
				{
					target.decrementAbnormalShieldBlocks();
					resisted = true;
				}
			}
			
			if (!resisted)
			{
				double sphericBarrierRange = target.getStat().getValue(Stat.SPHERIC_BARRIER_RANGE, 0);
				if (sphericBarrierRange > 0)
				{
					resisted = attacker.calculateDistance3D(target) > sphericBarrierRange;
				}
			}
			
			if (resisted)
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_HAS_RESISTED_YOUR_S2);
				sm.Params.addString(target.getName());
				sm.Params.addSkillName(skill);
				attacker.sendPacket(sm);
				attacker.sendPacket(new ExMagicAttackInfoPacket(attacker.getObjectId(), target.getObjectId(), ExMagicAttackInfoPacket.RESISTED));
				return false;
			}
		}
		
		double? activateRate = skill.getActivateRate();
		if ((activateRate is null))
		{
			return true;
		}
		
		int magicLevel = skill.getMagicLevel();
		if (magicLevel <= -1)
		{
			magicLevel = target.getLevel() + 3;
		}
		
		double targetBasicProperty = getAbnormalResist(skill.getBasicProperty(), target);
		double baseMod = ((((((magicLevel - target.getLevel()) + 3) * skill.getLvlBonusRate()) + activateRate.Value) + 30.0) - targetBasicProperty);
		double elementMod = calcAttributeBonus(attacker, target, skill);
		double traitMod = calcGeneralTraitBonus(attacker, target, skill.getTraitType(), false);
		double basicPropertyResist = getBasicPropertyResistBonus(skill.getBasicProperty(), target);
		double buffDebuffMod = skill.isDebuff() ? target.getStat().getValue(Stat.RESIST_ABNORMAL_DEBUFF, 1) : 1;
		double rate = baseMod * elementMod * traitMod * buffDebuffMod;
		double finalRate = traitMod > 0 ? CommonUtil.constrain(rate, skill.getMinChance(), skill.getMaxChance()) * basicPropertyResist : 0;
		
		if ((finalRate <= Rnd.get(100)) && (target != attacker))
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_HAS_RESISTED_YOUR_S2);
			sm.Params.addString(target.getName());
			sm.Params.addSkillName(skill);
			attacker.sendPacket(sm);
			attacker.sendPacket(new ExMagicAttackInfoPacket(attacker.getObjectId(), target.getObjectId(), ExMagicAttackInfoPacket.RESISTED));
			return false;
		}
		return true;
	}
	
	public static bool calcMagicSuccess(Creature attacker, Creature target, Skill skill)
	{
		double lvlModifier = 1;
		double targetModifier = 1;
		int mAccModifier = 1;
		if (attacker.isAttackable() || target.isAttackable())
		{
			lvlModifier = Math.Pow(1.3,
				target.getLevel() - (Config.CALCULATE_MAGIC_SUCCESS_BY_SKILL_MAGIC_LEVEL && (skill.getMagicLevel() > 0)
					? skill.getMagicLevel()
					: attacker.getLevel()));

			if ((attacker.getActingPlayer() != null) && !target.isRaid() && !target.isRaidMinion() &&
			    (target.getLevel() >= Config.MIN_NPC_LEVEL_MAGIC_PENALTY) &&
			    ((target.getLevel() - attacker.getActingPlayer().getLevel()) >= 3))
			{
				int levelDiff = target.getLevel() - attacker.getActingPlayer().getLevel() - 2;
				if (levelDiff >= Config.NPC_SKILL_CHANCE_PENALTY.Length)
				{
					targetModifier = Config.NPC_SKILL_CHANCE_PENALTY[Config.NPC_SKILL_CHANCE_PENALTY.Length - 1];
				}
				else
				{
					targetModifier = Config.NPC_SKILL_CHANCE_PENALTY[levelDiff];
				}
			}
		}
		else
		{
			int mAccDiff = attacker.getMagicAccuracy() - target.getMagicEvasionRate();
			mAccModifier = 100;
			if (mAccDiff > -20)
			{
				mAccModifier = 2;
			}
			else if (mAccDiff > -25)
			{
				mAccModifier = 30;
			}
			else if (mAccDiff > -30)
			{
				mAccModifier = 60;
			}
			else if (mAccDiff > -35)
			{
				mAccModifier = 90;
			}
		}
		
		// general magic resist
		double resModifier = target.getStat().getMul(Stat.MAGIC_SUCCESS_RES, 1);
		int rate = (int)(100 - Math.Round(mAccModifier * lvlModifier * targetModifier * resModifier));
		
		return (Rnd.get(100) < rate);
	}
	
	public static double calcManaDam(Creature attacker, Creature target, Skill skill, double power, byte shld, bool sps, bool bss, bool mcrit, double critLimit)
	{
		// Formula: (SQR(M.Atk)*Power*(Target Max MP/97))/M.Def
		double mAtk = attacker.getMAtk();
		double mDef = target.getMDef();
		double mp = target.getMaxMp();
		
		switch (shld)
		{
			case SHIELD_DEFENSE_SUCCEED:
			{
				mDef += target.getShldDef();
				break;
			}
			case SHIELD_DEFENSE_PERFECT_BLOCK: // perfect block
			{
				return 1;
			}
		}
		
		// Bonus Spiritshot
		double shotsBonus = attacker.getStat().getValue(Stat.SHOTS_BONUS) * target.getStat().getValue(Stat.SPIRITSHOT_RESISTANCE, 1);
		double sapphireBonus = 0;
		if (attacker.isPlayer())
		{
			sapphireBonus = attacker.getActingPlayer().getActiveShappireJewel()?.getBonus() ?? 0;
		}
		mAtk *= bss ? 4 * (shotsBonus + sapphireBonus) : sps ? 2 * (shotsBonus + sapphireBonus) : 1;
		
		double damage = (Math.Sqrt(mAtk) * power * (mp / 97)) / mDef;
		damage *= calcGeneralTraitBonus(attacker, target, skill.getTraitType(), false);
		damage *= calculatePvpPveBonus(attacker, target, skill, mcrit);
		
		// Failure calculation
		if (Config.ALT_GAME_MAGICFAILURES && !calcMagicSuccess(attacker, target, skill))
		{
			if (attacker.isPlayer())
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_HAS_RESISTED_C2_S_MAGIC_DAMAGE_IS_DECREASED);
				sm.Params.addString(target.getName());
				sm.Params.addString(attacker.getName());
				attacker.sendPacket(sm);
				damage /= 2;
			}
			
			if (target.isPlayer())
			{
				SystemMessagePacket sm2 = new SystemMessagePacket(SystemMessageId.C1_WEAKLY_RESISTED_C2_S_MAGIC);
				sm2.Params.addString(target.getName());
				sm2.Params.addString(attacker.getName());
				target.sendPacket(sm2);
			}
		}
		
		if (mcrit)
		{
			damage *= 3;
			damage = Math.Min(damage, critLimit);
			attacker.sendPacket(SystemMessageId.M_CRITICAL);
		}
		return damage;
	}
	
	public static double calculateSkillResurrectRestorePercent(double baseRestorePercent, Creature caster)
	{
		if ((baseRestorePercent == 0) || (baseRestorePercent == 100))
		{
			return baseRestorePercent;
		}
		
		double restorePercent = baseRestorePercent * BaseStat.WIT.calcBonus(caster);
		if ((restorePercent - baseRestorePercent) > 20.0)
		{
			restorePercent += 20.0;
		}
		
		restorePercent = Math.Max(restorePercent, baseRestorePercent);
		restorePercent = Math.Min(restorePercent, 90.0);
		
		return restorePercent;
	}
	
	public static bool calcSkillEvasion(Creature creature, Creature target, Skill skill)
	{
		if (Rnd.get(100) < target.getStat().getSkillEvasionTypeValue(skill.getMagicType()))
		{
			if (creature.isPlayer())
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_DODGED_THE_ATTACK);
				sm.Params.addString(target.getName());
				creature.getActingPlayer().sendPacket(sm);
			}
			if (target.isPlayer())
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_DODGED_C1_S_ATTACK);
				sm.Params.addString(creature.getName());
				target.getActingPlayer().sendPacket(sm);
			}
			return true;
		}
		return false;
	}
	
	public static bool calcSkillMastery(Creature actor, Skill skill)
	{
		// Non players are not affected by Skill Mastery.
		if (!actor.isPlayer())
		{
			return false;
		}
		
		int val = (int) actor.getStat().getAdd(Stat.SKILL_MASTERY, -1);
		if (val == -1)
		{
			return false;
		}

		ImmutableArray<BaseStat> baseStats = EnumUtil.GetValues<BaseStat>();
		double chance = baseStats[val].calcBonus(actor) * actor.getStat().getMul(Stat.SKILL_MASTERY_RATE, 1);

		return ((Rnd.nextDouble() * 100.0) < (chance *
		                                      CollectionExtensions.GetValueOrDefault(Config.SKILL_MASTERY_CHANCE_MULTIPLIERS, actor.getActingPlayer().getClassId(), 1)));
	}
	
	/**
	 * Calculates the attribute bonus with the following formula:<br>
	 * diff > 0, so AttBonus = 1,025 + Sqrt[(diff^3) / 2] * 0,0001, cannot be above 1,25!<br>
	 * diff < 0, so AttBonus = 0,975 - Sqrt[(diff^3) / 2] * 0,0001, cannot be below 0,75!<br>
	 * diff == 0, so AttBonus = 1<br>
	 * It has been tested that physical skills do get affected by attack attribute even<br>
	 * if they don't have any attribute. In that case only the biggest attack attribute is taken.
	 * @param attacker
	 * @param target
	 * @param skill Can be {@code null} if there is no skill used for the attack.
	 * @return The attribute bonus
	 */
	public static double calcAttributeBonus(Creature attacker, Creature target, Skill skill)
	{
		int attackAttribute;
		int defenceAttribute;
		
		if ((skill != null) && (skill.getAttributeType() != AttributeType.NONE))
		{
			attackAttribute = attacker.getAttackElementValue(skill.getAttributeType()) + skill.getAttributeValue();
			defenceAttribute = target.getDefenseElementValue(skill.getAttributeType());
		}
		else
		{
			attackAttribute = attacker.getAttackElementValue(attacker.getAttackElement());
			defenceAttribute = target.getDefenseElementValue(attacker.getAttackElement());
		}
		
		int diff = attackAttribute - defenceAttribute;
		if (diff > 0)
		{
			return Math.Min(1.025 + (Math.Sqrt(Math.Pow(diff, 3) / 2) * 0.0001), 1.25);
		}
		else if (diff < 0)
		{
			return Math.Max(0.975 - (Math.Sqrt(Math.Pow(-diff, 3) / 2) * 0.0001), 0.75);
		}
		
		return 1;
	}
	
	public static void calcCounterAttack(Creature attacker, Creature target, Skill skill, bool crit)
	{
		// Only melee skills can be reflected
		if (skill.isMagic() || (skill.getCastRange() > MELEE_ATTACK_RANGE))
		{
			return;
		}
		
		double chance = target.getStat().getValue(Stat.VENGEANCE_SKILL_PHYSICAL_DAMAGE, 0);
		if (Rnd.get(100) < chance)
		{
			if (target.isPlayer())
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_COUNTERED_C1_S_ATTACK);
				sm.Params.addString(attacker.getName());
				target.sendPacket(sm);
			}
			if (attacker.isPlayer())
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_IS_PERFORMING_A_COUNTERATTACK);
				sm.Params.addString(target.getName());
				attacker.sendPacket(sm);
			}
			
			double counterdmg = ((target.getPAtk() * 873) / attacker.getPDef()); // Old: (((target.getPAtk(attacker) * 10.0) * 70.0) / attacker.getPDef(target));
			counterdmg *= calcWeaponTraitBonus(attacker, target);
			counterdmg *= calcGeneralTraitBonus(attacker, target, skill.getTraitType(), true);
			counterdmg *= calcAttributeBonus(attacker, target, skill);
			
			attacker.reduceCurrentHp(counterdmg, target, skill);
		}
	}
	
	/**
	 * Calculate buff/debuff reflection.
	 * @param target
	 * @param skill
	 * @return {@code true} if reflect, {@code false} otherwise.
	 */
	public static bool calcBuffDebuffReflection(Creature target, Skill skill)
	{
		if (!skill.isDebuff() || (skill.getActivateRate() == -1))
		{
			return false;
		}

		return target.getStat().getValue(skill.isMagic() ? Stat.REFLECT_SKILL_MAGIC : Stat.REFLECT_SKILL_PHYSIC, 0) >
		       Rnd.get(100);
	}
	
	/**
	 * Calculate damage caused by falling
	 * @param creature
	 * @param fallHeight
	 * @return damage
	 */
	public static double calcFallDam(Creature creature, int fallHeight)
	{
		if (!Config.ENABLE_FALLING_DAMAGE || (fallHeight < 0))
		{
			return 0;
		}
		return creature.getStat().getValue(Stat.FALL, (fallHeight * creature.getMaxHp()) / 1000.0);
	}
	
	/**
	 * Basic chance formula:<br>
	 * <ul>
	 * <li>chance = weapon_critical * dex_bonus * crit_height_bonus * crit_pos_bonus * effect_bonus * fatal_blow_rate</li>
	 * <li>weapon_critical = (12 for daggers)</li>
	 * <li>dex_bonus = dex modifier bonus for current dex (Seems unused in GOD, so its not used in formula).</li>
	 * <li>crit_height_bonus = (z_diff * 4 / 5 + 10) / 100 + 1 or alternatively (z_diff * 0.008) + 1.1. Be aware of z_diff constraint of -25 to 25.</li>
	 * <li>crit_pos_bonus = crit_pos(front = 1, side = 1.1, back = 1.3) * p_critical_rate_position_bonus</li>
	 * <li>effect_bonus = (p2 + 100) / 100, p2 - 2nd param of effect. Blow chance of effect.</li>
	 * </ul>
	 * Chance cannot be higher than 80%.
	 * @param creature
	 * @param target
	 * @param skill
	 * @param chanceBoost
	 * @return
	 */
	public static bool calcBlowSuccess(Creature creature, Creature target, Skill skill, double chanceBoost)
	{
		Weapon weapon = creature.getActiveWeaponItem();
		double weaponCritical = weapon != null
			? weapon.getStats(Stat.CRITICAL_RATE, creature.getTemplate().getBaseCritRate())
			: creature.getTemplate().getBaseCritRate();
		
		// double dexBonus = BaseStats.DEX.calcBonus(activeChar); Not used in GOD
		double critHeightBonus = calcCriticalHeightBonus(creature, target);
		double criticalPosition = calcCriticalPositionBonus(creature, target); // 30% chance from back, 10% chance from side. Include buffs that give positional crit rate.
		double chanceBoostMod = (100 + chanceBoost) / 100;
		double blowRateMod = creature.getStat().getValue(Stat.BLOW_RATE, 1);
		double blowRateDefenseMod = target.getStat().getValue(Stat.BLOW_RATE_DEFENCE, 1);
		
		double rate = criticalPosition * critHeightBonus * weaponCritical * chanceBoostMod * blowRateMod * blowRateDefenseMod;
		
		// Blow rate change is limited (%).
		return Rnd.get(100) < Math.Min(rate, Config.BLOW_RATE_CHANCE_LIMIT);
	}
	
	public static List<BuffInfo> calcCancelStealEffects(Creature creature, Creature target, Skill skill, DispelSlotType slot, int rate, int Max)
	{
		List<BuffInfo> canceled = new(Max);
		switch (slot)
		{
			case DispelSlotType.BUFF:
			{
				// Resist Modifier.
				int cancelMagicLvl = skill.getMagicLevel();
				
				// Prevent initialization.
				List<BuffInfo> dances = target.getEffectList().getDances();
				for (int i = dances.size() - 1; i >= 0; i--) // reverse order
				{
					BuffInfo info = dances.get(i);
					if (!info.getSkill().canBeStolen() || ((rate < 100) && !calcCancelSuccess(info, cancelMagicLvl, rate, skill, target)))
					{
						continue;
					}
					canceled.add(info);
					if (canceled.size() >= Max)
					{
						break;
					}
				}
				
				if (canceled.size() < Max)
				{
					// Prevent initialization.
					List<BuffInfo> buffs = target.getEffectList().getBuffs();
					for (int i = buffs.size() - 1; i >= 0; i--) // reverse order
					{
						BuffInfo info = buffs.get(i);
						if (!info.getSkill().canBeStolen() || ((rate < 100) && !calcCancelSuccess(info, cancelMagicLvl, rate, skill, target)))
						{
							continue;
						}
						canceled.add(info);
						if (canceled.size() >= Max)
						{
							break;
						}
					}
				}
				break;
			}
			case DispelSlotType.DEBUFF:
			{
				List<BuffInfo> debuffs = target.getEffectList().getDebuffs();
				for (int i = debuffs.size() - 1; i >= 0; i--)
				{
					BuffInfo info = debuffs.get(i);
					if (info.getSkill().canBeDispelled() && (Rnd.get(100) <= rate))
					{
						canceled.add(info);
						if (canceled.size() >= Max)
						{
							break;
						}
					}
				}
				break;
			}
		}
		return canceled;
	}

	public static bool calcCancelSuccess(BuffInfo info, int cancelMagicLvl, int rate, Skill skill, Creature target)
	{
		int abnormalTimeSeconds = (int)(info.getAbnormalTime() ?? TimeSpan.Zero).TotalSeconds;
		int chance = (int)(rate + ((cancelMagicLvl - info.getSkill().getMagicLevel()) * 2) +
		                   ((abnormalTimeSeconds / 120) * target.getStat().getValue(Stat.RESIST_DISPEL_BUFF, 1)));

		return
			Rnd.get(100) <
			CommonUtil.constrain(chance, 25, 75); // TODO: i_dispel_by_slot_probability Min = 40, Max = 95.
	}

	/**
	 * Calculates the abnormal time for an effect.<br>
	 * The abnormal time is taken from the skill definition, and it's global for all effects present in the skills.
	 * @param caster the caster
	 * @param target the target
	 * @param skill the skill
	 * @return the time that the effect will last
	 */
	public static TimeSpan? calcEffectAbnormalTime(Creature caster, Creature target, Skill skill)
	{
		TimeSpan? time = (skill == null) || skill.isPassive() || skill.isToggle() ? null : skill.getAbnormalTime();
		
		// If the skill is a mastery skill, the effect will last twice the default time.
		if ((skill != null) && !skill.isStatic() && calcSkillMastery(caster, skill))
		{
			time *= 2;
		}
		
		return time;
	}
	
	/**
	 * Calculate Probability in following effects:<br>
	 * TargetCancel,<br>
	 * TargetMeProbability,<br>
	 * SkillTurning,<br>
	 * Betray,<br>
	 * Bluff,<br>
	 * DeleteHate,<br>
	 * RandomizeHate,<br>
	 * DeleteHateOfMe,<br>
	 * TransferHate,<br>
	 * Confuse<br>
	 * Compelling,<br>
	 * Knockback<br>
	 * Pull
	 * @param baseChance chance from effect parameter
	 * @param attacker
	 * @param target
	 * @param skill
	 * @return chance for effect to succeed
	 */
	public static bool calcProbability(double baseChance, Creature attacker, Creature target, Skill skill)
	{
		// Skills without set probability should only test against trait invulnerability.
		if (double.IsNaN(baseChance))
		{
			return calcGeneralTraitBonus(attacker, target, skill.getTraitType(), true) > 0;
		}
		
		// Outdated formula: return Rnd.get(100) < ((((((skill.getMagicLevel() + baseChance) - target.getLevel()) + 30) - target.getINT()) * calcAttributeBonus(attacker, target, skill)) * calcGeneralTraitBonus(attacker, target, skill.getTraitType(), false));
		// TODO: Find more retail-like formula
		return Rnd.get(100) <
		       (((((skill.getMagicLevel() + baseChance) - target.getLevel()) -
		          getAbnormalResist(skill.getBasicProperty(), target)) * calcAttributeBonus(attacker, target, skill)) *
		        calcGeneralTraitBonus(attacker, target, skill.getTraitType(), false));
	}
	
	/**
	 * Calculates karma lost upon death.
	 * @param player
	 * @param finalExp
	 * @return the amount of karma player has loosed.
	 */
	public static int calculateKarmaLost(Player player, double finalExp)
	{
		double karmaLooseMul = KarmaData.getInstance().getMultiplier(player.getLevel());
		if (finalExp > 0) // Received exp
		{
			return (int) ((Math.Abs(finalExp / Config.RATE_KARMA_LOST) / karmaLooseMul) / 30);
		}
		return (int) ((Math.Abs(finalExp) / karmaLooseMul) / 30);
	}
	
	/**
	 * Calculates karma gain upon playable kill.</br>
	 * Updated to High Five on 10.09.2014 by Zealar tested in retail.
	 * @param pkCount
	 * @param isSummon
	 * @return karma points that will be added to the player.
	 */
	public static int calculateKarmaGain(int pkCount, bool isSummon)
	{
		int result = 43200;
		
		if (isSummon)
		{
			result = (int) ((((pkCount * 0.375) + 1) * 60) * 4) - 150;
			
			if (result > 10800)
			{
				return 10800;
			}
		}
		
		if (pkCount < 99)
		{
			result = (int) ((((pkCount * 0.5) + 1) * 60) * 12);
		}
		else if (pkCount < 180)
		{
			result = (int) ((((pkCount * 0.125) + 37.75) * 60) * 12);
		}
		
		return result;
	}
	
	public static double calcGeneralTraitBonus(Creature attacker, Creature target, TraitType traitType, bool ignoreResistance)
	{
		if (traitType == TraitType.NONE)
		{
			return 1.0;
		}
		
		if (target.getStat().isInvulnerableTrait(traitType))
		{
			return 0;
		}
		
		switch (traitType.getType())
		{
			case 2:
			{
				if (!attacker.getStat().hasAttackTrait(traitType) || !target.getStat().hasDefenceTrait(traitType))
				{
					return 1.0;
				}
				break;
			}
			case 3:
			{
				if (ignoreResistance)
				{
					return 1.0;
				}
				break;
			}
			default:
			{
				return 1.0;
			}
		}
		
		return Math.Max(attacker.getStat().getAttackTrait(traitType) - target.getStat().getDefenceTrait(traitType), 0.05);
	}
	
	public static double calcWeaknessBonus(Creature attacker, Creature target, TraitType traitType)
	{
		double result = 1;
		foreach (TraitType trait in TraitTypeUtil.getAllWeakness())
		{
			if ((traitType != trait) && target.getStat().hasDefenceTrait(trait) &&
			    attacker.getStat().hasAttackTrait(trait) && !target.getStat().isInvulnerableTrait(traitType))
			{
				result *= Math.Max(attacker.getStat().getAttackTrait(trait) - target.getStat().getDefenceTrait(trait),
					0.05);
			}
		}

		return result;
	}
	
	public static double calcWeaponTraitBonus(Creature attacker, Creature target)
	{
		return Math.Max(0.22, 1.0 - target.getStat().getDefenceTrait(attacker.getAttackType().GetTraitType()));
	}
	
	public static double calcAttackTraitBonus(Creature attacker, Creature target)
	{
		double weaponTraitBonus = calcWeaponTraitBonus(attacker, target);
		if (weaponTraitBonus == 0)
		{
			return 0;
		}
		
		double weaknessBonus = 1.0;
		foreach (TraitType traitType in EnumUtil.GetValues<TraitType>())
		{
			if (traitType.getType() == 2)
			{
				weaknessBonus *= calcGeneralTraitBonus(attacker, target, traitType, true);
				if (weaknessBonus == 0)
				{
					return 0;
				}
			}
		}
		
		return Math.Max(weaponTraitBonus * weaknessBonus, 0.05);
	}
	
	public static double getBasicPropertyResistBonus(BasicProperty basicProperty, Creature target)
	{
		if ((basicProperty == BasicProperty.NONE) || !target.hasBasicPropertyResist())
		{
			return 1.0;
		}
		
		BasicPropertyResist resist = target.getBasicPropertyResist(basicProperty);
		switch (resist.getResistLevel())
		{
			case 0:
			{
				return 1.0;
			}
			case 1:
			{
				return 0.6;
			}
			case 2:
			{
				return 0.3;
			}
			default:
			{
				return 0;
			}
		}
	}
	
	/**
	 * Calculated damage caused by ATTACK of attacker on target.
	 * @param attacker player or NPC that makes ATTACK
	 * @param target player or NPC, target of ATTACK
	 * @param shld
	 * @param crit if the ATTACK have critical success
	 * @param ss if weapon item was charged by soulshot
	 * @param ssBlessed if shot was blessed
	 * @return
	 */
	public static double calcAutoAttackDamage(Creature attacker, Creature target, byte shld, bool crit, bool ss, bool ssBlessed)
	{
		// DEFENCE CALCULATION (pDef + sDef)
		double defence = target.getPDef();
		
		switch (shld)
		{
			case SHIELD_DEFENSE_SUCCEED:
			{
				defence += target.getShldDef();
				break;
			}
			case SHIELD_DEFENSE_PERFECT_BLOCK:
			{
				return 1;
			}
		}
		
		Weapon weapon = attacker.getActiveWeaponItem();
		bool isRanged = (weapon != null) && weapon.getItemType().isRanged();
		double shotsBonus = attacker.getStat().getValue(Stat.SHOTS_BONUS) * target.getStat().getValue(Stat.SOULSHOT_RESISTANCE, 1);
		
		double cAtk = crit ? calcCritDamage(attacker, target, null) : 1;
		double cAtkAdd = crit ? calcCritDamageAdd(attacker, target, null) : 0;
		double critMod = crit ? (isRanged ? 0.5 : 1) : 0;
		double ssBonus = ss ? (ssBlessed ? 2.15 : 2) * shotsBonus : 1;
		double randomDamage = attacker.getRandomDamageMultiplier();
		double proxBonus = (((ILocational)attacker).isInFrontOf(target) ? 0 : (((ILocational)attacker).isBehind(target) ? 0.2 : 0.05)) * attacker.getPAtk();
		double attack = (attacker.getPAtk() * randomDamage) + proxBonus;
		
		// ....................______________Critical Section___________________...._______Non-Critical Section______
		// ATTACK CALCULATION (((pAtk * cAtk * ss + cAtkAdd) * crit) * weaponMod) + (pAtk (1 - crit) * ss * weaponMod)
		// ````````````````````^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^````^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
		attack = ((((attack * cAtk * ssBonus) + cAtkAdd) * critMod) * (isRanged ? 154 : 77)) + (attack * (1 - critMod) * ssBonus * (isRanged ? 154 : 77));
		
		// DAMAGE CALCULATION (ATTACK / DEFENCE) * trait bonus * attr bonus * pvp bonus * pve bonus
		double damage = attack / defence;
		damage *= calcAttackTraitBonus(attacker, target);
		damage *= calcAttributeBonus(attacker, target, null);
		damage *= calculatePvpPveBonus(attacker, target, null, crit);
		damage *= attacker.getStat().getMul(Stat.AUTO_ATTACK_DAMAGE_BONUS);
		
		return Math.Max(0, damage);
	}
	
	public static double getAbnormalResist(BasicProperty basicProperty, Creature target)
	{
		switch (basicProperty)
		{
			case BasicProperty.PHYSICAL:
			{
				return target.getStat().getValue(Stat.ABNORMAL_RESIST_PHYSICAL);
			}
			case BasicProperty.MAGIC:
			{
				return target.getStat().getValue(Stat.ABNORMAL_RESIST_MAGICAL);
			}
			default:
			{
				return 0;
			}
		}
	}
	
	/**
	 * Calculates if the specified creature can get its stun effect removed due to damage taken.
	 * @param creature the creature to be checked
	 * @return {@code true} if character should get its stun effects removed, {@code false} otherwise.
	 */
	public static bool calcStunBreak(Creature creature)
	{
		// Check if target is stunned and break it with 14% chance. (retail is 14% and 35% on crit?)
		if (Config.ALT_GAME_STUN_BREAK && creature.hasBlockActions() && (Rnd.get(14) == 0))
		{
			// Any stun that has double duration due to skill mastery, doesn't get removed until its time reaches the usual abnormal time.
			return creature.getEffectList().hasAbnormalType(AbnormalType.STUN, info => info.getTime() <= info.getSkill().getAbnormalTime());
		}
		return false;
	}
	
	public static bool calcRealTargetBreak()
	{
		// Real Target breaks at 3% (Rnd > 3.0 doesn't break) probability.
		return Rnd.get(100) <= 3;
	}
	
	/**
	 * @param attackSpeed the attack speed of the Creature.
	 * @return {@code 500000 / attackSpeed}.
	 */
	public static int calculateTimeBetweenAttacks(int attackSpeed)
	{
		// Measured Nov 2015 by Nik. Formula: atk.spd/500 = hits per second.
		return Math.Max(50, (500000 / attackSpeed));
	}
	
	/**
	 * @param totalAttackTime the time needed to make a full attack.
	 * @param attackType the weapon type used for attack.
	 * @param twoHanded if the weapon is two handed.
	 * @param secondHit calculates the second hit for dual attacks.
	 * @return the time required from the start of the attack until you hit the target.
	 */
	public static int calculateTimeToHit(int totalAttackTime, WeaponType attackType, bool twoHanded, bool secondHit)
	{
		// Gracia Final Retail confirmed:
		// Time to damage (1 hand, 1 hit): TotalBasicAttackTime * 0.644
		// Time to damage (2 hand, 1 hit): TotalBasicAttackTime * 0.735
		// Time to damage (2 hand, 2 hit): TotalBasicAttackTime * 0.2726 and TotalBasicAttackTime * 0.6
		// Time to damage (bow/xbow): TotalBasicAttackTime * 0.978
		
		// Measured July 2016 by Nik.
		// Due to retail packet delay, we are unable to gather too accurate results. Therefore the below formulas are based on original Gracia Final values.
		// Any original values that appear higher than tested have been replaced with the tested values, because even with packet delay its obvious they are wrong.
		// All other original values are compared with the test results and differences are considered to be too insignificant and mostly caused due to packet delay.
		switch (attackType)
		{
			case WeaponType.BOW:
			case WeaponType.CROSSBOW:
			case WeaponType.TWOHANDCROSSBOW:
			{
				return (int) (totalAttackTime * 0.95);
			}
			case WeaponType.DUALBLUNT:
			case WeaponType.DUALDAGGER:
			case WeaponType.DUAL:
			case WeaponType.DUALFIST:
			{
				if (secondHit)
				{
					return (int) (totalAttackTime * 0.6);
				}
				
				return (int) (totalAttackTime * 0.2726);
			}
			default:
			{
				if (twoHanded)
				{
					return (int) (totalAttackTime * 0.735);
				}
				
				return (int) (totalAttackTime * 0.644);
			}
		}
	}
	
	/**
	 * @param creature
	 * @param weapon
	 * @return {@code 900000 / PAttackSpeed}
	 */
	public static TimeSpan calculateReuseTime(Creature creature, Weapon weapon)
	{
		if (weapon == null)
		{
			return TimeSpan.Zero;
		}
		
		WeaponType defaultAttackType = weapon.getItemType().AsWeaponType();
		Transform? transform = creature.getTransformation();
		WeaponType weaponType = transform != null ? transform.getBaseAttackType(creature, defaultAttackType) : defaultAttackType;
		
		// Only ranged weapons should continue for now.
		if (!weaponType.isRanged())
		{
			return TimeSpan.Zero;
		}
		
		return TimeSpan.FromMilliseconds(900000) / creature.getStat().getPAtkSpd();
	}
	
	public static double calculatePvpPveBonus(Creature attacker, Creature target, Skill skill, bool crit)
	{
		Player attackerPlayer = attacker.getActingPlayer();
		Player targetPlayer = attacker.getActingPlayer();
		
		// PvP bonus
		if (attacker.isPlayable() && target.isPlayable())
		{
			double pvpAttack;
			double pvpDefense;
			if (skill != null)
			{
				if (skill.isMagic())
				{
					// Magical Skill PvP
					pvpAttack = attacker.getStat().getMul(Stat.PVP_MAGICAL_SKILL_DAMAGE, 1) *
					            CollectionExtensions.GetValueOrDefault(Config.PVP_MAGICAL_SKILL_DAMAGE_MULTIPLIERS, attackerPlayer.getClassId(), 1);
					pvpDefense = target.getStat().getMul(Stat.PVP_MAGICAL_SKILL_DEFENCE, 1) *
					             CollectionExtensions.GetValueOrDefault(Config.PVP_MAGICAL_SKILL_DEFENCE_MULTIPLIERS, targetPlayer.getClassId(), 1);
				}
				else
				{
					// Physical Skill PvP
					pvpAttack = attacker.getStat().getMul(Stat.PVP_PHYSICAL_SKILL_DAMAGE, 1) *
					            CollectionExtensions.GetValueOrDefault(Config.PVP_PHYSICAL_SKILL_DAMAGE_MULTIPLIERS, attackerPlayer.getClassId(), 1);
					pvpDefense = target.getStat().getMul(Stat.PVP_PHYSICAL_SKILL_DEFENCE, 1) *
					             CollectionExtensions.GetValueOrDefault(Config.PVP_PHYSICAL_SKILL_DEFENCE_MULTIPLIERS, targetPlayer.getClassId(), 1);
				}
			}
			else
			{
				// Autoattack PvP
				pvpAttack = attacker.getStat().getMul(Stat.PVP_PHYSICAL_ATTACK_DAMAGE, 1) *
				            CollectionExtensions.GetValueOrDefault(Config.PVP_PHYSICAL_ATTACK_DAMAGE_MULTIPLIERS, attackerPlayer.getClassId(),
					            1);
				pvpDefense = target.getStat().getMul(Stat.PVP_PHYSICAL_ATTACK_DEFENCE, 1) *
				             CollectionExtensions.GetValueOrDefault(Config.PVP_PHYSICAL_ATTACK_DEFENCE_MULTIPLIERS, targetPlayer.getClassId(),
					             1);
			}

			return Math.Max(0.05, 1 + (pvpAttack - pvpDefense)); // Bonus should not be negative.
		}

		// PvE Bonus
		if (target.isAttackable() || attacker.isAttackable())
		{
			double pveAttack;
			double pveDefense;
			double pveRaidAttack;
			double pveRaidDefense;
			
			double pvePenalty = 1;
			if (!target.isRaid() && !target.isRaidMinion() && (target.getLevel() >= Config.MIN_NPC_LEVEL_DMG_PENALTY) && (attackerPlayer != null) && ((target.getLevel() - attackerPlayer.getLevel()) >= 2))
			{
				int levelDiff = target.getLevel() - attackerPlayer.getLevel() - 1;
				if (levelDiff >= Config.NPC_SKILL_DMG_PENALTY.Length)
				{
					pvePenalty = Config.NPC_SKILL_DMG_PENALTY[Config.NPC_SKILL_DMG_PENALTY.Length - 1];
				}
				else
				{
					pvePenalty = Config.NPC_SKILL_DMG_PENALTY[levelDiff];
				}
			}

			if (skill != null)
			{
				if (skill.isMagic())
				{
					// Magical Skill PvE
					pveAttack = attacker.getStat().getMul(Stat.PVE_MAGICAL_SKILL_DAMAGE, 1) * (attackerPlayer == null
						? 1
						: CollectionExtensions.GetValueOrDefault(Config.PVE_MAGICAL_SKILL_DAMAGE_MULTIPLIERS, attackerPlayer.getClassId(), 1));
					pveDefense = target.getStat().getMul(Stat.PVE_MAGICAL_SKILL_DEFENCE, 1) * (targetPlayer == null
						? 1
						: CollectionExtensions.GetValueOrDefault(Config.PVE_MAGICAL_SKILL_DEFENCE_MULTIPLIERS, targetPlayer.getClassId(), 1));
					pveRaidAttack = attacker.isRaid()
						? attacker.getStat().getMul(Stat.PVE_RAID_MAGICAL_SKILL_DAMAGE, 1)
						: 1;
					pveRaidDefense = attacker.isRaid()
						? attacker.getStat().getMul(Stat.PVE_RAID_MAGICAL_SKILL_DEFENCE, 1)
						: 1;
				}
				else
				{
					// Physical Skill PvE
					pveAttack = attacker.getStat().getMul(Stat.PVE_PHYSICAL_SKILL_DAMAGE, 1) * (attackerPlayer == null
						? 1
						: CollectionExtensions.GetValueOrDefault(Config.PVE_PHYSICAL_SKILL_DAMAGE_MULTIPLIERS, attackerPlayer.getClassId(), 1));
					pveDefense = target.getStat().getMul(Stat.PVE_PHYSICAL_SKILL_DEFENCE, 1) * (targetPlayer == null
						? 1
						: CollectionExtensions.GetValueOrDefault(Config.PVE_PHYSICAL_SKILL_DEFENCE_MULTIPLIERS, targetPlayer.getClassId(), 1));
					pveRaidAttack = attacker.isRaid()
						? attacker.getStat().getMul(Stat.PVE_RAID_PHYSICAL_SKILL_DAMAGE, 1)
						: 1;
					pveRaidDefense = attacker.isRaid()
						? attacker.getStat().getMul(Stat.PVE_RAID_PHYSICAL_SKILL_DEFENCE, 1)
						: 1;
				}
			}
			else
			{
				// Autoattack PvE
				pveAttack = attacker.getStat().getMul(Stat.PVE_PHYSICAL_ATTACK_DAMAGE, 1) * (attackerPlayer == null
					? 1
					: CollectionExtensions.GetValueOrDefault(Config.PVE_PHYSICAL_ATTACK_DAMAGE_MULTIPLIERS, attackerPlayer.getClassId(), 1));
				pveDefense = target.getStat().getMul(Stat.PVE_PHYSICAL_ATTACK_DEFENCE, 1) * (targetPlayer == null
					? 1
					: CollectionExtensions.GetValueOrDefault(Config.PVE_PHYSICAL_ATTACK_DEFENCE_MULTIPLIERS, targetPlayer.getClassId(), 1));
				pveRaidAttack = attacker.isRaid()
					? attacker.getStat().getMul(Stat.PVE_RAID_PHYSICAL_ATTACK_DAMAGE, 1)
					: 1;
				pveRaidDefense = attacker.isRaid()
					? attacker.getStat().getMul(Stat.PVE_RAID_PHYSICAL_ATTACK_DEFENCE, 1)
					: 1;
			}

			return Math.Max(0.05, (1 + ((pveAttack * pveRaidAttack) - (pveDefense * pveRaidDefense))) * pvePenalty); // Bonus should not be negative.
		}
		
		return 1;
	}
	
	public static bool calcSpiritElementalCrit(Creature attacker, Creature target)
	{
		if (attacker.isPlayer())
		{
			Player attackerPlayer = attacker.getActingPlayer();
			ElementalType type = attackerPlayer.getActiveElementalSpiritType();
			if (ElementalType.NONE == type)
			{
				return false;
			}
			
			double critRate = attackerPlayer.getElementalSpiritCritRate();
			return Math.Min(critRate * 10, 380) > Rnd.get(1000);
		}
		
		return false;
	}
	
	public static double calcSpiritElementalDamage(Creature attacker, Creature target, double baseDamage, bool isCrit)
	{
		if (attacker.isPlayer())
		{
			Player attackerPlayer = attacker.getActingPlayer();
			ElementalType type = attackerPlayer.getActiveElementalSpiritType();
			if (ElementalType.NONE == type)
			{
				return 0;
			}
			
			double critDamage = attackerPlayer.getElementalSpiritCritDamage();
			double attack = (attackerPlayer.getActiveElementalSpiritAttack() - target.getElementalSpiritDefenseOf(type)) + Rnd.get(-2, 6);
			if (target.isPlayer())
			{
				return calcSpiritElementalPvPDamage(attack, critDamage, isCrit, baseDamage);
			}
			return calcSpiritElementalPvEDamage(type, target.getElementalSpiritType(), attack, critDamage, isCrit, baseDamage);
		}
		
		return 0;
	}
	
	private static double calcSpiritElementalPvPDamage(double attack, double critDamage, bool isCrit, double baseDamage)
	{
		double damage = Math.Min(Math.Max(0, ((attack * 1.3) + (baseDamage * 0.03 * attack)) / Math.Log(Math.Max(attack, 5))), 2295);
		if (isCrit)
		{
			damage *= 1 + ((Rnd.get(13, 20) + critDamage) / 100);
		}
		return damage;
	}
	
	private static double calcSpiritElementalPvEDamage(ElementalType attackerType, ElementalType targetType, double attack, double critDamage, bool isCrit, double baseDamage)
	{
		double damage = Math.Abs(attack * 0.8);
		double bonus;
		
		if (attackerType.isSuperior(targetType))
		{
			damage *= 1.3;
			bonus = 1.3;
		}
		else if (targetType == attackerType)
		{
			bonus = 1.1;
		}
		else
		{
			damage *= 1.1;
			bonus = 1.1;
		}
		
		if (isCrit)
		{
			damage += Math.Abs(((40 + ((9.2 + (attack * 0.048)) * critDamage)) * bonus) + Rnd.get(-10, 30));
		}
		
		return ((damage + baseDamage) * bonus) / Math.Log(20 + baseDamage + damage);
	}
}