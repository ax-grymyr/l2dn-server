using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpCheckAbnormal")]
public sealed class OpCheckAbnormalSkillCondition: ISkillCondition
{
    private readonly AbnormalType _type;
    private readonly int _level;
    private readonly bool _hasAbnormal;
    private readonly SkillConditionAffectType _affectType;

    public OpCheckAbnormalSkillCondition(SkillConditionParameterSet parameters)
    {
        _type = parameters.GetEnum<AbnormalType>(XmlSkillConditionParameterType.Type);
        _level = parameters.GetInt32(XmlSkillConditionParameterType.Level);
        _hasAbnormal = parameters.GetBoolean(XmlSkillConditionParameterType.HasAbnormal);
        _affectType = parameters.GetEnum(XmlSkillConditionParameterType.AffectType, SkillConditionAffectType.TARGET);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        switch (_affectType)
        {
            case SkillConditionAffectType.CASTER:
            {
                return caster.getEffectList().
                    hasAbnormalType(_type, info => info.getSkill().AbnormalLevel >= _level) == _hasAbnormal;
            }
            case SkillConditionAffectType.TARGET:
            {
                if (target != null && target.isCreature())
                {
                    return ((Creature)target).getEffectList().
                        hasAbnormalType(_type, info => info.getSkill().AbnormalLevel >= _level) == _hasAbnormal;
                }

                break;
            }
        }

        return false;
    }

    public override int GetHashCode() => HashCode.Combine(_type, _level, _hasAbnormal, _affectType);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._type, x._level, x._hasAbnormal, x._affectType));
}