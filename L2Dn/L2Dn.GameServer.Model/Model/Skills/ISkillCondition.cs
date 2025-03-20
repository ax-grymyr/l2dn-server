using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Skills;

public interface ISkillCondition: ISkillConditionBase
{
    bool canUse(Creature caster, Skill skill, WorldObject? target);
}