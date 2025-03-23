using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpTargetMyPledgeAcademy")]
public sealed class OpTargetMyPledgeAcademySkillCondition: ISkillCondition
{
    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        if (caster.getClan() == null || target == null || !target.isPlayer())
        {
            return false;
        }

        Player? targetPlayer = target.getActingPlayer();
        return targetPlayer != null && targetPlayer.isAcademyMember() && targetPlayer.getClan() == caster.getClan();
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}