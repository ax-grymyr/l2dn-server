using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid
 */
public class OpPkcountSkillCondition: ISkillCondition
{
    private readonly SkillConditionAffectType _affectType;

    public OpPkcountSkillCondition(StatSet @params)
    {
        _affectType = @params.getEnum<SkillConditionAffectType>("affectType");
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