using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Conditions;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
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

    protected AbstractStatEffect(StatSet @params, Stat stat): this(@params, stat, stat)
    {
    }

    protected AbstractStatEffect(StatSet @params, Stat mulStat, Stat addStat)
    {
        _addStat = addStat;
        _mulStat = mulStat;
        _amount = @params.getDouble("amount", 0);
        _mode = @params.getEnum("mode", StatModifierType.DIFF);

        ItemTypeMask weaponTypesMask = ItemTypeMask.Zero;
        List<string>? weaponTypes = @params.getList<string>("weaponType");
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
        List<string>? armorTypes = @params.getList<string>("armorType");
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

        if (@params.contains("inCombat"))
            conditions.Add(new ConditionPlayerIsInCombat(@params.getBoolean("inCombat")));

        if (@params.contains("magicWeapon"))
            conditions.Add(new ConditionUsingMagicWeapon(@params.getBoolean("magicWeapon")));

        if (@params.contains("twoHandWeapon"))
            conditions.Add(new ConditionUsingTwoHandWeapon(@params.getBoolean("twoHandWeapon")));

        _conditions = conditions.ToImmutableArray();
    }

    public Stat AddStat => _addStat;
    public Stat MulStat => _mulStat;
    public double Amount => _amount;
    public StatModifierType Mode => _mode;
    public ImmutableArray<Condition> Conditions => _conditions;

    public override void pump(Creature effected, Skill skill)
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