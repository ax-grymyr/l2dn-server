using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

public sealed class OpPkcountSkillCondition: ISkillCondition
{
    private readonly SkillConditionAffectType _affectType;

    public OpPkcountSkillCondition(SkillConditionParameterSet parameters)
    {
        _affectType = parameters.GetEnum<SkillConditionAffectType>(XmlSkillConditionParameterType.AffectType);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        switch (_affectType)
        {
            case SkillConditionAffectType.CASTER:
            {
                Player? player = caster.getActingPlayer();
                return caster.isPlayer() && player != null && player.getPkKills() > 0;
            }
            case SkillConditionAffectType.TARGET:
            {
                if (target != null && target.isPlayer())
                {
                    Player? player = target.getActingPlayer();
                    return player != null && player.getPkKills() > 0;
                }

                break;
            }
        }

        return false;
    }
}