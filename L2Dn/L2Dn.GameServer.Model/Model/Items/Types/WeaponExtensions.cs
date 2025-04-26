using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Npcs;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Items.Types;

public static class WeaponExtensions
{
    /**
     * @param caster the Creature pointing out the caster
     * @param target the Creature pointing out the target
     * @param trigger
     * @param type
     */
    public static void applyConditionalSkills(this Weapon self, Creature caster, Creature target, Skill trigger,
        ItemSkillType type)
    {
        self.forEachSkill(type, holder =>
        {
            Skill skill = holder.getSkill();
            if (Rnd.get(100) >= holder.getChance())
            {
                return;
            }

            if (type == ItemSkillType.ON_MAGIC_SKILL)
            {
                // Trigger only if both are good or bad magic.
                if (trigger.IsBad != skill.IsBad)
                {
                    return;
                }

                // No Trigger if not Magic Skill or is toggle
                if (trigger.IsMagic != skill.IsMagic)
                {
                    return;
                }

                // No Trigger if skill is toggle
                if (trigger.IsToggle)
                {
                    return;
                }

                if (skill.IsBad && Formulas.calcShldUse(caster, target) == Formulas.SHIELD_DEFENSE_PERFECT_BLOCK)
                {
                    return;
                }
            }

            // Skill condition not met
            if (!skill.CheckCondition(caster, target, true))
            {
                return;
            }

            skill.ActivateSkill(caster, [target]);

            // TODO: Verify if this applies ONLY to ON_MAGIC_SKILL!
            if (type == ItemSkillType.ON_MAGIC_SKILL)
            {
                // notify quests of a skill use
                Player? casterPlayer = caster.getActingPlayer();
                if (caster.isPlayer() && casterPlayer != null)
                {
                    World.getInstance().forEachVisibleObjectInRange<Npc>(caster, 1000, npc =>
                    {
                        if (npc.Events.HasSubscribers<OnNpcSkillSee>())
                        {
                            npc.Events.NotifyAsync(new OnNpcSkillSee(npc, casterPlayer, skill, false, target));
                        }
                    });
                }

                if (caster.isPlayer())
                {
                    SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_HAS_BEEN_ACTIVATED);
                    sm.Params.addSkillName(skill);
                    caster.sendPacket(sm);
                }
            }
        });
    }
}