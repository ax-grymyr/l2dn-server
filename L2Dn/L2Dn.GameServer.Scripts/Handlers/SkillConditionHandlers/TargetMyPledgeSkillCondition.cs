using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

public sealed class TargetMyPledgeSkillCondition: ISkillCondition
{
    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        if (target == null || !target.isPlayer())
        {
            return false;
        }

        Clan? clan = caster.getClan();
        return clan != null && clan == target.getActingPlayer()?.getClan();
    }
}