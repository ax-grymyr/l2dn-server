using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("TargetMyMentee")]
public sealed class TargetMyMenteeSkillCondition: ISkillCondition
{
    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        if (target == null || !target.isPlayer())
        {
            return false;
        }

        return MentorManager.getInstance().getMentee(caster.ObjectId, target.ObjectId) != null;
    }
}