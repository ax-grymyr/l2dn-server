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

[HandlerStringKey("Speed")]
public sealed class Speed: AbstractEffect
{
    private readonly double _amount;
    private readonly StatModifierType _mode;
    private readonly ConditionUsingItemType? _condition;

    public Speed(EffectParameterSet parameters)
    {
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
                    weaponTypesMask |= Enum.Parse<WeaponType>(weaponType);
                }
                catch (ArgumentException e)
                {
                    throw new ArgumentException(
                        "weaponType should contain WeaponType enum value but found " + weaponType, e);
                }
            }
        }

        if (weaponTypesMask != ItemTypeMask.Zero)
            _condition = new ConditionUsingItemType(weaponTypesMask);
    }

    public override void Pump(Creature effected, Skill skill)
    {
        if (_condition == null || _condition.test(effected, effected, skill))
        {
            switch (_mode)
            {
                case StatModifierType.DIFF:
                {
                    effected.getStat().mergeAdd(Stat.RUN_SPEED, _amount);
                    effected.getStat().mergeAdd(Stat.WALK_SPEED, _amount);
                    effected.getStat().mergeAdd(Stat.SWIM_RUN_SPEED, _amount);
                    effected.getStat().mergeAdd(Stat.SWIM_WALK_SPEED, _amount);
                    effected.getStat().mergeAdd(Stat.FLY_RUN_SPEED, _amount);
                    effected.getStat().mergeAdd(Stat.FLY_WALK_SPEED, _amount);
                    break;
                }
                case StatModifierType.PER:
                {
                    effected.getStat().mergeMul(Stat.RUN_SPEED, _amount / 100 + 1);
                    effected.getStat().mergeMul(Stat.WALK_SPEED, _amount / 100 + 1);
                    effected.getStat().mergeMul(Stat.SWIM_RUN_SPEED, _amount / 100 + 1);
                    effected.getStat().mergeMul(Stat.SWIM_WALK_SPEED, _amount / 100 + 1);
                    effected.getStat().mergeMul(Stat.FLY_RUN_SPEED, _amount / 100 + 1);
                    effected.getStat().mergeMul(Stat.FLY_WALK_SPEED, _amount / 100 + 1);
                    break;
                }
            }
        }
    }

    public override int GetHashCode() => HashCode.Combine(_amount, _mode, _condition);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._amount, x._mode, x._condition));
}