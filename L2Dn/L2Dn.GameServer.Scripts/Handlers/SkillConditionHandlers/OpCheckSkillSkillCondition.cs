using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerName("OpCheckSkill")]
public sealed class OpCheckSkillSkillCondition: ISkillCondition
{
    private readonly int _skillId;
    private readonly SkillConditionAffectType _affectType;

    public OpCheckSkillSkillCondition(SkillConditionParameterSet parameters)
    {
        _skillId = parameters.GetInt32(XmlSkillConditionParameterType.SkillId);
        _affectType = parameters.GetEnum<SkillConditionAffectType>(XmlSkillConditionParameterType.AffectType);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        switch (_affectType)
        {
            case SkillConditionAffectType.CASTER:
            {
                return caster.getSkillLevel(_skillId) > 0;
            }
            case SkillConditionAffectType.TARGET:
            {
                Player? player = target?.getActingPlayer();
                if (target != null && !target.isPlayer() && player != null)
                {
                    return player.getSkillLevel(_skillId) > 0;
                }

                break;
            }
        }

        return false;
    }
}