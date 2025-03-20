using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Conditions;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class TwoHandedBluntBonus: AbstractEffect
{
    private static readonly Condition _weaponTypeCondition = new ConditionUsingItemType(WeaponType.BLUNT);
    private static readonly Condition _slotCondition = new ConditionUsingSlotType(ItemTemplate.SLOT_LR_HAND);

    private readonly double _pAtkAmount;
    private readonly StatModifierType _pAtkMode;

    private readonly double _mAtkAmount;
    private readonly StatModifierType _mAtkMode;

    private readonly double _pAtkSpeedAmount;
    private readonly StatModifierType _pAtkSpeedMode;

    private readonly double _mAtkSpeedAmount;
    private readonly StatModifierType _mAtkSpeedMode;

    private readonly double _pAccuracyAmount;
    private readonly StatModifierType _pAccuracyMode;

    private readonly double _mAccuracyAmount;
    private readonly StatModifierType _mAccuracyMode;

    private readonly double _pCritRateAmount;
    private readonly StatModifierType _pCritRateMode;

    private readonly double _mCritRateAmount;
    private readonly StatModifierType _mCritRateMode;

    private readonly double _pCritDamageAmount;
    private readonly StatModifierType _pCritDamageMode;

    private readonly double _mCritDamageAmount;
    private readonly StatModifierType _mCritDamageMode;

    private readonly double _speedAmount;
    private readonly StatModifierType _speedMode;

    private readonly double _physicalAttackRange;
    private readonly StatModifierType _physicalAttackRangeMode;

    private readonly double _skillBonusRange;
    private readonly StatModifierType _skillBonusRangeMode;

    public TwoHandedBluntBonus(EffectParameterSet parameters)
    {
        _pAtkAmount = parameters.GetDouble(XmlSkillEffectParameterType.PAtkAmount, 0);
        _pAtkMode = parameters.GetEnum(XmlSkillEffectParameterType.PAtkMode, StatModifierType.DIFF);

        _mAtkAmount = parameters.GetDouble(XmlSkillEffectParameterType.MAtkAmount, 0);
        _mAtkMode = parameters.GetEnum(XmlSkillEffectParameterType.MAtkMode, StatModifierType.DIFF);

        _pAtkSpeedAmount = parameters.GetDouble(XmlSkillEffectParameterType.PAtkSpeedAmount, 0);
        _pAtkSpeedMode = parameters.GetEnum(XmlSkillEffectParameterType.PAtkSpeedMode, StatModifierType.DIFF);

        _mAtkSpeedAmount = parameters.GetDouble(XmlSkillEffectParameterType.MAtkSpeedAmount, 0);
        _mAtkSpeedMode = parameters.GetEnum(XmlSkillEffectParameterType.MAtkSpeedMode, StatModifierType.DIFF);

        _pAccuracyAmount = parameters.GetDouble(XmlSkillEffectParameterType.PAccuracyAmount, 0);
        _pAccuracyMode = parameters.GetEnum(XmlSkillEffectParameterType.PAccuracyMode, StatModifierType.DIFF);

        _mAccuracyAmount = parameters.GetDouble(XmlSkillEffectParameterType.MAccuracyAmount, 0);
        _mAccuracyMode = parameters.GetEnum(XmlSkillEffectParameterType.MAccuracyMode, StatModifierType.DIFF);

        _pCritRateAmount = parameters.GetDouble(XmlSkillEffectParameterType.PCritRateAmount, 0);
        _pCritRateMode = parameters.GetEnum(XmlSkillEffectParameterType.PCritRateMode, StatModifierType.DIFF);

        _mCritRateAmount = parameters.GetDouble(XmlSkillEffectParameterType.MCritRateAmount, 0);
        _mCritRateMode = parameters.GetEnum(XmlSkillEffectParameterType.MCritRateMode, StatModifierType.DIFF);

        _pCritDamageAmount = parameters.GetDouble(XmlSkillEffectParameterType.PCritDamageAmount, 0);
        _pCritDamageMode = parameters.GetEnum(XmlSkillEffectParameterType.PCritDamageMode, StatModifierType.DIFF);

        _mCritDamageAmount = parameters.GetDouble(XmlSkillEffectParameterType.MCritDamageAmount, 0);
        _mCritDamageMode = parameters.GetEnum(XmlSkillEffectParameterType.MCritDamageMode, StatModifierType.DIFF);

        _speedAmount = parameters.GetDouble(XmlSkillEffectParameterType.SpeedAmount, 0);
        _speedMode = parameters.GetEnum(XmlSkillEffectParameterType.SpeedMode, StatModifierType.DIFF);

        _physicalAttackRange = parameters.GetDouble(XmlSkillEffectParameterType.PhysicalAttackRange, 0);
        _physicalAttackRangeMode = parameters.GetEnum(XmlSkillEffectParameterType.PhysicalAttackRangeMode, StatModifierType.DIFF);

        _skillBonusRange = parameters.GetDouble(XmlSkillEffectParameterType.SkillBonusRange, 0);
        _skillBonusRangeMode = parameters.GetEnum(XmlSkillEffectParameterType.SkillBonusRangeMode, StatModifierType.DIFF);
    }

    public override void Pump(Creature effected, Skill skill)
    {
        if (!_weaponTypeCondition.test(effected, effected, skill) || !_slotCondition.test(effected, effected, skill))
            return;

        if (_pAtkAmount != 0)
        {
            if (_pAtkMode == StatModifierType.DIFF)
                effected.getStat().mergeAdd(Stat.PHYSICAL_ATTACK, _pAtkAmount);
            else // PER
                effected.getStat().mergeMul(Stat.PHYSICAL_ATTACK, _pAtkAmount / 100 + 1);
        }

        if (_mAtkAmount != 0)
        {
            if (_mAtkMode == StatModifierType.DIFF)
                effected.getStat().mergeAdd(Stat.MAGIC_ATTACK, _mAtkAmount);
            else // PER
                effected.getStat().mergeMul(Stat.MAGIC_ATTACK, _mAtkAmount / 100 + 1);
        }

        if (_pAtkSpeedAmount != 0)
        {
            if (_pAtkSpeedMode == StatModifierType.DIFF)
                effected.getStat().mergeAdd(Stat.PHYSICAL_ATTACK_SPEED, _pAtkSpeedAmount);
            else // PER
                effected.getStat().mergeMul(Stat.PHYSICAL_ATTACK_SPEED, _pAtkSpeedAmount / 100 + 1);
        }

        if (_mAtkSpeedAmount != 0)
        {
            if (_mAtkSpeedMode == StatModifierType.DIFF)
                effected.getStat().mergeAdd(Stat.MAGIC_ATTACK_SPEED, _mAtkSpeedAmount);
            else // PER
                effected.getStat().mergeMul(Stat.MAGIC_ATTACK_SPEED, _mAtkSpeedAmount / 100 + 1);
        }

        if (_pAccuracyAmount != 0)
        {
            if (_pAccuracyMode == StatModifierType.DIFF)
                effected.getStat().mergeAdd(Stat.ACCURACY_COMBAT, _pAccuracyAmount);
            else // PER
                effected.getStat().mergeMul(Stat.ACCURACY_COMBAT, _pAccuracyAmount / 100 + 1);
        }

        if (_mAccuracyAmount != 0)
        {
            if (_mAccuracyMode == StatModifierType.DIFF)
                effected.getStat().mergeAdd(Stat.ACCURACY_MAGIC, _mAccuracyAmount);
            else // PER
                effected.getStat().mergeMul(Stat.ACCURACY_MAGIC, _mAccuracyAmount / 100 + 1);
        }

        if (_pCritRateAmount != 0)
        {
            if (_pCritRateMode == StatModifierType.DIFF)
                effected.getStat().mergeAdd(Stat.CRITICAL_RATE, _pCritRateAmount);
            else // PER
                effected.getStat().mergeMul(Stat.CRITICAL_RATE, _pCritRateAmount / 100 + 1);
        }

        if (_mCritRateAmount != 0)
        {
            if (_mCritRateMode == StatModifierType.DIFF)
                effected.getStat().mergeAdd(Stat.MAGIC_CRITICAL_RATE, _mCritRateAmount);
            else // PER
                effected.getStat().mergeMul(Stat.MAGIC_CRITICAL_RATE, _mCritRateAmount / 100 + 1);
        }

        if (_pCritDamageAmount != 0)
        {
            if (_pCritDamageMode == StatModifierType.DIFF)
                effected.getStat().mergeAdd(Stat.CRITICAL_DAMAGE_ADD, _pCritDamageAmount);
            else // PER
                effected.getStat().mergeMul(Stat.CRITICAL_DAMAGE, _pCritDamageAmount / 100 + 1);
        }

        if (_mCritDamageAmount != 0)
        {
            if (_mCritDamageMode == StatModifierType.DIFF)
                effected.getStat().mergeAdd(Stat.MAGIC_CRITICAL_DAMAGE_ADD, _mCritDamageAmount);
            else // PER
                effected.getStat().mergeMul(Stat.MAGIC_CRITICAL_DAMAGE, _mCritDamageAmount / 100 + 1);
        }

        if (_speedAmount != 0)
        {
            if (_speedMode == StatModifierType.DIFF)
                effected.getStat().mergeAdd(Stat.MOVE_SPEED, _speedAmount);
            else // PER
                effected.getStat().mergeMul(Stat.MOVE_SPEED, _speedAmount / 100 + 1);
        }

        if (_physicalAttackRange != 0)
        {
            if (_physicalAttackRangeMode == StatModifierType.DIFF)
                effected.getStat().mergeAdd(Stat.PHYSICAL_ATTACK_RANGE, _physicalAttackRange);
            else // PER
                effected.getStat().mergeMul(Stat.PHYSICAL_ATTACK_RANGE, _physicalAttackRange / 100 + 1);
        }

        if (_skillBonusRange != 0)
        {
            if (_skillBonusRangeMode == StatModifierType.DIFF)
                effected.getStat().mergeAdd(Stat.MAGIC_ATTACK_RANGE, _skillBonusRange);
            else // PER
                effected.getStat().mergeMul(Stat.MAGIC_ATTACK_RANGE, _skillBonusRange / 100 + 1);
        }
    }

    public override int GetHashCode() =>
        HashCode.Combine(HashCode.Combine(_pAtkAmount, _pAtkMode, _mAtkAmount, _mAtkMode, _pAtkSpeedAmount,
                _pAtkSpeedMode, _mAtkSpeedAmount, _mAtkSpeedMode),
            HashCode.Combine(_pAccuracyAmount, _pAccuracyMode, _mAccuracyAmount, _mAccuracyMode, _pCritRateAmount,
                _pCritRateMode, _mCritRateAmount, _mCritRateMode),
            HashCode.Combine(_pCritDamageAmount, _pCritDamageMode, _mCritDamageAmount, _mCritDamageMode, _speedAmount,
                _speedMode, _physicalAttackRange, _physicalAttackRangeMode),
            _skillBonusRange, _skillBonusRangeMode);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj,
            static x => (x._pAtkAmount, x._pAtkMode, x._mAtkAmount, x._mAtkMode, x._pAtkSpeedAmount, x._pAtkSpeedMode,
                x._mAtkSpeedAmount, x._mAtkSpeedMode, x._pAccuracyAmount, x._pAccuracyMode, x._mAccuracyAmount,
                x._mAccuracyMode, x._pCritRateAmount, x._pCritRateMode, x._mCritRateAmount, x._mCritRateMode,
                x._pCritDamageAmount, x._pCritDamageMode, x._mCritDamageAmount, x._mCritDamageMode, x._speedAmount,
                x._speedMode, x._physicalAttackRange, x._physicalAttackRangeMode, x._skillBonusRange,
                x._skillBonusRangeMode));
}