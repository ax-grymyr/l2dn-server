using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Checks Sweeper conditions:
 * <ul>
 * <li>Minimum checks, player not null, skill not null.</li>
 * <li>Checks if the target isn't null, is dead and spoiled.</li>
 * <li>Checks if the sweeper player is the target spoiler, or is in the spoiler party.</li>
 * <li>Checks if the corpse is too old.</li>
 * <li>Checks inventory limit and weight max load won't be exceed after sweep.</li>
 * </ul>
 * If two or more conditions aren't meet at the same time, one message per condition will be shown.
 * @author Zoey76
 */
public sealed class ConditionPlayerCanSweep(bool value): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Player? sweeper = effector.getActingPlayer();
        if (sweeper is null)
            return !value;

        bool canSweep = false;
        if (skill != null)
        {
            List<WorldObject>? targets = skill.GetTargetsAffected(sweeper, effected);
            if (targets is not null)
            {
                foreach (WorldObject wo in targets)
                {
                    if (wo.isAttackable())
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
                                        Config.Npc.CORPSE_CONSUME_SKILL_ALLOWED_TIME_BEFORE_DECAY, true);
                                }

                                if (canSweep)
                                {
                                    canSweep = sweeper.getInventory().
                                        checkInventorySlotsAndWeight(attackable.getSpoilLootItems(), true, true);
                                }
                            }
                            else
                            {
                                sweeper.sendPacket(SystemMessageId.THE_SWEEPER_HAS_FAILED_AS_THE_TARGET_IS_NOT_SPOILED);
                            }
                        }
                    }
                }
            }
        }

        return value == canSweep;
    }
}