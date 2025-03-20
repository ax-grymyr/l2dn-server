using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

public sealed class CanTransformInDominionSkillCondition: ISkillCondition
{
    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        return true; // TODO
    }
}