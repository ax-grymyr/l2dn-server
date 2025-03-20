using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Skills;

public enum XmlSkillEffectParameterType
{
    [XmlEnum(XmlSkillEffectParameterNames.AddedSkillId)]
    AddedSkillId,

    [XmlEnum(XmlSkillEffectParameterNames.AddedSkillLevel)]
    AddedSkillLevel,

    [XmlEnum(XmlSkillEffectParameterNames.AffectSummoner)]
    AffectSummoner,

    [XmlEnum(XmlSkillEffectParameterNames.AirBind)]
    AirBind,

    [XmlEnum(XmlSkillEffectParameterNames.AncientSword)]
    AncientSword,

    [XmlEnum(XmlSkillEffectParameterNames.AnimalWeakness)]
    AnimalWeakness,

    [XmlEnum(XmlSkillEffectParameterNames.AnimationSpeed)]
    AnimationSpeed,

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

    [XmlEnum(XmlSkillEffectParameterNames.DamageModifier)]
    DamageModifier,

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

    [XmlEnum(XmlSkillEffectParameterNames.Fame)]
    Fame,

    [XmlEnum(XmlSkillEffectParameterNames.Fear)]
    Fear,

    [XmlEnum(XmlSkillEffectParameterNames.Fist)]
    Fist,

    [XmlEnum(XmlSkillEffectParameterNames.FlyType)]
    FlyType,

    [XmlEnum(XmlSkillEffectParameterNames.GiantWeakness)]
    GiantWeakness,

    [XmlEnum(XmlSkillEffectParameterNames.Hold)]
    Hold,

    [XmlEnum(XmlSkillEffectParameterNames.Ids)]
    Ids,

    [XmlEnum(XmlSkillEffectParameterNames.Imprison)]
    Imprison,

    [XmlEnum(XmlSkillEffectParameterNames.InCombat)]
    InCombat,

    [XmlEnum(XmlSkillEffectParameterNames.Index)]
    Index,

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

    [XmlEnum(XmlSkillEffectParameterNames.MaxAttackerLevel)]
    MaxAttackerLevel,

    [XmlEnum(XmlSkillEffectParameterNames.MaxHp)]
    MaxHp,

    [XmlEnum(XmlSkillEffectParameterNames.MaxMp)]
    MaxMp,

    [XmlEnum(XmlSkillEffectParameterNames.MinAttackerLevel)]
    MinAttackerLevel,

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

    [XmlEnum(XmlSkillEffectParameterNames.Radius)]
    Radius,

    [XmlEnum(XmlSkillEffectParameterNames.Rapier)]
    Rapier,

    [XmlEnum(XmlSkillEffectParameterNames.RootPhysically)]
    RootPhysically,

    [XmlEnum(XmlSkillEffectParameterNames.SaveHp)]
    SaveHp,

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

    [XmlEnum(XmlSkillEffectParameterNames.AbnormalPower)]
    AbnormalPower,

    [XmlEnum(XmlSkillEffectParameterNames.AbnormalType)]
    AbnormalType,

    [XmlEnum(XmlSkillEffectParameterNames.AccuracyAmount)]
    AccuracyAmount,

    [XmlEnum(XmlSkillEffectParameterNames.AccuracyMode)]
    AccuracyMode,

    [XmlEnum(XmlSkillEffectParameterNames.AddStat)]
    AddStat,

    [XmlEnum(XmlSkillEffectParameterNames.AdjustLevel)]
    AdjustLevel,

    [XmlEnum(XmlSkillEffectParameterNames.Aggressive)]
    Aggressive,

    [XmlEnum(XmlSkillEffectParameterNames.AllowBadSkills)]
    AllowBadSkills,

    [XmlEnum(XmlSkillEffectParameterNames.AllowGoodSkills)]
    AllowGoodSkills,

    [XmlEnum(XmlSkillEffectParameterNames.AllowNormalAttack)]
    AllowNormalAttack,

    [XmlEnum(XmlSkillEffectParameterNames.AllowReflect)]
    AllowReflect,

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

    [XmlEnum(XmlSkillEffectParameterNames.AttackSkillLevel)]
    AttackSkillLevel,

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

    [XmlEnum(XmlSkillEffectParameterNames.ConsumeItemInterval)]
    ConsumeItemInterval,

    [XmlEnum(XmlSkillEffectParameterNames.Cp)]
    Cp,

    [XmlEnum(XmlSkillEffectParameterNames.CpPercent)]
    CpPercent,

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

    [XmlEnum(XmlSkillEffectParameterNames.DespawnTime)]
    DespawnTime,

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

    [XmlEnum(XmlSkillEffectParameterNames.ExistingSkillLevel)]
    ExistingSkillLevel,

    [XmlEnum(XmlSkillEffectParameterNames.Exp)]
    Exp,

    [XmlEnum(XmlSkillEffectParameterNames.ExpMultiplier)]
    ExpMultiplier,

    [XmlEnum(XmlSkillEffectParameterNames.ExpNeeded)]
    ExpNeeded,

    [XmlEnum(XmlSkillEffectParameterNames.From)]
    From,

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

    [XmlEnum(XmlSkillEffectParameterNames.IsItem)]
    IsItem,

    [XmlEnum(XmlSkillEffectParameterNames.IsSummonSpawn)]
    IsSummonSpawn,

    [XmlEnum(XmlSkillEffectParameterNames.Item)]
    Item,

    [XmlEnum(XmlSkillEffectParameterNames.ItemEnchantmentLevel)]
    ItemEnchantmentLevel,

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

    [XmlEnum(XmlSkillEffectParameterNames.MAccuracyAmount)]
    MAccuracyAmount,

    [XmlEnum(XmlSkillEffectParameterNames.MAccuracyMode)]
    MAccuracyMode,

    [XmlEnum(XmlSkillEffectParameterNames.MagicType)]
    MagicType,

    [XmlEnum(XmlSkillEffectParameterNames.MagicTypes)]
    MagicTypes,

    [XmlEnum(XmlSkillEffectParameterNames.MagicWeapon)]
    MagicWeapon,

    [XmlEnum(XmlSkillEffectParameterNames.MAtkAmount)]
    MAtkAmount,

    [XmlEnum(XmlSkillEffectParameterNames.MAtkMode)]
    MAtkMode,

    [XmlEnum(XmlSkillEffectParameterNames.MAtkSpeedAmount)]
    MAtkSpeedAmount,

    [XmlEnum(XmlSkillEffectParameterNames.MAtkSpeedMode)]
    MAtkSpeedMode,

    [XmlEnum(XmlSkillEffectParameterNames.Max)]
    Max,

    [XmlEnum(XmlSkillEffectParameterNames.MaxCharges)]
    MaxCharges,

    [XmlEnum(XmlSkillEffectParameterNames.MaxSouls)]
    MaxSouls,

    [XmlEnum(XmlSkillEffectParameterNames.MCritDamageAmount)]
    MCritDamageAmount,

    [XmlEnum(XmlSkillEffectParameterNames.MCritDamageMode)]
    MCritDamageMode,

    [XmlEnum(XmlSkillEffectParameterNames.MCritRateAmount)]
    MCritRateAmount,

    [XmlEnum(XmlSkillEffectParameterNames.MCritRateMode)]
    MCritRateMode,

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

    [XmlEnum(XmlSkillEffectParameterNames.MpPercent)]
    MpPercent,

    [XmlEnum(XmlSkillEffectParameterNames.MulStat)]
    MulStat,

    [XmlEnum(XmlSkillEffectParameterNames.Normal)]
    Normal,

    [XmlEnum(XmlSkillEffectParameterNames.NpcCount)]
    NpcCount,

    [XmlEnum(XmlSkillEffectParameterNames.NpcId)]
    NpcId,

    [XmlEnum(XmlSkillEffectParameterNames.NpcIds)]
    NpcIds,

    [XmlEnum(XmlSkillEffectParameterNames.OnlyMagicSkill)]
    OnlyMagicSkill,

    [XmlEnum(XmlSkillEffectParameterNames.OnlyPhysicalSkill)]
    OnlyPhysicalSkill,

    [XmlEnum(XmlSkillEffectParameterNames.OptionalSlots)]
    OptionalSlots,

    [XmlEnum(XmlSkillEffectParameterNames.OverHit)]
    OverHit,

    [XmlEnum(XmlSkillEffectParameterNames.PAccuracyAmount)]
    PAccuracyAmount,

    [XmlEnum(XmlSkillEffectParameterNames.PAccuracyMode)]
    PAccuracyMode,

    [XmlEnum(XmlSkillEffectParameterNames.Party)]
    Party,

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

    [XmlEnum(XmlSkillEffectParameterNames.PCritDamageAmount)]
    PCritDamageAmount,

    [XmlEnum(XmlSkillEffectParameterNames.PCritDamageMode)]
    PCritDamageMode,

    [XmlEnum(XmlSkillEffectParameterNames.PCritRateAmount)]
    PCritRateAmount,

    [XmlEnum(XmlSkillEffectParameterNames.PCritRateMode)]
    PCritRateMode,

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

    [XmlEnum(XmlSkillEffectParameterNames.RandomOffset)]
    RandomOffset,

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

    [XmlEnum(XmlSkillEffectParameterNames.Replace)]
    Replace,

    [XmlEnum(XmlSkillEffectParameterNames.ReplacementSkillId)]
    ReplacementSkillId,

    [XmlEnum(XmlSkillEffectParameterNames.ReplacementSkillLevel)]
    ReplacementSkillLevel,

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

    [XmlEnum(XmlSkillEffectParameterNames.SkillIds)]
    SkillIds,

    [XmlEnum(XmlSkillEffectParameterNames.SkillLevel)]
    SkillLevel,

    [XmlEnum(XmlSkillEffectParameterNames.SkillLevels)]
    SkillLevels,

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

    [XmlEnum(XmlSkillEffectParameterNames.SpeedAmount)]
    SpeedAmount,

    [XmlEnum(XmlSkillEffectParameterNames.SpeedMode)]
    SpeedMode,

    [XmlEnum(XmlSkillEffectParameterNames.Stat)]
    Stat,

    [XmlEnum(XmlSkillEffectParameterNames.StaticChance)]
    StaticChance,

    [XmlEnum(XmlSkillEffectParameterNames.StealAmount)]
    StealAmount,

    [XmlEnum(XmlSkillEffectParameterNames.SummonerLevels)]
    SummonerLevels,

    [XmlEnum(XmlSkillEffectParameterNames.SummonPoints)]
    SummonPoints,

    [XmlEnum(XmlSkillEffectParameterNames.TargetType)]
    TargetType,

    [XmlEnum(XmlSkillEffectParameterNames.Ticks)]
    Ticks,

    [XmlEnum(XmlSkillEffectParameterNames.Time)]
    Time,

    [XmlEnum(XmlSkillEffectParameterNames.Times)]
    Times,

    [XmlEnum(XmlSkillEffectParameterNames.To)]
    To,

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