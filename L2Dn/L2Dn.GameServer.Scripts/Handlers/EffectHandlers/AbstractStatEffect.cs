using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Conditions;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public abstract class AbstractStatEffect: AbstractEffect
{
    private readonly Stat _addStat;
    private readonly Stat _mulStat;
    private readonly double _amount;
    private readonly StatModifierType _mode;
    private readonly ImmutableArray<Condition> _conditions;

    protected AbstractStatEffect(EffectParameterSet parameters, Stat stat): this(parameters, stat, stat)
    {
    }

    protected AbstractStatEffect(EffectParameterSet parameters, Stat mulStat, Stat addStat)
    {
        _addStat = addStat;
        _mulStat = mulStat;
        _amount = parameters.GetDouble(XmlSkillEffectParameterType.Amount, 0);
        _mode = parameters.GetEnum(XmlSkillEffectParameterType.Mode, StatModifierType.DIFF);

        ItemTypeMask weaponTypesMask = ItemTypeMask.Zero;
        List<string>? weaponTypes = parameters.GetStringListOptional(XmlSkillEffectParameterType.WeaponType);
        if (weaponTypes != null)
        {
            foreach (string weaponType in weaponTypes)
            {
                try
                {
                    weaponTypesMask |= Enum.Parse<WeaponType>(weaponType, true);
                }
                catch (ArgumentException e)
                {
                    throw new ArgumentException("weaponType should contain WeaponType enum value but found " +
                        weaponType, e);
                }
            }
        }

        ItemTypeMask armorTypesMask = ItemTypeMask.Zero;
        List<string>? armorTypes = parameters.GetStringListOptional(XmlSkillEffectParameterType.ArmorType);
        if (armorTypes != null)
        {
            foreach (string armorType in armorTypes)
            {
                try
                {
                    armorTypesMask |= Enum.Parse<ArmorType>(armorType);
                }
                catch (ArgumentException e)
                {
                    throw new ArgumentException("armorTypes should contain ArmorType enum value but found " + armorType,
                        e);
                }
            }
        }

        List<Condition> conditions = [];
        if (weaponTypesMask != ItemTypeMask.Zero)
            conditions.Add(new ConditionUsingItemType(weaponTypesMask));

        if (armorTypesMask != ItemTypeMask.Zero)
            conditions.Add(new ConditionUsingItemType(armorTypesMask));

        if (parameters.Contains(XmlSkillEffectParameterType.InCombat))
            conditions.Add(new ConditionPlayerIsInCombat(parameters.GetBoolean(XmlSkillEffectParameterType.InCombat)));

        if (parameters.Contains(XmlSkillEffectParameterType.MagicWeapon))
            conditions.Add(new ConditionUsingMagicWeapon(parameters.GetBoolean(XmlSkillEffectParameterType.MagicWeapon)));

        if (parameters.Contains(XmlSkillEffectParameterType.TwoHandWeapon))
            conditions.Add(new ConditionUsingTwoHandWeapon(parameters.GetBoolean(XmlSkillEffectParameterType.TwoHandWeapon)));

        _conditions = conditions.ToImmutableArray();
    }

    public Stat AddStat => _addStat;
    public Stat MulStat => _mulStat;
    public double Amount => _amount;
    public StatModifierType Mode => _mode;
    public ImmutableArray<Condition> Conditions => _conditions;

    public override void Pump(Creature effected, Skill skill)
    {
        foreach (Condition cond in _conditions)
        {
            if (!cond.test(effected, effected, skill))
                return;
        }

        switch (_mode)
        {
            case StatModifierType.DIFF:
            {
                effected.getStat().mergeAdd(_addStat, _amount);
                break;
            }
            case StatModifierType.PER:
            {
                effected.getStat().mergeMul(_mulStat, _amount / 100 + 1);
                break;
            }
        }
    }

    public override int GetHashCode() =>
        HashCode.Combine(_addStat, _mulStat, _amount, _mode, _conditions.GetSequenceHashCode());

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj,
            static x => (x._addStat, x._mulStat, x._amount, x._mode, x._conditions.GetSequentialComparable()));
}