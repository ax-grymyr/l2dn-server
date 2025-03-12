using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Pets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class ReplaceSkillBySkill: AbstractEffect
{
    private readonly SkillHolder _existingSkill;
    private readonly SkillHolder _replacementSkill;

    public ReplaceSkillBySkill(StatSet @params)
    {
        _existingSkill = new SkillHolder(@params.getInt("existingSkillId"), @params.getInt("existingSkillLevel", -1));
        _replacementSkill = new SkillHolder(@params.getInt("replacementSkillId"),
            @params.getInt("replacementSkillLevel", -1));
    }

    public override bool canStart(Creature effector, Creature effected, Skill skill)
    {
        return effected.isPlayable() &&
            (!effected.isTransformed() || effected.hasAbnormalType(AbnormalType.KAMAEL_TRANSFORM));
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Playable playable = (Playable)effected;
        Skill? knownSkill = playable.getKnownSkill(_existingSkill.getSkillId());
        if (knownSkill == null || knownSkill.getLevel() < _existingSkill.getSkillLevel())
            return;

        Skill? addedSkill = SkillData.getInstance().getSkill(_replacementSkill.getSkillId(),
            _replacementSkill.getSkillLevel() < 1 ? knownSkill.getLevel() : _replacementSkill.getSkillLevel(),
            knownSkill.getSubLevel());

        if (addedSkill == null)
            return;

        Player? player = effected.getActingPlayer();
        if (playable.isPlayer() && player != null)
        {
            player.addSkill(addedSkill, false);
            player.addReplacedSkill(_existingSkill.getSkillId(), _replacementSkill.getSkillId());
            foreach (Shortcut shortcut in player.getAllShortCuts())
            {
                if (shortcut.isAutoUse() && shortcut.getType() == ShortcutType.SKILL &&
                    shortcut.getId() == knownSkill.getId())
                {
                    if (knownSkill.isBad())
                    {
                        if (player.getAutoUseSettings().getAutoSkills().Contains(knownSkill.getId()))
                        {
                            player.getAutoUseSettings().getAutoSkills().Add(addedSkill.getId());
                            player.getAutoUseSettings().getAutoSkills().Remove(knownSkill.getId());
                        }
                    }
                    else if (player.getAutoUseSettings().getAutoBuffs().Contains(knownSkill.getId()))
                    {
                        player.getAutoUseSettings().getAutoBuffs().Add(addedSkill.getId());
                        player.getAutoUseSettings().getAutoBuffs().Remove(knownSkill.getId());
                    }
                }
            }

            // Replace continuous effects.
            if (knownSkill.isContinuous() && player.isAffectedBySkill(knownSkill.getId()))
            {
                TimeSpan? abnormalTime = null;
                foreach (BuffInfo info in player.getEffectList().getEffects())
                {
                    if (info.getSkill().getId() == knownSkill.getId())
                    {
                        abnormalTime = info.getAbnormalTime();
                        break;
                    }
                }

                if (abnormalTime > TimeSpan.FromMilliseconds(2000))
                {
                    addedSkill.applyEffects(player, player);
                    List<BuffInfo> skills = [];
                    foreach (BuffInfo info in player.getEffectList().getEffects())
                    {
                        if (info.getSkill().getId() == addedSkill.getId())
                        {
                            info.resetAbnormalTime(abnormalTime);
                            skills.Add(info);
                        }
                    }

                    AbnormalStatusUpdatePacket asu = new AbnormalStatusUpdatePacket(skills);
                    player.sendPacket(asu);
                }
            }

            player.removeSkill(knownSkill, false);
            player.sendSkillList();
        }
        else // Not player.
        {
            playable.addSkill(addedSkill);
            playable.removeSkill(knownSkill, false);
            if (playable.isPet())
            {
                Pet pet = (Pet)playable;
                pet.sendPacket(new ExPetSkillListPacket(false, pet));
            }
        }
    }

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        Playable playable = (Playable)effected;
        int existingSkillId = _existingSkill.getSkillId();
        if (playable.getReplacementSkill(existingSkillId) == existingSkillId)
            return;

        Skill? knownSkill = playable.getKnownSkill(_replacementSkill.getSkillId());
        if (knownSkill == null)
            return;

        Skill? addedSkill = SkillData.getInstance().
            getSkill(existingSkillId, knownSkill.getLevel(), knownSkill.getSubLevel());

        if (addedSkill == null)
            return;

        Player? player = effected.getActingPlayer();
        if (playable.isPlayer() && player != null)
        {
            player.addSkill(addedSkill, knownSkill.getLevel() != _existingSkill.getSkillLevel());
            player.removeReplacedSkill(existingSkillId);
            foreach (Shortcut shortcut in player.getAllShortCuts())
            {
                if (shortcut.isAutoUse() && shortcut.getType() == ShortcutType.SKILL &&
                    shortcut.getId() == addedSkill.getId())
                {
                    if (knownSkill.isBad())
                    {
                        if (player.getAutoUseSettings().getAutoSkills().Contains(knownSkill.getId()))
                        {
                            player.getAutoUseSettings().getAutoSkills().Add(addedSkill.getId());
                            player.getAutoUseSettings().getAutoSkills().Remove(knownSkill.getId());
                        }
                    }
                    else if (player.getAutoUseSettings().getAutoBuffs().Contains(knownSkill.getId()))
                    {
                        player.getAutoUseSettings().getAutoBuffs().Add(addedSkill.getId());
                        player.getAutoUseSettings().getAutoBuffs().Remove(knownSkill.getId());
                    }
                }
            }

            // Replace continuous effects.
            if (knownSkill.isContinuous() && player.isAffectedBySkill(knownSkill.getId()))
            {
                TimeSpan? abnormalTime = null;
                foreach (BuffInfo info in player.getEffectList().getEffects())
                {
                    if (info.getSkill().getId() == knownSkill.getId())
                    {
                        abnormalTime = info.getAbnormalTime();
                        break;
                    }
                }

                if (abnormalTime > TimeSpan.FromMilliseconds(2000))
                {
                    addedSkill.applyEffects(player, player);
                    List<BuffInfo> skills = [];
                    foreach (BuffInfo info in player.getEffectList().getEffects())
                    {
                        if (info.getSkill().getId() == addedSkill.getId())
                        {
                            info.resetAbnormalTime(abnormalTime);
                            skills.Add(info);
                        }
                    }

                    AbnormalStatusUpdatePacket asu = new AbnormalStatusUpdatePacket(skills);
                    player.sendPacket(asu);
                }
            }

            player.removeSkill(knownSkill, false);
            player.sendSkillList();
        }
        else // Not player.
        {
            playable.addSkill(addedSkill);
            playable.removeSkill(knownSkill, false);
            if (playable.isPet())
            {
                Pet pet = (Pet)playable;
                pet.sendPacket(new ExPetSkillListPacket(false, pet));
            }
        }
    }

    public override int GetHashCode() => HashCode.Combine(_existingSkill, _replacementSkill);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._existingSkill, x._replacementSkill));
}