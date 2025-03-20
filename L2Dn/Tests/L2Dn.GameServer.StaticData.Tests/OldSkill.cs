using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.StaticData.Tests;

public sealed class OldSkill: IIdentifiable
{
    /** Skill ID. */
    private readonly int _id;

    /** Skill level. */
    private readonly int _level;

    /** Skill sub level. */
    private readonly int _subLevel;

    /** Custom skill ID displayed by the client. */
    private readonly int _displayId;

    /** Custom skill level displayed by the client. */
    private readonly int _displayLevel;

    /** Skill client's name. */
    private readonly string _name;

    /** Operative type: passive, active, toggle. */
    private readonly SkillOperateType _operateType;

    private readonly int _magic;
    private readonly TraitType _traitType;
    private readonly bool _staticReuse;

    /** MP consumption. */
    private readonly int _mpConsume;

    /** Initial MP consumption. */
    private readonly int _mpInitialConsume;

    /** MP consumption per channeling. */
    private readonly int _mpPerChanneling;

    /** HP consumption. */
    private readonly int _hpConsume;

    /** Amount of items consumed by this skill from caster. */
    private readonly int _itemConsumeCount;

    /** Id of item consumed by this skill from caster. */
    private readonly int _itemConsumeId;

    /** Fame points consumed by this skill from caster */
    private readonly int _famePointConsume;

    /** Clan points consumed by this skill from caster's clan */
    private readonly int _clanRepConsume;

    /** Cast range: how far can be the target. */
    private readonly int _castRange;

    /** Effect range: how far the skill affect the target. */
    private readonly int _effectRange;

    /** Abnormal instant, used for herbs mostly. */
    private readonly bool _isAbnormalInstant;

    /** Abnormal level, global effect level. */
    private readonly int _abnormalLevel;

    /** Abnormal type: global effect "group". */
    private readonly AbnormalType _abnormalType;

    /** Abnormal type: local effect "group". */
    private readonly AbnormalType _subordinationAbnormalType;

    /** Abnormal time: global effect duration time. */
    private readonly TimeSpan? _abnormalTime;

    /** Abnormal visual effect: the visual effect displayed ingame. */
    private readonly FrozenSet<AbnormalVisualEffect> _abnormalVisualEffects;

    /** If {@code true} this skill's effect should stay after death. */
    private readonly bool _stayAfterDeath;

    /** If {@code true} this skill's effect recovery HP/MP or CP from herb. */
    private readonly bool _isRecoveryHerb;

    private readonly int _refId;

    // all times in milliseconds
    private readonly TimeSpan _hitTime;
    private readonly TimeSpan _hitCancelTime;
    private readonly TimeSpan _coolTime;
    private readonly long _reuseHashCode;
    private readonly TimeSpan _reuseDelay;
    private readonly int _reuseDelayGroup;

    private readonly int _magicLevel;
    private readonly int _lvlBonusRate;
    private readonly double? _activateRate;
    private readonly int _minChance;
    private readonly int _maxChance;

    // Effecting area of the skill, in radius.
    // The radius center varies according to the _targetType:
    // "caster" if targetType = AURA/PARTY/CLAN or "target" if targetType = AREA
    private readonly TargetType _targetType;
    private readonly AffectScope _affectScope;
    private readonly AffectObject _affectObject;
    private readonly int _affectRange;
    private readonly int[] _fanRange; // unk;startDegree;fanAffectRange;fanAffectAngle
    private readonly int[] _affectLimit; // TODO: Third value is unknown... find it out!
    private readonly int[] _affectHeight;

    private readonly NextActionType _nextAction;

    private readonly bool _removedOnAnyActionExceptMove;
    private readonly bool _removedOnDamage;
    private readonly bool _removedOnUnequipWeapon;

    private readonly bool _blockedInOlympiad;

    private readonly AttributeType _attributeType;
    private readonly int _attributeValue;

    private readonly BasicProperty _basicProperty;

    private readonly SocialClass _minPledgeClass;
    private readonly int _lightSoulMaxConsume;
    private readonly int _shadowSoulMaxConsume;
    private readonly int _chargeConsume;

    private readonly bool
        _isTriggeredSkill; // If true the skill will take activation buff slot instead of a normal buff slot

    private readonly int _effectPoint;

    private readonly Map<SkillConditionScope, List<ISkillCondition>> _conditionLists = [];
    private readonly Map<SkillEffectScope, List<AbstractEffect>> _effectLists = [];

    private readonly bool _isDebuff;

    private readonly bool _isSuicideAttack;
    private readonly bool _canBeDispelled;

    private readonly bool _excludedFromCheck;
    private readonly bool _withoutAction;

    private readonly string _icon;

    private volatile EffectType[]? _effectTypes;

    // Channeling data
    private readonly int _channelingSkillId;
    private readonly TimeSpan _channelingStart;
    private readonly TimeSpan _channelingTickInterval;

    // Mentoring
    private readonly bool _isMentoring;

    // Stance skill IDs
    private readonly int _doubleCastSkill;

    private readonly bool _canDoubleCast;
    private readonly bool _canCastWhileDisabled;
    private readonly bool _isSharedWithSummon;
    private readonly bool _isNecessaryToggle;
    private readonly bool _deleteAbnormalOnLeave;
    private readonly bool _irreplacableBuff; // Stays after death, on subclass change, cannot be canceled.
    private readonly bool _blockActionUseSkill; // Blocks the use skill client action and is not showed on skill list.

    private readonly int _toggleGroupId;
    private readonly int _attachToggleGroupId;
    private readonly ImmutableArray<AttachSkillHolder> _attachSkills;
    private readonly FrozenSet<AbnormalType> _abnormalResists;

    private readonly double _magicCriticalRate;
    private readonly SkillBuffType _buffType;
    private readonly bool _displayInList;
    private readonly bool _isHidingMessages;

    public OldSkill(StatSet set)
    {
        _id = set.getInt(".id");
        _level = set.getInt(".level");
        _subLevel = set.getInt(".subLevel", 0);
        _refId = set.getInt(".referenceId", 0);
        _displayId = set.getInt(".displayId", _id);
        _displayLevel = set.getInt(".displayLevel", _level);
        _name = set.getString(".name", string.Empty);
        _operateType = set.getEnum<SkillOperateType>("operateType");
        _magic = set.getInt("isMagic", 0);
        _traitType = set.getEnum("trait", TraitType.NONE);
        _staticReuse = set.getBoolean("staticReuse", false);
        _mpConsume = set.getInt("mpConsume", 0);
        _mpInitialConsume = set.getInt("mpInitialConsume", 0);
        _mpPerChanneling = set.getInt("mpPerChanneling", _mpConsume);
        _hpConsume = set.getInt("hpConsume", 0);
        _itemConsumeCount = set.getInt("itemConsumeCount", 0);
        _itemConsumeId = set.getInt("itemConsumeId", 0);
        _famePointConsume = set.getInt("famePointConsume", 0);
        _clanRepConsume = set.getInt("clanRepConsume", 0);
        _castRange = set.getInt("castRange", -1);
        _effectRange = set.getInt("effectRange", -1);
        _abnormalLevel = set.getInt("abnormalLevel", 0);
        _abnormalType = set.getEnum("abnormalType", AbnormalType.NONE);
        _subordinationAbnormalType = set.getEnum("subordinationAbnormalType", AbnormalType.NONE);
        TimeSpan abnormalTime = TimeSpan.FromSeconds(set.getDouble("abnormalTime", 0));
        if (Config.Character.ENABLE_MODIFY_SKILL_DURATION && _operateType != SkillOperateType.T &&
            Config.Character.SKILL_DURATION_LIST.TryGetValue(_id, out TimeSpan temp))
        {
            if (_level < 100 || _level > 140)
                abnormalTime = temp;
            else if (_level >= 100 && _level < 140)
                abnormalTime += temp;
        }

        _abnormalTime = abnormalTime;
        _isAbnormalInstant = set.getBoolean("abnormalInstant", false);
        _abnormalVisualEffects = ParseAbnormalVisualEffect(_id, set.getString("abnormalVisualEffect", string.Empty));
        _stayAfterDeath = set.getBoolean("stayAfterDeath", false);
        _hitTime = TimeSpan.FromMilliseconds(set.getInt("hitTime", 0));
        _hitCancelTime = TimeSpan.FromMilliseconds(set.getDouble("hitCancelTime", 0));
        _coolTime = TimeSpan.FromMilliseconds(set.getInt("coolTime", 0));
        _isDebuff = set.getBoolean("isDebuff", false);
        _isRecoveryHerb = set.getBoolean("isRecoveryHerb", false);
        if (!Config.Character.ENABLE_MODIFY_SKILL_REUSE ||
            !Config.Character.SKILL_REUSE_LIST.TryGetValue(_id, out _reuseDelay))
            _reuseDelay = TimeSpan.FromMilliseconds(set.getInt("reuseDelay", 0));

        _reuseDelayGroup = set.getInt("reuseDelayGroup", -1);
        _reuseHashCode = Skill.GetSkillHashCode(_reuseDelayGroup > 0 ? _reuseDelayGroup : _id, _level, _subLevel);
        _targetType = set.getEnum("targetType", TargetType.SELF);
        _affectScope = set.getEnum("affectScope", AffectScope.SINGLE);
        _affectObject = set.getEnum("affectObject", AffectObject.ALL);
        _affectRange = set.getInt("affectRange", 0);

        string fanRange = set.getString("fanRange", string.Empty);
        if (string.IsNullOrEmpty(fanRange))
            _fanRange = [0, 0, 0, 0];
        else
        {
            try
            {
                string[] valuesSplit = fanRange.Split(";");
                _fanRange =
                [
                    int.Parse(valuesSplit[0]), int.Parse(valuesSplit[1]), int.Parse(valuesSplit[2]),
                    int.Parse(valuesSplit[3]),
                ];
            }
            catch
            {
                throw new InvalidOperationException($"SkillId: {_id} invalid fanRange value: {fanRange}, " +
                    "'unk;startDegree;fanAffectRange;fanAffectAngle' required");
            }
        }

        string affectLimit = set.getString("affectLimit", string.Empty);
        if (string.IsNullOrEmpty(affectLimit))
            _affectLimit = [0, 0, 0];
        else
        {
            try
            {
                string[] valuesSplit = affectLimit.Split("-");
                _affectLimit = [int.Parse(valuesSplit[0]), int.Parse(valuesSplit[1]), 0];
                if (valuesSplit.Length > 2)
                    _affectLimit[2] = int.Parse(valuesSplit[2]);
            }
            catch
            {
                throw new InvalidOperationException($"SkillId: {_id} invalid affectLimit value: {affectLimit}, " +
                    "'minAffected-additionalRandom' required");
            }
        }

        string affectHeight = set.getString("affectHeight", string.Empty);
        if (string.IsNullOrEmpty(affectHeight))
            _affectHeight = [0, 0];
        else
        {
            try
            {
                string[] valuesSplit = affectHeight.Split(";");
                _affectHeight = [int.Parse(valuesSplit[0]), int.Parse(valuesSplit[1])];
            }
            catch
            {
                throw new InvalidOperationException(
                    $"SkillId: {_id} invalid affectHeight value: {affectHeight}, 'minHeight-maxHeight' required");
            }

            if (_affectHeight[0] > _affectHeight[1])
            {
                throw new InvalidOperationException($"SkillId: {_id} invalid affectHeight value: {affectHeight}, " +
                    "'minHeight-maxHeight' required, minHeight is higher than maxHeight!");
            }
        }

        _magicLevel = set.getInt("magicLevel", 0);
        _lvlBonusRate = set.getInt("lvlBonusRate", 0);
        _activateRate = set.contains("activateRate") ? set.getDouble("activateRate") : null;
        _minChance = set.getInt("minChance", Config.Character.MIN_ABNORMAL_STATE_SUCCESS_RATE);
        _maxChance = set.getInt("maxChance", Config.Character.MAX_ABNORMAL_STATE_SUCCESS_RATE);
        _nextAction = set.getEnum("nextAction", NextActionType.NONE);
        _removedOnAnyActionExceptMove = set.getBoolean("removedOnAnyActionExceptMove", false);
        _removedOnDamage = set.getBoolean("removedOnDamage", false);
        _removedOnUnequipWeapon = set.getBoolean("removedOnUnequipWeapon", false);
        _blockedInOlympiad = set.getBoolean("blockedInOlympiad", false);
        _attributeType = set.getEnum("attributeType", AttributeType.NONE);
        _attributeValue = set.getInt("attributeValue", 0);
        _basicProperty = set.getEnum("basicProperty", BasicProperty.NONE);
        _isSuicideAttack = set.getBoolean("isSuicideAttack", false);
        _minPledgeClass = (SocialClass)set.getInt("minPledgeClass", 0);
        _lightSoulMaxConsume = set.getInt("lightSoulMaxConsume", 0);
        _shadowSoulMaxConsume = set.getInt("shadowSoulMaxConsume", 0);
        _chargeConsume = set.getInt("chargeConsume", 0);
        _isTriggeredSkill = set.getBoolean("isTriggeredSkill", false);
        _effectPoint = set.getInt("effectPoint", 0);
        _canBeDispelled = set.getBoolean("canBeDispelled", true);
        _excludedFromCheck = set.getBoolean("excludedFromCheck", false);
        _withoutAction = set.getBoolean("withoutAction", false);
        _icon = set.getString("icon", "icon.skill0000");
        _channelingSkillId = set.getInt("channelingSkillId", 0);
        _channelingTickInterval = TimeSpan.FromSeconds(set.getFloat("channelingTickInterval", 2f));
        _channelingStart = TimeSpan.FromSeconds(set.getFloat("channelingStart", 0f));
        _isMentoring = set.getBoolean("isMentoring", false);
        _doubleCastSkill = set.getInt("doubleCastSkill", 0);
        _canDoubleCast = set.getBoolean("canDoubleCast", false);
        _canCastWhileDisabled = set.getBoolean("canCastWhileDisabled", false);
        _isSharedWithSummon = set.getBoolean("isSharedWithSummon", true);
        _isNecessaryToggle = set.getBoolean("isNecessaryToggle", false);
        _deleteAbnormalOnLeave = set.getBoolean("deleteAbnormalOnLeave", false);
        _irreplacableBuff = set.getBoolean("irreplacableBuff", false);
        _blockActionUseSkill = set.getBoolean("blockActionUseSkill", false);
        _toggleGroupId = set.getInt("toggleGroupId", -1);
        _attachToggleGroupId = set.getInt("attachToggleGroupId", -1);
        _attachSkills = set.getList("attachSkillList", new List<StatSet>()).Select(AttachSkillHolder.FromStatSet).
            ToImmutableArray();

        string abnormalResist = set.getString("abnormalResists", string.Empty);
        _abnormalResists = ParseList<AbnormalType>(_id, abnormalResist);

        _magicCriticalRate = set.getDouble("magicCriticalRate", 0);
        _buffType = _isTriggeredSkill ? SkillBuffType.TRIGGER :
            isToggle() ? SkillBuffType.TOGGLE :
            isDance() ? SkillBuffType.DANCE :
            _isDebuff ? SkillBuffType.DEBUFF :
            !isHealingPotionSkill() ? SkillBuffType.BUFF : SkillBuffType.NONE;

        _displayInList = set.getBoolean("displayInList", true);
        _isHidingMessages = set.getBoolean("isHidingMessages", false);
    }

    public TraitType getTraitType() => _traitType;

    public AttributeType getAttributeType() => _attributeType;

    public int getAttributeValue() => _attributeValue;

    public bool isAOE() =>
        _affectScope is AffectScope.FAN or AffectScope.FAN_PB or AffectScope.POINT_BLANK or AffectScope.RANGE
            or AffectScope.RING_RANGE or AffectScope.SQUARE or AffectScope.SQUARE_PB;

    public bool isSuicideAttack() => _isSuicideAttack;

    public bool allowOnTransform() => isPassive();

    /**
     * Verify if this skill is abnormal instant.
     * Herb buff skills yield {@code true} for this check.
     * @return {@code true} if the skill is abnormal instant, {@code false} otherwise
     */
    public bool isAbnormalInstant() => _isAbnormalInstant;

    /**
     * Gets the skill abnormal type.
     * @return the abnormal type
     */
    public AbnormalType getAbnormalType() => _abnormalType;

    /**
     * Gets the skill subordination abnormal type.
     * @return the abnormal type
     */
    public AbnormalType getSubordinationAbnormalType() => _subordinationAbnormalType;

    /**
     * Gets the skill abnormal level.
     * @return the skill abnormal level
     */
    public int getAbnormalLevel() => _abnormalLevel;

    /**
     * Gets the skill abnormal time.
     * Is the base to calculate the duration of the continuous effects of this skill.
     * @return the abnormal time
     */
    public TimeSpan? getAbnormalTime() => _abnormalTime;

    /**
     * Gets the skill abnormal visual effect.
     * @return the abnormal visual effect
     */
    public FrozenSet<AbnormalVisualEffect> getAbnormalVisualEffects() => _abnormalVisualEffects;

    /**
     * Verify if the skill has abnormal visual effects.
     * @return {@code true} if the skill has abnormal visual effects, {@code false} otherwise
     */
    public bool hasAbnormalVisualEffects() => _abnormalVisualEffects.Count != 0;

    /**
     * Gets the skill magic level.
     * @return the skill magic level
     */
    public int getMagicLevel() => _magicLevel;

    public int getLvlBonusRate() => _lvlBonusRate;

    public double? getActivateRate() => _activateRate;

    /**
     * Return custom minimum skill/effect chance.
     * @return
     */
    public int getMinChance() => _minChance;

    /**
     * Return custom maximum skill/effect chance.
     * @return
     */
    public int getMaxChance() => _maxChance;

    /**
     * Return true if skill effects should be removed on any action except movement
     * @return
     */
    public bool isRemovedOnAnyActionExceptMove() => _removedOnAnyActionExceptMove;

    /**
     * @return {@code true} if skill effects should be removed on damage
     */
    public bool isRemovedOnDamage() => _removedOnDamage;

    /**
     * @return {@code true} if skill effects should be removed on unequip weapon
     */
    public bool isRemovedOnUnequipWeapon() => _removedOnUnequipWeapon;

    /**
     * @return {@code true} if skill can not be used in olympiad.
     */
    public bool isBlockedInOlympiad() => _blockedInOlympiad;

    /**
     * Return the additional effect Id.
     * @return
     */
    public int getChannelingSkillId() => _channelingSkillId;

    /**
     * Return character action after cast
     * @return
     */
    public NextActionType getNextAction() => _nextAction;

    /**
     * @return Returns the castRange.
     */
    public int getCastRange() => _castRange;

    /**
     * @return Returns the effectRange.
     */
    public int getEffectRange() => _effectRange;

    /**
     * @return Returns the hpConsume.
     */
    public int getHpConsume() => _hpConsume;

    /**
     * Gets the skill ID.
     * @return the skill ID
     */
    public int Id => _id;

    /**
     * Verify if this skill is a debuff.
     * @return {@code true} if this skill is a debuff, {@code false} otherwise
     */
    public bool isDebuff() => _isDebuff;

    /**
     * Verify if this skill is coming from Recovery Herb.
     * @return {@code true} if this skill is a recover herb, {@code false} otherwise
     */
    public bool isRecoveryHerb() => _isRecoveryHerb;

    public int getDisplayId() => _displayId;

    public int getDisplayLevel() => _displayLevel;

    /**
     * Return skill basic property type.
     * @return
     */
    public BasicProperty getBasicProperty() => _basicProperty;

    /**
     * @return Returns the how much items will be consumed.
     */
    public int getItemConsumeCount() => _itemConsumeCount;

    /**
     * @return Returns the ID of item for consume.
     */
    public int getItemConsumeId() => _itemConsumeId;

    /**
     * @return Fame points consumed by this skill from caster
     */
    public int getFamePointConsume() => _famePointConsume;

    /**
     * @return Clan points consumed by this skill from caster's clan
     */
    public int getClanRepConsume() => _clanRepConsume;

    /**
     * @return Returns the level.
     */
    public int getLevel() => _level;

    /**
     * @return Returns the sub level.
     */
    public int getSubLevel() => _subLevel;

    /**
     * @return isMagic integer value from the XML.
     */
    public int getMagicType() => _magic; // TODO: enum

    /**
     * @return Returns true to set physical skills.
     */
    public bool isPhysical() => _magic == 0;

    /**
     * @return Returns true to set magic skills.
     */
    public bool isMagic() => _magic == 1;

    /**
     * @return Returns true to set static skills.
     */
    public bool isStatic() => _magic == 2;

    /**
     * @return Returns true to set dance skills.
     */
    public bool isDance() => _magic == 3;

    /**
     * @return Returns true to set static reuse.
     */
    public bool isStaticReuse() => _staticReuse;

    /**
     * @return Returns the mpConsume.
     */
    public int getMpConsume() => _mpConsume;

    /**
     * @return Returns the mpInitialConsume.
     */
    public int getMpInitialConsume() => _mpInitialConsume;

    /**
     * @return Mana consumption per channeling tick.
     */
    public int getMpPerChanneling() => _mpPerChanneling;

    /**
     * @return the skill name
     */
    public string getName() => _name;

    /**
     * @return the reuse delay
     */
    public TimeSpan getReuseDelay() => _reuseDelay;

    /**
     * @return the skill ID from which the reuse delay should be taken.
     */
    public int getReuseDelayGroup() => _reuseDelayGroup;

    public long getReuseHashCode() => _reuseHashCode;

    public TimeSpan getHitTime() => _hitTime;

    public TimeSpan getHitCancelTime() => _hitCancelTime;

    /**
     * @return the cool time
     */
    public TimeSpan getCoolTime() => _coolTime;

    /**
     * @return the target type of the skill : SELF, TARGET, SUMMON, GROUND...
     */
    public TargetType getTargetType() => _targetType;

    /**
     * @return the affect scope of the skill : SINGLE, FAN, SQUARE, PARTY, PLEDGE...
     */
    public AffectScope getAffectScope() => _affectScope;

    /**
     * @return the affect object of the skill : All, Clan, Friend, NotFriend, Invisible...
     */
    public AffectObject getAffectObject() => _affectObject;

    /**
     * @return the AOE range of the skill.
     */
    public int getAffectRange() => _affectRange;

    /**
     * @return the AOE fan range of the skill.
     */
    public int[] getFanRange() => _fanRange;

    /**
     * @return the maximum amount of targets the skill can affect or 0 if unlimited.
     */
    public int getAffectLimit()
    {
        if (_affectLimit[0] > 0 || _affectLimit[1] > 0)
        {
            return _affectLimit[0] + Rnd.get(_affectLimit[1]);
        }

        return 0;
    }

    public int getAffectHeightMin() => _affectHeight[0];

    public int getAffectHeightMax() => _affectHeight[1];

    public bool isActive() => _operateType.isActive();

    public bool isPassive() => _operateType.isPassive();

    public bool isToggle() => _operateType.isToggle();

    public bool isAura() => _operateType.isAura();

    public bool isHidingMessages() => _isHidingMessages || _operateType.isHidingMessages();

    public bool isNotBroadcastable() => _operateType.isNotBroadcastable();

    public bool isContinuous() => _operateType.isContinuous() || isSelfContinuous();

    public bool isFlyType() => _operateType.isFlyType();

    public bool isSelfContinuous() => _operateType.isSelfContinuous();

    public bool isChanneling() => _operateType.isChanneling();

    public bool isTriggeredSkill() => _isTriggeredSkill;

    public bool isSynergySkill() => _operateType.isSynergy();

    public SkillOperateType getOperateType() => _operateType;

    /**
     * Verify if the skill is a transformation skill.
     * @return {@code true} if the skill is a transformation, {@code false} otherwise
     */
    public bool isTransformation() => _abnormalType is AbnormalType.TRANSFORM or AbnormalType.CHANGEBODY;

    public int getEffectPoint() => _effectPoint;

    public bool useSoulShot() => hasEffectType(EffectType.PHYSICAL_ATTACK, EffectType.PHYSICAL_ATTACK_HP_LINK);

    public bool useSpiritShot() => _magic == 1;

    public bool useFishShot() => hasEffectType(EffectType.FISHING);

    public SocialClass getMinPledgeClass() => _minPledgeClass;

    public bool is7Signs() => _id is > 4360 and < 4367; // TODO: move these constants out

    /**
     * Verify if this is a healing potion skill.
     * @return {@code true} if this is a healing potion skill, {@code false} otherwise
     */
    public bool isHealingPotionSkill() => _abnormalType == AbnormalType.HP_RECOVER;

    public int getMaxLightSoulConsumeCount() => _lightSoulMaxConsume;

    public int getMaxShadowSoulConsumeCount() => _shadowSoulMaxConsume;

    public int getChargeConsumeCount() => _chargeConsume;

    public bool isStayAfterDeath() => _stayAfterDeath || _irreplacableBuff || _isNecessaryToggle;

    public bool isBad() => _effectPoint < 0;

    /**
     * Adds an effect to the effect list for the given effect scope.
     * @param effectScope the effect scope
     * @param effect the effect
     */
    public void addEffect(SkillEffectScope effectScope, AbstractEffect effect)
    {
        _effectLists.GetOrAdd(effectScope, _ => []).Add(effect);
    }

    /**
     * Gets the skill effects.
     * @param effectScope the effect scope
     * @return the list of effects for the give scope
     */
    public List<AbstractEffect>? getEffects(SkillEffectScope effectScope)
    {
        return _effectLists.get(effectScope);
    }

    /**
     * Verify if this skill has effects for the given scope.
     * @param effectScope the effect scope
     * @return {@code true} if this skill has effects for the given scope, {@code false} otherwise
     */
    public bool hasEffects(SkillEffectScope effectScope)
    {
        List<AbstractEffect>? effects = _effectLists.get(effectScope);
        return effects != null && effects.Count != 0;
    }

    /**
     * Adds a condition to the condition list for the given condition scope.
     * @param skillConditionScope the condition scope
     * @param skillCondition the condition
     */
    public void addCondition(SkillConditionScope skillConditionScope, ISkillCondition skillCondition)
    {
        _conditionLists.GetOrAdd(skillConditionScope, _ => []).Add(skillCondition);
    }

    public override string ToString() => $"Skill {_name} ({_id},{_level},{_subLevel})";

    /**
     * used for tracking item id in case that item consume cannot be used
     * @return reference item id
     */
    public int getReferenceItemId() => _refId;

    public bool canBeDispelled() => _canBeDispelled;

    public bool isExcludedFromCheck()
    {
        return _excludedFromCheck;
    }

    public bool isWithoutAction()
    {
        return _withoutAction;
    }

    /**
     * Parses all the abnormal visual effects.
     * @param abnormalVisualEffects the abnormal visual effects list
     */
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

    /**
     * @param effectType Effect type to check if its present on this skill effects.
     * @param effectTypes Effect types to check if are present on this skill effects.
     * @return {@code true} if at least one of specified {@link EffectType} types is present on this skill effects, {@code false} otherwise.
     */
    public bool hasEffectType(EffectType effectType, params ReadOnlySpan<EffectType> effectTypes)
    {
        if (_effectTypes == null)
        {
            lock (this)
            {
                if (_effectTypes == null)
                {
                    Set<EffectType> effectTypesSet = new();
                    foreach (List<AbstractEffect> effectList in _effectLists.Values)
                    {
                        if (effectList != null)
                        {
                            foreach (AbstractEffect effect in effectList)
                            {
                                effectTypesSet.add(effect.getEffectType());
                            }
                        }
                    }

                    EffectType[] effectTypesArray = effectTypesSet.ToArray();
                    Array.Sort(effectTypesArray);
                    _effectTypes = effectTypesArray;
                }
            }
        }

        if (Array.BinarySearch(_effectTypes, effectType) >= 0)
        {
            return true;
        }

        foreach (EffectType type in effectTypes)
        {
            if (Array.BinarySearch(_effectTypes, type) >= 0)
            {
                return true;
            }
        }

        return false;
    }

    /**
     * @param effectScope Effect Scope to look inside for the specific effect type.
     * @param effectType Effect type to check if its present on this skill effects.
     * @param effectTypes Effect types to check if are present on this skill effects.
     * @return {@code true} if at least one of specified {@link EffectType} types is present on this skill effects, {@code false} otherwise.
     */
    public bool hasEffectType(SkillEffectScope effectScope, params ReadOnlySpan<EffectType> effectTypes)
    {
        List<AbstractEffect>? effects = _effectLists.get(effectScope);
        if (hasEffects(effectScope) || effects == null)
            return false;

        foreach (AbstractEffect effect in effects)
        foreach (EffectType type in effectTypes)
        {
            if (type == effect.getEffectType())
                return true;
        }

        return false;
    }

    /**
     * @return icon of the current skill.
     */
    public string getIcon() => _icon;

    public TimeSpan getChannelingTickInterval() => _channelingTickInterval;

    public TimeSpan getChannelingTickInitialDelay() => _channelingStart;

    public bool isMentoring() => _isMentoring;

    public bool canDoubleCast() => _canDoubleCast;

    public int getDoubleCastSkill() => _doubleCastSkill;

    public bool canCastWhileDisabled() => _canCastWhileDisabled;

    public bool isSharedWithSummon() => _isSharedWithSummon;

    public bool isNecessaryToggle() => _isNecessaryToggle;

    public bool isDeleteAbnormalOnLeave() => _deleteAbnormalOnLeave;

    /**
     * @return {@code true} if the buff cannot be replaced, canceled, removed on death, etc.
     *         It can be only overriden by higher stack, but buff still remains ticking and activates once the higher stack buff has passed away.
     */
    public bool isIrreplacableBuff() => _irreplacableBuff;

    public bool isDisplayInList() => _displayInList;

    /**
     * @return if skill could not be requested for use by players.
     */
    public bool isBlockActionUseSkill() => _blockActionUseSkill;

    public int getToggleGroupId() => _toggleGroupId;

    public int getAttachToggleGroupId() => _attachToggleGroupId;

    public ImmutableArray<AttachSkillHolder> getAttachSkills() => _attachSkills;

    public FrozenSet<AbnormalType> getAbnormalResists() => _abnormalResists;

    public double getMagicCriticalRate() => _magicCriticalRate;

    public SkillBuffType getBuffType() => _buffType;
}