using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Sdw
 */
public class OpCheckSkillSkillCondition: ISkillCondition
{
    private readonly int _skillId;
    private readonly SkillConditionAffectType _affectType;

    public OpCheckSkillSkillCondition(StatSet @params)
    {
        _skillId = @params.getInt("skillId");
        _affectType = @params.getEnum<SkillConditionAffectType>("affectType");
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