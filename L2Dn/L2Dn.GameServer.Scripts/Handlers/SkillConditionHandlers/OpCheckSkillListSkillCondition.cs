using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Mobius
 */
public class OpCheckSkillListSkillCondition: ISkillCondition
{
    private readonly List<int> _skillIds;
    private readonly SkillConditionAffectType _affectType;

    public OpCheckSkillListSkillCondition(StatSet @params)
    {
        _skillIds = @params.getList<int>("skillIds");
        _affectType = @params.getEnum<SkillConditionAffectType>("affectType");
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        switch (_affectType)
        {
            case SkillConditionAffectType.CASTER:
            {
                foreach (int id in _skillIds)
                {
                    if (caster.getSkillLevel(id) > 0)
                    {
                        return true;
                    }
                }

                break;
            }
            case SkillConditionAffectType.TARGET:
            {
                Player? player = target?.getActingPlayer();
                if (target != null && !target.isPlayer() && player != null)
                {
                    foreach (int id in _skillIds)
                    {
                        if (player.getSkillLevel(id) > 0)
                        {
                            return true;
                        }
                    }
                }

                break;
            }
        }

        return false;
    }
}