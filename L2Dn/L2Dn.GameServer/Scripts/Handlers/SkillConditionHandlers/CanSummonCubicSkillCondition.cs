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
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		if (!caster.isPlayer() || caster.isAlikeDead() || caster.getActingPlayer().inObserverMode())
		{
			return false;
		}
		
		Player player = caster.getActingPlayer();
		if (player.getAutoUseSettings().isAutoSkill(skill.getId()))
		{
			foreach (AbstractEffect effect in skill.getEffects(EffectScope.GENERAL))
			{
				if ((effect is SummonCubic) && (player.getCubicById(((SummonCubic) effect).getCubicId()) != null))
				{
					return false;
				}
			}
		}
		
		return !player.inObserverMode() && !player.isMounted() && !player.isSpawnProtected() && !player.isTeleportProtected();
	}
}