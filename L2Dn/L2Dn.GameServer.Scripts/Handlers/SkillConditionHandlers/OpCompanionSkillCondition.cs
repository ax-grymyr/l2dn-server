using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpCompanion")]
public sealed class OpCompanionSkillCondition: ISkillCondition
{
    private readonly SkillConditionCompanionType _type;

    public OpCompanionSkillCondition(SkillConditionParameterSet parameters)
    {
        _type = parameters.GetEnum<SkillConditionCompanionType>(XmlSkillConditionParameterType.Type);
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