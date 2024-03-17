using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Skills;

public class Skill: IIdentifiable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Skill));

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
	private Set<AbnormalVisualEffect> _abnormalVisualEffects;

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
	private readonly int[] _fanRange = new int[4]; // unk;startDegree;fanAffectRange;fanAffectAngle
	private readonly int[] _affectLimit = new int[3]; // TODO: Third value is unknown... find it out!
	private readonly int[] _affectHeight = new int[2];

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

	private readonly Map<SkillConditionScope, List<ISkillCondition>> _conditionLists = new();
	private readonly Map<EffectScope, List<AbstractEffect>> _effectLists = new();

	private readonly bool _isDebuff;

	private readonly bool _isSuicideAttack;
	private readonly bool _canBeDispelled;

	private readonly bool _excludedFromCheck;
	private readonly bool _withoutAction;

	private readonly string _icon;

	private volatile EffectType[] _effectTypes;

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
	private readonly bool _irreplacableBuff; // Stays after death, on subclass change, cant be canceled.
	private readonly bool _blockActionUseSkill; // Blocks the use skill client action and is not showed on skill list.

	private readonly int _toggleGroupId;
	private readonly int _attachToggleGroupId;
	private readonly List<AttachSkillHolder> _attachSkills;
	private readonly Set<AbnormalType> _abnormalResists;

	private readonly double _magicCriticalRate;
	private readonly SkillBuffType _buffType;
	private readonly bool _displayInList;
	private readonly bool _isHidingMessages;

	public Skill(StatSet set)
	{
		_id = set.getInt(".id");
		_level = set.getInt(".level");
		_subLevel = set.getInt(".subLevel", 0);
		_refId = set.getInt(".referenceId", 0);
		_displayId = set.getInt(".displayId", _id);
		_displayLevel = set.getInt(".displayLevel", _level);
		_name = set.getString(".name", "");
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
		TimeSpan abnormalTime = TimeSpan.FromMilliseconds(set.getDouble("abnormalTime", 0));
		if (Config.ENABLE_MODIFY_SKILL_DURATION && Config.SKILL_DURATION_LIST.ContainsKey(_id) &&
		    (_operateType != SkillOperateType.T))
		{
			if ((_level < 100) || (_level > 140))
			{
				abnormalTime = Config.SKILL_DURATION_LIST[_id];
			}
			else if ((_level >= 100) && (_level < 140))
			{
				abnormalTime += Config.SKILL_DURATION_LIST[_id];
			}
		}

		_abnormalTime = abnormalTime;
		_isAbnormalInstant = set.getBoolean("abnormalInstant", false);
		parseAbnormalVisualEffect(set.getString("abnormalVisualEffect", null));
		_stayAfterDeath = set.getBoolean("stayAfterDeath", false);
		_hitTime = TimeSpan.FromMilliseconds(set.getInt("hitTime", 0));
		_hitCancelTime = TimeSpan.FromMilliseconds(set.getDouble("hitCancelTime", 0));
		_coolTime = TimeSpan.FromMilliseconds(set.getInt("coolTime", 0));
		_isDebuff = set.getBoolean("isDebuff", false);
		_isRecoveryHerb = set.getBoolean("isRecoveryHerb", false);
		if (Config.ENABLE_MODIFY_SKILL_REUSE && Config.SKILL_REUSE_LIST.TryGetValue(_id, out _reuseDelay))
		{
		}
		else
		{
			_reuseDelay = TimeSpan.FromMilliseconds(set.getInt("reuseDelay", 0));
		}

		_reuseDelayGroup = set.getInt("reuseDelayGroup", -1);
		_reuseHashCode = SkillData.getSkillHashCode(_reuseDelayGroup > 0 ? _reuseDelayGroup : _id, _level, _subLevel);
		_targetType = set.getEnum("targetType", TargetType.SELF);
		_affectScope = set.getEnum("affectScope", AffectScope.SINGLE);
		_affectObject = set.getEnum("affectObject", AffectObject.ALL);
		_affectRange = set.getInt("affectRange", 0);

		string fanRange = set.getString("fanRange", null);
		if (fanRange != null)
		{
			try
			{
				string[] valuesSplit = fanRange.Split(";");
				_fanRange[0] = int.Parse(valuesSplit[0]);
				_fanRange[1] = int.Parse(valuesSplit[1]);
				_fanRange[2] = int.Parse(valuesSplit[2]);
				_fanRange[3] = int.Parse(valuesSplit[3]);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException("SkillId: " + _id + " invalid fanRange value: " + fanRange +
				                                    ", \"unk;startDegree;fanAffectRange;fanAffectAngle\" required");
			}
		}

		string affectLimit = set.getString("affectLimit", null);
		if (affectLimit != null)
		{
			try
			{
				string[] valuesSplit = affectLimit.Split("-");
				_affectLimit[0] = int.Parse(valuesSplit[0]);
				_affectLimit[1] = int.Parse(valuesSplit[1]);
				if (valuesSplit.Length > 2)
				{
					_affectLimit[2] = int.Parse(valuesSplit[2]);
				}
			}
			catch (Exception e)
			{
				throw new InvalidOperationException("SkillId: " + _id + " invalid affectLimit value: " + affectLimit +
				                                    ", \"minAffected-additionalRandom\" required");
			}
		}

		string affectHeight = set.getString("affectHeight", null);
		if (affectHeight != null)
		{
			try
			{
				string[] valuesSplit = affectHeight.Split(";");
				_affectHeight[0] = int.Parse(valuesSplit[0]);
				_affectHeight[1] = int.Parse(valuesSplit[1]);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException("SkillId: " + _id + " invalid affectHeight value: " + affectHeight +
				                                    ", \"minHeight-maxHeight\" required");
			}

			if (_affectHeight[0] > _affectHeight[1])
			{
				throw new InvalidOperationException("SkillId: " + _id + " invalid affectHeight value: " + affectHeight +
				                                    ", \"minHeight-maxHeight\" required, minHeight is higher than maxHeight!");
			}
		}

		_magicLevel = set.getInt("magicLevel", 0);
		_lvlBonusRate = set.getInt("lvlBonusRate", 0);
		_activateRate = set.contains("activateRate") ? set.getDouble("activateRate") : null;
		_minChance = set.getInt("minChance", Config.MIN_ABNORMAL_STATE_SUCCESS_RATE);
		_maxChance = set.getInt("maxChance", Config.MAX_ABNORMAL_STATE_SUCCESS_RATE);
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
		_channelingTickInterval = TimeSpan.FromMilliseconds(set.getFloat("channelingTickInterval", 2000f) * 1000);
		_channelingStart = TimeSpan.FromMilliseconds(set.getFloat("channelingStart", 0f) * 1000);
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
		_attachSkills = set.getList<StatSet>("attachSkillList", new List<StatSet>())
			.Select(AttachSkillHolder.fromStatSet).ToList();

		string abnormalResist = set.getString("abnormalResists", null);
		if (abnormalResist != null)
		{
			string[] abnormalResistStrings = abnormalResist.Split(";");
			if (abnormalResistStrings.Length > 0)
			{
				_abnormalResists = new();
				foreach (string s in abnormalResistStrings)
				{
					try
					{
						_abnormalResists.add(Enum.Parse<AbnormalType>(s));
					}
					catch (Exception e)
					{
						LOGGER.Warn("Skill ID[" + _id + "] Expected AbnormalType for abnormalResists but found " + s +
						            ": " + e);
					}
				}
			}
			else
			{
				_abnormalResists = new();
			}
		}
		else
		{
			_abnormalResists = new();
		}

		_magicCriticalRate = set.getDouble("magicCriticalRate", 0);
		_buffType = _isTriggeredSkill ? SkillBuffType.TRIGGER :
			isToggle() ? SkillBuffType.TOGGLE :
			isDance() ? SkillBuffType.DANCE :
			_isDebuff ? SkillBuffType.DEBUFF :
			!isHealingPotionSkill() ? SkillBuffType.BUFF : SkillBuffType.NONE;

		_displayInList = set.getBoolean("displayInList", true);
		_isHidingMessages = set.getBoolean("isHidingMessages", false);
	}

	public TraitType getTraitType()
	{
		return _traitType;
	}

	public AttributeType getAttributeType()
	{
		return _attributeType;
	}

	public int getAttributeValue()
	{
		return _attributeValue;
	}

	public bool isAOE()
	{
		switch (_affectScope)
		{
			case AffectScope.FAN:
			case AffectScope.FAN_PB:
			case AffectScope.POINT_BLANK:
			case AffectScope.RANGE:
			case AffectScope.RING_RANGE:
			case AffectScope.SQUARE:
			case AffectScope.SQUARE_PB:
			{
				return true;
			}
		}

		return false;
	}

	public bool isSuicideAttack()
	{
		return _isSuicideAttack;
	}

	public bool allowOnTransform()
	{
		return isPassive();
	}

	/**
	 * Verify if this skill is abnormal instant.<br>
	 * Herb buff skills yield {@code true} for this check.
	 * @return {@code true} if the skill is abnormal instant, {@code false} otherwise
	 */
	public bool isAbnormalInstant()
	{
		return _isAbnormalInstant;
	}

	/**
	 * Gets the skill abnormal type.
	 * @return the abnormal type
	 */
	public AbnormalType getAbnormalType()
	{
		return _abnormalType;
	}

	/**
	 * Gets the skill subordination abnormal type.
	 * @return the abnormal type
	 */
	public AbnormalType getSubordinationAbnormalType()
	{
		return _subordinationAbnormalType;
	}

	/**
	 * Gets the skill abnormal level.
	 * @return the skill abnormal level
	 */
	public int getAbnormalLevel()
	{
		return _abnormalLevel;
	}

	/**
	 * Gets the skill abnormal time.<br>
	 * Is the base to calculate the duration of the continuous effects of this skill.
	 * @return the abnormal time
	 */
	public TimeSpan? getAbnormalTime()
	{
		return _abnormalTime;
	}

	/**
	 * Gets the skill abnormal visual effect.
	 * @return the abnormal visual effect
	 */
	public Set<AbnormalVisualEffect> getAbnormalVisualEffects()
	{
		return (_abnormalVisualEffects != null) ? _abnormalVisualEffects : new();
	}

	/**
	 * Verify if the skill has abnormal visual effects.
	 * @return {@code true} if the skill has abnormal visual effects, {@code false} otherwise
	 */
	public bool hasAbnormalVisualEffects()
	{
		return (_abnormalVisualEffects != null) && !_abnormalVisualEffects.isEmpty();
	}

	/**
	 * Gets the skill magic level.
	 * @return the skill magic level
	 */
	public int getMagicLevel()
	{
		return _magicLevel;
	}

	public int getLvlBonusRate()
	{
		return _lvlBonusRate;
	}

	public double? getActivateRate()
	{
		return _activateRate;
	}

	/**
	 * Return custom minimum skill/effect chance.
	 * @return
	 */
	public int getMinChance()
	{
		return _minChance;
	}

	/**
	 * Return custom maximum skill/effect chance.
	 * @return
	 */
	public int getMaxChance()
	{
		return _maxChance;
	}

	/**
	 * Return true if skill effects should be removed on any action except movement
	 * @return
	 */
	public bool isRemovedOnAnyActionExceptMove()
	{
		return _removedOnAnyActionExceptMove;
	}

	/**
	 * @return {@code true} if skill effects should be removed on damage
	 */
	public bool isRemovedOnDamage()
	{
		return _removedOnDamage;
	}

	/**
	 * @return {@code true} if skill effects should be removed on unequip weapon
	 */
	public bool isRemovedOnUnequipWeapon()
	{
		return _removedOnUnequipWeapon;
	}

	/**
	 * @return {@code true} if skill can not be used in olympiad.
	 */
	public bool isBlockedInOlympiad()
	{
		return _blockedInOlympiad;
	}

	/**
	 * Return the additional effect Id.
	 * @return
	 */
	public int getChannelingSkillId()
	{
		return _channelingSkillId;
	}

	/**
	 * Return character action after cast
	 * @return
	 */
	public NextActionType getNextAction()
	{
		return _nextAction;
	}

	/**
	 * @return Returns the castRange.
	 */
	public int getCastRange()
	{
		return _castRange;
	}

	/**
	 * @return Returns the effectRange.
	 */
	public int getEffectRange()
	{
		return _effectRange;
	}

	/**
	 * @return Returns the hpConsume.
	 */
	public int getHpConsume()
	{
		return _hpConsume;
	}

	/**
	 * Gets the skill ID.
	 * @return the skill ID
	 */
	public int getId()
	{
		return _id;
	}

	/**
	 * Verify if this skill is a debuff.
	 * @return {@code true} if this skill is a debuff, {@code false} otherwise
	 */
	public bool isDebuff()
	{
		return _isDebuff;
	}

	/**
	 * Verify if this skill is coming from Recovery Herb.
	 * @return {@code true} if this skill is a recover herb, {@code false} otherwise
	 */
	public bool isRecoveryHerb()
	{
		return _isRecoveryHerb;
	}

	public int getDisplayId()
	{
		return _displayId;
	}

	public int getDisplayLevel()
	{
		return _displayLevel;
	}

	/**
	 * Return skill basic property type.
	 * @return
	 */
	public BasicProperty getBasicProperty()
	{
		return _basicProperty;
	}

	/**
	 * @return Returns the how much items will be consumed.
	 */
	public int getItemConsumeCount()
	{
		return _itemConsumeCount;
	}

	/**
	 * @return Returns the ID of item for consume.
	 */
	public int getItemConsumeId()
	{
		return _itemConsumeId;
	}

	/**
	 * @return Fame points consumed by this skill from caster
	 */
	public int getFamePointConsume()
	{
		return _famePointConsume;
	}

	/**
	 * @return Clan points consumed by this skill from caster's clan
	 */
	public int getClanRepConsume()
	{
		return _clanRepConsume;
	}

	/**
	 * @return Returns the level.
	 */
	public int getLevel()
	{
		return _level;
	}

	/**
	 * @return Returns the sub level.
	 */
	public int getSubLevel()
	{
		return _subLevel;
	}

	/**
	 * @return isMagic integer value from the XML.
	 */
	public int getMagicType()
	{
		return _magic;
	}

	/**
	 * @return Returns true to set physical skills.
	 */
	public bool isPhysical()
	{
		return _magic == 0;
	}

	/**
	 * @return Returns true to set magic skills.
	 */
	public bool isMagic()
	{
		return _magic == 1;
	}

	/**
	 * @return Returns true to set static skills.
	 */
	public bool isStatic()
	{
		return _magic == 2;
	}

	/**
	 * @return Returns true to set dance skills.
	 */
	public bool isDance()
	{
		return _magic == 3;
	}

	/**
	 * @return Returns true to set static reuse.
	 */
	public bool isStaticReuse()
	{
		return _staticReuse;
	}

	/**
	 * @return Returns the mpConsume.
	 */
	public int getMpConsume()
	{
		return _mpConsume;
	}

	/**
	 * @return Returns the mpInitialConsume.
	 */
	public int getMpInitialConsume()
	{
		return _mpInitialConsume;
	}

	/**
	 * @return Mana consumption per channeling tick.
	 */
	public int getMpPerChanneling()
	{
		return _mpPerChanneling;
	}

	/**
	 * @return the skill name
	 */
	public string getName()
	{
		return _name;
	}

	/**
	 * @return the reuse delay
	 */
	public TimeSpan getReuseDelay()
	{
		return _reuseDelay;
	}

	/**
	 * @return the skill ID from which the reuse delay should be taken.
	 */
	public int getReuseDelayGroup()
	{
		return _reuseDelayGroup;
	}

	public long getReuseHashCode()
	{
		return _reuseHashCode;
	}

	public TimeSpan getHitTime()
	{
		return _hitTime;
	}

	public TimeSpan getHitCancelTime()
	{
		return _hitCancelTime;
	}

	/**
	 * @return the cool time
	 */
	public TimeSpan getCoolTime()
	{
		return _coolTime;
	}

	/**
	 * @return the target type of the skill : SELF, TARGET, SUMMON, GROUND...
	 */
	public TargetType getTargetType()
	{
		return _targetType;
	}

	/**
	 * @return the affect scope of the skill : SINGLE, FAN, SQUARE, PARTY, PLEDGE...
	 */
	public AffectScope getAffectScope()
	{
		return _affectScope;
	}

	/**
	 * @return the affect object of the skill : All, Clan, Friend, NotFriend, Invisible...
	 */
	public AffectObject getAffectObject()
	{
		return _affectObject;
	}

	/**
	 * @return the AOE range of the skill.
	 */
	public int getAffectRange()
	{
		return _affectRange;
	}

	/**
	 * @return the AOE fan range of the skill.
	 */
	public int[] getFanRange()
	{
		return _fanRange;
	}

	/**
	 * @return the maximum amount of targets the skill can affect or 0 if unlimited.
	 */
	public int getAffectLimit()
	{
		if ((_affectLimit[0] > 0) || (_affectLimit[1] > 0))
		{
			return (_affectLimit[0] + Rnd.get(_affectLimit[1]));
		}

		return 0;
	}

	public int getAffectHeightMin()
	{
		return _affectHeight[0];
	}

	public int getAffectHeightMax()
	{
		return _affectHeight[1];
	}

	public bool isActive()
	{
		return _operateType.isActive();
	}

	public bool isPassive()
	{
		return _operateType.isPassive();
	}

	public bool isToggle()
	{
		return _operateType.isToggle();
	}

	public bool isAura()
	{
		return _operateType.isAura();
	}

	public bool isHidingMessages()
	{
		return _isHidingMessages || _operateType.isHidingMessages();
	}

	public bool isNotBroadcastable()
	{
		return _operateType.isNotBroadcastable();
	}

	public bool isContinuous()
	{
		return _operateType.isContinuous() || isSelfContinuous();
	}

	public bool isFlyType()
	{
		return _operateType.isFlyType();
	}

	public bool isSelfContinuous()
	{
		return _operateType.isSelfContinuous();
	}

	public bool isChanneling()
	{
		return _operateType.isChanneling();
	}

	public bool isTriggeredSkill()
	{
		return _isTriggeredSkill;
	}

	public bool isSynergySkill()
	{
		return _operateType.isSynergy();
	}

	public SkillOperateType getOperateType()
	{
		return _operateType;
	}

	/**
	 * Verify if the skill is a transformation skill.
	 * @return {@code true} if the skill is a transformation, {@code false} otherwise
	 */
	public bool isTransformation()
	{
		return (_abnormalType == AbnormalType.TRANSFORM) || (_abnormalType == AbnormalType.CHANGEBODY);
	}

	public int getEffectPoint()
	{
		return _effectPoint;
	}

	public bool useSoulShot()
	{
		return hasEffectType(EffectType.PHYSICAL_ATTACK, EffectType.PHYSICAL_ATTACK_HP_LINK);
	}

	public bool useSpiritShot()
	{
		return _magic == 1;
	}

	public bool useFishShot()
	{
		return hasEffectType(EffectType.FISHING);
	}

	public SocialClass getMinPledgeClass()
	{
		return _minPledgeClass;
	}

	public bool isHeroSkill()
	{
		return SkillTreeData.getInstance().isHeroSkill(_id, _level);
	}

	public bool isGMSkill()
	{
		return SkillTreeData.getInstance().isGMSkill(_id, _level);
	}

	public bool is7Signs()
	{
		return (_id > 4360) && (_id < 4367);
	}

	/**
	 * Verify if this is a healing potion skill.
	 * @return {@code true} if this is a healing potion skill, {@code false} otherwise
	 */
	public bool isHealingPotionSkill()
	{
		return _abnormalType == AbnormalType.HP_RECOVER;
	}

	public int getMaxLightSoulConsumeCount()
	{
		return _lightSoulMaxConsume;
	}

	public int getMaxShadowSoulConsumeCount()
	{
		return _shadowSoulMaxConsume;
	}

	public int getChargeConsumeCount()
	{
		return _chargeConsume;
	}

	public bool isStayAfterDeath()
	{
		return _stayAfterDeath || _irreplacableBuff || _isNecessaryToggle;
	}

	public bool isBad()
	{
		return _effectPoint < 0;
	}

	public bool checkCondition(Creature creature, WorldObject @object, bool sendMessage)
	{
		if (creature.isFakePlayer() || (creature.canOverrideCond(PlayerCondOverride.SKILL_CONDITIONS) &&
		                                !Config.GM_SKILL_RESTRICTION))
		{
			return true;
		}

		if (creature.isPlayer() && creature.getActingPlayer().isMounted() && isBad() &&
		    !MountEnabledSkillList.contains(_id))
		{
			SystemMessagePacket sm =
				new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
			sm.Params.addSkillName(_id);
			creature.sendPacket(sm);
			return false;
		}

		if (!checkConditions(SkillConditionScope.GENERAL, creature, @object) ||
		    !checkConditions(SkillConditionScope.TARGET, creature, @object))
		{
			if (sendMessage &&
			    !((creature == @object) && isBad())) // Self targeted bad skills should not send a message.
			{
				SystemMessagePacket sm =
					new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
				sm.Params.addSkillName(_id);
				creature.sendPacket(sm);
			}

			return false;
		}

		return true;
	}

	/**
	 * @param creature the creature that requests getting the skill target.
	 * @param forceUse if character pressed ctrl (force pick target)
	 * @param dontMove if character pressed shift (dont move and pick target only if in range)
	 * @param sendMessage send SystemMessageId packet if target is incorrect.
	 * @return {@code WorldObject} this skill can be used on, or {@code null} if there is no such.
	 */
	public WorldObject getTarget(Creature creature, bool forceUse, bool dontMove, bool sendMessage)
	{
		return getTarget(creature, creature.getTarget(), forceUse, dontMove, sendMessage);
	}

	/**
	 * @param creature the creature that requests getting the skill target.
	 * @param seletedTarget the target that has been selected by this character to be checked.
	 * @param forceUse if character pressed ctrl (force pick target)
	 * @param dontMove if character pressed shift (dont move and pick target only if in range)
	 * @param sendMessage send SystemMessageId packet if target is incorrect.
	 * @return the selected {@code WorldObject} this skill can be used on, or {@code null} if there is no such.
	 */
	public WorldObject getTarget(Creature creature, WorldObject seletedTarget, bool forceUse, bool dontMove,
		bool sendMessage)
	{
		ITargetTypeHandler handler = TargetHandler.getInstance().getHandler(getTargetType());
		if (handler != null)
		{
			try
			{
				return handler.getTarget(creature, seletedTarget, this, forceUse, dontMove, sendMessage);
			}
			catch (Exception e)
			{
				LOGGER.Warn("Exception in Skill.getTarget(): " + e);
			}
		}

		creature.sendMessage("Target type of skill " + this + " is not currently handled.");
		return null;
	}

	/**
	 * @param creature the creature that needs to gather targets.
	 * @param target the initial target activeChar is focusing upon.
	 * @return list containing objects gathered in a specific geometric way that are valid to be affected by this skill.
	 */
	public List<WorldObject> getTargetsAffected(Creature creature, WorldObject target)
	{
		if (target == null)
		{
			return null;
		}

		IAffectScopeHandler handler = AffectScopeHandler.getInstance().getHandler(getAffectScope());
		if (handler != null)
		{
			try
			{
				List<WorldObject> result = new();
				handler.forEachAffected<WorldObject>(creature, target, this, x => result.Add(x));
				return result;
			}
			catch (Exception e)
			{
				LOGGER.Warn("Exception in Skill.getTargetsAffected(): " + e);
			}
		}

		creature.sendMessage("Target affect scope of skill " + this + " is not currently handled.");
		return null;
	}

	/**
	 * @param creature the creature that needs to gather targets.
	 * @param target the initial target activeChar is focusing upon.
	 * @param action for each affected target.
	 */
	public void forEachTargetAffected<T>(Creature creature, WorldObject target, Action<T> action)
		where T: WorldObject
	{
		if (target == null)
		{
			return;
		}

		IAffectScopeHandler handler = AffectScopeHandler.getInstance().getHandler(getAffectScope());
		if (handler != null)
		{
			try
			{
				handler.forEachAffected(creature, target, this, action);
			}
			catch (Exception e)
			{
				LOGGER.Warn("Exception in Skill.forEachTargetAffected(): " + e);
			}
		}
		else
		{
			creature.sendMessage("Target affect scope of skill " + this + " is not currently handled.");
		}
	}

	/**
	 * Adds an effect to the effect list for the given effect scope.
	 * @param effectScope the effect scope
	 * @param effect the effect
	 */
	public void addEffect(EffectScope effectScope, AbstractEffect effect)
	{
		_effectLists.computeIfAbsent(effectScope, k => new()).Add(effect);
	}

	/**
	 * Gets the skill effects.
	 * @param effectScope the effect scope
	 * @return the list of effects for the give scope
	 */
	public List<AbstractEffect> getEffects(EffectScope effectScope)
	{
		return _effectLists.get(effectScope);
	}

	/**
	 * Verify if this skill has effects for the given scope.
	 * @param effectScope the effect scope
	 * @return {@code true} if this skill has effects for the given scope, {@code false} otherwise
	 */
	public bool hasEffects(EffectScope effectScope)
	{
		List<AbstractEffect> effects = _effectLists.get(effectScope);
		return (effects != null) && effects.Count != 0;
	}

	/**
	 * Applies the effects from this skill to the target for the given effect scope.
	 * @param effectScope the effect scope
	 * @param info the buff info
	 * @param applyInstantEffects if {@code true} instant effects will be applied to the effected
	 * @param addContinuousEffects if {@code true} continuous effects will be applied to the effected
	 */
	public void applyEffectScope(EffectScope? effectScope, BuffInfo info, bool applyInstantEffects,
		bool addContinuousEffects)
	{
		if ((effectScope != null) && hasEffects(effectScope.Value))
		{
			foreach (AbstractEffect effect in getEffects(effectScope.Value))
			{
				if (effect.isInstant())
				{
					if (applyInstantEffects && effect.calcSuccess(info.getEffector(), info.getEffected(), this))
					{
						effect.instant(info.getEffector(), info.getEffected(), this, info.getItem());
					}
				}
				else if (addContinuousEffects)
				{
					if (applyInstantEffects)
					{
						effect.continuousInstant(info.getEffector(), info.getEffected(), this, info.getItem());
					}

					if (effect.canStart(info.getEffector(), info.getEffected(), this))
					{
						info.addEffect(effect);
					}

					// tempfix for hp/mp regeneration
					// TODO: Find where regen stops and make a proper fix
					if (info.getEffected().isPlayer() && !isBad())
					{
						info.getEffected().getActingPlayer().getStatus().startHpMpRegeneration();
					}
				}
			}
		}
	}

	/**
	 * Method overload for {@link Skill#applyEffects(Creature, Creature, bool, bool, bool, int, Item)}.<br>
	 * Simplify the calls.
	 * @param effector the caster of the skill
	 * @param effected the target of the effect
	 */
	public void applyEffects(Creature effector, Creature effected)
	{
		applyEffects(effector, effected, false, false, true, TimeSpan.Zero, null);
	}

	/**
	 * Method overload for {@link Skill#applyEffects(Creature, Creature, bool, bool, bool, int, Item)}.<br>
	 * Simplify the calls.
	 * @param effector the caster of the skill
	 * @param effected the target of the effect
	 * @param item
	 */
	public void applyEffects(Creature effector, Creature effected, Item item)
	{
		applyEffects(effector, effected, false, false, true, TimeSpan.Zero, item);
	}

	/**
	 * Method overload for {@link Skill#applyEffects(Creature, Creature, bool, bool, bool, int, Item)}.<br>
	 * Simplify the calls, allowing abnormal time time customization.
	 * @param effector the caster of the skill
	 * @param effected the target of the effect
	 * @param instant if {@code true} instant effects will be applied to the effected
	 * @param abnormalTime custom abnormal time, if equal or lesser than zero will be ignored
	 */
	public void applyEffects(Creature effector, Creature effected, bool instant, TimeSpan abnormalTime)
	{
		applyEffects(effector, effected, false, false, instant, abnormalTime, null);
	}

	/**
	 * Applies the effects from this skill to the target.
	 * @param effector the caster of the skill
	 * @param effected the target of the effect
	 * @param self if {@code true} self-effects will be casted on the caster
	 * @param passive if {@code true} passive effects will be applied to the effector
	 * @param instant if {@code true} instant effects will be applied to the effected
	 * @param abnormalTime custom abnormal time, if equal or lesser than zero will be ignored
	 * @param item
	 */
	public void applyEffects(Creature effector, Creature effected, bool self, bool passive, bool instant,
		TimeSpan abnormalTime, Item item)
	{
		// null targets cannot receive any effects.
		if (effected == null)
		{
			return;
		}

		if (effected.isIgnoringSkillEffects(_id, _level))
		{
			return;
		}

		bool addContinuousEffects = !passive && (_operateType.isToggle() ||
		                                         (_operateType.isContinuous() &&
		                                          Formulas.calcEffectSuccess(effector, effected, this)));
		if (!self && !passive)
		{
			BuffInfo info = new BuffInfo(effector, effected, this, !instant, item, null);
			if (addContinuousEffects && (abnormalTime > TimeSpan.Zero))
			{
				info.setAbnormalTime(abnormalTime);
			}

			applyEffectScope(EffectScope.GENERAL, info, instant, addContinuousEffects);

			EffectScope? pvpOrPveEffectScope = effector.isPlayable() && effected.isAttackable() ? EffectScope.PVE :
				effector.isPlayable() && effected.isPlayable() ? EffectScope.PVP : null;
			applyEffectScope(pvpOrPveEffectScope, info, instant, addContinuousEffects);
			if (addContinuousEffects)
			{
				// Aura skills reset the abnormal time.
				BuffInfo existingInfo =
					_operateType.isAura() ? effected.getEffectList().getBuffInfoBySkillId(_id) : null;
				if (existingInfo != null)
				{
					existingInfo.resetAbnormalTime(info.getAbnormalTime());
				}
				else
				{
					effected.getEffectList().add(info);
				}

				// Check for mesmerizing debuffs and increase resist level.
				if (_isDebuff && (_basicProperty != BasicProperty.NONE) && effected.hasBasicPropertyResist())
				{
					BasicPropertyResist resist = effected.getBasicPropertyResist(_basicProperty);
					resist.increaseResistLevel();
				}
			}

			// Support for buff sharing feature including healing herbs.
			if (_isSharedWithSummon && effected.isPlayer() && !isTransformation() &&
			    ((addContinuousEffects && isContinuous() && !_isDebuff) || _isRecoveryHerb))
			{
				if (effected.hasServitors())
				{
					effected.getServitors().values()
						.ForEach(s => applyEffects(effector, s, _isRecoveryHerb, TimeSpan.Zero));
				}

				if (effected.hasPet())
				{
					applyEffects(effector, effector.getPet(), _isRecoveryHerb, TimeSpan.Zero);
				}
			}
		}

		if (self)
		{
			addContinuousEffects = !passive && (_operateType.isToggle() ||
			                                    (_operateType.isSelfContinuous() &&
			                                     Formulas.calcEffectSuccess(effector, effector, this)));

			BuffInfo info = new BuffInfo(effector, effector, this, !instant, item, null);
			if (addContinuousEffects && (abnormalTime > TimeSpan.Zero))
			{
				info.setAbnormalTime(abnormalTime);
			}

			applyEffectScope(EffectScope.SELF, info, instant, addContinuousEffects);
			if (addContinuousEffects)
			{
				// Aura skills reset the abnormal time.
				BuffInfo existingInfo =
					_operateType.isAura() ? effector.getEffectList().getBuffInfoBySkillId(_id) : null;
				if (existingInfo != null)
				{
					existingInfo.resetAbnormalTime(info.getAbnormalTime());
				}
				else
				{
					info.getEffector().getEffectList().add(info);
				}
			}

			// Support for buff sharing feature.
			// Avoiding Servitor Share since it's implementation already "shares" the effect.
			if (addContinuousEffects && _isSharedWithSummon && info.getEffected().isPlayer() && isContinuous() &&
			    !_isDebuff && info.getEffected().hasServitors())
			{
				info.getEffected().getServitors().values()
					.forEach(s => applyEffects(effector, s, false, TimeSpan.Zero));
			}
		}

		if (passive)
		{
			BuffInfo info = new BuffInfo(effector, effector, this, true, item, null);
			applyEffectScope(EffectScope.GENERAL, info, false, true);
			effector.getEffectList().add(info);
		}
	}

	/**
	 * Applies the channeling effects from this skill to the target.
	 * @param effector the caster of the skill
	 * @param effected the target of the effect
	 */
	public void applyChannelingEffects(Creature effector, Creature effected)
	{
		// null targets cannot receive any effects.
		if (effected == null)
		{
			return;
		}

		BuffInfo info = new BuffInfo(effector, effected, this, false, null, null);
		applyEffectScope(EffectScope.CHANNELING, info, true, true);
	}

	/**
	 * Activates a skill for the given creature and targets.
	 * @param caster the caster
	 * @param targets the targets
	 */
	public void activateSkill(Creature caster, params WorldObject[] targets)
	{
		activateSkill(caster, null, targets);
	}

	/**
	 * Activates the skill to the targets.
	 * @param caster the caster
	 * @param item
	 * @param targets the targets
	 */
	public void activateSkill(Creature caster, Item item, params WorldObject[] targets)
	{
		foreach (WorldObject targetObject in targets)
		{
			if (!targetObject.isCreature())
			{
				continue;
			}

			if (targetObject.isSummon() && !isSharedWithSummon())
			{
				continue;
			}

			Creature target = (Creature)targetObject;
			if (Formulas.calcBuffDebuffReflection(target, this))
			{
				// if skill is reflected instant effects should be casted on target
				// and continuous effects on caster
				applyEffects(target, caster, false, TimeSpan.Zero);

				BuffInfo info = new BuffInfo(caster, target, this, false, item, null);
				applyEffectScope(EffectScope.GENERAL, info, true, false);

				EffectScope? pvpOrPveEffectScope = caster.isPlayable() && target.isAttackable() ? EffectScope.PVE :
					caster.isPlayable() && target.isPlayable() ? EffectScope.PVP : null;
				applyEffectScope(pvpOrPveEffectScope, info, true, false);
			}
			else
			{
				applyEffects(caster, target, item);
			}
		}

		// Self Effect
		if (hasEffects(EffectScope.SELF))
		{
			if (caster.isAffectedBySkill(_id))
			{
				caster.stopSkillEffects(SkillFinishType.REMOVED, _id);
			}

			applyEffects(caster, caster, true, false, true, TimeSpan.Zero, item);
		}

		if (!caster.isCubic())
		{
			if (useSpiritShot())
			{
				caster.unchargeShot(caster.isChargedShot(ShotType.BLESSED_SPIRITSHOTS)
					? ShotType.BLESSED_SPIRITSHOTS
					: ShotType.SPIRITSHOTS);
			}
			else if (useSoulShot())
			{
				caster.unchargeShot(caster.isChargedShot(ShotType.BLESSED_SOULSHOTS)
					? ShotType.BLESSED_SOULSHOTS
					: ShotType.SOULSHOTS);
			}
		}

		if (_isSuicideAttack)
		{
			caster.doDie(caster);
		}
	}

	/**
	 * Adds a condition to the condition list for the given condition scope.
	 * @param skillConditionScope the condition scope
	 * @param skillCondition the condition
	 */
	public void addCondition(SkillConditionScope skillConditionScope, ISkillCondition skillCondition)
	{
		_conditionLists.computeIfAbsent(skillConditionScope, k => new()).Add(skillCondition);
	}

	/**
	 * Checks the conditions of this skills for the given condition scope.
	 * @param skillConditionScope the condition scope
	 * @param caster the caster
	 * @param target the target
	 * @return {@code false} if at least one condition returns false, {@code true} otherwise
	 */
	public bool checkConditions(SkillConditionScope skillConditionScope, Creature caster, WorldObject target)
	{
		List<ISkillCondition> conditions = _conditionLists.get(skillConditionScope);
		if (conditions == null)
		{
			return true;
		}

		foreach (ISkillCondition condition in conditions)
		{
			if (!condition.canUse(caster, this, target))
			{
				return false;
			}
		}

		return true;
	}

	public override string ToString()
	{
		return "Skill " + _name + "(" + _id + "," + _level + "," + _subLevel + ")";
	}

	/**
	 * used for tracking item id in case that item consume cannot be used
	 * @return reference item id
	 */
	public int getReferenceItemId()
	{
		return _refId;
	}

	public bool canBeDispelled()
	{
		return _canBeDispelled;
	}

	/**
	 * Verify if the skill can be stolen.
	 * @return {@code true} if skill can be stolen, {@code false} otherwise
	 */
	public bool canBeStolen()
	{
		return !isPassive() && !isToggle() && !_isDebuff && !_irreplacableBuff && !isHeroSkill() && !isGMSkill() &&
		       !(isStatic() && (getId() != (int)CommonSkill.CARAVANS_SECRET_MEDICINE)) && _canBeDispelled;
	}

	public bool isClanSkill()
	{
		return SkillTreeData.getInstance().isClanSkill(_id, _level);
	}

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
	private void parseAbnormalVisualEffect(string abnormalVisualEffects)
	{
		if (abnormalVisualEffects != null)
		{
			string[] data = abnormalVisualEffects.Split(";");
			Set<AbnormalVisualEffect> aves = new();
			foreach (string aveString in data)
			{
				AbnormalVisualEffect ave = Enum.Parse<AbnormalVisualEffect>(aveString);
				if (ave != null)
				{
					aves.add(ave);
				}
				else
				{
					LOGGER.Warn("Invalid AbnormalVisualEffect(" + this + ") found for Skill(" + aveString + ")");
				}
			}

			if (!aves.isEmpty())
			{
				_abnormalVisualEffects = aves;
			}
		}
	}

	/**
	 * @param effectType Effect type to check if its present on this skill effects.
	 * @param effectTypes Effect types to check if are present on this skill effects.
	 * @return {@code true} if at least one of specified {@link EffectType} types is present on this skill effects, {@code false} otherwise.
	 */
	public bool hasEffectType(EffectType effectType, params EffectType[] effectTypes)
	{
		if (_effectTypes == null)
		{
			lock (this)
			{
				if (_effectTypes == null)
				{
					Set<EffectType> effectTypesSet = new();
					foreach (List<AbstractEffect> effectList in _effectLists.values())
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
	public bool hasEffectType(EffectScope effectScope, EffectType effectType, params EffectType[] effectTypes)
	{
		if (hasEffects(effectScope))
		{
			return false;
		}

		foreach (AbstractEffect effect in _effectLists.get(effectScope))
		{
			if (effectType == effect.getEffectType())
			{
				return true;
			}

			foreach (EffectType type in effectTypes)
			{
				if (type == effect.getEffectType())
				{
					return true;
				}
			}
		}

		return false;
	}

	/**
	 * @return icon of the current skill.
	 */
	public string getIcon()
	{
		return _icon;
	}

	public TimeSpan getChannelingTickInterval()
	{
		return _channelingTickInterval;
	}

	public TimeSpan getChannelingTickInitialDelay()
	{
		return _channelingStart;
	}

	public bool isMentoring()
	{
		return _isMentoring;
	}

	/**
	 * @param creature
	 * @return alternative skill that has been attached due to the effect of toggle skills on the player (e.g Fire Stance, Water Stance).
	 */
	public Skill getAttachedSkill(Creature creature)
	{
		// If character is double casting, return double cast skill.
		if ((_doubleCastSkill > 0) && creature.isAffected(EffectFlag.DOUBLE_CAST))
		{
			return SkillData.getInstance().getSkill(getDoubleCastSkill(), getLevel(), getSubLevel());
		}

		// Default toggle group ID, assume nothing attached.
		if ((_attachToggleGroupId <= 0) || (_attachSkills == null))
		{
			return null;
		}

		int toggleSkillId = 0;
		foreach (BuffInfo info in creature.getEffectList().getEffects())
		{
			if (info.getSkill().getToggleGroupId() == _attachToggleGroupId)
			{
				toggleSkillId = info.getSkill().getId();
				break;
			}
		}

		// No active toggles with this toggle group ID found.
		if (toggleSkillId == 0)
		{
			return null;
		}

		AttachSkillHolder attachedSkill = null;
		foreach (AttachSkillHolder ash in _attachSkills)
		{
			if (ash.getRequiredSkillId() == toggleSkillId)
			{
				attachedSkill = ash;
				break;
			}
		}

		// No attached skills for this toggle found.
		if (attachedSkill == null)
		{
			return null;
		}

		return SkillData.getInstance().getSkill(attachedSkill.getSkillId(),
			Math.Min(SkillData.getInstance().getMaxLevel(attachedSkill.getSkillId()), _level), _subLevel);
	}

	public bool canDoubleCast()
	{
		return _canDoubleCast;
	}

	public int getDoubleCastSkill()
	{
		return _doubleCastSkill;
	}

	public bool canCastWhileDisabled()
	{
		return _canCastWhileDisabled;
	}

	public bool isSharedWithSummon()
	{
		return _isSharedWithSummon;
	}

	public bool isNecessaryToggle()
	{
		return _isNecessaryToggle;
	}

	public bool isDeleteAbnormalOnLeave()
	{
		return _deleteAbnormalOnLeave;
	}

	/**
	 * @return {@code true} if the buff cannot be replaced, canceled, removed on death, etc.<br>
	 *         It can be only overriden by higher stack, but buff still remains ticking and activates once the higher stack buff has passed away.
	 */
	public bool isIrreplacableBuff()
	{
		return _irreplacableBuff;
	}

	public bool isDisplayInList()
	{
		return _displayInList;
	}

	/**
	 * @return if skill could not be requested for use by players.
	 */
	public bool isBlockActionUseSkill()
	{
		return _blockActionUseSkill;
	}

	public int getToggleGroupId()
	{
		return _toggleGroupId;
	}

	public int getAttachToggleGroupId()
	{
		return _attachToggleGroupId;
	}

	public List<AttachSkillHolder> getAttachSkills()
	{
		return _attachSkills;
	}

	public Set<AbnormalType> getAbnormalResists()
	{
		return _abnormalResists;
	}

	public double getMagicCriticalRate()
	{
		return _magicCriticalRate;
	}

	public SkillBuffType getBuffType()
	{
		return _buffType;
	}

	public bool isEnchantable()
	{
		return SkillEnchantData.getInstance().getSkillEnchant(getId()) != null;
	}
}