using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Globalization;
using L2Dn.Collections;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Skills;

public sealed class Skill: IIdentifiable
{
    private readonly InlineArray4<int> _fanRange; // unk;startDegree;fanAffectRange;fanAffectAngle
    private readonly InlineArray3<int> _affectLimit; // TODO: Third value is unknown... find it out!
    private readonly InlineArray2<int> _affectHeight;

    // outer array index is SkillEffectScope
    private readonly InlineArray7<ImmutableArray<IAbstractEffect>> _effectLists;

    // outer array index is SkillConditionScope
    private readonly InlineArray3<ImmutableArray<ISkillConditionBase>> _conditionLists;

    private readonly EffectTypes _effectTypes;

    // outer array index is SkillEffectScope
    private readonly InlineArray7<EffectTypes> _effectTypesByScope;

    // If true this skill's effect should stay after death.
    private readonly bool _stayAfterDeath;

    private readonly bool _isHidingMessages;

    internal Skill(SkillParameters parameters)
    {
        Id = parameters.Id;
        Name = parameters.Name;
        Level = parameters.Level;
        SubLevel = parameters.SubLevel;
        DisplayId = parameters.DisplayId ?? parameters.Id;
        DisplayLevel = parameters.DisplayLevel ?? parameters.Level;
        ReferenceItemId = parameters.ReferenceId ?? 0;

        ParameterSet<XmlSkillParameterType> pars = parameters.Parameters;

        OperateType = pars.GetEnum<SkillOperateType>(XmlSkillParameterType.OperateType);
        MagicType = (SkillMagicType)pars.GetInt32(XmlSkillParameterType.IsMagic, 0);
        TraitType = pars.GetEnum(XmlSkillParameterType.Trait, TraitType.NONE);
        IsStaticReuse = pars.GetBoolean(XmlSkillParameterType.StaticReuse, false);
        MpConsume = pars.GetInt32(XmlSkillParameterType.MpConsume, 0);
        MpInitialConsume = pars.GetInt32(XmlSkillParameterType.MpInitialConsume, 0);
        MpPerChanneling = pars.GetInt32(XmlSkillParameterType.MpPerChanneling, MpConsume);
        HpConsume = pars.GetInt32(XmlSkillParameterType.HpConsume, 0);
        ItemConsumeCount = pars.GetInt32(XmlSkillParameterType.ItemConsumeCount, 0);
        ItemConsumeId = pars.GetInt32(XmlSkillParameterType.ItemConsumeId, 0);
        FamePointConsume = pars.GetInt32(XmlSkillParameterType.FamePointConsume, 0);
        ClanRepConsume = pars.GetInt32(XmlSkillParameterType.ClanRepConsume, 0);
        CastRange = pars.GetInt32(XmlSkillParameterType.CastRange, -1);
        EffectRange = pars.GetInt32(XmlSkillParameterType.EffectRange, -1);
        AbnormalLevel = pars.GetInt32(XmlSkillParameterType.AbnormalLevel, 0);
        AbnormalType = pars.GetEnum(XmlSkillParameterType.AbnormalType, AbnormalType.NONE);
        SubordinationAbnormalType = pars.GetEnum(XmlSkillParameterType.SubordinationAbnormalType, AbnormalType.NONE);
        TimeSpan abnormalTime = pars.GetTimeSpanSeconds(XmlSkillParameterType.AbnormalTime, TimeSpan.Zero);
        if (Config.Character.ENABLE_MODIFY_SKILL_DURATION && OperateType != SkillOperateType.T &&
            Config.Character.SKILL_DURATION_LIST.TryGetValue(Id, out TimeSpan temp))
        {
            if (Level < 100 || Level > 140)
                abnormalTime = temp;
            else if (Level >= 100 && Level < 140)
                abnormalTime += temp;
        }

        AbnormalTime = abnormalTime;
        IsAbnormalInstant = pars.GetBoolean(XmlSkillParameterType.AbnormalInstant, false);
        AbnormalVisualEffects = ParseAbnormalVisualEffect(Id,
            pars.GetString(XmlSkillParameterType.AbnormalVisualEffect, string.Empty));

        _stayAfterDeath = pars.GetBoolean(XmlSkillParameterType.StayAfterDeath, false);
        HitTime = pars.GetTimeSpanMilliSeconds(XmlSkillParameterType.HitTime, TimeSpan.Zero);
        HitCancelTime = pars.GetTimeSpanMilliSeconds(XmlSkillParameterType.HitCancelTime, TimeSpan.Zero);
        CoolTime = pars.GetTimeSpanMilliSeconds(XmlSkillParameterType.CoolTime, TimeSpan.Zero);
        IsDebuff = pars.GetBoolean(XmlSkillParameterType.IsDebuff, false);
        IsRecoveryHerb = pars.GetBoolean(XmlSkillParameterType.IsRecoveryHerb, false);
        if (Config.Character.ENABLE_MODIFY_SKILL_REUSE &&
            Config.Character.SKILL_REUSE_LIST.TryGetValue(Id, out TimeSpan reuseDelay))
            ReuseDelay = reuseDelay;
        else
            ReuseDelay = pars.GetTimeSpanMilliSeconds(XmlSkillParameterType.ReuseDelay, TimeSpan.Zero);

        ReuseDelayGroup = pars.GetInt32(XmlSkillParameterType.ReuseDelayGroup, -1);
        ReuseHashCode = GetSkillHashCode(ReuseDelayGroup > 0 ? ReuseDelayGroup : Id, Level, SubLevel);
        TargetType = pars.GetEnum(XmlSkillParameterType.TargetType, TargetType.SELF);
        AffectScope = pars.GetEnum(XmlSkillParameterType.AffectScope, AffectScope.SINGLE);
        AffectObject = pars.GetEnum(XmlSkillParameterType.AffectObject, AffectObject.ALL);
        AffectRange = pars.GetInt32(XmlSkillParameterType.AffectRange, 0);

        string? fanRange = pars.GetStringOptional(XmlSkillParameterType.FanRange);
        if (!string.IsNullOrEmpty(fanRange))
        {
            try
            {
                string[] valuesSplit = fanRange.Split(";");
                for (int i = 0; i < 4; i++)
                    _fanRange[i] = int.Parse(valuesSplit[i], CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new InvalidOperationException($"SkillId: {Id} invalid fanRange value: {fanRange}, " +
                    "'unk;startDegree;fanAffectRange;fanAffectAngle' required");
            }
        }

        string affectLimit = pars.GetString(XmlSkillParameterType.AffectLimit, string.Empty);
        if (!string.IsNullOrEmpty(affectLimit))
        {
            try
            {
                string[] valuesSplit = affectLimit.Split("-");
                _affectLimit[0] = int.Parse(valuesSplit[0], CultureInfo.InvariantCulture);
                _affectLimit[1] = int.Parse(valuesSplit[1], CultureInfo.InvariantCulture);
                if (valuesSplit.Length > 2)
                    _affectLimit[2] = int.Parse(valuesSplit[2], CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new InvalidOperationException($"SkillId: {Id} invalid affectLimit value: {affectLimit}, " +
                    "'minAffected-additionalRandom' required");
            }
        }

        string affectHeight = pars.GetString(XmlSkillParameterType.AffectHeight, string.Empty);
        if (!string.IsNullOrEmpty(affectHeight))
        {
            try
            {
                string[] valuesSplit = affectHeight.Split(";");
                _affectHeight[0] = int.Parse(valuesSplit[0], CultureInfo.InvariantCulture);
                _affectHeight[1] = int.Parse(valuesSplit[1], CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new InvalidOperationException(
                    $"SkillId: {Id} invalid affectHeight value: {affectHeight}, 'minHeight-maxHeight' required");
            }

            if (_affectHeight[0] > _affectHeight[1])
            {
                throw new InvalidOperationException($"SkillId: {Id} invalid affectHeight value: {affectHeight}, " +
                    "'minHeight-maxHeight' required, minHeight is higher than maxHeight!");
            }
        }

        MagicLevel = pars.GetInt32(XmlSkillParameterType.MagicLevel, 0);
        LevelBonusRate = pars.GetInt32(XmlSkillParameterType.LvlBonusRate, 0);
        ActivateRate = pars.GetDoubleOptional(XmlSkillParameterType.ActivateRate);
        MinChance = pars.GetInt32(XmlSkillParameterType.MinChance, Config.Character.MIN_ABNORMAL_STATE_SUCCESS_RATE);
        MaxChance = pars.GetInt32(XmlSkillParameterType.MaxChance, Config.Character.MAX_ABNORMAL_STATE_SUCCESS_RATE);
        NextAction = pars.GetEnum(XmlSkillParameterType.NextAction, NextActionType.NONE);
        IsRemovedOnAnyActionExceptMove = pars.GetBoolean(XmlSkillParameterType.RemovedOnAnyActionExceptMove, false);
        IsRemovedOnDamage = pars.GetBoolean(XmlSkillParameterType.RemovedOnDamage, false);
        IsRemovedOnUnequipWeapon = pars.GetBoolean(XmlSkillParameterType.RemovedOnUnequipWeapon, false);
        IsBlockedInOlympiad = pars.GetBoolean(XmlSkillParameterType.BlockedInOlympiad, false);
        AttributeType = pars.GetEnum(XmlSkillParameterType.AttributeType, AttributeType.NONE);
        AttributeValue = pars.GetInt32(XmlSkillParameterType.AttributeValue, 0);
        BasicProperty = pars.GetEnum(XmlSkillParameterType.BasicProperty, BasicProperty.NONE);
        IsSuicideAttack = pars.GetBoolean(XmlSkillParameterType.IsSuicideAttack, false);
        MinPledgeClass = (SocialClass)pars.GetInt32(XmlSkillParameterType.MinPledgeClass, 0);
        MaxLightSoulConsumeCount = pars.GetInt32(XmlSkillParameterType.LightSoulMaxConsume, 0);
        MaxShadowSoulConsumeCount = pars.GetInt32(XmlSkillParameterType.ShadowSoulMaxConsume, 0);
        ChargeConsumeCount = pars.GetInt32(XmlSkillParameterType.ChargeConsume, 0);
        IsTriggeredSkill = pars.GetBoolean(XmlSkillParameterType.IsTriggeredSkill, false);
        EffectPoint = pars.GetInt32(XmlSkillParameterType.EffectPoint, 0);
        CanBeDispelled = pars.GetBoolean(XmlSkillParameterType.CanBeDispelled, true);
        IsExcludedFromCheck = pars.GetBoolean(XmlSkillParameterType.ExcludedFromCheck, false);
        IsWithoutAction = pars.GetBoolean(XmlSkillParameterType.WithoutAction, false);
        Icon = pars.GetString(XmlSkillParameterType.Icon, "icon.skill0000");
        ChannelingSkillId = pars.GetInt32(XmlSkillParameterType.ChannelingSkillId, 0);
        ChannelingTickInterval =
            pars.GetTimeSpanSeconds(XmlSkillParameterType.ChannelingTickInterval, TimeSpan.FromSeconds(2));

        ChannelingTickInitialDelay = pars.GetTimeSpanSeconds(XmlSkillParameterType.ChannelingStart, TimeSpan.Zero);
        IsMentoring = pars.GetBoolean(XmlSkillParameterType.IsMentoring, false);
        DoubleCastSkill = pars.GetInt32(XmlSkillParameterType.DoubleCastSkill, 0);
        CanDoubleCast = pars.GetBoolean(XmlSkillParameterType.CanDoubleCast, false);
        CanCastWhileDisabled = pars.GetBoolean(XmlSkillParameterType.CanCastWhileDisabled, false);
        IsSharedWithSummon = pars.GetBoolean(XmlSkillParameterType.IsSharedWithSummon, true);
        IsNecessaryToggle = pars.GetBoolean(XmlSkillParameterType.IsNecessaryToggle, false);
        IsDeleteAbnormalOnLeave = pars.GetBoolean(XmlSkillParameterType.DeleteAbnormalOnLeave, false);
        IsIrreplacableBuff = pars.GetBoolean(XmlSkillParameterType.IrreplacableBuff, false);
        IsBlockActionUseSkill = pars.GetBoolean(XmlSkillParameterType.BlockActionUseSkill, false);
        ToggleGroupId = pars.GetInt32(XmlSkillParameterType.ToggleGroupId, -1);
        AttachToggleGroupId = pars.GetInt32(XmlSkillParameterType.AttachToggleGroupId, -1);

        // TODO: add to XML schema, not used
        AttachSkills = ImmutableArray<AttachSkillHolder>.Empty;
        //_attachSkills = set.getList(XmlSkillParameterType.AttachSkillList, new List<StatSet>())
        // .Select(AttachSkillHolder.FromStatSet).ToList();

        string abnormalResist = pars.GetString(XmlSkillParameterType.AbnormalResists, string.Empty);
        AbnormalResists = ParseList<AbnormalType>(Id, abnormalResist);

        MagicCriticalRate = pars.GetDouble(XmlSkillParameterType.MagicCriticalRate, 0);
        BuffType = IsTriggeredSkill ? SkillBuffType.TRIGGER :
            IsToggle ? SkillBuffType.TOGGLE :
            IsDance ? SkillBuffType.DANCE :
            IsDebuff ? SkillBuffType.DEBUFF :
            !IsHealingPotionSkill ? SkillBuffType.BUFF : SkillBuffType.NONE;

        IsDisplayInList = pars.GetBoolean(XmlSkillParameterType.DisplayInList, true);
        _isHidingMessages = pars.GetBoolean(XmlSkillParameterType.IsHidingMessages, false);

        // effects
        for (int i = 0; i < _effectLists.Length; i++)
            _effectLists[i] = ImmutableArray<IAbstractEffect>.Empty;

        foreach (KeyValuePair<SkillEffectScope, List<IAbstractEffect>> pair in parameters.Effects)
            _effectLists[(int)pair.Key] = pair.Value.ToImmutableArray();

        // effect types
        EffectTypes allEffectTypes = EffectTypes.NONE;
        foreach (SkillEffectScope effectScope in EnumUtil.GetValues<SkillEffectScope>())
        {
            EffectTypes effectTypes = EffectTypes.NONE;
            ImmutableArray<IAbstractEffect> effects = _effectLists[(int)effectScope];
            if (!effects.IsDefaultOrEmpty)
            {
                foreach (IAbstractEffect effect in effects)
                    effectTypes |= effect.EffectTypes;
            }

            _effectTypesByScope[(int)effectScope] = effectTypes;
            allEffectTypes |= effectTypes;
        }

        _effectTypes = allEffectTypes;

        // conditions
        for (int i = 0; i < _conditionLists.Length; i++)
            _conditionLists[i] = ImmutableArray<ISkillConditionBase>.Empty;

        foreach (KeyValuePair<SkillConditionScope, List<ISkillConditionBase>> pair in parameters.Conditions)
            _conditionLists[(int)pair.Key] = pair.Value.ToImmutableArray();
    }

    /// <summary>
    /// The skill id.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Skill level.
    /// </summary>
    public int Level { get; }

    /// <summary>
    /// Skill sublevel.
    /// </summary>
    public int SubLevel { get; }

    /// <summary>
    /// Custom skill id displayed by the client.
    /// </summary>
    public int DisplayId { get; }

    /// <summary>
    /// Custom skill level displayed by the client.
    /// </summary>
    public int DisplayLevel { get; }

    /// <summary>
    /// Icon of the skill.
    /// </summary>
    public string Icon { get; }

    /// <summary>
    /// Skill client's name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Operative type: passive, active, toggle.
    /// </summary>
    public SkillOperateType OperateType { get; }

    public bool IsActive => OperateType.isActive();

    public bool IsPassive => OperateType.isPassive();

    public bool IsToggle => OperateType.isToggle();

    public bool IsAura => OperateType.isAura();

    public bool IsNotBroadcastable => OperateType.isNotBroadcastable();

    public bool IsContinuous => OperateType.isContinuous() || IsSelfContinuous;

    public bool IsFlyType => OperateType.isFlyType();

    public bool IsSelfContinuous => OperateType.isSelfContinuous();

    public bool IsChanneling => OperateType.isChanneling();

    public bool IsSynergy => OperateType.isSynergy();

    /// <summary>
    /// Skill magic type.
    /// </summary>
    public SkillMagicType MagicType { get; }

    /// <summary>
    /// Returns true to set physical skills.
    /// </summary>
    public bool IsPhysical => MagicType == SkillMagicType.Physical;

    /// <summary>
    /// Returns true to set magic skills.
    /// </summary>
    public bool IsMagic => MagicType == SkillMagicType.Magic;

    /// <summary>
    /// Returns true to set static skills.
    /// </summary>
    public bool IsStatic => MagicType == SkillMagicType.Static;

    /// <summary>
    /// Returns true to set dance skills.
    /// </summary>
    public bool IsDance => MagicType == SkillMagicType.Dance;

    public TraitType TraitType { get; }

    /// <summary>
    /// Initial MP consumption.
    /// </summary>
    public int MpInitialConsume { get; }

    /// <summary>
    /// MP consumption.
    /// </summary>
    public int MpConsume { get; }

    /// <summary>
    /// MP consumption per channeling tick.
    /// </summary>
    public int MpPerChanneling { get; }

    /// <summary>
    /// HP consumption.
    /// </summary>
    public int HpConsume { get; }

    /// <summary>
    /// Id of item consumed by this skill from caster.
    /// </summary>
    public int ItemConsumeId { get; }

    /// <summary>
    /// Amount of items consumed by this skill from caster.
    /// </summary>
    public int ItemConsumeCount { get; }

    /// <summary>
    /// Used for tracking item id in case that item consume cannot be used.
    /// </summary>
    public int ReferenceItemId { get; }

    /// <summary>
    /// Fame points consumed by this skill from caster.
    /// </summary>
    public int FamePointConsume { get; }

    /// <summary>
    /// Clan points consumed by this skill from caster's clan.
    /// </summary>
    public int ClanRepConsume { get; }

    public int MaxLightSoulConsumeCount { get; }

    public int MaxShadowSoulConsumeCount { get; }

    public int ChargeConsumeCount { get; }

    /// <summary>
    /// Cast range: how far can be the target.
    /// </summary>
    public int CastRange { get; }

    /// <summary>
    /// Effect range: how far the skill affect the target.
    /// </summary>
    public int EffectRange { get; }

    /// <summary>
    /// The skill abnormal level.
    /// </summary>
    public int AbnormalLevel { get; }

    /// <summary>
    /// The skill abnormal time. It is the base to calculate the duration of the continuous effects of this skill.
    /// </summary>
    public TimeSpan? AbnormalTime { get; }

    /// <summary>
    /// The skill abnormal type (global effect "group").
    /// </summary>
    public AbnormalType AbnormalType { get; }

    /// <summary>
    /// The skill subordination abnormal type (local effect "group").
    /// </summary>
    public AbnormalType SubordinationAbnormalType { get; }

    /// <summary>
    /// The skill abnormal visual effect. The visual effects displayed in game.
    /// </summary>
    public FrozenSet<AbnormalVisualEffect> AbnormalVisualEffects { get; }

    /// <summary>
    /// Verify if the skill has abnormal visual effects.
    /// </summary>
    public bool HasAbnormalVisualEffects => AbnormalVisualEffects.Count != 0;

    public TimeSpan HitTime { get; }

    public TimeSpan HitCancelTime { get; }

    public TimeSpan CoolTime { get; }

    public TimeSpan ReuseDelay { get; }

    public long ReuseHashCode { get; }

    /// <summary>
    /// The skill id from which the reuse delay should be taken.
    /// </summary>
    public int ReuseDelayGroup { get; }

    /// <summary>
    /// The skill magic level.
    /// </summary>
    public int MagicLevel { get; }

    public int LevelBonusRate { get; }

    public double? ActivateRate { get; }

    /// <summary>
    /// Custom minimum skill/effect chance.
    /// </summary>
    public int MinChance { get; }

    /// <summary>
    /// Custom maximum skill/effect chance.
    /// </summary>
    public int MaxChance { get; }

    /// <summary>
    /// The target type of the skill: SELF, TARGET, SUMMON, GROUND, etc
    /// </summary>
    public TargetType TargetType { get; }

    /// <summary>
    /// The affect scope of the skill: SINGLE, FAN, SQUARE, PARTY, PLEDGE, etc
    /// </summary>
    public AffectScope AffectScope { get; }

    /// <summary>
    /// The affect object of the skill: All, Clan, Friend, NotFriend, Invisible, etc
    /// </summary>
    public AffectObject AffectObject { get; }

    /// <summary>
    /// Effecting area of the skill (radius).
    /// The center varies according to the target type:
    /// "caster" if targetType = AURA/PARTY/CLAN or "target" if targetType = AREA.
    /// </summary>
    public int AffectRange { get; }

    /// <summary>
    /// The AOE fan range of the skill.
    /// </summary>
    public ReadOnlySpan<int> FanRange => _fanRange;

    /// <summary>
    /// The maximum amount of targets the skill can affect or 0 if unlimited.
    /// </summary>
    public int GetAffectLimit()
    {
        if (_affectLimit[0] > 0 || _affectLimit[1] > 0)
            return _affectLimit[0] + Rnd.get(_affectLimit[1]);

        return 0;
    }

    public int AffectHeightMin => _affectHeight[0];

    public int AffectHeightMax => _affectHeight[1];

    /// <summary>
    /// Character action after cast.
    /// </summary>
    public NextActionType NextAction { get; }

    public AttributeType AttributeType { get; }

    public int AttributeValue { get; }

    /// <summary>
    /// Skill basic property type.
    /// </summary>
    public BasicProperty BasicProperty { get; }

    public SocialClass MinPledgeClass { get; }

    public int EffectPoint { get; }

    /// <summary>
    /// The additional effect id.
    /// </summary>
    public int ChannelingSkillId { get; }

    public TimeSpan ChannelingTickInterval { get; }

    public TimeSpan ChannelingTickInitialDelay { get; }

    /// <summary>
    /// Stance skill id.
    /// </summary>
    public int DoubleCastSkill { get; }

    public int ToggleGroupId { get; }

    public int AttachToggleGroupId { get; }

    public ImmutableArray<AttachSkillHolder> AttachSkills { get; }

    public FrozenSet<AbnormalType> AbnormalResists { get; }

    public double MagicCriticalRate { get; }

    public SkillBuffType BuffType { get; }

    /// <summary>
    /// The skill effects by scope.
    /// </summary>
    public ImmutableArray<IAbstractEffect> GetAbstractEffects(SkillEffectScope effectScope)
    {
        if (effectScope >= 0 && (int)effectScope < _effectLists.Length)
            return _effectLists[(int)effectScope];

        return ImmutableArray<IAbstractEffect>.Empty;
    }

    /// <summary>
    /// Verify if this skill has effects for the given scope.
    /// </summary>
    public bool HasEffects(SkillEffectScope effectScope) =>
        effectScope >= 0 && (int)effectScope < _effectLists.Length && !_effectLists[(int)effectScope].IsDefaultOrEmpty;

    /// <summary>
    /// Effect type to check if its present on this skill effects.
    /// </summary>
    /// <param name="effectTypes"></param>
    /// <returns>True if at least one effect type is present.</returns>
    public bool HasEffectType(EffectTypes effectTypes) => (_effectTypes & effectTypes) != 0;

    /// <summary>
    /// Effect type to check if its present on this skill effects.
    /// </summary>
    /// <param name="effectScope"></param>
    /// <param name="effectTypes"></param>
    /// <returns>True if at least one effect type is present.</returns>
    public bool HasEffectType(SkillEffectScope effectScope, EffectTypes effectTypes)
    {
        if (effectScope < 0 || (int)effectScope >= _effectTypesByScope.Length)
            return false;

        return (_effectTypesByScope[(int)effectScope] & effectTypes) != 0;
    }

    /// <summary>
    /// The skill conditions by scope.
    /// </summary>
    public ImmutableArray<ISkillConditionBase> GetConditions(SkillConditionScope skillConditionScope)
    {
        if (skillConditionScope >= 0 && (int)skillConditionScope < _conditionLists.Length)
            return _conditionLists[(int)skillConditionScope];

        return ImmutableArray<ISkillConditionBase>.Empty;
    }

    /// <summary>
    /// Returns true to set static reuse.
    /// </summary>
    public bool IsStaticReuse { get; }

    /// <summary>
    /// Verify if this skill is abnormal instant. Herb buff skills yield true for this check.
    /// </summary>
    public bool IsAbnormalInstant { get; }

    public bool IsStayAfterDeath => _stayAfterDeath || IsIrreplacableBuff || IsNecessaryToggle;

    /// <summary>
    /// Verify if this skill is coming from Recovery Herb.
    /// </summary>
    public bool IsRecoveryHerb { get; }

    /// <summary>
    /// True if skill effects should be removed on any action except movement.
    /// </summary>
    /// <value></value>
    public bool IsRemovedOnAnyActionExceptMove { get; }

    /// <summary>
    /// True if skill effects should be removed on damage.
    /// </summary>
    public bool IsRemovedOnDamage { get; }

    /// <summary>
    /// True if skill effects should be removed on unequip weapon.
    /// </summary>
    /// <value></value>
    public bool IsRemovedOnUnequipWeapon { get; }

    /// <summary>
    /// True if skill cannot be used in olympiad.
    /// </summary>
    public bool IsBlockedInOlympiad { get; }

    /// <summary>
    /// True if the skill will take activation buff slot instead of a normal buff slot.
    /// </summary>
    public bool IsTriggeredSkill { get; }

    /// <summary>
    /// Verify if this skill is a debuff.
    /// </summary>
    public bool IsDebuff { get; }

    public bool IsSuicideAttack { get; }

    public bool CanBeDispelled { get; }

    public bool IsExcludedFromCheck { get; }

    public bool IsWithoutAction { get; }

    public bool IsMentoring { get; }

    public bool CanDoubleCast { get; }

    public bool CanCastWhileDisabled { get; }

    public bool IsSharedWithSummon { get; }

    public bool IsNecessaryToggle { get; }

    public bool IsDeleteAbnormalOnLeave { get; }

    /// <summary>
    /// True if the buff cannot be replaced, canceled, removed on death, etc.
    /// It can be only overriden by higher stack, but buff still remains ticking and
    /// activates once the higher stack buff has passed away.
    /// </summary>
    public bool IsIrreplacableBuff { get; }

    /// <summary>
    /// True if skill could not be requested for use by players.
    /// Blocks the use skill client action and is not showed on skill list.
    /// </summary>
    /// <value></value>
    public bool IsBlockActionUseSkill { get; }

    public bool IsDisplayInList { get; }

    public bool IsHidingMessages => _isHidingMessages || OperateType.isHidingMessages();

    public bool IsAoe =>
        AffectScope is AffectScope.FAN or AffectScope.FAN_PB or AffectScope.POINT_BLANK or AffectScope.RANGE
            or AffectScope.RING_RANGE or AffectScope.SQUARE or AffectScope.SQUARE_PB;


    public bool AllowOnTransform => IsPassive;

    /// <summary>
    /// Verify if the skill is a transformation skill.
    /// </summary>
    public bool IsTransformation => AbnormalType is AbnormalType.TRANSFORM or AbnormalType.CHANGEBODY;

    public bool UseSoulShot => HasEffectType(EffectTypes.PHYSICAL_ATTACK | EffectTypes.PHYSICAL_ATTACK_HP_LINK);

    public bool UseSpiritShot => MagicType == SkillMagicType.Magic;

    public bool UseFishShot => HasEffectType(EffectTypes.FISHING);

    public bool Is7Signs => Id is > 4360 and < 4367;

    /// <summary>
    /// Verify if this is a healing potion skill.
    /// </summary>
    public bool IsHealingPotionSkill => AbnormalType == AbnormalType.HP_RECOVER;

    public bool IsBad => EffectPoint < 0;

    public override string ToString() => $"Skill {Name} ({Id},{Level},{SubLevel})";

    public override int GetHashCode() => HashCode.Combine(Id, Level, SubLevel);

    /// <summary>
    /// Constructs the skill hash.
    /// </summary>
    public static long GetSkillHashCode(int skillId, int skillLevel) => skillId * 4294967296 + skillLevel;

    /// <summary>
    /// Constructs the skill hash.
    /// </summary>
    public static long GetSkillHashCode(int skillId, int skillLevel, int skillSubLevel) =>
        skillId * 4294967296 + skillSubLevel * 65536 + skillLevel;

    /// <summary>
    /// Returns the skill hash.
    /// </summary>
    public long GetSkillHashCode() => GetSkillHashCode(Id, Level, SubLevel);

    /// <summary>
    /// Parses all the abnormal visual effects.
    /// </summary>
    private static FrozenSet<AbnormalVisualEffect> ParseAbnormalVisualEffect(int skillId, string str)
    {
        FrozenSet<AbnormalVisualEffect> set = ParseList<AbnormalVisualEffect>(skillId, str);
        if (set.Contains(AbnormalVisualEffect.None))
            throw new InvalidOperationException(
                $"Invalid skill id={skillId} definition: invalid AbnormalVisualEffect '{str}'");

        return set;
    }

    private static FrozenSet<T> ParseList<T>(int skillId, string str, char separator = ';')
        where T: unmanaged, Enum
    {
        if (!string.IsNullOrEmpty(str))
        {
            HashSet<T> vals = [];
            foreach (string s in str.Split(separator))
            {
                if (!Enum.TryParse(s, true, out T v))
                    throw new InvalidOperationException(
                        $"Invalid skill id={skillId} definition: invalid {typeof(T).Name} '{s}'");

                vals.Add(v);
            }

            if (vals.Count != 0)
                return vals.ToFrozenSet();
        }

        return FrozenSet<T>.Empty;
    }
}