using System.Xml.Serialization;

namespace L2Dn.Model.Enums;

/// <summary>
/// Enum of basic stats.
/// </summary>
public enum Stat
{
    // HP, MP & CP
    [XmlEnum("hpLimit")]
    HP_LIMIT,

    [XmlEnum("maxHp")]
    MAX_HP,

    [XmlEnum("maxMp")]
    MAX_MP,

    [XmlEnum("maxCp")]
    MAX_CP,

    [XmlEnum("maxRecoverableHp")]
    MAX_RECOVERABLE_HP,

    [XmlEnum("maxRecoverableMp")]
    MAX_RECOVERABLE_MP,

    [XmlEnum("maxRecoverableCp")]
    MAX_RECOVERABLE_CP,

    [XmlEnum("regHp")]
    REGENERATE_HP_RATE,

    [XmlEnum("regCp")]
    REGENERATE_CP_RATE,

    [XmlEnum("regMp")]
    REGENERATE_MP_RATE,

    [XmlEnum("addPotionHp")]
    ADDITIONAL_POTION_HP,

    [XmlEnum("addPotionMp")]
    ADDITIONAL_POTION_MP,

    [XmlEnum("addPotionCp")]
    ADDITIONAL_POTION_CP,

    [XmlEnum("manaCharge")]
    MANA_CHARGE,

    [XmlEnum("healEffect")]
    HEAL_EFFECT,

    [XmlEnum("healEffectAdd")]
    HEAL_EFFECT_ADD,

    [XmlEnum("feedModify")]
    FEED_MODIFY,

    // ATTACK & DEFENCE
    [XmlEnum("pDef")]
    PHYSICAL_DEFENCE,

    [XmlEnum("mDef")]
    MAGICAL_DEFENCE,

    [XmlEnum("pAtk")]
    PHYSICAL_ATTACK,

    [XmlEnum("mAtk")]
    MAGIC_ATTACK,

    [XmlEnum("weaponBonusPAtk")]
    WEAPON_BONUS_PHYSICAL_ATTACK,

    [XmlEnum("weaponBonusPAtkMultiplier")]
    WEAPON_BONUS_PHYSICAL_ATTACK_MULTIPIER,

    [XmlEnum("weaponBonusMAtk")]
    WEAPON_BONUS_MAGIC_ATTACK,

    [XmlEnum("weaponBonusMAtkMultiplier")]
    WEAPON_BONUS_MAGIC_ATTACK_MULTIPIER,

    [XmlEnum("mAtkByPAtk")]
    MAGIC_ATTACK_BY_PHYSICAL_ATTACK,

    [XmlEnum("pAtkSpd")]
    PHYSICAL_ATTACK_SPEED,

    [XmlEnum("mAtkSpd")]
    MAGIC_ATTACK_SPEED, // Magic Skill Casting Time Rate

    [XmlEnum("atkReuse")]
    ATK_REUSE, // Bows Hits Reuse Rate

    [XmlEnum("sDef")]
    SHIELD_DEFENCE,

    [XmlEnum("shieldDefIgnoreRemoval")]
    SHIELD_DEFENCE_IGNORE_REMOVAL,

    [XmlEnum("shieldDefIgnoreRemovalAdd")]
    SHIELD_DEFENCE_IGNORE_REMOVAL_ADD,

    [XmlEnum("cAtk")]
    CRITICAL_DAMAGE,

    [XmlEnum("cAtkAdd")]
    CRITICAL_DAMAGE_ADD, // this is another type for special critical damage mods - vicious stance, critical power and critical damage SA

    [XmlEnum("attackHate")]
    HATE_ATTACK,

    [XmlEnum("rearDamage")]
    REAR_DAMAGE_RATE,

    [XmlEnum("autoAttackDamageBonus")]
    AUTO_ATTACK_DAMAGE_BONUS,

    [XmlEnum("ignoreReduceDamage")]
    IGNORE_REDUCE_DAMAGE,

    // ELEMENTAL SPIRITS
    [XmlEnum("elementalSpiritFireAttack")]
    ELEMENTAL_SPIRIT_FIRE_ATTACK,

    [XmlEnum("elementalSpiritWaterAttack")]
    ELEMENTAL_SPIRIT_WATER_ATTACK,

    [XmlEnum("elementalSpiritWindAttack")]
    ELEMENTAL_SPIRIT_WIND_ATTACK,

    [XmlEnum("elementalSpiritEarthAttack")]
    ELEMENTAL_SPIRIT_EARTH_ATTACK,

    [XmlEnum("elementalSpiritFireDefense")]
    ELEMENTAL_SPIRIT_FIRE_DEFENSE,

    [XmlEnum("elementalSpiritWaterDefense")]
    ELEMENTAL_SPIRIT_WATER_DEFENSE,

    [XmlEnum("elementalSpiritWindDefense")]
    ELEMENTAL_SPIRIT_WIND_DEFENSE,

    [XmlEnum("elementalSpiritEarthDefense")]
    ELEMENTAL_SPIRIT_EARTH_DEFENSE,

    [XmlEnum("elementalSpiritCriticalRate")]
    ELEMENTAL_SPIRIT_CRITICAL_RATE,

    [XmlEnum("elementalSpiritCriticalDamage")]
    ELEMENTAL_SPIRIT_CRITICAL_DAMAGE,

    [XmlEnum("elementalSpiritExp")]
    ELEMENTAL_SPIRIT_BONUS_EXP,

    // PVP BONUS
    [XmlEnum("pvpPhysDmg")]
    PVP_PHYSICAL_ATTACK_DAMAGE,

    [XmlEnum("pvpMagicalDmg")]
    PVP_MAGICAL_SKILL_DAMAGE,

    [XmlEnum("pvpPhysSkillsDmg")]
    PVP_PHYSICAL_SKILL_DAMAGE,

    [XmlEnum("pvpPhysDef")]
    PVP_PHYSICAL_ATTACK_DEFENCE,

    [XmlEnum("pvpMagicalDef")]
    PVP_MAGICAL_SKILL_DEFENCE,

    [XmlEnum("pvpPhysSkillsDef")]
    PVP_PHYSICAL_SKILL_DEFENCE,

    // PVE BONUS
    [XmlEnum("pvePhysDmg")]
    PVE_PHYSICAL_ATTACK_DAMAGE,

    [XmlEnum("pvePhysSkillDmg")]
    PVE_PHYSICAL_SKILL_DAMAGE,

    [XmlEnum("pveMagicalDmg")]
    PVE_MAGICAL_SKILL_DAMAGE,

    [XmlEnum("pvePhysDef")]
    PVE_PHYSICAL_ATTACK_DEFENCE,

    [XmlEnum("pvePhysSkillDef")]
    PVE_PHYSICAL_SKILL_DEFENCE,

    [XmlEnum("pveMagicalDef")]
    PVE_MAGICAL_SKILL_DEFENCE,

    [XmlEnum("pveRaidPhysDmg")]
    PVE_RAID_PHYSICAL_ATTACK_DAMAGE,

    [XmlEnum("pveRaidPhysSkillDmg")]
    PVE_RAID_PHYSICAL_SKILL_DAMAGE,

    [XmlEnum("pveRaidMagicalDmg")]
    PVE_RAID_MAGICAL_SKILL_DAMAGE,

    [XmlEnum("pveRaidPhysDef")]
    PVE_RAID_PHYSICAL_ATTACK_DEFENCE,

    [XmlEnum("pveRaidPhysSkillDef")]
    PVE_RAID_PHYSICAL_SKILL_DEFENCE,

    [XmlEnum("pveRaidMagicalDef")]
    PVE_RAID_MAGICAL_SKILL_DEFENCE,

    // FIXED BONUS
    [XmlEnum("pvpDamageTaken")]
    PVP_DAMAGE_TAKEN,

    [XmlEnum("pveDamageTaken")]
    PVE_DAMAGE_TAKEN,

    [XmlEnum("pveDamageTakenMonster")]
    PVE_DAMAGE_TAKEN_MONSTER,

    [XmlEnum("pveDamageTakenRaid")]
    PVE_DAMAGE_TAKEN_RAID,

    // ATTACK & DEFENCE RATES
    [XmlEnum("mCritPower")]
    MAGIC_CRITICAL_DAMAGE,

    [XmlEnum("skillPowerAdd")]
    SKILL_POWER_ADD,

    [XmlEnum("physicalSkillPower")]
    PHYSICAL_SKILL_POWER,

    [XmlEnum("magicalSkillPower")]
    MAGICAL_SKILL_POWER,

    [XmlEnum("cAtkSkill")]
    PHYSICAL_SKILL_CRITICAL_DAMAGE,

    [XmlEnum("cAtkSkillAdd")]
    PHYSICAL_SKILL_CRITICAL_DAMAGE_ADD,

    [XmlEnum("mCritPowerAdd")]
    MAGIC_CRITICAL_DAMAGE_ADD,

    [XmlEnum("rShld")]
    SHIELD_DEFENCE_RATE,

    [XmlEnum("rCrit")]
    CRITICAL_RATE,

    [XmlEnum("physicalSkillCriticalRate")]
    CRITICAL_RATE_SKILL,

    [XmlEnum("addMaxMagicCritRate")]
    ADD_MAX_MAGIC_CRITICAL_RATE,

    [XmlEnum("addMaxPhysicalCritRate")]
    ADD_MAX_PHYSICAL_CRITICAL_RATE,

    [XmlEnum("mCritRate")]
    MAGIC_CRITICAL_RATE,

    [XmlEnum("mCritRateByRCrit")]
    MAGIC_CRITICAL_RATE_BY_CRITICAL_RATE,

    [XmlEnum("defCritRate")]
    DEFENCE_CRITICAL_RATE,

    [XmlEnum("defCritRateAdd")]
    DEFENCE_CRITICAL_RATE_ADD,

    [XmlEnum("defMCritRate")]
    DEFENCE_MAGIC_CRITICAL_RATE,

    [XmlEnum("defMCritRateAdd")]
    DEFENCE_MAGIC_CRITICAL_RATE_ADD,

    [XmlEnum("defCritDamage")]
    DEFENCE_CRITICAL_DAMAGE,

    [XmlEnum("defMCritDamage")]
    DEFENCE_MAGIC_CRITICAL_DAMAGE,

    [XmlEnum("defMCritDamageAdd")]
    DEFENCE_MAGIC_CRITICAL_DAMAGE_ADD,

    [XmlEnum("defCritDamageAdd")]
    DEFENCE_CRITICAL_DAMAGE_ADD, // Resistance to critical damage in value (Example: +100 will be 100 more critical damage, NOT 100% more).

    [XmlEnum("defCAtkSkill")]
    DEFENCE_PHYSICAL_SKILL_CRITICAL_DAMAGE,

    [XmlEnum("defCAtkSkillAdd")]
    DEFENCE_PHYSICAL_SKILL_CRITICAL_DAMAGE_ADD,

    [XmlEnum("defPhysSkillCritRate")]
    DEFENCE_PHYSICAL_SKILL_CRITICAL_RATE,

    [XmlEnum("defPhysSkillCritRateAdd")]
    DEFENCE_PHYSICAL_SKILL_CRITICAL_RATE_ADD,

    [XmlEnum("defIgnoreRemoval")]
    DEFENCE_IGNORE_REMOVAL,

    [XmlEnum("defIgnoreRemovalAdd")]
    DEFENCE_IGNORE_REMOVAL_ADD,

    [XmlEnum("aoeDamageDefence")]
    AREA_OF_EFFECT_DAMAGE_DEFENCE,

    [XmlEnum("aoeDamageModify")]
    AREA_OF_EFFECT_DAMAGE_MODIFY,

    [XmlEnum("blowRate")]
    BLOW_RATE,

    [XmlEnum("blowRateDefence")]
    BLOW_RATE_DEFENCE,

    [XmlEnum("instantKillResist")]
    INSTANT_KILL_RESIST,

    [XmlEnum("rExp")]
    EXPSP_RATE,

    [XmlEnum("activeBonusExp")]
    ACTIVE_BONUS_EXP, // Used to measure active skill bonus exp.

    [XmlEnum("bonusExpBuffs")]
    BONUS_EXP_BUFFS, // Used to count active skill exp.

    [XmlEnum("bonusExpPassives")]
    BONUS_EXP_PASSIVES, // Used to count passive skill exp.

    [XmlEnum("bonusExpPet")]
    BONUS_EXP_PET,

    [XmlEnum("bonusExp")]
    BONUS_EXP,

    [XmlEnum("bonusSp")]
    BONUS_SP,

    [XmlEnum("bonusDropAdena")]
    BONUS_DROP_ADENA,

    [XmlEnum("bonusDropAmount")]
    BONUS_DROP_AMOUNT,

    [XmlEnum("bonusDropRate")]
    BONUS_DROP_RATE,

    [XmlEnum("bonusDropRateLCoin")]
    BONUS_DROP_RATE_LCOIN,

    [XmlEnum("bonusSpoilRate")]
    BONUS_SPOIL_RATE,

    [XmlEnum("bonusRaidPoints")]
    BONUS_RAID_POINTS,

    [XmlEnum("cancel")]
    ATTACK_CANCEL,

    // ACCURACY & RANGE
    [XmlEnum("accCombat")]
    ACCURACY_COMBAT,

    [XmlEnum("accMagic")]
    ACCURACY_MAGIC,

    [XmlEnum("rEvas")]
    EVASION_RATE,

    [XmlEnum("mEvas")]
    MAGIC_EVASION_RATE,

    [XmlEnum("pAtkRange")]
    PHYSICAL_ATTACK_RANGE,

    [XmlEnum("mAtkRange")]
    MAGIC_ATTACK_RANGE,

    [XmlEnum("atkCountMax")]
    ATTACK_COUNT_MAX,

    [XmlEnum("polearmSingleTarget")]
    PHYSICAL_POLEARM_TARGET_SINGLE,

    [XmlEnum("weaponAttackAngleBonus")]
    WEAPON_ATTACK_ANGLE_BONUS,

    // Run speed, walk & escape speed are calculated proportionally, magic speed is a buff
    [XmlEnum("moveSpeed")]
    MOVE_SPEED,

    [XmlEnum("speedLimit")]
    SPEED_LIMIT,

    [XmlEnum("runSpd")]
    RUN_SPEED,

    [XmlEnum("walkSpd")]
    WALK_SPEED,

    [XmlEnum("fastSwimSpd")]
    SWIM_RUN_SPEED,

    [XmlEnum("slowSimSpd")]
    SWIM_WALK_SPEED,

    [XmlEnum("fastFlySpd")]
    FLY_RUN_SPEED,

    [XmlEnum("slowFlySpd")]
    FLY_WALK_SPEED,

    // BASIC STATS
    [XmlEnum("STR")]
    STAT_STR,

    [XmlEnum("CON")]
    STAT_CON,

    [XmlEnum("DEX")]
    STAT_DEX,

    [XmlEnum("INT")]
    STAT_INT,

    [XmlEnum("WIT")]
    STAT_WIT,

    [XmlEnum("MEN")]
    STAT_MEN,

    // Special stats, share one slot in Calculator

    // VARIOUS
    [XmlEnum("breath")]
    BREATH,

    [XmlEnum("fall")]
    FALL,

    [XmlEnum("fishingExpSpBonus")]
    FISHING_EXP_SP_BONUS,

    [XmlEnum("enchantRate")]
    ENCHANT_RATE,

    // VULNERABILITIES
    [XmlEnum("damageZoneVuln")]
    DAMAGE_ZONE_VULN,

    [XmlEnum("cancelVuln")]
    RESIST_DISPEL_BUFF, // Resistance for cancel type skills

    [XmlEnum("debuffVuln")]
    RESIST_ABNORMAL_DEBUFF,

    // RESISTANCES
    [XmlEnum("fireRes")]
    FIRE_RES,

    [XmlEnum("windRes")]
    WIND_RES,

    [XmlEnum("waterRes")]
    WATER_RES,

    [XmlEnum("earthRes")]
    EARTH_RES,

    [XmlEnum("holyRes")]
    HOLY_RES,

    [XmlEnum("darkRes")]
    DARK_RES,

    [XmlEnum("baseAttrRes")]
    BASE_ATTRIBUTE_RES,

    [XmlEnum("magicSuccRes")]
    MAGIC_SUCCESS_RES,

    [XmlEnum("buffImmunity")]
    BUFF_IMMUNITY, // TODO: Implement me

    [XmlEnum("abnormalResPhysical")]
    ABNORMAL_RESIST_PHYSICAL,

    [XmlEnum("abnormalResMagical")]
    ABNORMAL_RESIST_MAGICAL,

    [XmlEnum("realDamageResist")]
    REAL_DAMAGE_RESIST,

    // ELEMENT POWER
    [XmlEnum("firePower")]
    FIRE_POWER,

    [XmlEnum("waterPower")]
    WATER_POWER,

    [XmlEnum("windPower")]
    WIND_POWER,

    [XmlEnum("earthPower")]
    EARTH_POWER,

    [XmlEnum("holyPower")]
    HOLY_POWER,

    [XmlEnum("darkPower")]
    DARK_POWER,

    // PROFICIENCY
    [XmlEnum("reflectDam")]
    REFLECT_DAMAGE_PERCENT,

    [XmlEnum("reflectDamDef")]
    REFLECT_DAMAGE_PERCENT_DEFENSE,

    [XmlEnum("reflectSkillMagic")]
    REFLECT_SKILL_MAGIC, // Need rework

    [XmlEnum("reflectSkillPhysic")]
    REFLECT_SKILL_PHYSIC, // Need rework

    [XmlEnum("vengeanceMdam")]
    VENGEANCE_SKILL_MAGIC_DAMAGE,

    [XmlEnum("vengeancePdam")]
    VENGEANCE_SKILL_PHYSICAL_DAMAGE,

    [XmlEnum("absorbDam")]
    ABSORB_DAMAGE_PERCENT,

    [XmlEnum("absorbDamChance")]
    ABSORB_DAMAGE_CHANCE,

    [XmlEnum("absorbDamDefence")]
    ABSORB_DAMAGE_DEFENCE,

    [XmlEnum("transDam")]
    TRANSFER_DAMAGE_SUMMON_PERCENT,

    [XmlEnum("manaShield")]
    MANA_SHIELD_PERCENT,

    [XmlEnum("transDamToPlayer")]
    TRANSFER_DAMAGE_TO_PLAYER,

    [XmlEnum("absorbDamMana")]
    ABSORB_MANA_DAMAGE_PERCENT,

    [XmlEnum("absorbDamManaChance")]
    ABSORB_MANA_DAMAGE_CHANCE,

    [XmlEnum("weightLimit")]
    WEIGHT_LIMIT,

    [XmlEnum("weightPenalty")]
    WEIGHT_PENALTY,

    // ExSkill
    [XmlEnum("inventoryLimit")]
    INVENTORY_NORMAL,

    [XmlEnum("whLimit")]
    STORAGE_PRIVATE,

    [XmlEnum("PrivateSellLimit")]
    TRADE_SELL,

    [XmlEnum("PrivateBuyLimit")]
    TRADE_BUY,

    [XmlEnum("DwarfRecipeLimit")]
    RECIPE_DWARVEN,

    [XmlEnum("CommonRecipeLimit")]
    RECIPE_COMMON,

    // Skill mastery
    [XmlEnum("skillMastery")]
    SKILL_MASTERY,

    [XmlEnum("skillMasteryRate")]
    SKILL_MASTERY_RATE,

    // Vitality
    [XmlEnum("vitalityConsumeRate")]
    VITALITY_CONSUME_RATE,

    [XmlEnum("vitalityExpRate")]
    VITALITY_EXP_RATE,

    [XmlEnum("vitalitySkills")]
    VITALITY_SKILLS, // Used to count vitality skill bonuses.

    // Magic Lamp
    [XmlEnum("magicLampExpRate")]
    MAGIC_LAMP_EXP_RATE,

    [XmlEnum("LampBonusExp")]
    LAMP_BONUS_EXP,

    [XmlEnum("LampBonusBuffCount")]
    LAMP_BONUS_BUFFS_COUNT,

    // Henna
    [XmlEnum("hennaSlots")]
    HENNA_SLOTS_AVAILABLE,

    // Souls
    [XmlEnum("maxSouls")]
    MAX_SOULS,

    [XmlEnum("reduceExpLostByPvp")]
    REDUCE_EXP_LOST_BY_PVP,

    [XmlEnum("reduceExpLostByMob")]
    REDUCE_EXP_LOST_BY_MOB,

    [XmlEnum("reduceExpLostByRaid")]
    REDUCE_EXP_LOST_BY_RAID,

    [XmlEnum("reduceDeathPenaltyByPvp")]
    REDUCE_DEATH_PENALTY_BY_PVP,

    [XmlEnum("reduceDeathPenaltyByMob")]
    REDUCE_DEATH_PENALTY_BY_MOB,

    [XmlEnum("reduceDeathPenaltyByRaid")]
    REDUCE_DEATH_PENALTY_BY_RAID,

    // Brooches
    [XmlEnum("broochJewels")]
    BROOCH_JEWELS,

    // Agathions
    [XmlEnum("agathionSlots")]
    AGATHION_SLOTS,

    // Artifacts
    [XmlEnum("artifactSlots")]
    ARTIFACT_SLOTS,

    // Summon Points
    [XmlEnum("summonPoints")]
    MAX_SUMMON_POINTS,

    // Cubic Count
    [XmlEnum("cubicCount")]
    MAX_CUBIC,

    // The maximum allowed range to be damaged/debuffed from.
    [XmlEnum("sphericBarrier")]
    SPHERIC_BARRIER_RANGE,

    // Blocks given amount of debuffs.
    [XmlEnum("debuffBlock")]
    DEBUFF_BLOCK,

    // Affects the random weapon damage.
    [XmlEnum("randomDamage")]
    RANDOM_DAMAGE,

    // Affects the random weapon damage.
    [XmlEnum("damageCap")]
    DAMAGE_LIMIT,

    // Maximun momentum one can charge
    [XmlEnum("maxMomentum")]
    MAX_MOMENTUM,

    // Which base stat ordinal should alter skill critical formula.
    [XmlEnum("statSkillCritical")]
    STAT_BONUS_SKILL_CRITICAL,

    [XmlEnum("statSpeed")]
    STAT_BONUS_SPEED,

    [XmlEnum("craftingCritical")]
    CRAFTING_CRITICAL,

    [XmlEnum("shotBonus")]
    SHOTS_BONUS,

    [XmlEnum("soulshotResistance")]
    SOULSHOT_RESISTANCE,

    [XmlEnum("spiritshotResistance")]
    SPIRITSHOT_RESISTANCE,

    [XmlEnum("worldChatPoints")]
    WORLD_CHAT_POINTS,

    [XmlEnum("attackDamage")]
    ATTACK_DAMAGE,

    [XmlEnum("immobileBonus")]
    IMMOBILE_DAMAGE_BONUS,

    [XmlEnum("immobileResist")]
    IMMOBILE_DAMAGE_RESIST,

    [XmlEnum("CraftRate")]
    CRAFT_RATE,

    [XmlEnum("elixirUsageLimit")]
    ELIXIR_USAGE_LIMIT,

    [XmlEnum("resurrectionFeeModifier")]
    RESURRECTION_FEE_MODIFIER,
}