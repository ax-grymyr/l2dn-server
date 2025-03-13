using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.StaticData;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Zoey76
 */
public class OpSweeperSkillCondition: ISkillCondition
{
	public OpSweeperSkillCondition(StatSet @params)
	{
	}

	public bool canUse(Creature caster, Skill skill, WorldObject? target)
	{
		bool canSweep = false;
        Player? sweeper = caster.getActingPlayer();
		if (sweeper != null)
		{
            if (skill != null)
            {
                List<WorldObject>? targets = skill.getTargetsAffected(sweeper, target);
                if (targets != null)
                {
                    foreach (WorldObject wo in targets)
                    {
                        if (wo != null && wo.isAttackable())
                        {
                            Attackable attackable = (Attackable)wo;
                            if (attackable.isDead())
                            {
                                if (attackable.isSpoiled())
                                {
                                    canSweep = attackable.checkSpoilOwner(sweeper, true);
                                    if (canSweep)
                                    {
                                        canSweep = !attackable.isOldCorpse(sweeper,
                                            Config.CORPSE_CONSUME_SKILL_ALLOWED_TIME_BEFORE_DECAY, true);
                                    }

                                    if (canSweep)
                                    {
                                        canSweep = sweeper.getInventory().
                                            checkInventorySlotsAndWeight(attackable.getSpoilLootItems(), true, true);
                                    }
                                }
                                else
                                {
                                    sweeper.sendPacket(SystemMessageId.
                                        THE_SWEEPER_HAS_FAILED_AS_THE_TARGET_IS_NOT_SPOILED);
                                }
                            }
                        }
                    }
                }
            }
        }
		return canSweep;
	}
}