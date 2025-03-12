using System.Xml.Serialization;

namespace L2Dn.Model.Xml.Skills;

public enum XmlSkillEffectParameterType
{
    [XmlEnum(XmlSkillEffectParameterNames.AirBind)]
    AirBind,

    [XmlEnum(XmlSkillEffectParameterNames.AncientSword)]
    AncientSword,

    [XmlEnum(XmlSkillEffectParameterNames.AnimalWeakness)]
    AnimalWeakness,

    [XmlEnum(XmlSkillEffectParameterNames.Anomaly)]
    Anomaly,

    [XmlEnum(XmlSkillEffectParameterNames.BeastWeakness)]
    BeastWeakness,

    [XmlEnum(XmlSkillEffectParameterNames.Bleed)]
    Bleed,

    [XmlEnum(XmlSkillEffectParameterNames.Bluff)]
    Bluff,

    [XmlEnum(XmlSkillEffectParameterNames.Blunt)]
    Blunt,

    [XmlEnum(XmlSkillEffectParameterNames.Boss)]
    Boss,

    [XmlEnum(XmlSkillEffectParameterNames.Bow)]
    Bow,

    [XmlEnum(XmlSkillEffectParameterNames.BugWeakness)]
    BugWeakness,

    [XmlEnum(XmlSkillEffectParameterNames.ChangeBody)]
    ChangeBody,

    [XmlEnum(XmlSkillEffectParameterNames.ConstructWeakness)]
    ConstructWeakness,

    [XmlEnum(XmlSkillEffectParameterNames.Crossbow)]
    Crossbow,

    [XmlEnum(XmlSkillEffectParameterNames.Dagger)]
    Dagger,

    [XmlEnum(XmlSkillEffectParameterNames.DemonicWeakness)]
    DemonicWeakness,

    [XmlEnum(XmlSkillEffectParameterNames.Deport)]
    Deport,

    [XmlEnum(XmlSkillEffectParameterNames.Derangement)]
    Derangement,

    [XmlEnum(XmlSkillEffectParameterNames.Disarm)]
    Disarm,

    [XmlEnum(XmlSkillEffectParameterNames.DragonWeakness)]
    DragonWeakness,

    [XmlEnum(XmlSkillEffectParameterNames.Dual)]
    Dual,

    [XmlEnum(XmlSkillEffectParameterNames.DualDagger)]
    DualDagger,

    [XmlEnum(XmlSkillEffectParameterNames.DualFist)]
    DualFist,

    [XmlEnum(XmlSkillEffectParameterNames.Fear)]
    Fear,

    [XmlEnum(XmlSkillEffectParameterNames.Fist)]
    Fist,

    [XmlEnum(XmlSkillEffectParameterNames.GiantWeakness)]
    GiantWeakness,

    [XmlEnum(XmlSkillEffectParameterNames.Hold)]
    Hold,

    [XmlEnum(XmlSkillEffectParameterNames.Imprison)]
    Imprison,

    [XmlEnum(XmlSkillEffectParameterNames.Infection)]
    Infection,

    [XmlEnum(XmlSkillEffectParameterNames.Knockback)]
    Knockback,

    [XmlEnum(XmlSkillEffectParameterNames.Knockdown)]
    Knockdown,

    [XmlEnum(XmlSkillEffectParameterNames.MagicalDefence)]
    MagicalDefence,

    [XmlEnum(XmlSkillEffectParameterNames.MagicAttack)]
    MagicAttack,

    [XmlEnum(XmlSkillEffectParameterNames.MagicAttackSpeed)]
    MagicAttackSpeed,

    [XmlEnum(XmlSkillEffectParameterNames.MaxHp)]
    MaxHp,

    [XmlEnum(XmlSkillEffectParameterNames.MaxMp)]
    MaxMp,

    [XmlEnum(XmlSkillEffectParameterNames.Paralyze)]
    Paralyze,

    [XmlEnum(XmlSkillEffectParameterNames.PhysicalAttack)]
    PhysicalAttack,

    [XmlEnum(XmlSkillEffectParameterNames.PhysicalAttackRange)]
    PhysicalAttackRange,

    [XmlEnum(XmlSkillEffectParameterNames.PhysicalAttackRangeMode)]
    PhysicalAttackRangeMode,

    [XmlEnum(XmlSkillEffectParameterNames.PhysicalAttackSpeed)]
    PhysicalAttackSpeed,

    [XmlEnum(XmlSkillEffectParameterNames.PhysicalBlockade)]
    PhysicalBlockade,

    [XmlEnum(XmlSkillEffectParameterNames.PhysicalDefence)]
    PhysicalDefence,

    [XmlEnum(XmlSkillEffectParameterNames.Pistols)]
    Pistols,

    [XmlEnum(XmlSkillEffectParameterNames.PlantWeakness)]
    PlantWeakness,

    [XmlEnum(XmlSkillEffectParameterNames.Poison)]
    Poison,

    [XmlEnum(XmlSkillEffectParameterNames.Pole)]
    Pole,

    [XmlEnum(XmlSkillEffectParameterNames.Psychic)]
    Psychic,

    [XmlEnum(XmlSkillEffectParameterNames.Pull)]
    Pull,

    [XmlEnum(XmlSkillEffectParameterNames.Rapier)]
    Rapier,

    [XmlEnum(XmlSkillEffectParameterNames.RootPhysically)]
    RootPhysically,

    [XmlEnum(XmlSkillEffectParameterNames.Shock)]
    Shock,

    [XmlEnum(XmlSkillEffectParameterNames.Silence)]
    Silence,

    [XmlEnum(XmlSkillEffectParameterNames.SkillBonusRange)]
    SkillBonusRange,

    [XmlEnum(XmlSkillEffectParameterNames.SkillBonusRangeMode)]
    SkillBonusRangeMode,

    [XmlEnum(XmlSkillEffectParameterNames.Sleep)]
    Sleep,

    [XmlEnum(XmlSkillEffectParameterNames.Suppression)]
    Suppression,

    [XmlEnum(XmlSkillEffectParameterNames.Sword)]
    Sword,

    [XmlEnum(XmlSkillEffectParameterNames.TurnStone)]
    TurnStone,

    [XmlEnum(XmlSkillEffectParameterNames.UndeadWeakness)]
    UndeadWeakness,

    [XmlEnum(XmlSkillEffectParameterNames.Valakas)]
    Valakas,

    [XmlEnum(XmlSkillEffectParameterNames.Zone)]
    Zone,

    [XmlEnum(XmlSkillEffectParameterNames.AbnormalType)]
    AbnormalType,

    [XmlEnum(XmlSkillEffectParameterNames.AccuracyAmount)]
    AccuracyAmount,

    [XmlEnum(XmlSkillEffectParameterNames.AccuracyMode)]
    AccuracyMode,

    [XmlEnum(XmlSkillEffectParameterNames.AddStat)]
    AddStat,

    [XmlEnum(XmlSkillEffectParameterNames.Aggressive)]
    Aggressive,

    [XmlEnum(XmlSkillEffectParameterNames.AllowBadSkills)]
    AllowBadSkills,

    [XmlEnum(XmlSkillEffectParameterNames.AllowGoodSkills)]
    AllowGoodSkills,

    [XmlEnum(XmlSkillEffectParameterNames.AllowNormalAttack)]
    AllowNormalAttack,

    [XmlEnum(XmlSkillEffectParameterNames.AllowSkillAttack)]
    AllowSkillAttack,

    [XmlEnum(XmlSkillEffectParameterNames.AllowWeapons)]
    AllowWeapons,

    [XmlEnum(XmlSkillEffectParameterNames.AllowedSkills)]
    AllowedSkills,

    [XmlEnum(XmlSkillEffectParameterNames.Amount)]
    Amount,

    [XmlEnum(XmlSkillEffectParameterNames.Angle)]
    Angle,

    [XmlEnum(XmlSkillEffectParameterNames.ArmorType)]
    ArmorType,

    [XmlEnum(XmlSkillEffectParameterNames.AttackerType)]
    AttackerType,

    [XmlEnum(XmlSkillEffectParameterNames.AttackSkillId)]
    AttackSkillId,

    [XmlEnum(XmlSkillEffectParameterNames.Attribute)]
    Attribute,

    [XmlEnum(XmlSkillEffectParameterNames.BaseStat)]
    BaseStat,

    [XmlEnum(XmlSkillEffectParameterNames.BlockedActions)]
    BlockedActions,

    [XmlEnum(XmlSkillEffectParameterNames.CanKill)]
    CanKill,

    [XmlEnum(XmlSkillEffectParameterNames.CastSkillId)]
    CastSkillId,

    [XmlEnum(XmlSkillEffectParameterNames.Chance)]
    Chance,

    [XmlEnum(XmlSkillEffectParameterNames.ChanceBoost)]
    ChanceBoost,

    [XmlEnum(XmlSkillEffectParameterNames.ChanceToRepeat)]
    ChanceToRepeat,

    [XmlEnum(XmlSkillEffectParameterNames.Charge)]
    Charge,

    [XmlEnum(XmlSkillEffectParameterNames.ChargeConsume)]
    ChargeConsume,

    [XmlEnum(XmlSkillEffectParameterNames.CloseSkill)]
    CloseSkill,

    [XmlEnum(XmlSkillEffectParameterNames.CloseSkillLevel)]
    CloseSkillLevel,

    [XmlEnum(XmlSkillEffectParameterNames.ConsumeItemCount)]
    ConsumeItemCount,

    [XmlEnum(XmlSkillEffectParameterNames.ConsumeItemId)]
    ConsumeItemId,

    [XmlEnum(XmlSkillEffectParameterNames.Cp)]
    Cp,

    [XmlEnum(XmlSkillEffectParameterNames.Critical)]
    Critical,

    [XmlEnum(XmlSkillEffectParameterNames.CriticalChance)]
    CriticalChance,

    [XmlEnum(XmlSkillEffectParameterNames.CriticalLimit)]
    CriticalLimit,

    [XmlEnum(XmlSkillEffectParameterNames.CubicId)]
    CubicId,

    [XmlEnum(XmlSkillEffectParameterNames.CubicLvl)]
    CubicLvl,

    [XmlEnum(XmlSkillEffectParameterNames.Damage)]
    Damage,

    [XmlEnum(XmlSkillEffectParameterNames.DeathPenalty)]
    DeathPenalty,

    [XmlEnum(XmlSkillEffectParameterNames.DebuffModifier)]
    DebuffModifier,

    [XmlEnum(XmlSkillEffectParameterNames.DebuffType)]
    DebuffType,

    [XmlEnum(XmlSkillEffectParameterNames.Delay)]
    Delay,

    [XmlEnum(XmlSkillEffectParameterNames.DespawnDelay)]
    DespawnDelay,

    [XmlEnum(XmlSkillEffectParameterNames.Disable)]
    Disable,

    [XmlEnum(XmlSkillEffectParameterNames.Dispel)]
    Dispel,

    [XmlEnum(XmlSkillEffectParameterNames.Distance)]
    Distance,

    [XmlEnum(XmlSkillEffectParameterNames.EscapeType)]
    EscapeType,

    [XmlEnum(XmlSkillEffectParameterNames.ExistingSkillId)]
    ExistingSkillId,

    [XmlEnum(XmlSkillEffectParameterNames.Exp)]
    Exp,

    [XmlEnum(XmlSkillEffectParameterNames.ExpMultiplier)]
    ExpMultiplier,

    [XmlEnum(XmlSkillEffectParameterNames.ExpNeeded)]
    ExpNeeded,

    [XmlEnum(XmlSkillEffectParameterNames.FullLethal)]
    FullLethal,

    [XmlEnum(XmlSkillEffectParameterNames.HalfLethal)]
    HalfLethal,

    [XmlEnum(XmlSkillEffectParameterNames.Heal)]
    Heal,

    [XmlEnum(XmlSkillEffectParameterNames.Hp)]
    Hp,

    [XmlEnum(XmlSkillEffectParameterNames.HpPercent)]
    HpPercent,

    [XmlEnum(XmlSkillEffectParameterNames.Id)]
    Id,

    [XmlEnum(XmlSkillEffectParameterNames.IgnoreShieldDefence)]
    IgnoreShieldDefence,

    [XmlEnum(XmlSkillEffectParameterNames.InstanceId)]
    InstanceId,

    [XmlEnum(XmlSkillEffectParameterNames.IsAdvanced)]
    IsAdvanced,

    [XmlEnum(XmlSkillEffectParameterNames.IsCritical)]
    IsCritical,

    [XmlEnum(XmlSkillEffectParameterNames.Item)]
    Item,

    [XmlEnum(XmlSkillEffectParameterNames.ItemCount)]
    ItemCount,

    [XmlEnum(XmlSkillEffectParameterNames.ItemId)]
    ItemId,

    [XmlEnum(XmlSkillEffectParameterNames.Items)]
    Items,

    [XmlEnum(XmlSkillEffectParameterNames.KnockDown)]
    KnockDown,

    [XmlEnum(XmlSkillEffectParameterNames.Level)]
    Level,

    [XmlEnum(XmlSkillEffectParameterNames.LifeTime)]
    LifeTime,

    [XmlEnum(XmlSkillEffectParameterNames.MagicType)]
    MagicType,

    [XmlEnum(XmlSkillEffectParameterNames.MagicTypes)]
    MagicTypes,

    [XmlEnum(XmlSkillEffectParameterNames.MagicWeapon)]
    MagicWeapon,

    [XmlEnum(XmlSkillEffectParameterNames.Max)]
    Max,

    [XmlEnum(XmlSkillEffectParameterNames.MaxCharges)]
    MaxCharges,

    [XmlEnum(XmlSkillEffectParameterNames.MaxSouls)]
    MaxSouls,

    [XmlEnum(XmlSkillEffectParameterNames.Min)]
    Min,

    [XmlEnum(XmlSkillEffectParameterNames.MinDamage)]
    MinDamage,

    [XmlEnum(XmlSkillEffectParameterNames.MinSlot)]
    MinSlot,

    [XmlEnum(XmlSkillEffectParameterNames.Mode)]
    Mode,

    [XmlEnum(XmlSkillEffectParameterNames.Mp)]
    Mp,

    [XmlEnum(XmlSkillEffectParameterNames.MulStat)]
    MulStat,

    [XmlEnum(XmlSkillEffectParameterNames.Normal)]
    Normal,

    [XmlEnum(XmlSkillEffectParameterNames.NpcCount)]
    NpcCount,

    [XmlEnum(XmlSkillEffectParameterNames.NpcId)]
    NpcId,

    [XmlEnum(XmlSkillEffectParameterNames.OnlyMagicSkill)]
    OnlyMagicSkill,

    [XmlEnum(XmlSkillEffectParameterNames.OnlyPhysicalSkill)]
    OnlyPhysicalSkill,

    [XmlEnum(XmlSkillEffectParameterNames.OptionalSlots)]
    OptionalSlots,

    [XmlEnum(XmlSkillEffectParameterNames.OverHit)]
    OverHit,

    [XmlEnum(XmlSkillEffectParameterNames.PartyBuffSkillId)]
    PartyBuffSkillId,

    [XmlEnum(XmlSkillEffectParameterNames.PAtkAmount)]
    PAtkAmount,

    [XmlEnum(XmlSkillEffectParameterNames.PAtkMod)]
    PAtkMod,

    [XmlEnum(XmlSkillEffectParameterNames.PAtkMode)]
    PAtkMode,

    [XmlEnum(XmlSkillEffectParameterNames.PAtkSpeedAmount)]
    PAtkSpeedAmount,

    [XmlEnum(XmlSkillEffectParameterNames.PAtkSpeedMode)]
    PAtkSpeedMode,

    [XmlEnum(XmlSkillEffectParameterNames.PDefMod)]
    PDefMod,

    [XmlEnum(XmlSkillEffectParameterNames.Percentage)]
    Percentage,

    [XmlEnum(XmlSkillEffectParameterNames.PercentFrom)]
    PercentFrom,

    [XmlEnum(XmlSkillEffectParameterNames.PercentTo)]
    PercentTo,

    [XmlEnum(XmlSkillEffectParameterNames.Position)]
    Position,

    [XmlEnum(XmlSkillEffectParameterNames.Power)]
    Power,

    [XmlEnum(XmlSkillEffectParameterNames.PowerModifier)]
    PowerModifier,

    [XmlEnum(XmlSkillEffectParameterNames.RaceModifier)]
    RaceModifier,

    [XmlEnum(XmlSkillEffectParameterNames.Races)]
    Races,

    [XmlEnum(XmlSkillEffectParameterNames.Range)]
    Range,

    [XmlEnum(XmlSkillEffectParameterNames.RangeSkill)]
    RangeSkill,

    [XmlEnum(XmlSkillEffectParameterNames.RangeSkillLevel)]
    RangeSkillLevel,

    [XmlEnum(XmlSkillEffectParameterNames.Rate)]
    Rate,

    [XmlEnum(XmlSkillEffectParameterNames.RenewDuration)]
    RenewDuration,

    [XmlEnum(XmlSkillEffectParameterNames.RepeatCount)]
    RepeatCount,

    [XmlEnum(XmlSkillEffectParameterNames.ReplacementSkillId)]
    ReplacementSkillId,

    [XmlEnum(XmlSkillEffectParameterNames.Reputation)]
    Reputation,

    [XmlEnum(XmlSkillEffectParameterNames.RequiredSlots)]
    RequiredSlots,

    [XmlEnum(XmlSkillEffectParameterNames.Ride)]
    Ride,

    [XmlEnum(XmlSkillEffectParameterNames.ShieldDefPercent)]
    ShieldDefPercent,

    [XmlEnum(XmlSkillEffectParameterNames.Side)]
    Side,

    [XmlEnum(XmlSkillEffectParameterNames.SingleInstance)]
    SingleInstance,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId)]
    SkillId,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId1)]
    SkillId1,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId10)]
    SkillId10,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId11)]
    SkillId11,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId12)]
    SkillId12,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId13)]
    SkillId13,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId14)]
    SkillId14,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId15)]
    SkillId15,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId16)]
    SkillId16,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId17)]
    SkillId17,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId18)]
    SkillId18,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId19)]
    SkillId19,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId2)]
    SkillId2,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId20)]
    SkillId20,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId21)]
    SkillId21,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId22)]
    SkillId22,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId23)]
    SkillId23,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId24)]
    SkillId24,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId25)]
    SkillId25,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId26)]
    SkillId26,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId27)]
    SkillId27,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId28)]
    SkillId28,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId29)]
    SkillId29,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId3)]
    SkillId3,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId30)]
    SkillId30,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId31)]
    SkillId31,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId32)]
    SkillId32,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId33)]
    SkillId33,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId34)]
    SkillId34,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId35)]
    SkillId35,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId36)]
    SkillId36,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId37)]
    SkillId37,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId38)]
    SkillId38,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId39)]
    SkillId39,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId4)]
    SkillId4,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId40)]
    SkillId40,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId41)]
    SkillId41,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId42)]
    SkillId42,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId43)]
    SkillId43,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId44)]
    SkillId44,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId45)]
    SkillId45,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId46)]
    SkillId46,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId47)]
    SkillId47,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId48)]
    SkillId48,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId49)]
    SkillId49,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId5)]
    SkillId5,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId50)]
    SkillId50,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId51)]
    SkillId51,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId6)]
    SkillId6,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId7)]
    SkillId7,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId8)]
    SkillId8,

    [XmlEnum(XmlSkillEffectParameterNames.SkillId9)]
    SkillId9,

    [XmlEnum(XmlSkillEffectParameterNames.SkillIds)]
    SkillIds,

    [XmlEnum(XmlSkillEffectParameterNames.SkillLevel)]
    SkillLevel,

    [XmlEnum(XmlSkillEffectParameterNames.SkillLevelScaleTo)]
    SkillLevelScaleTo,

    [XmlEnum(XmlSkillEffectParameterNames.Skills)]
    Skills,

    [XmlEnum(XmlSkillEffectParameterNames.SkillSubLevel)]
    SkillSubLevel,

    [XmlEnum(XmlSkillEffectParameterNames.Slot)]
    Slot,

    [XmlEnum(XmlSkillEffectParameterNames.Slots)]
    Slots,

    [XmlEnum(XmlSkillEffectParameterNames.Sp)]
    Sp,

    [XmlEnum(XmlSkillEffectParameterNames.Speed)]
    Speed,

    [XmlEnum(XmlSkillEffectParameterNames.Stat)]
    Stat,

    [XmlEnum(XmlSkillEffectParameterNames.StaticChance)]
    StaticChance,

    [XmlEnum(XmlSkillEffectParameterNames.TargetType)]
    TargetType,

    [XmlEnum(XmlSkillEffectParameterNames.Ticks)]
    Ticks,

    [XmlEnum(XmlSkillEffectParameterNames.Time)]
    Time,

    [XmlEnum(XmlSkillEffectParameterNames.Times)]
    Times,

    [XmlEnum(XmlSkillEffectParameterNames.TransformationId)]
    TransformationId,

    [XmlEnum(XmlSkillEffectParameterNames.TriggerSkills)]
    TriggerSkills,

    [XmlEnum(XmlSkillEffectParameterNames.TwoHandWeapon)]
    TwoHandWeapon,

    [XmlEnum(XmlSkillEffectParameterNames.Type)]
    Type,

    [XmlEnum(XmlSkillEffectParameterNames.Value)]
    Value,

    [XmlEnum(XmlSkillEffectParameterNames.WeaponType)]
    WeaponType,

    [XmlEnum(XmlSkillEffectParameterNames.Wyvern)]
    Wyvern,

    [XmlEnum(XmlSkillEffectParameterNames.X)]
    X,

    [XmlEnum(XmlSkillEffectParameterNames.Xp)]
    Xp,

    [XmlEnum(XmlSkillEffectParameterNames.Y)]
    Y,

    [XmlEnum(XmlSkillEffectParameterNames.Z)]
    Z,

    [XmlEnum(XmlSkillEffectParameterNames.ZoneId)]
    ZoneId,
}