using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid
 */
public class CanSummonCubicSkillCondition: ISkillCondition
{
    public CanSummonCubicSkillCondition(StatSet @params)
    {
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Player? player = caster.getActingPlayer();
        if (!caster.isPlayer() || caster.isAlikeDead() || player == null || player.inObserverMode())
        {
            return false;
        }

        if (player.getAutoUseSettings().isAutoSkill(skill.getId()))
        {
            List<AbstractEffect>? generalEffects = skill.getEffects(EffectScope.GENERAL);
            if (generalEffects != null)
            {
                foreach (AbstractEffect effect in generalEffects)
                {
                    if (effect is SummonCubic cubic && player.getCubicById(cubic.getCubicId()) != null)
                    {
                        return false;
                    }
                }
            }
        }

        return !player.inObserverMode() && !player.isMounted() && !player.isSpawnProtected() &&
            !player.isTeleportProtected();
    }
}