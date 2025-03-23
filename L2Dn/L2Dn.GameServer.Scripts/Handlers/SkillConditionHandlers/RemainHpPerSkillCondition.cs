using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("RemainHpPer")]
public sealed class RemainHpPerSkillCondition: ISkillCondition
{
    private readonly int _amount;
    private readonly SkillConditionPercentType _percentType;
    private readonly SkillConditionAffectType _affectType;

    public RemainHpPerSkillCondition(SkillConditionParameterSet parameters)
    {
        _amount = parameters.GetInt32(XmlSkillConditionParameterType.Amount);
        _percentType = parameters.GetEnum<SkillConditionPercentType>(XmlSkillConditionParameterType.PercentType);
        _affectType = parameters.GetEnum<SkillConditionAffectType>(XmlSkillConditionParameterType.AffectType);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        switch (_affectType)
        {
            case SkillConditionAffectType.CASTER:
            {
                return _percentType.test(caster.getCurrentHpPercent(), _amount);
            }
            case SkillConditionAffectType.TARGET:
            {
                if (target != null && target.isCreature())
                {
                    return _percentType.test(((Creature)target).getCurrentHpPercent(), _amount);
                }

                break;
            }
            case SkillConditionAffectType.SUMMON:
            {
                Summon? summon = caster.getActingPlayer()?.getAnyServitor();
                if (caster.hasServitors() && summon != null)
                {
                    return _percentType.test(summon.getCurrentHpPercent(), _amount);
                }

                break;
            }
        }

        return false;
    }

    public override int GetHashCode() => HashCode.Combine(_amount, _percentType, _affectType);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._amount, x._percentType, x._affectType));
}