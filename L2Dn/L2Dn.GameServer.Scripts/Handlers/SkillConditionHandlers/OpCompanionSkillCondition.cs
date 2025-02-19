using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Sdw
 */
public class OpCompanionSkillCondition: ISkillCondition
{
    private readonly SkillConditionCompanionType _type;

    public OpCompanionSkillCondition(StatSet @params)
    {
        _type = @params.getEnum<SkillConditionCompanionType>("type");
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        if (target != null)
        {
            switch (_type)
            {
                case SkillConditionCompanionType.PET:
                {
                    return target.isPet();
                }
                case SkillConditionCompanionType.MY_SUMMON:
                {
                    return target.isSummon() && caster.getServitor(target.ObjectId) != null;
                }
            }
        }

        return false;
    }
}