namespace L2Dn.GameServer.Model.Stats;

/**
 * Enum of basic stats.
 * @author mkizub
 */
public enum Stat
{
	// HP, MP & CP
	HP_LIMIT,
	MAX_HP,
	MAX_MP,
	MAX_CP,
	MAX_RECOVERABLE_HP, // The maximum HP that is able to be recovered trough heals
	MAX_RECOVERABLE_MP,
	MAX_RECOVERABLE_CP,
	REGENERATE_HP_RATE,
	REGENERATE_CP_RATE,
	REGENERATE_MP_RATE,
	ADDITIONAL_POTION_HP,
	ADDITIONAL_POTION_MP,
	ADDITIONAL_POTION_CP,
	MANA_CHARGE,
	HEAL_EFFECT,
	HEAL_EFFECT_ADD,
	FEED_MODIFY,

	// ATTACK & DEFENCE
	PHYSICAL_DEFENCE,
	MAGICAL_DEFENCE,
	PHYSICAL_ATTACK,
	MAGIC_ATTACK,
	WEAPON_BONUS_PHYSICAL_ATTACK,
	WEAPON_BONUS_PHYSICAL_ATTACK_MULTIPIER,
	WEAPON_BONUS_MAGIC_ATTACK,
	WEAPON_BONUS_MAGIC_ATTACK_MULTIPIER,
	MAGIC_ATTACK_BY_PHYSICAL_ATTACK,
	PHYSICAL_ATTACK_SPEED,
	MAGIC_ATTACK_SPEED, // Magic Skill Casting Time Rate
	ATK_REUSE, // Bows Hits Reuse Rate
	SHIELD_DEFENCE,
	SHIELD_DEFENCE_IGNORE_REMOVAL,
	SHIELD_DEFENCE_IGNORE_REMOVAL_ADD,
	CRITICAL_DAMAGE,
	CRITICAL_DAMAGE_ADD, // this is another type for special critical damage mods - vicious stance, critical power and critical damage SA
	HATE_ATTACK,
	REAR_DAMAGE_RATE,
	AUTO_ATTACK_DAMAGE_BONUS,
	IGNORE_REDUCE_DAMAGE,

	// ELEMENTAL SPIRITS
	ELEMENTAL_SPIRIT_FIRE_ATTACK,
	ELEMENTAL_SPIRIT_WATER_ATTACK,
	ELEMENTAL_SPIRIT_WIND_ATTACK,
	ELEMENTAL_SPIRIT_EARTH_ATTACK,
	ELEMENTAL_SPIRIT_FIRE_DEFENSE,
	ELEMENTAL_SPIRIT_WATER_DEFENSE,
	ELEMENTAL_SPIRIT_WIND_DEFENSE,
	ELEMENTAL_SPIRIT_EARTH_DEFENSE,
	ELEMENTAL_SPIRIT_CRITICAL_RATE,
	ELEMENTAL_SPIRIT_CRITICAL_DAMAGE,
	ELEMENTAL_SPIRIT_BONUS_EXP,

	// PVP BONUS
	PVP_PHYSICAL_ATTACK_DAMAGE,
	PVP_MAGICAL_SKILL_DAMAGE,
	PVP_PHYSICAL_SKILL_DAMAGE,
	PVP_PHYSICAL_ATTACK_DEFENCE,
	PVP_MAGICAL_SKILL_DEFENCE,
	PVP_PHYSICAL_SKILL_DEFENCE,

	// PVE BONUS
	PVE_PHYSICAL_ATTACK_DAMAGE,
	PVE_PHYSICAL_SKILL_DAMAGE,
	PVE_MAGICAL_SKILL_DAMAGE,
	PVE_PHYSICAL_ATTACK_DEFENCE,
	PVE_PHYSICAL_SKILL_DEFENCE,
	PVE_MAGICAL_SKILL_DEFENCE,
	PVE_RAID_PHYSICAL_ATTACK_DAMAGE,
	PVE_RAID_PHYSICAL_SKILL_DAMAGE,
	PVE_RAID_MAGICAL_SKILL_DAMAGE,
	PVE_RAID_PHYSICAL_ATTACK_DEFENCE,
	PVE_RAID_PHYSICAL_SKILL_DEFENCE,
	PVE_RAID_MAGICAL_SKILL_DEFENCE,

	// FIXED BONUS
	PVP_DAMAGE_TAKEN,
	PVE_DAMAGE_TAKEN,
	PVE_DAMAGE_TAKEN_MONSTER,
	PVE_DAMAGE_TAKEN_RAID,

	// ATTACK & DEFENCE RATES
	MAGIC_CRITICAL_DAMAGE,
	SKILL_POWER_ADD,
	PHYSICAL_SKILL_POWER,
	MAGICAL_SKILL_POWER,
	PHYSICAL_SKILL_CRITICAL_DAMAGE,
	PHYSICAL_SKILL_CRITICAL_DAMAGE_ADD,
	MAGIC_CRITICAL_DAMAGE_ADD,
	SHIELD_DEFENCE_RATE,
	CRITICAL_RATE,
	CRITICAL_RATE_SKILL,
	ADD_MAX_MAGIC_CRITICAL_RATE,
	ADD_MAX_PHYSICAL_CRITICAL_RATE,
	MAGIC_CRITICAL_RATE,
	MAGIC_CRITICAL_RATE_BY_CRITICAL_RATE,
	DEFENCE_CRITICAL_RATE,
	DEFENCE_CRITICAL_RATE_ADD,
	DEFENCE_MAGIC_CRITICAL_RATE,
	DEFENCE_MAGIC_CRITICAL_RATE_ADD,
	DEFENCE_CRITICAL_DAMAGE,
	DEFENCE_MAGIC_CRITICAL_DAMAGE,
	DEFENCE_MAGIC_CRITICAL_DAMAGE_ADD,
	DEFENCE_CRITICAL_DAMAGE_ADD, // Resistance to critical damage in value (Example: +100 will be 100 more critical damage, NOT 100% more).
	DEFENCE_PHYSICAL_SKILL_CRITICAL_DAMAGE,
	DEFENCE_PHYSICAL_SKILL_CRITICAL_DAMAGE_ADD,
	DEFENCE_PHYSICAL_SKILL_CRITICAL_RATE,
	DEFENCE_PHYSICAL_SKILL_CRITICAL_RATE_ADD,
	DEFENCE_IGNORE_REMOVAL,
	DEFENCE_IGNORE_REMOVAL_ADD,
	AREA_OF_EFFECT_DAMAGE_DEFENCE,
	AREA_OF_EFFECT_DAMAGE_MODIFY,
	BLOW_RATE,
	BLOW_RATE_DEFENCE,
	INSTANT_KILL_RESIST,
	EXPSP_RATE,
	ACTIVE_BONUS_EXP, // Used to measure active skill bonus exp.
	BONUS_EXP_BUFFS, // Used to count active skill exp.
	BONUS_EXP_PASSIVES, // Used to count passive skill exp.
	BONUS_EXP_PET,
	BONUS_EXP,
	BONUS_SP,
	BONUS_DROP_ADENA,
	BONUS_DROP_AMOUNT,
	BONUS_DROP_RATE,
	BONUS_DROP_RATE_LCOIN,
	BONUS_SPOIL_RATE,
	BONUS_RAID_POINTS,
	ATTACK_CANCEL,

	// ACCURACY & RANGE
	ACCURACY_COMBAT,
	ACCURACY_MAGIC,
	EVASION_RATE,
	MAGIC_EVASION_RATE,
	PHYSICAL_ATTACK_RANGE,
	MAGIC_ATTACK_RANGE,
	ATTACK_COUNT_MAX,
	PHYSICAL_POLEARM_TARGET_SINGLE,
	WEAPON_ATTACK_ANGLE_BONUS,

	// Run speed, walk & escape speed are calculated proportionally, magic speed is a buff
	MOVE_SPEED,
	SPEED_LIMIT,
	RUN_SPEED,
	WALK_SPEED,
	SWIM_RUN_SPEED,
	SWIM_WALK_SPEED,
	FLY_RUN_SPEED,
	FLY_WALK_SPEED,

	// BASIC STATS
	STAT_STR,
	STAT_CON,
	STAT_DEX,
	STAT_INT,
	STAT_WIT,
	STAT_MEN,

	// Special stats, share one slot in Calculator

	// VARIOUS
	BREATH,
	FALL,
	FISHING_EXP_SP_BONUS,
	ENCHANT_RATE,

	// VULNERABILITIES
	DAMAGE_ZONE_VULN,
	RESIST_DISPEL_BUFF, // Resistance for cancel type skills
	RESIST_ABNORMAL_DEBUFF,

	// RESISTANCES
	FIRE_RES,
	WIND_RES,
	WATER_RES,
	EARTH_RES,
	HOLY_RES,
	DARK_RES,
	BASE_ATTRIBUTE_RES,
	MAGIC_SUCCESS_RES,

	// BUFF_IMMUNITY("buffImmunity"), // TODO: Implement me
	ABNORMAL_RESIST_PHYSICAL,
	ABNORMAL_RESIST_MAGICAL,
	REAL_DAMAGE_RESIST,

	// ELEMENT POWER
	FIRE_POWER,
	WATER_POWER,
	WIND_POWER,
	EARTH_POWER,
	HOLY_POWER,
	DARK_POWER,

	// PROFICIENCY
	REFLECT_DAMAGE_PERCENT,
	REFLECT_DAMAGE_PERCENT_DEFENSE,
	REFLECT_SKILL_MAGIC, // Need rework
	REFLECT_SKILL_PHYSIC, // Need rework
	VENGEANCE_SKILL_MAGIC_DAMAGE,
	VENGEANCE_SKILL_PHYSICAL_DAMAGE,
	ABSORB_DAMAGE_PERCENT,
	ABSORB_DAMAGE_CHANCE,
	ABSORB_DAMAGE_DEFENCE,
	TRANSFER_DAMAGE_SUMMON_PERCENT,
	MANA_SHIELD_PERCENT,
	TRANSFER_DAMAGE_TO_PLAYER,
	ABSORB_MANA_DAMAGE_PERCENT,
	ABSORB_MANA_DAMAGE_CHANCE,

	WEIGHT_LIMIT,
	WEIGHT_PENALTY,

	// ExSkill
	INVENTORY_NORMAL,
	STORAGE_PRIVATE,
	TRADE_SELL,
	TRADE_BUY,
	RECIPE_DWARVEN,
	RECIPE_COMMON,

	// Skill mastery
	SKILL_MASTERY,
	SKILL_MASTERY_RATE,

	// Vitality
	VITALITY_CONSUME_RATE,
	VITALITY_EXP_RATE,
	VITALITY_SKILLS, // Used to count vitality skill bonuses.

	// Magic Lamp
	MAGIC_LAMP_EXP_RATE,

	LAMP_BONUS_EXP,
	LAMP_BONUS_BUFFS_COUNT,

	// Henna
	HENNA_SLOTS_AVAILABLE,

	// Souls
	MAX_SOULS,

	REDUCE_EXP_LOST_BY_PVP,
	REDUCE_EXP_LOST_BY_MOB,
	REDUCE_EXP_LOST_BY_RAID,

	REDUCE_DEATH_PENALTY_BY_PVP,
	REDUCE_DEATH_PENALTY_BY_MOB,
	REDUCE_DEATH_PENALTY_BY_RAID,

	// Brooches
	BROOCH_JEWELS,

	// Agathions
	AGATHION_SLOTS,

	// Artifacts
	ARTIFACT_SLOTS,

	// Summon Points
	MAX_SUMMON_POINTS,

	// Cubic Count
	MAX_CUBIC,

	// The maximum allowed range to be damaged/debuffed from.
	SPHERIC_BARRIER_RANGE,

	// Blocks given amount of debuffs.
	DEBUFF_BLOCK,

	// Affects the random weapon damage.
	RANDOM_DAMAGE,

	// Affects the random weapon damage.
	DAMAGE_LIMIT,

	// Maximun momentum one can charge
	MAX_MOMENTUM,

	// Which base stat ordinal should alter skill critical formula.
	STAT_BONUS_SKILL_CRITICAL,
	STAT_BONUS_SPEED,
	CRAFTING_CRITICAL,
	SHOTS_BONUS,
	SOULSHOT_RESISTANCE,
	SPIRITSHOT_RESISTANCE,
	WORLD_CHAT_POINTS,
	ATTACK_DAMAGE,

	IMMOBILE_DAMAGE_BONUS,
	IMMOBILE_DAMAGE_RESIST,

	CRAFT_RATE,
	ELIXIR_USAGE_LIMIT,
	RESURRECTION_FEE_MODIFIER
}

// /**
//  * Enum of basic stats.
//  * @author mkizub
//  */
// public class StatInfo
// {
// 	// HP, MP & CP
// 	HP_LIMIT("hpLimit"),
// 	MAX_HP("maxHp", new MaxHpFinalizer()),
// 	MAX_MP("maxMp", new MaxMpFinalizer()),
// 	MAX_CP("maxCp", new MaxCpFinalizer()),
// 	MAX_RECOVERABLE_HP("maxRecoverableHp"), // The maximum HP that is able to be recovered trough heals
// 	MAX_RECOVERABLE_MP("maxRecoverableMp"),
// 	MAX_RECOVERABLE_CP("maxRecoverableCp"),
// 	REGENERATE_HP_RATE("regHp", new RegenHPFinalizer()),
// 	REGENERATE_CP_RATE("regCp", new RegenCPFinalizer()),
// 	REGENERATE_MP_RATE("regMp", new RegenMPFinalizer()),
// 	ADDITIONAL_POTION_HP("addPotionHp"),
// 	ADDITIONAL_POTION_MP("addPotionMp"),
// 	ADDITIONAL_POTION_CP("addPotionCp"),
// 	MANA_CHARGE("manaCharge"),
// 	HEAL_EFFECT("healEffect"),
// 	HEAL_EFFECT_ADD("healEffectAdd"),
// 	FEED_MODIFY("feedModify"),
// 	
// 	// ATTACK & DEFENCE
// 	PHYSICAL_DEFENCE("pDef", new PDefenseFinalizer()),
// 	MAGICAL_DEFENCE("mDef", new MDefenseFinalizer()),
// 	PHYSICAL_ATTACK("pAtk", new PAttackFinalizer()),
// 	MAGIC_ATTACK("mAtk", new MAttackFinalizer()),
// 	WEAPON_BONUS_PHYSICAL_ATTACK("weaponBonusPAtk"),
// 	WEAPON_BONUS_PHYSICAL_ATTACK_MULTIPIER("weaponBonusPAtkMultiplier"),
// 	WEAPON_BONUS_MAGIC_ATTACK("weaponBonusMAtk"),
// 	WEAPON_BONUS_MAGIC_ATTACK_MULTIPIER("weaponBonusMAtkMultiplier"),
// 	MAGIC_ATTACK_BY_PHYSICAL_ATTACK("mAtkByPAtk", Stat::defaultValue, MathUtil::add, MathUtil::mul, 0, 0),
// 	PHYSICAL_ATTACK_SPEED("pAtkSpd", new PAttackSpeedFinalizer()),
// 	MAGIC_ATTACK_SPEED("mAtkSpd", new MAttackSpeedFinalizer()), // Magic Skill Casting Time Rate
// 	ATK_REUSE("atkReuse"), // Bows Hits Reuse Rate
// 	SHIELD_DEFENCE("sDef", new ShieldDefenceFinalizer()),
// 	SHIELD_DEFENCE_IGNORE_REMOVAL("shieldDefIgnoreRemoval"),
// 	SHIELD_DEFENCE_IGNORE_REMOVAL_ADD("shieldDefIgnoreRemovalAdd"),
// 	CRITICAL_DAMAGE("cAtk"),
// 	CRITICAL_DAMAGE_ADD("cAtkAdd"), // this is another type for special critical damage mods - vicious stance, critical power and critical damage SA
// 	HATE_ATTACK("attackHate"),
// 	REAR_DAMAGE_RATE("rearDamage"),
// 	AUTO_ATTACK_DAMAGE_BONUS("autoAttackDamageBonus"),
// 	IGNORE_REDUCE_DAMAGE("ignoreReduceDamage"),
// 	
// 	// ELEMENTAL SPIRITS
// 	ELEMENTAL_SPIRIT_FIRE_ATTACK("elementalSpiritFireAttack"),
// 	ELEMENTAL_SPIRIT_WATER_ATTACK("elementalSpiritWaterAttack"),
// 	ELEMENTAL_SPIRIT_WIND_ATTACK("elementalSpiritWindAttack"),
// 	ELEMENTAL_SPIRIT_EARTH_ATTACK("elementalSpiritEarthAttack"),
// 	ELEMENTAL_SPIRIT_FIRE_DEFENSE("elementalSpiritFireDefense"),
// 	ELEMENTAL_SPIRIT_WATER_DEFENSE("elementalSpiritWaterDefense"),
// 	ELEMENTAL_SPIRIT_WIND_DEFENSE("elementalSpiritWindDefense"),
// 	ELEMENTAL_SPIRIT_EARTH_DEFENSE("elementalSpiritEarthDefense"),
// 	ELEMENTAL_SPIRIT_CRITICAL_RATE("elementalSpiritCriticalRate"),
// 	ELEMENTAL_SPIRIT_CRITICAL_DAMAGE("elementalSpiritCriticalDamage"),
// 	ELEMENTAL_SPIRIT_BONUS_EXP("elementalSpiritExp"),
// 	
// 	// PVP BONUS
// 	PVP_PHYSICAL_ATTACK_DAMAGE("pvpPhysDmg"),
// 	PVP_MAGICAL_SKILL_DAMAGE("pvpMagicalDmg"),
// 	PVP_PHYSICAL_SKILL_DAMAGE("pvpPhysSkillsDmg"),
// 	PVP_PHYSICAL_ATTACK_DEFENCE("pvpPhysDef"),
// 	PVP_MAGICAL_SKILL_DEFENCE("pvpMagicalDef"),
// 	PVP_PHYSICAL_SKILL_DEFENCE("pvpPhysSkillsDef"),
// 	
// 	// PVE BONUS
// 	PVE_PHYSICAL_ATTACK_DAMAGE("pvePhysDmg"),
// 	PVE_PHYSICAL_SKILL_DAMAGE("pvePhysSkillDmg"),
// 	PVE_MAGICAL_SKILL_DAMAGE("pveMagicalDmg"),
// 	PVE_PHYSICAL_ATTACK_DEFENCE("pvePhysDef"),
// 	PVE_PHYSICAL_SKILL_DEFENCE("pvePhysSkillDef"),
// 	PVE_MAGICAL_SKILL_DEFENCE("pveMagicalDef"),
// 	PVE_RAID_PHYSICAL_ATTACK_DAMAGE("pveRaidPhysDmg"),
// 	PVE_RAID_PHYSICAL_SKILL_DAMAGE("pveRaidPhysSkillDmg"),
// 	PVE_RAID_MAGICAL_SKILL_DAMAGE("pveRaidMagicalDmg"),
// 	PVE_RAID_PHYSICAL_ATTACK_DEFENCE("pveRaidPhysDef"),
// 	PVE_RAID_PHYSICAL_SKILL_DEFENCE("pveRaidPhysSkillDef"),
// 	PVE_RAID_MAGICAL_SKILL_DEFENCE("pveRaidMagicalDef"),
// 	
// 	// FIXED BONUS
// 	PVP_DAMAGE_TAKEN("pvpDamageTaken"),
// 	PVE_DAMAGE_TAKEN("pveDamageTaken"),
// 	PVE_DAMAGE_TAKEN_MONSTER("pveDamageTakenMonster"),
// 	PVE_DAMAGE_TAKEN_RAID("pveDamageTakenRaid"),
// 	
// 	// ATTACK & DEFENCE RATES
// 	MAGIC_CRITICAL_DAMAGE("mCritPower"),
// 	SKILL_POWER_ADD("skillPowerAdd"),
// 	PHYSICAL_SKILL_POWER("physicalSkillPower"),
// 	MAGICAL_SKILL_POWER("magicalSkillPower"),
// 	PHYSICAL_SKILL_CRITICAL_DAMAGE("cAtkSkill"),
// 	PHYSICAL_SKILL_CRITICAL_DAMAGE_ADD("cAtkSkillAdd"),
// 	MAGIC_CRITICAL_DAMAGE_ADD("mCritPowerAdd"),
// 	SHIELD_DEFENCE_RATE("rShld", new ShieldDefenceRateFinalizer()),
// 	CRITICAL_RATE("rCrit", new PCriticalRateFinalizer(), MathUtil::add, MathUtil::add, 0, 1),
// 	CRITICAL_RATE_SKILL("physicalSkillCriticalRate"),
// 	ADD_MAX_MAGIC_CRITICAL_RATE("addMaxMagicCritRate"),
// 	ADD_MAX_PHYSICAL_CRITICAL_RATE("addMaxPhysicalCritRate"),
// 	MAGIC_CRITICAL_RATE("mCritRate", new MCritRateFinalizer()),
// 	MAGIC_CRITICAL_RATE_BY_CRITICAL_RATE("mCritRateByRCrit", Stat::defaultValue, MathUtil::add, MathUtil::mul, 0, 0),
// 	DEFENCE_CRITICAL_RATE("defCritRate"),
// 	DEFENCE_CRITICAL_RATE_ADD("defCritRateAdd"),
// 	DEFENCE_MAGIC_CRITICAL_RATE("defMCritRate"),
// 	DEFENCE_MAGIC_CRITICAL_RATE_ADD("defMCritRateAdd"),
// 	DEFENCE_CRITICAL_DAMAGE("defCritDamage"),
// 	DEFENCE_MAGIC_CRITICAL_DAMAGE("defMCritDamage"),
// 	DEFENCE_MAGIC_CRITICAL_DAMAGE_ADD("defMCritDamageAdd"),
// 	DEFENCE_CRITICAL_DAMAGE_ADD("defCritDamageAdd"), // Resistance to critical damage in value (Example: +100 will be 100 more critical damage, NOT 100% more).
// 	DEFENCE_PHYSICAL_SKILL_CRITICAL_DAMAGE("defCAtkSkill"),
// 	DEFENCE_PHYSICAL_SKILL_CRITICAL_DAMAGE_ADD("defCAtkSkillAdd"),
// 	DEFENCE_PHYSICAL_SKILL_CRITICAL_RATE("defPhysSkillCritRate"),
// 	DEFENCE_PHYSICAL_SKILL_CRITICAL_RATE_ADD("defPhysSkillCritRateAdd"),
// 	DEFENCE_IGNORE_REMOVAL("defIgnoreRemoval"),
// 	DEFENCE_IGNORE_REMOVAL_ADD("defIgnoreRemovalAdd"),
// 	AREA_OF_EFFECT_DAMAGE_DEFENCE("aoeDamageDefence"),
// 	AREA_OF_EFFECT_DAMAGE_MODIFY("aoeDamageModify"),
// 	BLOW_RATE("blowRate"),
// 	BLOW_RATE_DEFENCE("blowRateDefence"),
// 	INSTANT_KILL_RESIST("instantKillResist"),
// 	EXPSP_RATE("rExp"),
// 	ACTIVE_BONUS_EXP("activeBonusExp"), // Used to measure active skill bonus exp.
// 	BONUS_EXP_BUFFS("bonusExpBuffs"), // Used to count active skill exp.
// 	BONUS_EXP_PASSIVES("bonusExpPassives"), // Used to count passive skill exp.
// 	BONUS_EXP_PET("bonusExpPet"),
// 	BONUS_EXP("bonusExp"),
// 	BONUS_SP("bonusSp"),
// 	BONUS_DROP_ADENA("bonusDropAdena"),
// 	BONUS_DROP_AMOUNT("bonusDropAmount"),
// 	BONUS_DROP_RATE("bonusDropRate"),
// 	BONUS_DROP_RATE_LCOIN("bonusDropRateLCoin"),
// 	BONUS_SPOIL_RATE("bonusSpoilRate"),
// 	BONUS_RAID_POINTS("bonusRaidPoints"),
// 	ATTACK_CANCEL("cancel"),
// 	
// 	// ACCURACY & RANGE
// 	ACCURACY_COMBAT("accCombat", new PAccuracyFinalizer()),
// 	ACCURACY_MAGIC("accMagic", new MAccuracyFinalizer()),
// 	EVASION_RATE("rEvas", new PEvasionRateFinalizer()),
// 	MAGIC_EVASION_RATE("mEvas", new MEvasionRateFinalizer()),
// 	PHYSICAL_ATTACK_RANGE("pAtkRange", new PRangeFinalizer()),
// 	MAGIC_ATTACK_RANGE("mAtkRange"),
// 	ATTACK_COUNT_MAX("atkCountMax"),
// 	PHYSICAL_POLEARM_TARGET_SINGLE("polearmSingleTarget"),
// 	WEAPON_ATTACK_ANGLE_BONUS("weaponAttackAngleBonus"),
// 	
// 	// Run speed, walk & escape speed are calculated proportionally, magic speed is a buff
// 	MOVE_SPEED("moveSpeed"),
// 	SPEED_LIMIT("speedLimit"),
// 	RUN_SPEED("runSpd", new SpeedFinalizer()),
// 	WALK_SPEED("walkSpd", new SpeedFinalizer()),
// 	SWIM_RUN_SPEED("fastSwimSpd", new SpeedFinalizer()),
// 	SWIM_WALK_SPEED("slowSimSpd", new SpeedFinalizer()),
// 	FLY_RUN_SPEED("fastFlySpd", new SpeedFinalizer()),
// 	FLY_WALK_SPEED("slowFlySpd", new SpeedFinalizer()),
// 	
// 	// BASIC STATS
// 	STAT_STR("STR", new BaseStatFinalizer()),
// 	STAT_CON("CON", new BaseStatFinalizer()),
// 	STAT_DEX("DEX", new BaseStatFinalizer()),
// 	STAT_INT("INT", new BaseStatFinalizer()),
// 	STAT_WIT("WIT", new BaseStatFinalizer()),
// 	STAT_MEN("MEN", new BaseStatFinalizer()),
// 	
// 	// Special stats, share one slot in Calculator
// 	
// 	// VARIOUS
// 	BREATH("breath"),
// 	FALL("fall"),
// 	FISHING_EXP_SP_BONUS("fishingExpSpBonus"),
// 	ENCHANT_RATE("enchantRate"),
// 	
// 	// VULNERABILITIES
// 	DAMAGE_ZONE_VULN("damageZoneVuln"),
// 	RESIST_DISPEL_BUFF("cancelVuln"), // Resistance for cancel type skills
// 	RESIST_ABNORMAL_DEBUFF("debuffVuln"),
// 	
// 	// RESISTANCES
// 	FIRE_RES("fireRes", new AttributeFinalizer(AttributeType.FIRE, false)),
// 	WIND_RES("windRes", new AttributeFinalizer(AttributeType.WIND, false)),
// 	WATER_RES("waterRes", new AttributeFinalizer(AttributeType.WATER, false)),
// 	EARTH_RES("earthRes", new AttributeFinalizer(AttributeType.EARTH, false)),
// 	HOLY_RES("holyRes", new AttributeFinalizer(AttributeType.HOLY, false)),
// 	DARK_RES("darkRes", new AttributeFinalizer(AttributeType.DARK, false)),
// 	BASE_ATTRIBUTE_RES("baseAttrRes"),
// 	MAGIC_SUCCESS_RES("magicSuccRes"),
// 	// BUFF_IMMUNITY("buffImmunity"), // TODO: Implement me
// 	ABNORMAL_RESIST_PHYSICAL("abnormalResPhysical"),
// 	ABNORMAL_RESIST_MAGICAL("abnormalResMagical"),
// 	REAL_DAMAGE_RESIST("realDamageResist"),
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
// 	REFLECT_DAMAGE_PERCENT("reflectDam"),
// 	REFLECT_DAMAGE_PERCENT_DEFENSE("reflectDamDef"),
// 	REFLECT_SKILL_MAGIC("reflectSkillMagic"), // Need rework
// 	REFLECT_SKILL_PHYSIC("reflectSkillPhysic"), // Need rework
// 	VENGEANCE_SKILL_MAGIC_DAMAGE("vengeanceMdam"),
// 	VENGEANCE_SKILL_PHYSICAL_DAMAGE("vengeancePdam"),
// 	ABSORB_DAMAGE_PERCENT("absorbDam"),
// 	ABSORB_DAMAGE_CHANCE("absorbDamChance", new VampiricChanceFinalizer()),
// 	ABSORB_DAMAGE_DEFENCE("absorbDamDefence"),
// 	TRANSFER_DAMAGE_SUMMON_PERCENT("transDam"),
// 	MANA_SHIELD_PERCENT("manaShield"),
// 	TRANSFER_DAMAGE_TO_PLAYER("transDamToPlayer"),
// 	ABSORB_MANA_DAMAGE_PERCENT("absorbDamMana"),
// 	ABSORB_MANA_DAMAGE_CHANCE("absorbDamManaChance", new MpVampiricChanceFinalizer()),
// 	
// 	WEIGHT_LIMIT("weightLimit"),
// 	WEIGHT_PENALTY("weightPenalty"),
// 	
// 	// ExSkill
// 	INVENTORY_NORMAL("inventoryLimit"),
// 	STORAGE_PRIVATE("whLimit"),
// 	TRADE_SELL("PrivateSellLimit"),
// 	TRADE_BUY("PrivateBuyLimit"),
// 	RECIPE_DWARVEN("DwarfRecipeLimit"),
// 	RECIPE_COMMON("CommonRecipeLimit"),
// 	
// 	// Skill mastery
// 	SKILL_MASTERY("skillMastery"),
// 	SKILL_MASTERY_RATE("skillMasteryRate"),
// 	
// 	// Vitality
// 	VITALITY_CONSUME_RATE("vitalityConsumeRate"),
// 	VITALITY_EXP_RATE("vitalityExpRate"),
// 	VITALITY_SKILLS("vitalitySkills"), // Used to count vitality skill bonuses.
// 	
// 	// Magic Lamp
// 	MAGIC_LAMP_EXP_RATE("magicLampExpRate"),
// 	
// 	LAMP_BONUS_EXP("LampBonusExp"),
// 	LAMP_BONUS_BUFFS_COUNT("LampBonusBuffCount"),
// 	
// 	// Henna
// 	HENNA_SLOTS_AVAILABLE("hennaSlots"),
// 	
// 	// Souls
// 	MAX_SOULS("maxSouls"),
// 	
// 	REDUCE_EXP_LOST_BY_PVP("reduceExpLostByPvp"),
// 	REDUCE_EXP_LOST_BY_MOB("reduceExpLostByMob"),
// 	REDUCE_EXP_LOST_BY_RAID("reduceExpLostByRaid"),
// 	
// 	REDUCE_DEATH_PENALTY_BY_PVP("reduceDeathPenaltyByPvp"),
// 	REDUCE_DEATH_PENALTY_BY_MOB("reduceDeathPenaltyByMob"),
// 	REDUCE_DEATH_PENALTY_BY_RAID("reduceDeathPenaltyByRaid"),
// 	
// 	// Brooches
// 	BROOCH_JEWELS("broochJewels"),
// 	
// 	// Agathions
// 	AGATHION_SLOTS("agathionSlots"),
// 	
// 	// Artifacts
// 	ARTIFACT_SLOTS("artifactSlots"),
// 	
// 	// Summon Points
// 	MAX_SUMMON_POINTS("summonPoints"),
// 	
// 	// Cubic Count
// 	MAX_CUBIC("cubicCount"),
// 	
// 	// The maximum allowed range to be damaged/debuffed from.
// 	SPHERIC_BARRIER_RANGE("sphericBarrier"),
// 	
// 	// Blocks given amount of debuffs.
// 	DEBUFF_BLOCK("debuffBlock"),
// 	
// 	// Affects the random weapon damage.
// 	RANDOM_DAMAGE("randomDamage", new RandomDamageFinalizer()),
// 	
// 	// Affects the random weapon damage.
// 	DAMAGE_LIMIT("damageCap"),
// 	
// 	// Maximun momentum one can charge
// 	MAX_MOMENTUM("maxMomentum"),
// 	
// 	// Which base stat ordinal should alter skill critical formula.
// 	STAT_BONUS_SKILL_CRITICAL("statSkillCritical"),
// 	STAT_BONUS_SPEED("statSpeed"),
// 	CRAFTING_CRITICAL("craftingCritical"),
// 	SHOTS_BONUS("shotBonus", new ShotsBonusFinalizer()),
// 	SOULSHOT_RESISTANCE("soulshotResistance"),
// 	SPIRITSHOT_RESISTANCE("spiritshotResistance"),
// 	WORLD_CHAT_POINTS("worldChatPoints"),
// 	ATTACK_DAMAGE("attackDamage"),
// 	
// 	IMMOBILE_DAMAGE_BONUS("immobileBonus"),
// 	IMMOBILE_DAMAGE_RESIST("immobileResist"),
// 	
// 	CRAFT_RATE("CraftRate"),
// 	ELIXIR_USAGE_LIMIT("elixirUsageLimit"),
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
