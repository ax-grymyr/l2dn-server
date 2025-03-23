using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpSweeper")]
public sealed class OpSweeperSkillCondition: ISkillCondition
{
    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        bool canSweep = false;
        Player? sweeper = caster.getActingPlayer();
        if (sweeper != null)
        {
            if (skill != null)
            {
                List<WorldObject>? targets = skill.GetTargetsAffected(sweeper, target);
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

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}