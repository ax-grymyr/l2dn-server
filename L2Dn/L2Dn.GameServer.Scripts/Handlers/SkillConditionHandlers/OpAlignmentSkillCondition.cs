using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpAlignment")]
public sealed class OpAlignmentSkillCondition: ISkillCondition
{
    private readonly SkillConditionAffectType _affectType;
    private readonly SkillConditionAlignment _alignment;

    public OpAlignmentSkillCondition(SkillConditionParameterSet parameters)
    {
        _affectType = parameters.GetEnum<SkillConditionAffectType>(XmlSkillConditionParameterType.AffectType);
        _alignment = parameters.GetEnum<SkillConditionAlignment>(XmlSkillConditionParameterType.Alignment);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        switch (_affectType)
        {
            case SkillConditionAffectType.CASTER:
            {
                Player? player = caster.getActingPlayer();
                return player != null && _alignment.test(player);
            }

            case SkillConditionAffectType.TARGET:
            {
                Player? player = target?.getActingPlayer();
                if (target != null && target.isPlayer() && player != null)
                {
                    return _alignment.test(player);
                }

                break;
            }
        }

        return false;
    }

    public override int GetHashCode() => HashCode.Combine(_affectType, _alignment);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._affectType, x._alignment));
}