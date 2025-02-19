using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Sdw
 */
public class OpAlignmentSkillCondition: ISkillCondition
{
    private readonly SkillConditionAffectType _affectType;
    private readonly SkillConditionAlignment _alignment;

    public OpAlignmentSkillCondition(StatSet @params)
    {
        _affectType = @params.getEnum<SkillConditionAffectType>("affectType");
        _alignment = @params.getEnum<SkillConditionAlignment>("alignment");
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
}