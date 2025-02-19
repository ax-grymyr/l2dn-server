using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid
 */
public class OpCheckAbnormalSkillCondition: ISkillCondition
{
    private readonly AbnormalType _type;
    private readonly int _level;
    private readonly bool _hasAbnormal;
    private readonly SkillConditionAffectType _affectType;

    public OpCheckAbnormalSkillCondition(StatSet @params)
    {
        _type = @params.getEnum<AbnormalType>("type");
        _level = @params.getInt("level");
        _hasAbnormal = @params.getBoolean("hasAbnormal");
        _affectType = @params.getEnum("affectType", SkillConditionAffectType.TARGET);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        switch (_affectType)
        {
            case SkillConditionAffectType.CASTER:
            {
                return caster.getEffectList().
                    hasAbnormalType(_type, info => info.getSkill().getAbnormalLevel() >= _level) == _hasAbnormal;
            }
            case SkillConditionAffectType.TARGET:
            {
                if (target != null && target.isCreature())
                {
                    return ((Creature)target).getEffectList().
                        hasAbnormalType(_type, info => info.getSkill().getAbnormalLevel() >= _level) == _hasAbnormal;
                }

                break;
            }
        }

        return false;
    }
}