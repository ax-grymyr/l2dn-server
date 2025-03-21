using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpCheckOnGoingEventCampaign")]
public sealed class OpCheckOnGoingEventCampaignSkillCondition: ISkillCondition
{
    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        return true; // TODO
    }
}