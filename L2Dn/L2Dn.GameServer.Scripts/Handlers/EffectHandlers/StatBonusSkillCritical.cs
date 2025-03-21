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

[HandlerStringKey("StatBonusSkillCritical")]
public sealed class StatBonusSkillCritical: AbstractEffect
{
    private readonly double _stat;
    private readonly Condition? _armorTypeCondition;

    public StatBonusSkillCritical(EffectParameterSet parameters)
    {
        _stat = (int)parameters.GetEnum(XmlSkillEffectParameterType.Stat, BaseStat.DEX);

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

        _armorTypeCondition = armorTypesMask != ItemTypeMask.Zero ? new ConditionUsingItemType(armorTypesMask) : null;
    }

    public override void Pump(Creature effected, Skill skill)
    {
        if (_armorTypeCondition == null || _armorTypeCondition.test(effected, effected, skill))
            effected.getStat().mergeAdd(Stat.STAT_BONUS_SKILL_CRITICAL, _stat); // TODO: why ordinal value is added
    }

    public override int GetHashCode() => HashCode.Combine(_stat, _armorTypeCondition);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._stat, x._armorTypeCondition));
}