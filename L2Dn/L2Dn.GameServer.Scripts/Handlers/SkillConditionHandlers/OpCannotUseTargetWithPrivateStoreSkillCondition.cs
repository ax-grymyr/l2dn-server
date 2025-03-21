using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpCannotUseTargetWithPrivateStore")]
public sealed class OpCannotUseTargetWithPrivateStoreSkillCondition: ISkillCondition
{
    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Player? player = target?.getActingPlayer();
        return target == null || !target.isPlayer() || player == null ||
            player.getPrivateStoreType() == PrivateStoreType.NONE;
    }
}