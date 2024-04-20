using System.Collections.Immutable;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Stats.Finalizers;
using L2Dn.Model.Enums;
using NLog;
using StatInfo = L2Dn.GameServer.Model.Stats.StatInfo;

namespace L2Dn.GameServer.Model.Stats;

public record StatInfo(
	Stat Stat,
	string XmlName,
	IStatFunction Finalizer,
	Func<double, double, double> AddFunction,
	Func<double, double, double> MulFunction,
	double ResetAddValue = 0,
	double ResetMulValue = 1)
{
	public StatInfo(Stat stat, string xmlName, IStatFunction valueFinalizer)
		: this(stat, xmlName, valueFinalizer, DefaultAddFunction, DefaultMulFunction)
	{
	}

	public StatInfo(Stat stat, string xmlName)
		: this(stat, xmlName, StatDefaultFinalizer.Instance, DefaultAddFunction, DefaultMulFunction)
	{
	}
	
	public static double DefaultAddFunction(double x, double y) => x + y;
	public static double DefaultMulFunction(double x, double y) => x * y;
    
}

public static class StatUtil
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(StatUtil));
	
	private static readonly ImmutableDictionary<Stat, StatInfo> _stats =
		new StatInfo[]
		{
			new(Stat.HP_LIMIT, "hpLimit"),
			new(Stat.MAX_HP, "maxHp", new MaxHpFinalizer()),
			new(Stat.MAX_MP, "maxMp", new MaxMpFinalizer()),
			new(Stat.MAX_CP, "maxCp", new MaxCpFinalizer()),
			new(Stat.MAX_RECOVERABLE_HP,
				"maxRecoverableHp"), // The maximum HP that is able to be recovered trough heals
			new(Stat.MAX_RECOVERABLE_MP, "maxRecoverableMp"),
			new(Stat.MAX_RECOVERABLE_CP, "maxRecoverableCp"),
			new(Stat.REGENERATE_HP_RATE, "regHp", new RegenHPFinalizer()),
			new(Stat.REGENERATE_CP_RATE, "regCp", new RegenCPFinalizer()),
			new(Stat.REGENERATE_MP_RATE, "regMp", new RegenMPFinalizer()),
			new(Stat.ADDITIONAL_POTION_HP, "addPotionHp"),
			new(Stat.ADDITIONAL_POTION_MP, "addPotionMp"),
			new(Stat.ADDITIONAL_POTION_CP, "addPotionCp"),
			new(Stat.MANA_CHARGE, "manaCharge"),
			new(Stat.HEAL_EFFECT, "healEffect"),
			new(Stat.HEAL_EFFECT_ADD, "healEffectAdd"),
			new(Stat.FEED_MODIFY, "feedModify"),

			// ATTACK & DEFENCE
			new(Stat.PHYSICAL_DEFENCE, "pDef", new PDefenseFinalizer()),
			new(Stat.MAGICAL_DEFENCE, "mDef", new MDefenseFinalizer()),
			new(Stat.PHYSICAL_ATTACK, "pAtk", new PAttackFinalizer()),
			new(Stat.MAGIC_ATTACK, "mAtk", new MAttackFinalizer()),
			new(Stat.WEAPON_BONUS_PHYSICAL_ATTACK, "weaponBonusPAtk"),
			new(Stat.WEAPON_BONUS_PHYSICAL_ATTACK_MULTIPIER, "weaponBonusPAtkMultiplier"),
			new(Stat.WEAPON_BONUS_MAGIC_ATTACK, "weaponBonusMAtk"),
			new(Stat.WEAPON_BONUS_MAGIC_ATTACK_MULTIPIER, "weaponBonusMAtkMultiplier"),
			new(Stat.MAGIC_ATTACK_BY_PHYSICAL_ATTACK, "mAtkByPAtk", StatDefaultFinalizer.Instance, StatInfo.DefaultAddFunction, StatInfo.DefaultMulFunction, 0, 0),
			new(Stat.PHYSICAL_ATTACK_SPEED, "pAtkSpd", new PAttackSpeedFinalizer()),
			new(Stat.MAGIC_ATTACK_SPEED, "mAtkSpd", new MAttackSpeedFinalizer()), // Magic Skill Casting Time Rate
			new(Stat.ATK_REUSE, "atkReuse"), // Bows Hits Reuse Rate
			new(Stat.SHIELD_DEFENCE, "sDef", new ShieldDefenceFinalizer()),
			new(Stat.SHIELD_DEFENCE_IGNORE_REMOVAL, "shieldDefIgnoreRemoval"),
			new(Stat.SHIELD_DEFENCE_IGNORE_REMOVAL_ADD, "shieldDefIgnoreRemovalAdd"),
			new(Stat.CRITICAL_DAMAGE, "cAtk"),
			new(Stat.CRITICAL_DAMAGE_ADD, "cAtkAdd"), // this is another type for special critical damage mods - vicious stance, critical power and critical damage SA
			new(Stat.HATE_ATTACK, "attackHate"),
			new(Stat.REAR_DAMAGE_RATE, "rearDamage"),
			new(Stat.AUTO_ATTACK_DAMAGE_BONUS, "autoAttackDamageBonus"),
			new(Stat.IGNORE_REDUCE_DAMAGE, "ignoreReduceDamage"),

			// ELEMENTAL SPIRITS
			new(Stat.ELEMENTAL_SPIRIT_FIRE_ATTACK, "elementalSpiritFireAttack"),
			new(Stat.ELEMENTAL_SPIRIT_WATER_ATTACK, "elementalSpiritWaterAttack"),
			new(Stat.ELEMENTAL_SPIRIT_WIND_ATTACK, "elementalSpiritWindAttack"),
			new(Stat.ELEMENTAL_SPIRIT_EARTH_ATTACK, "elementalSpiritEarthAttack"),
			new(Stat.ELEMENTAL_SPIRIT_FIRE_DEFENSE, "elementalSpiritFireDefense"),
			new(Stat.ELEMENTAL_SPIRIT_WATER_DEFENSE, "elementalSpiritWaterDefense"),
			new(Stat.ELEMENTAL_SPIRIT_WIND_DEFENSE, "elementalSpiritWindDefense"),
			new(Stat.ELEMENTAL_SPIRIT_EARTH_DEFENSE, "elementalSpiritEarthDefense"),
			new(Stat.ELEMENTAL_SPIRIT_CRITICAL_RATE, "elementalSpiritCriticalRate"),
			new(Stat.ELEMENTAL_SPIRIT_CRITICAL_DAMAGE, "elementalSpiritCriticalDamage"),
			new(Stat.ELEMENTAL_SPIRIT_BONUS_EXP, "elementalSpiritExp"),

			// PVP BONUS
			new(Stat.PVP_PHYSICAL_ATTACK_DAMAGE, "pvpPhysDmg"),
			new(Stat.PVP_MAGICAL_SKILL_DAMAGE, "pvpMagicalDmg"),
			new(Stat.PVP_PHYSICAL_SKILL_DAMAGE, "pvpPhysSkillsDmg"),
			new(Stat.PVP_PHYSICAL_ATTACK_DEFENCE, "pvpPhysDef"),
			new(Stat.PVP_MAGICAL_SKILL_DEFENCE, "pvpMagicalDef"),
			new(Stat.PVP_PHYSICAL_SKILL_DEFENCE, "pvpPhysSkillsDef"),

			// PVE BONUS
			new(Stat.PVE_PHYSICAL_ATTACK_DAMAGE, "pvePhysDmg"),
			new(Stat.PVE_PHYSICAL_SKILL_DAMAGE, "pvePhysSkillDmg"),
			new(Stat.PVE_MAGICAL_SKILL_DAMAGE, "pveMagicalDmg"),
			new(Stat.PVE_PHYSICAL_ATTACK_DEFENCE, "pvePhysDef"),
			new(Stat.PVE_PHYSICAL_SKILL_DEFENCE, "pvePhysSkillDef"),
			new(Stat.PVE_MAGICAL_SKILL_DEFENCE, "pveMagicalDef"),
			new(Stat.PVE_RAID_PHYSICAL_ATTACK_DAMAGE, "pveRaidPhysDmg"),
			new(Stat.PVE_RAID_PHYSICAL_SKILL_DAMAGE, "pveRaidPhysSkillDmg"),
			new(Stat.PVE_RAID_MAGICAL_SKILL_DAMAGE, "pveRaidMagicalDmg"),
			new(Stat.PVE_RAID_PHYSICAL_ATTACK_DEFENCE, "pveRaidPhysDef"),
			new(Stat.PVE_RAID_PHYSICAL_SKILL_DEFENCE, "pveRaidPhysSkillDef"),
			new(Stat.PVE_RAID_MAGICAL_SKILL_DEFENCE, "pveRaidMagicalDef"),

			// FIXED BONUS
			new(Stat.PVP_DAMAGE_TAKEN, "pvpDamageTaken"),
			new(Stat.PVE_DAMAGE_TAKEN, "pveDamageTaken"),
			new(Stat.PVE_DAMAGE_TAKEN_MONSTER, "pveDamageTakenMonster"),
			new(Stat.PVE_DAMAGE_TAKEN_RAID, "pveDamageTakenRaid"),

			// ATTACK & DEFENCE RATES
			new(Stat.MAGIC_CRITICAL_DAMAGE, "mCritPower"),
			new(Stat.SKILL_POWER_ADD, "skillPowerAdd"),
			new(Stat.PHYSICAL_SKILL_POWER, "physicalSkillPower"),
			new(Stat.MAGICAL_SKILL_POWER, "magicalSkillPower"),
			new(Stat.PHYSICAL_SKILL_CRITICAL_DAMAGE, "cAtkSkill"),
			new(Stat.PHYSICAL_SKILL_CRITICAL_DAMAGE_ADD, "cAtkSkillAdd"),
			new(Stat.MAGIC_CRITICAL_DAMAGE_ADD, "mCritPowerAdd"),
			new(Stat.SHIELD_DEFENCE_RATE, "rShld", new ShieldDefenceRateFinalizer()),
			new(Stat.CRITICAL_RATE, "rCrit", new PCriticalRateFinalizer(), StatInfo.DefaultAddFunction, StatInfo.DefaultAddFunction),
			new(Stat.CRITICAL_RATE_SKILL, "physicalSkillCriticalRate"),
			new(Stat.ADD_MAX_MAGIC_CRITICAL_RATE, "addMaxMagicCritRate"),
			new(Stat.ADD_MAX_PHYSICAL_CRITICAL_RATE, "addMaxPhysicalCritRate"),
			new(Stat.MAGIC_CRITICAL_RATE, "mCritRate", new MCritRateFinalizer()),
			new(Stat.MAGIC_CRITICAL_RATE_BY_CRITICAL_RATE, "mCritRateByRCrit", StatDefaultFinalizer.Instance, StatInfo.DefaultAddFunction, StatInfo.DefaultMulFunction, 0, 0),
			new(Stat.DEFENCE_CRITICAL_RATE, "defCritRate"),
			new(Stat.DEFENCE_CRITICAL_RATE_ADD, "defCritRateAdd"),
			new(Stat.DEFENCE_MAGIC_CRITICAL_RATE, "defMCritRate"),
			new(Stat.DEFENCE_MAGIC_CRITICAL_RATE_ADD, "defMCritRateAdd"),
			new(Stat.DEFENCE_CRITICAL_DAMAGE, "defCritDamage"),
			new(Stat.DEFENCE_MAGIC_CRITICAL_DAMAGE, "defMCritDamage"),
			new(Stat.DEFENCE_MAGIC_CRITICAL_DAMAGE_ADD, "defMCritDamageAdd"),
			new(Stat.DEFENCE_CRITICAL_DAMAGE_ADD,
				"defCritDamageAdd"), // Resistance to critical damage in value (Example: +100 will be 100 more critical damage, NOT 100% more).
			new(Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_DAMAGE, "defCAtkSkill"),
			new(Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_DAMAGE_ADD, "defCAtkSkillAdd"),
			new(Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_RATE, "defPhysSkillCritRate"),
			new(Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_RATE_ADD, "defPhysSkillCritRateAdd"),
			new(Stat.DEFENCE_IGNORE_REMOVAL, "defIgnoreRemoval"),
			new(Stat.DEFENCE_IGNORE_REMOVAL_ADD, "defIgnoreRemovalAdd"),
			new(Stat.AREA_OF_EFFECT_DAMAGE_DEFENCE, "aoeDamageDefence"),
			new(Stat.AREA_OF_EFFECT_DAMAGE_MODIFY, "aoeDamageModify"),
			new(Stat.BLOW_RATE, "blowRate"),
			new(Stat.BLOW_RATE_DEFENCE, "blowRateDefence"),
			new(Stat.INSTANT_KILL_RESIST, "instantKillResist"),
			new(Stat.EXPSP_RATE, "rExp"),
			new(Stat.ACTIVE_BONUS_EXP, "activeBonusExp"), // Used to measure active skill bonus exp.
			new(Stat.BONUS_EXP_BUFFS, "bonusExpBuffs"), // Used to count active skill exp.
			new(Stat.BONUS_EXP_PASSIVES, "bonusExpPassives"), // Used to count passive skill exp.
			new(Stat.BONUS_EXP_PET, "bonusExpPet"),
			new(Stat.BONUS_EXP, "bonusExp"),
			new(Stat.BONUS_SP, "bonusSp"),
			new(Stat.BONUS_DROP_ADENA, "bonusDropAdena"),
			new(Stat.BONUS_DROP_AMOUNT, "bonusDropAmount"),
			new(Stat.BONUS_DROP_RATE, "bonusDropRate"),
			new(Stat.BONUS_DROP_RATE_LCOIN, "bonusDropRateLCoin"),
			new(Stat.BONUS_SPOIL_RATE, "bonusSpoilRate"),
			new(Stat.BONUS_RAID_POINTS, "bonusRaidPoints"),
			new(Stat.ATTACK_CANCEL, "cancel"),

			// ACCURACY & RANGE
			new(Stat.ACCURACY_COMBAT, "accCombat", new PAccuracyFinalizer()),
			new(Stat.ACCURACY_MAGIC, "accMagic", new MAccuracyFinalizer()),
			new(Stat.EVASION_RATE, "rEvas", new PEvasionRateFinalizer()),
			new(Stat.MAGIC_EVASION_RATE, "mEvas", new MEvasionRateFinalizer()),
			new(Stat.PHYSICAL_ATTACK_RANGE, "pAtkRange", new PRangeFinalizer()),
			new(Stat.MAGIC_ATTACK_RANGE, "mAtkRange"),
			new(Stat.ATTACK_COUNT_MAX, "atkCountMax"),
			new(Stat.PHYSICAL_POLEARM_TARGET_SINGLE, "polearmSingleTarget"),
			new(Stat.WEAPON_ATTACK_ANGLE_BONUS, "weaponAttackAngleBonus"),

			// Run speed, walk & escape speed are calculated proportionally, magic speed is a buff
			new(Stat.MOVE_SPEED, "moveSpeed"),
			new(Stat.SPEED_LIMIT, "speedLimit"),
			new(Stat.RUN_SPEED, "runSpd", new SpeedFinalizer()),
			new(Stat.WALK_SPEED, "walkSpd", new SpeedFinalizer()),
			new(Stat.SWIM_RUN_SPEED, "fastSwimSpd", new SpeedFinalizer()),
			new(Stat.SWIM_WALK_SPEED, "slowSimSpd", new SpeedFinalizer()),
			new(Stat.FLY_RUN_SPEED, "fastFlySpd", new SpeedFinalizer()),
			new(Stat.FLY_WALK_SPEED, "slowFlySpd", new SpeedFinalizer()),

			// BASIC STATS
			new(Stat.STAT_STR, "STR", new BaseStatFinalizer()),
			new(Stat.STAT_CON, "CON", new BaseStatFinalizer()),
			new(Stat.STAT_DEX, "DEX", new BaseStatFinalizer()),
			new(Stat.STAT_INT, "INT", new BaseStatFinalizer()),
			new(Stat.STAT_WIT, "WIT", new BaseStatFinalizer()),
			new(Stat.STAT_MEN, "MEN", new BaseStatFinalizer()),

			// Special stats, share one slot in Calculator

			// VARIOUS
			new(Stat.BREATH, "breath"),
			new(Stat.FALL, "fall"),
			new(Stat.FISHING_EXP_SP_BONUS, "fishingExpSpBonus"),
			new(Stat.ENCHANT_RATE, "enchantRate"),

			// VULNERABILITIES
			new(Stat.DAMAGE_ZONE_VULN, "damageZoneVuln"),
			new(Stat.RESIST_DISPEL_BUFF, "cancelVuln"), // Resistance for cancel type skills
			new(Stat.RESIST_ABNORMAL_DEBUFF, "debuffVuln"),

			// RESISTANCES
			new(Stat.FIRE_RES, "fireRes", new AttributeFinalizer(AttributeType.FIRE, false)),
			new(Stat.WIND_RES, "windRes", new AttributeFinalizer(AttributeType.WIND, false)),
			new(Stat.WATER_RES, "waterRes", new AttributeFinalizer(AttributeType.WATER, false)),
			new(Stat.EARTH_RES, "earthRes", new AttributeFinalizer(AttributeType.EARTH, false)),
			new(Stat.HOLY_RES, "holyRes", new AttributeFinalizer(AttributeType.HOLY, false)),
			new(Stat.DARK_RES, "darkRes", new AttributeFinalizer(AttributeType.DARK, false)),
			new(Stat.BASE_ATTRIBUTE_RES, "baseAttrRes"),
			new(Stat.MAGIC_SUCCESS_RES, "magicSuccRes"),
			// new StatInfo(Stat.BUFF_IMMUNITY, "buffImmunity"), // TODO: Implement me
			new(Stat.ABNORMAL_RESIST_PHYSICAL, "abnormalResPhysical"),
			new(Stat.ABNORMAL_RESIST_MAGICAL, "abnormalResMagical"),
			new(Stat.REAL_DAMAGE_RESIST, "realDamageResist"),

			// ELEMENT POWER
			new(Stat.FIRE_POWER, "firePower", new AttributeFinalizer(AttributeType.FIRE, true)),
			new(Stat.WATER_POWER, "waterPower", new AttributeFinalizer(AttributeType.WATER, true)),
			new(Stat.WIND_POWER, "windPower", new AttributeFinalizer(AttributeType.WIND, true)),
			new(Stat.EARTH_POWER, "earthPower", new AttributeFinalizer(AttributeType.EARTH, true)),
			new(Stat.HOLY_POWER, "holyPower", new AttributeFinalizer(AttributeType.HOLY, true)),
			new(Stat.DARK_POWER, "darkPower", new AttributeFinalizer(AttributeType.DARK, true)),

			// PROFICIENCY
			new(Stat.REFLECT_DAMAGE_PERCENT, "reflectDam"),
			new(Stat.REFLECT_DAMAGE_PERCENT_DEFENSE, "reflectDamDef"),
			new(Stat.REFLECT_SKILL_MAGIC, "reflectSkillMagic"), // Need rework
			new(Stat.REFLECT_SKILL_PHYSIC, "reflectSkillPhysic"), // Need rework
			new(Stat.VENGEANCE_SKILL_MAGIC_DAMAGE, "vengeanceMdam"),
			new(Stat.VENGEANCE_SKILL_PHYSICAL_DAMAGE, "vengeancePdam"),
			new(Stat.ABSORB_DAMAGE_PERCENT, "absorbDam"),
			new(Stat.ABSORB_DAMAGE_CHANCE, "absorbDamChance", new VampiricChanceFinalizer()),
			new(Stat.ABSORB_DAMAGE_DEFENCE, "absorbDamDefence"),
			new(Stat.TRANSFER_DAMAGE_SUMMON_PERCENT, "transDam"),
			new(Stat.MANA_SHIELD_PERCENT, "manaShield"),
			new(Stat.TRANSFER_DAMAGE_TO_PLAYER, "transDamToPlayer"),
			new(Stat.ABSORB_MANA_DAMAGE_PERCENT, "absorbDamMana"),
			new(Stat.ABSORB_MANA_DAMAGE_CHANCE, "absorbDamManaChance", new MpVampiricChanceFinalizer()),

			new(Stat.WEIGHT_LIMIT, "weightLimit"),
			new(Stat.WEIGHT_PENALTY, "weightPenalty"),

			// ExSkill
			new(Stat.INVENTORY_NORMAL, "inventoryLimit"),
			new(Stat.STORAGE_PRIVATE, "whLimit"),
			new(Stat.TRADE_SELL, "PrivateSellLimit"),
			new(Stat.TRADE_BUY, "PrivateBuyLimit"),
			new(Stat.RECIPE_DWARVEN, "DwarfRecipeLimit"),
			new(Stat.RECIPE_COMMON, "CommonRecipeLimit"),

			// Skill mastery
			new(Stat.SKILL_MASTERY, "skillMastery"),
			new(Stat.SKILL_MASTERY_RATE, "skillMasteryRate"),

			// Vitality
			new(Stat.VITALITY_CONSUME_RATE, "vitalityConsumeRate"),
			new(Stat.VITALITY_EXP_RATE, "vitalityExpRate"),
			new(Stat.VITALITY_SKILLS, "vitalitySkills"), // Used to count vitality skill bonuses.

			// Magic Lamp
			new(Stat.MAGIC_LAMP_EXP_RATE, "magicLampExpRate"),

			new(Stat.LAMP_BONUS_EXP, "LampBonusExp"),
			new(Stat.LAMP_BONUS_BUFFS_COUNT, "LampBonusBuffCount"),

			// Henna
			new(Stat.HENNA_SLOTS_AVAILABLE, "hennaSlots"),

			// Souls
			new(Stat.MAX_SOULS, "maxSouls"),

			new(Stat.REDUCE_EXP_LOST_BY_PVP, "reduceExpLostByPvp"),
			new(Stat.REDUCE_EXP_LOST_BY_MOB, "reduceExpLostByMob"),
			new(Stat.REDUCE_EXP_LOST_BY_RAID, "reduceExpLostByRaid"),

			new(Stat.REDUCE_DEATH_PENALTY_BY_PVP, "reduceDeathPenaltyByPvp"),
			new(Stat.REDUCE_DEATH_PENALTY_BY_MOB, "reduceDeathPenaltyByMob"),
			new(Stat.REDUCE_DEATH_PENALTY_BY_RAID, "reduceDeathPenaltyByRaid"),

			// Brooches
			new(Stat.BROOCH_JEWELS, "broochJewels"),

			// Agathions
			new(Stat.AGATHION_SLOTS, "agathionSlots"),

			// Artifacts
			new(Stat.ARTIFACT_SLOTS, "artifactSlots"),

			// Summon Points
			new(Stat.MAX_SUMMON_POINTS, "summonPoints"),

			// Cubic Count
			new(Stat.MAX_CUBIC, "cubicCount"),

			// The maximum allowed range to be damaged/debuffed from.
			new(Stat.SPHERIC_BARRIER_RANGE, "sphericBarrier"),

			// Blocks given amount of debuffs.
			new(Stat.DEBUFF_BLOCK, "debuffBlock"),

			// Affects the random weapon damage.
			new(Stat.RANDOM_DAMAGE, "randomDamage", new RandomDamageFinalizer()),

			// Affects the random weapon damage.
			new(Stat.DAMAGE_LIMIT, "damageCap"),

			// Maximun momentum one can charge
			new(Stat.MAX_MOMENTUM, "maxMomentum"),

			// Which base stat ordinal should alter skill critical formula.
			new(Stat.STAT_BONUS_SKILL_CRITICAL, "statSkillCritical"),
			new(Stat.STAT_BONUS_SPEED, "statSpeed"),
			new(Stat.CRAFTING_CRITICAL, "craftingCritical"),
			new(Stat.SHOTS_BONUS, "shotBonus", new ShotsBonusFinalizer()),
			new(Stat.SOULSHOT_RESISTANCE, "soulshotResistance"),
			new(Stat.SPIRITSHOT_RESISTANCE, "spiritshotResistance"),
			new(Stat.WORLD_CHAT_POINTS, "worldChatPoints"),
			new(Stat.ATTACK_DAMAGE, "attackDamage"),

			new(Stat.IMMOBILE_DAMAGE_BONUS, "immobileBonus"),
			new(Stat.IMMOBILE_DAMAGE_RESIST, "immobileResist"),

			new(Stat.CRAFT_RATE, "CraftRate"),
			new(Stat.ELIXIR_USAGE_LIMIT, "elixirUsageLimit"),
			new(Stat.RESURRECTION_FEE_MODIFIER, "resurrectionFeeModifier"),
		}.ToImmutableDictionary(s => s.Stat);

	public static Stat SearchByXmlName(string str)
	{
		var kvp = _stats.FirstOrDefault(r => string.Equals(r.Value.XmlName, str, StringComparison.OrdinalIgnoreCase));
		if (kvp.Value is null)
			throw new ArgumentException("Invalid stat name");
		
		return kvp.Key;
	}

	public static StatInfo? GetInfo(this Stat stat) => CollectionExtensions.GetValueOrDefault(_stats, stat);

	public static double DoFinalize(this Stat stat, Creature creature, double? baseValue)
	{
		IStatFunction? finalizer = stat.GetInfo()?.Finalizer;
		if (finalizer is null)
			return defaultValue(creature, baseValue, stat);
		
		try
		{
			return finalizer.calc(creature, baseValue, stat);
		}
		catch (Exception e)
		{
			_logger.Error("Exception during finalization for : " + creature + " stat: " + stat + " : " + e);
			return defaultValue(creature, baseValue, stat);
		}
	}

	public static double defaultValue(Creature creature, double? @base, Stat stat)
	{
		double mul = creature.getStat().getMulValue(stat);
		double add = creature.getStat().getAddValue(stat);
		return @base != null
			? defaultValue(creature, stat, @base.Value)
			: mul * (add + creature.getStat().getMoveTypeValue(stat, creature.getMoveType()));
	}

	public static double defaultValue(Creature creature, Stat stat, double baseValue)
	{
		double mul = creature.getStat().getMulValue(stat);
		double add = creature.getStat().getAddValue(stat);
		return (mul * baseValue) + add + creature.getStat().getMoveTypeValue(stat, creature.getMoveType());
	}
}

public sealed class StatDefaultFinalizer: IStatFunction
{
	public static readonly IStatFunction Instance = new StatDefaultFinalizer();
	public double calc(Creature creature, double? @base, Stat stat) => StatUtil.defaultValue(creature, @base, stat);
}

// /**
//  * Enum of basic stats.
//  * @author mkizub
//  */
// public class StatInfo
// {
// 	// HP, MP & CP
// 	new StatInfo(Stat.HP_LIMIT, "hpLimit"),
// 	new StatInfo(Stat.MAX_HP, "maxHp", new MaxHpFinalizer()),
// 	new StatInfo(Stat.MAX_MP, "maxMp", new MaxMpFinalizer()),
// 	new StatInfo(Stat.MAX_CP, "maxCp", new MaxCpFinalizer()),
// 	new StatInfo(Stat.MAX_RECOVERABLE_HP, "maxRecoverableHp"), // The maximum HP that is able to be recovered trough heals
// 	new StatInfo(Stat.MAX_RECOVERABLE_MP, "maxRecoverableMp"),
// 	new StatInfo(Stat.MAX_RECOVERABLE_CP, "maxRecoverableCp"),
// 	new StatInfo(Stat.REGENERATE_HP_RATE, "regHp", new RegenHPFinalizer()),
// 	new StatInfo(Stat.REGENERATE_CP_RATE, "regCp", new RegenCPFinalizer()),
// 	new StatInfo(Stat.REGENERATE_MP_RATE, "regMp", new RegenMPFinalizer()),
// 	new StatInfo(Stat.ADDITIONAL_POTION_HP, "addPotionHp"),
// 	new StatInfo(Stat.ADDITIONAL_POTION_MP, "addPotionMp"),
// 	new StatInfo(Stat.ADDITIONAL_POTION_CP, "addPotionCp"),
// 	new StatInfo(Stat.MANA_CHARGE, "manaCharge"),
// 	new StatInfo(Stat.HEAL_EFFECT, "healEffect"),
// 	new StatInfo(Stat.HEAL_EFFECT_ADD, "healEffectAdd"),
// 	new StatInfo(Stat.FEED_MODIFY, "feedModify"),
// 	
// 	// ATTACK & DEFENCE
// 	new StatInfo(Stat.PHYSICAL_DEFENCE, "pDef", new PDefenseFinalizer()),
// 	new StatInfo(Stat.MAGICAL_DEFENCE, "mDef", new MDefenseFinalizer()),
// 	new StatInfo(Stat.PHYSICAL_ATTACK, "pAtk", new PAttackFinalizer()),
// 	new StatInfo(Stat.MAGIC_ATTACK, "mAtk", new MAttackFinalizer()),
// 	new StatInfo(Stat.WEAPON_BONUS_PHYSICAL_ATTACK, "weaponBonusPAtk"),
// 	new StatInfo(Stat.WEAPON_BONUS_PHYSICAL_ATTACK_MULTIPIER, "weaponBonusPAtkMultiplier"),
// 	new StatInfo(Stat.WEAPON_BONUS_MAGIC_ATTACK, "weaponBonusMAtk"),
// 	new StatInfo(Stat.WEAPON_BONUS_MAGIC_ATTACK_MULTIPIER, "weaponBonusMAtkMultiplier"),
// 	MAGIC_ATTACK_BY_PHYSICAL_ATTACK("mAtkByPAtk", Stat::defaultValue, MathUtil::add, MathUtil::mul, 0, 0),
// 	new StatInfo(Stat.PHYSICAL_ATTACK_SPEED, "pAtkSpd", new PAttackSpeedFinalizer()),
// 	new StatInfo(Stat.MAGIC_ATTACK_SPEED, "mAtkSpd", new MAttackSpeedFinalizer()), // Magic Skill Casting Time Rate
// 	new StatInfo(Stat.ATK_REUSE, "atkReuse"), // Bows Hits Reuse Rate
// 	new StatInfo(Stat.SHIELD_DEFENCE, "sDef", new ShieldDefenceFinalizer()),
// 	new StatInfo(Stat.SHIELD_DEFENCE_IGNORE_REMOVAL, "shieldDefIgnoreRemoval"),
// 	new StatInfo(Stat.SHIELD_DEFENCE_IGNORE_REMOVAL_ADD, "shieldDefIgnoreRemovalAdd"),
// 	new StatInfo(Stat.CRITICAL_DAMAGE, "cAtk"),
// 	new StatInfo(Stat.CRITICAL_DAMAGE_ADD, "cAtkAdd"), // this is another type for special critical damage mods - vicious stance, critical power and critical damage SA
// 	new StatInfo(Stat.HATE_ATTACK, "attackHate"),
// 	new StatInfo(Stat.REAR_DAMAGE_RATE, "rearDamage"),
// 	new StatInfo(Stat.AUTO_ATTACK_DAMAGE_BONUS, "autoAttackDamageBonus"),
// 	new StatInfo(Stat.IGNORE_REDUCE_DAMAGE, "ignoreReduceDamage"),
// 	
// 	// ELEMENTAL SPIRITS
// 	new StatInfo(Stat.ELEMENTAL_SPIRIT_FIRE_ATTACK, "elementalSpiritFireAttack"),
// 	new StatInfo(Stat.ELEMENTAL_SPIRIT_WATER_ATTACK, "elementalSpiritWaterAttack"),
// 	new StatInfo(Stat.ELEMENTAL_SPIRIT_WIND_ATTACK, "elementalSpiritWindAttack"),
// 	new StatInfo(Stat.ELEMENTAL_SPIRIT_EARTH_ATTACK, "elementalSpiritEarthAttack"),
// 	new StatInfo(Stat.ELEMENTAL_SPIRIT_FIRE_DEFENSE, "elementalSpiritFireDefense"),
// 	new StatInfo(Stat.ELEMENTAL_SPIRIT_WATER_DEFENSE, "elementalSpiritWaterDefense"),
// 	new StatInfo(Stat.ELEMENTAL_SPIRIT_WIND_DEFENSE, "elementalSpiritWindDefense"),
// 	new StatInfo(Stat.ELEMENTAL_SPIRIT_EARTH_DEFENSE, "elementalSpiritEarthDefense"),
// 	new StatInfo(Stat.ELEMENTAL_SPIRIT_CRITICAL_RATE, "elementalSpiritCriticalRate"),
// 	new StatInfo(Stat.ELEMENTAL_SPIRIT_CRITICAL_DAMAGE, "elementalSpiritCriticalDamage"),
// 	new StatInfo(Stat.ELEMENTAL_SPIRIT_BONUS_EXP, "elementalSpiritExp"),
// 	
// 	// PVP BONUS
// 	new StatInfo(Stat.PVP_PHYSICAL_ATTACK_DAMAGE, "pvpPhysDmg"),
// 	new StatInfo(Stat.PVP_MAGICAL_SKILL_DAMAGE, "pvpMagicalDmg"),
// 	new StatInfo(Stat.PVP_PHYSICAL_SKILL_DAMAGE, "pvpPhysSkillsDmg"),
// 	new StatInfo(Stat.PVP_PHYSICAL_ATTACK_DEFENCE, "pvpPhysDef"),
// 	new StatInfo(Stat.PVP_MAGICAL_SKILL_DEFENCE, "pvpMagicalDef"),
// 	new StatInfo(Stat.PVP_PHYSICAL_SKILL_DEFENCE, "pvpPhysSkillsDef"),
// 	
// 	// PVE BONUS
// 	new StatInfo(Stat.PVE_PHYSICAL_ATTACK_DAMAGE, "pvePhysDmg"),
// 	new StatInfo(Stat.PVE_PHYSICAL_SKILL_DAMAGE, "pvePhysSkillDmg"),
// 	new StatInfo(Stat.PVE_MAGICAL_SKILL_DAMAGE, "pveMagicalDmg"),
// 	new StatInfo(Stat.PVE_PHYSICAL_ATTACK_DEFENCE, "pvePhysDef"),
// 	new StatInfo(Stat.PVE_PHYSICAL_SKILL_DEFENCE, "pvePhysSkillDef"),
// 	new StatInfo(Stat.PVE_MAGICAL_SKILL_DEFENCE, "pveMagicalDef"),
// 	new StatInfo(Stat.PVE_RAID_PHYSICAL_ATTACK_DAMAGE, "pveRaidPhysDmg"),
// 	new StatInfo(Stat.PVE_RAID_PHYSICAL_SKILL_DAMAGE, "pveRaidPhysSkillDmg"),
// 	new StatInfo(Stat.PVE_RAID_MAGICAL_SKILL_DAMAGE, "pveRaidMagicalDmg"),
// 	new StatInfo(Stat.PVE_RAID_PHYSICAL_ATTACK_DEFENCE, "pveRaidPhysDef"),
// 	new StatInfo(Stat.PVE_RAID_PHYSICAL_SKILL_DEFENCE, "pveRaidPhysSkillDef"),
// 	new StatInfo(Stat.PVE_RAID_MAGICAL_SKILL_DEFENCE, "pveRaidMagicalDef"),
// 	
// 	// FIXED BONUS
// 	new StatInfo(Stat.PVP_DAMAGE_TAKEN, "pvpDamageTaken"),
// 	new StatInfo(Stat.PVE_DAMAGE_TAKEN, "pveDamageTaken"),
// 	new StatInfo(Stat.PVE_DAMAGE_TAKEN_MONSTER, "pveDamageTakenMonster"),
// 	new StatInfo(Stat.PVE_DAMAGE_TAKEN_RAID, "pveDamageTakenRaid"),
// 	
// 	// ATTACK & DEFENCE RATES
// 	new StatInfo(Stat.MAGIC_CRITICAL_DAMAGE, "mCritPower"),
// 	new StatInfo(Stat.SKILL_POWER_ADD, "skillPowerAdd"),
// 	new StatInfo(Stat.PHYSICAL_SKILL_POWER, "physicalSkillPower"),
// 	new StatInfo(Stat.MAGICAL_SKILL_POWER, "magicalSkillPower"),
// 	new StatInfo(Stat.PHYSICAL_SKILL_CRITICAL_DAMAGE, "cAtkSkill"),
// 	new StatInfo(Stat.PHYSICAL_SKILL_CRITICAL_DAMAGE_ADD, "cAtkSkillAdd"),
// 	new StatInfo(Stat.MAGIC_CRITICAL_DAMAGE_ADD, "mCritPowerAdd"),
// 	new StatInfo(Stat.SHIELD_DEFENCE_RATE, "rShld", new ShieldDefenceRateFinalizer()),
// 	CRITICAL_RATE("rCrit", new PCriticalRateFinalizer(), MathUtil::add, MathUtil::add, 0, 1),
// 	new StatInfo(Stat.CRITICAL_RATE_SKILL, "physicalSkillCriticalRate"),
// 	new StatInfo(Stat.ADD_MAX_MAGIC_CRITICAL_RATE, "addMaxMagicCritRate"),
// 	new StatInfo(Stat.ADD_MAX_PHYSICAL_CRITICAL_RATE, "addMaxPhysicalCritRate"),
// 	new StatInfo(Stat.MAGIC_CRITICAL_RATE, "mCritRate", new MCritRateFinalizer()),
// 	MAGIC_CRITICAL_RATE_BY_CRITICAL_RATE("mCritRateByRCrit", Stat::defaultValue, MathUtil::add, MathUtil::mul, 0, 0),
// 	new StatInfo(Stat.DEFENCE_CRITICAL_RATE, "defCritRate"),
// 	new StatInfo(Stat.DEFENCE_CRITICAL_RATE_ADD, "defCritRateAdd"),
// 	new StatInfo(Stat.DEFENCE_MAGIC_CRITICAL_RATE, "defMCritRate"),
// 	new StatInfo(Stat.DEFENCE_MAGIC_CRITICAL_RATE_ADD, "defMCritRateAdd"),
// 	new StatInfo(Stat.DEFENCE_CRITICAL_DAMAGE, "defCritDamage"),
// 	new StatInfo(Stat.DEFENCE_MAGIC_CRITICAL_DAMAGE, "defMCritDamage"),
// 	new StatInfo(Stat.DEFENCE_MAGIC_CRITICAL_DAMAGE_ADD, "defMCritDamageAdd"),
// 	new StatInfo(Stat.DEFENCE_CRITICAL_DAMAGE_ADD, "defCritDamageAdd"), // Resistance to critical damage in value (Example: +100 will be 100 more critical damage, NOT 100% more).
// 	new StatInfo(Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_DAMAGE, "defCAtkSkill"),
// 	new StatInfo(Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_DAMAGE_ADD, "defCAtkSkillAdd"),
// 	new StatInfo(Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_RATE, "defPhysSkillCritRate"),
// 	new StatInfo(Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_RATE_ADD, "defPhysSkillCritRateAdd"),
// 	new StatInfo(Stat.DEFENCE_IGNORE_REMOVAL, "defIgnoreRemoval"),
// 	new StatInfo(Stat.DEFENCE_IGNORE_REMOVAL_ADD, "defIgnoreRemovalAdd"),
// 	new StatInfo(Stat.AREA_OF_EFFECT_DAMAGE_DEFENCE, "aoeDamageDefence"),
// 	new StatInfo(Stat.AREA_OF_EFFECT_DAMAGE_MODIFY, "aoeDamageModify"),
// 	new StatInfo(Stat.BLOW_RATE, "blowRate"),
// 	new StatInfo(Stat.BLOW_RATE_DEFENCE, "blowRateDefence"),
// 	new StatInfo(Stat.INSTANT_KILL_RESIST, "instantKillResist"),
// 	new StatInfo(Stat.EXPSP_RATE, "rExp"),
// 	new StatInfo(Stat.ACTIVE_BONUS_EXP, "activeBonusExp"), // Used to measure active skill bonus exp.
// 	new StatInfo(Stat.BONUS_EXP_BUFFS, "bonusExpBuffs"), // Used to count active skill exp.
// 	new StatInfo(Stat.BONUS_EXP_PASSIVES, "bonusExpPassives"), // Used to count passive skill exp.
// 	new StatInfo(Stat.BONUS_EXP_PET, "bonusExpPet"),
// 	new StatInfo(Stat.BONUS_EXP, "bonusExp"),
// 	new StatInfo(Stat.BONUS_SP, "bonusSp"),
// 	new StatInfo(Stat.BONUS_DROP_ADENA, "bonusDropAdena"),
// 	new StatInfo(Stat.BONUS_DROP_AMOUNT, "bonusDropAmount"),
// 	new StatInfo(Stat.BONUS_DROP_RATE, "bonusDropRate"),
// 	new StatInfo(Stat.BONUS_DROP_RATE_LCOIN, "bonusDropRateLCoin"),
// 	new StatInfo(Stat.BONUS_SPOIL_RATE, "bonusSpoilRate"),
// 	new StatInfo(Stat.BONUS_RAID_POINTS, "bonusRaidPoints"),
// 	new StatInfo(Stat.ATTACK_CANCEL, "cancel"),
// 	
// 	// ACCURACY & RANGE
// 	new StatInfo(Stat.ACCURACY_COMBAT, "accCombat", new PAccuracyFinalizer()),
// 	new StatInfo(Stat.ACCURACY_MAGIC, "accMagic", new MAccuracyFinalizer()),
// 	new StatInfo(Stat.EVASION_RATE, "rEvas", new PEvasionRateFinalizer()),
// 	new StatInfo(Stat.MAGIC_EVASION_RATE, "mEvas", new MEvasionRateFinalizer()),
// 	new StatInfo(Stat.PHYSICAL_ATTACK_RANGE, "pAtkRange", new PRangeFinalizer()),
// 	new StatInfo(Stat.MAGIC_ATTACK_RANGE, "mAtkRange"),
// 	new StatInfo(Stat.ATTACK_COUNT_MAX, "atkCountMax"),
// 	new StatInfo(Stat.PHYSICAL_POLEARM_TARGET_SINGLE, "polearmSingleTarget"),
// 	new StatInfo(Stat.WEAPON_ATTACK_ANGLE_BONUS, "weaponAttackAngleBonus"),
// 	
// 	// Run speed, walk & escape speed are calculated proportionally, magic speed is a buff
// 	new StatInfo(Stat.MOVE_SPEED, "moveSpeed"),
// 	new StatInfo(Stat.SPEED_LIMIT, "speedLimit"),
// 	new StatInfo(Stat.RUN_SPEED, "runSpd", new SpeedFinalizer()),
// 	new StatInfo(Stat.WALK_SPEED, "walkSpd", new SpeedFinalizer()),
// 	new StatInfo(Stat.SWIM_RUN_SPEED, "fastSwimSpd", new SpeedFinalizer()),
// 	new StatInfo(Stat.SWIM_WALK_SPEED, "slowSimSpd", new SpeedFinalizer()),
// 	new StatInfo(Stat.FLY_RUN_SPEED, "fastFlySpd", new SpeedFinalizer()),
// 	new StatInfo(Stat.FLY_WALK_SPEED, "slowFlySpd", new SpeedFinalizer()),
// 	
// 	// BASIC STATS
// 	new StatInfo(Stat.STAT_STR, "STR", new BaseStatFinalizer()),
// 	new StatInfo(Stat.STAT_CON, "CON", new BaseStatFinalizer()),
// 	new StatInfo(Stat.STAT_DEX, "DEX", new BaseStatFinalizer()),
// 	new StatInfo(Stat.STAT_INT, "INT", new BaseStatFinalizer()),
// 	new StatInfo(Stat.STAT_WIT, "WIT", new BaseStatFinalizer()),
// 	new StatInfo(Stat.STAT_MEN, "MEN", new BaseStatFinalizer()),
// 	
// 	// Special stats, share one slot in Calculator
// 	
// 	// VARIOUS
// 	new StatInfo(Stat.BREATH, "breath"),
// 	new StatInfo(Stat.FALL, "fall"),
// 	new StatInfo(Stat.FISHING_EXP_SP_BONUS, "fishingExpSpBonus"),
// 	new StatInfo(Stat.ENCHANT_RATE, "enchantRate"),
// 	
// 	// VULNERABILITIES
// 	new StatInfo(Stat.DAMAGE_ZONE_VULN, "damageZoneVuln"),
// 	new StatInfo(Stat.RESIST_DISPEL_BUFF, "cancelVuln"), // Resistance for cancel type skills
// 	new StatInfo(Stat.RESIST_ABNORMAL_DEBUFF, "debuffVuln"),
// 	
// 	// RESISTANCES
// 	FIRE_RES("fireRes", new AttributeFinalizer(AttributeType.FIRE, false)),
// 	WIND_RES("windRes", new AttributeFinalizer(AttributeType.WIND, false)),
// 	WATER_RES("waterRes", new AttributeFinalizer(AttributeType.WATER, false)),
// 	EARTH_RES("earthRes", new AttributeFinalizer(AttributeType.EARTH, false)),
// 	HOLY_RES("holyRes", new AttributeFinalizer(AttributeType.HOLY, false)),
// 	DARK_RES("darkRes", new AttributeFinalizer(AttributeType.DARK, false)),
// 	new StatInfo(Stat.BASE_ATTRIBUTE_RES, "baseAttrRes"),
// 	new StatInfo(Stat.MAGIC_SUCCESS_RES, "magicSuccRes"),
// 	// new StatInfo(Stat.BUFF_IMMUNITY, "buffImmunity"), // TODO: Implement me
// 	new StatInfo(Stat.ABNORMAL_RESIST_PHYSICAL, "abnormalResPhysical"),
// 	new StatInfo(Stat.ABNORMAL_RESIST_MAGICAL, "abnormalResMagical"),
// 	new StatInfo(Stat.REAL_DAMAGE_RESIST, "realDamageResist"),
// 	
// 	// ELEMENT POWER
// 	FIRE_POWER("firePower", new AttributeFinalizer(AttributeType.FIRE, true)),
// 	WATER_POWER("waterPower", new AttributeFinalizer(AttributeType.WATER, true)),
// 	WIND_POWER("windPower", new AttributeFinalizer(AttributeType.WIND, true)),
// 	EARTH_POWER("earthPower", new AttributeFinalizer(AttributeType.EARTH, true)),
// 	HOLY_POWER("holyPower", new AttributeFinalizer(AttributeType.HOLY, true)),
// 	DARK_POWER("darkPower", new AttributeFinalizer(AttributeType.DARK, true)),
// 	
// 	// PROFICIENCY
// 	new StatInfo(Stat.REFLECT_DAMAGE_PERCENT, "reflectDam"),
// 	new StatInfo(Stat.REFLECT_DAMAGE_PERCENT_DEFENSE, "reflectDamDef"),
// 	new StatInfo(Stat.REFLECT_SKILL_MAGIC, "reflectSkillMagic"), // Need rework
// 	new StatInfo(Stat.REFLECT_SKILL_PHYSIC, "reflectSkillPhysic"), // Need rework
// 	new StatInfo(Stat.VENGEANCE_SKILL_MAGIC_DAMAGE, "vengeanceMdam"),
// 	new StatInfo(Stat.VENGEANCE_SKILL_PHYSICAL_DAMAGE, "vengeancePdam"),
// 	new StatInfo(Stat.ABSORB_DAMAGE_PERCENT, "absorbDam"),
// 	new StatInfo(Stat.ABSORB_DAMAGE_CHANCE, "absorbDamChance", new VampiricChanceFinalizer()),
// 	new StatInfo(Stat.ABSORB_DAMAGE_DEFENCE, "absorbDamDefence"),
// 	new StatInfo(Stat.TRANSFER_DAMAGE_SUMMON_PERCENT, "transDam"),
// 	new StatInfo(Stat.MANA_SHIELD_PERCENT, "manaShield"),
// 	new StatInfo(Stat.TRANSFER_DAMAGE_TO_PLAYER, "transDamToPlayer"),
// 	new StatInfo(Stat.ABSORB_MANA_DAMAGE_PERCENT, "absorbDamMana"),
// 	new StatInfo(Stat.ABSORB_MANA_DAMAGE_CHANCE, "absorbDamManaChance", new MpVampiricChanceFinalizer()),
// 	
// 	new StatInfo(Stat.WEIGHT_LIMIT, "weightLimit"),
// 	new StatInfo(Stat.WEIGHT_PENALTY, "weightPenalty"),
// 	
// 	// ExSkill
// 	new StatInfo(Stat.INVENTORY_NORMAL, "inventoryLimit"),
// 	new StatInfo(Stat.STORAGE_PRIVATE, "whLimit"),
// 	new StatInfo(Stat.TRADE_SELL, "PrivateSellLimit"),
// 	new StatInfo(Stat.TRADE_BUY, "PrivateBuyLimit"),
// 	new StatInfo(Stat.RECIPE_DWARVEN, "DwarfRecipeLimit"),
// 	new StatInfo(Stat.RECIPE_COMMON, "CommonRecipeLimit"),
// 	
// 	// Skill mastery
// 	new StatInfo(Stat.SKILL_MASTERY, "skillMastery"),
// 	new StatInfo(Stat.SKILL_MASTERY_RATE, "skillMasteryRate"),
// 	
// 	// Vitality
// 	new StatInfo(Stat.VITALITY_CONSUME_RATE, "vitalityConsumeRate"),
// 	new StatInfo(Stat.VITALITY_EXP_RATE, "vitalityExpRate"),
// 	new StatInfo(Stat.VITALITY_SKILLS, "vitalitySkills"), // Used to count vitality skill bonuses.
// 	
// 	// Magic Lamp
// 	new StatInfo(Stat.MAGIC_LAMP_EXP_RATE, "magicLampExpRate"),
// 	
// 	new StatInfo(Stat.LAMP_BONUS_EXP, "LampBonusExp"),
// 	new StatInfo(Stat.LAMP_BONUS_BUFFS_COUNT, "LampBonusBuffCount"),
// 	
// 	// Henna
// 	new StatInfo(Stat.HENNA_SLOTS_AVAILABLE, "hennaSlots"),
// 	
// 	// Souls
// 	new StatInfo(Stat.MAX_SOULS, "maxSouls"),
// 	
// 	new StatInfo(Stat.REDUCE_EXP_LOST_BY_PVP, "reduceExpLostByPvp"),
// 	new StatInfo(Stat.REDUCE_EXP_LOST_BY_MOB, "reduceExpLostByMob"),
// 	new StatInfo(Stat.REDUCE_EXP_LOST_BY_RAID, "reduceExpLostByRaid"),
// 	
// 	new StatInfo(Stat.REDUCE_DEATH_PENALTY_BY_PVP, "reduceDeathPenaltyByPvp"),
// 	new StatInfo(Stat.REDUCE_DEATH_PENALTY_BY_MOB, "reduceDeathPenaltyByMob"),
// 	new StatInfo(Stat.REDUCE_DEATH_PENALTY_BY_RAID, "reduceDeathPenaltyByRaid"),
// 	
// 	// Brooches
// 	new StatInfo(Stat.BROOCH_JEWELS, "broochJewels"),
// 	
// 	// Agathions
// 	new StatInfo(Stat.AGATHION_SLOTS, "agathionSlots"),
// 	
// 	// Artifacts
// 	new StatInfo(Stat.ARTIFACT_SLOTS, "artifactSlots"),
// 	
// 	// Summon Points
// 	new StatInfo(Stat.MAX_SUMMON_POINTS, "summonPoints"),
// 	
// 	// Cubic Count
// 	new StatInfo(Stat.MAX_CUBIC, "cubicCount"),
// 	
// 	// The maximum allowed range to be damaged/debuffed from.
// 	new StatInfo(Stat.SPHERIC_BARRIER_RANGE, "sphericBarrier"),
// 	
// 	// Blocks given amount of debuffs.
// 	new StatInfo(Stat.DEBUFF_BLOCK, "debuffBlock"),
// 	
// 	// Affects the random weapon damage.
// 	new StatInfo(Stat.RANDOM_DAMAGE, "randomDamage", new RandomDamageFinalizer()),
// 	
// 	// Affects the random weapon damage.
// 	new StatInfo(Stat.DAMAGE_LIMIT, "damageCap"),
// 	
// 	// Maximun momentum one can charge
// 	new StatInfo(Stat.MAX_MOMENTUM, "maxMomentum"),
// 	
// 	// Which base stat ordinal should alter skill critical formula.
// 	new StatInfo(Stat.STAT_BONUS_SKILL_CRITICAL, "statSkillCritical"),
// 	new StatInfo(Stat.STAT_BONUS_SPEED, "statSpeed"),
// 	new StatInfo(Stat.CRAFTING_CRITICAL, "craftingCritical"),
// 	new StatInfo(Stat.SHOTS_BONUS, "shotBonus", new ShotsBonusFinalizer()),
// 	new StatInfo(Stat.SOULSHOT_RESISTANCE, "soulshotResistance"),
// 	new StatInfo(Stat.SPIRITSHOT_RESISTANCE, "spiritshotResistance"),
// 	new StatInfo(Stat.WORLD_CHAT_POINTS, "worldChatPoints"),
// 	new StatInfo(Stat.ATTACK_DAMAGE, "attackDamage"),
// 	
// 	new StatInfo(Stat.IMMOBILE_DAMAGE_BONUS, "immobileBonus"),
// 	new StatInfo(Stat.IMMOBILE_DAMAGE_RESIST, "immobileResist"),
// 	
// 	new StatInfo(Stat.CRAFT_RATE, "CraftRate"),
// 	new StatInfo(Stat.ELIXIR_USAGE_LIMIT, "elixirUsageLimit"),
// 	RESURRECTION_FEE_MODIFIER("resurrectionFeeModifier");
// 	
// 	public static final int NUM_STATS = values().length;
// 	
// 	private final String _value;
// 	private final IStatFunction _valueFinalizer;
// 	private final DoubleBinaryOperator _addFunction;
// 	private final DoubleBinaryOperator _mulFunction;
// 	private final Double _resetAddValue;
// 	private final Double _resetMulValue;
// 	
// 	public String getValue()
// 	{
// 		return _value;
// 	}
// 	
// 	Stat(String xmlString)
// 	{
// 		this(xmlString, Stat::defaultValue, MathUtil::add, MathUtil::mul, 0, 1);
// 	}
// 	
// 	Stat(String xmlString, IStatFunction valueFinalizer)
// 	{
// 		this(xmlString, valueFinalizer, MathUtil::add, MathUtil::mul, 0, 1);
// 	}
// 	
// 	Stat(String xmlString, IStatFunction valueFinalizer, DoubleBinaryOperator addFunction, DoubleBinaryOperator mulFunction, double resetAddValue, double resetMulValue)
// 	{
// 		_value = xmlString;
// 		_valueFinalizer = valueFinalizer;
// 		_addFunction = addFunction;
// 		_mulFunction = mulFunction;
// 		_resetAddValue = resetAddValue;
// 		_resetMulValue = resetMulValue;
// 	}
// 	
// 	public static Stat valueOfXml(String name)
// 	{
// 		String internName = name.intern();
// 		for (Stat s : values())
// 		{
// 			if (s.getValue().equals(internName))
// 			{
// 				return s;
// 			}
// 		}
// 		
// 		throw new NoSuchElementException("Unknown name '" + internName + "' for enum " + Stat.class.getSimpleName());
// 	}
// 	
// 	/**
// 	 * @param creature
// 	 * @param baseValue
// 	 * @return the final value
// 	 */
// 	public double finalize(Creature creature, OptionalDouble baseValue)
// 	{
// 		try
// 		{
// 			return _valueFinalizer.calc(creature, baseValue, this);
// 		}
// 		catch (Exception e)
// 		{
// 			// LOGGER.log(Level.WARNING, "Exception during finalization for : " + creature + " stat: " + toString() + " : ", e);
// 			return defaultValue(creature, baseValue, this);
// 		}
// 	}
// 	
// 	public double functionAdd(double oldValue, double value)
// 	{
// 		return _addFunction.applyAsDouble(oldValue, value);
// 	}
// 	
// 	public double functionMul(double oldValue, double value)
// 	{
// 		return _mulFunction.applyAsDouble(oldValue, value);
// 	}
// 	
// 	public Double getResetAddValue()
// 	{
// 		return _resetAddValue;
// 	}
// 	
// 	public Double getResetMulValue()
// 	{
// 		return _resetMulValue;
// 	}
// 	
// 	public static double weaponBaseValue(Creature creature, Stat stat)
// 	{
// 		return stat._valueFinalizer.calcWeaponBaseValue(creature, stat);
// 	}
// 	
// 	public static double defaultValue(Creature creature, OptionalDouble base, Stat stat)
// 	{
// 		final double mul = creature.getStat().getMul(stat);
// 		final double add = creature.getStat().getAdd(stat);
// 		return base.isPresent() ? defaultValue(creature, stat, base.getAsDouble()) : mul * (add + creature.getStat().getMoveTypeValue(stat, creature.getMoveType()));
// 	}
// 	
// 	public static double defaultValue(Creature creature, Stat stat, double baseValue)
// 	{
// 		final double mul = creature.getStat().getMul(stat);
// 		final double add = creature.getStat().getAdd(stat);
// 		return (mul * baseValue) + add + creature.getStat().getMoveTypeValue(stat, creature.getMoveType());
// 	}
// }
