using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpUnlock")]
public sealed class OpUnlockSkillCondition: ISkillCondition
{
    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        return target != null && (target.isDoor() || target is Chest);
    }
}