using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Scripts.Handlers.EffectHandlers;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("CanSummonCubic")]
public sealed class CanSummonCubicSkillCondition: ISkillCondition
{
    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Player? player = caster.getActingPlayer();
        if (!caster.isPlayer() || caster.isAlikeDead() || player == null || player.inObserverMode())
            return false;

        if (player.getAutoUseSettings().isAutoSkill(skill.Id))
        {
            foreach (AbstractEffect effect in skill.GetEffects(SkillEffectScope.General))
            {
                if (effect is SummonCubic cubic && player.getCubicById(cubic.getCubicId()) != null)
                    return false;
            }
        }

        return !player.inObserverMode() && !player.isMounted() && !player.isSpawnProtected() &&
            !player.isTeleportProtected();
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}