using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Pets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class ReplaceSkillBySkill: AbstractEffect
{
    private readonly SkillHolder _existingSkill;
    private readonly SkillHolder _replacementSkill;

    public ReplaceSkillBySkill(EffectParameterSet parameters)
    {
        _existingSkill = new SkillHolder(parameters.GetInt32(XmlSkillEffectParameterType.ExistingSkillId),
            parameters.GetInt32(XmlSkillEffectParameterType.ExistingSkillLevel, -1));

        _replacementSkill = new SkillHolder(parameters.GetInt32(XmlSkillEffectParameterType.ReplacementSkillId),
            parameters.GetInt32(XmlSkillEffectParameterType.ReplacementSkillLevel, -1));
    }

    public override bool CanStart(Creature effector, Creature effected, Skill skill)
    {
        return effected.isPlayable() &&
            (!effected.isTransformed() || effected.hasAbnormalType(AbnormalType.KAMAEL_TRANSFORM));
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Playable playable = (Playable)effected;
        Skill? knownSkill = playable.getKnownSkill(_existingSkill.getSkillId());
        if (knownSkill == null || knownSkill.Level < _existingSkill.getSkillLevel())
            return;

        Skill? addedSkill = SkillData.Instance.GetSkill(_replacementSkill.getSkillId(),
            _replacementSkill.getSkillLevel() < 1 ? knownSkill.Level : _replacementSkill.getSkillLevel(),
            knownSkill.SubLevel);

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
                    shortcut.getId() == knownSkill.Id)
                {
                    if (knownSkill.IsBad)
                    {
                        if (player.getAutoUseSettings().getAutoSkills().Contains(knownSkill.Id))
                        {
                            player.getAutoUseSettings().getAutoSkills().Add(addedSkill.Id);
                            player.getAutoUseSettings().getAutoSkills().Remove(knownSkill.Id);
                        }
                    }
                    else if (player.getAutoUseSettings().getAutoBuffs().Contains(knownSkill.Id))
                    {
                        player.getAutoUseSettings().getAutoBuffs().Add(addedSkill.Id);
                        player.getAutoUseSettings().getAutoBuffs().Remove(knownSkill.Id);
                    }
                }
            }

            // Replace continuous effects.
            if (knownSkill.IsContinuous && player.isAffectedBySkill(knownSkill.Id))
            {
                TimeSpan? abnormalTime = null;
                foreach (BuffInfo info in player.getEffectList().getEffects())
                {
                    if (info.getSkill().Id == knownSkill.Id)
                    {
                        abnormalTime = info.getAbnormalTime();
                        break;
                    }
                }

                if (abnormalTime > TimeSpan.FromMilliseconds(2000))
                {
                    addedSkill.ApplyEffects(player, player);
                    List<BuffInfo> skills = [];
                    foreach (BuffInfo info in player.getEffectList().getEffects())
                    {
                        if (info.getSkill().Id == addedSkill.Id)
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

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        Playable playable = (Playable)effected;
        int existingSkillId = _existingSkill.getSkillId();
        if (playable.getReplacementSkill(existingSkillId) == existingSkillId)
            return;

        Skill? knownSkill = playable.getKnownSkill(_replacementSkill.getSkillId());
        if (knownSkill == null)
            return;

        Skill? addedSkill = SkillData.Instance.
            GetSkill(existingSkillId, knownSkill.Level, knownSkill.SubLevel);

        if (addedSkill == null)
            return;

        Player? player = effected.getActingPlayer();
        if (playable.isPlayer() && player != null)
        {
            player.addSkill(addedSkill, knownSkill.Level != _existingSkill.getSkillLevel());
            player.removeReplacedSkill(existingSkillId);
            foreach (Shortcut shortcut in player.getAllShortCuts())
            {
                if (shortcut.isAutoUse() && shortcut.getType() == ShortcutType.SKILL &&
                    shortcut.getId() == addedSkill.Id)
                {
                    if (knownSkill.IsBad)
                    {
                        if (player.getAutoUseSettings().getAutoSkills().Contains(knownSkill.Id))
                        {
                            player.getAutoUseSettings().getAutoSkills().Add(addedSkill.Id);
                            player.getAutoUseSettings().getAutoSkills().Remove(knownSkill.Id);
                        }
                    }
                    else if (player.getAutoUseSettings().getAutoBuffs().Contains(knownSkill.Id))
                    {
                        player.getAutoUseSettings().getAutoBuffs().Add(addedSkill.Id);
                        player.getAutoUseSettings().getAutoBuffs().Remove(knownSkill.Id);
                    }
                }
            }

            // Replace continuous effects.
            if (knownSkill.IsContinuous && player.isAffectedBySkill(knownSkill.Id))
            {
                TimeSpan? abnormalTime = null;
                foreach (BuffInfo info in player.getEffectList().getEffects())
                {
                    if (info.getSkill().Id == knownSkill.Id)
                    {
                        abnormalTime = info.getAbnormalTime();
                        break;
                    }
                }

                if (abnormalTime > TimeSpan.FromMilliseconds(2000))
                {
                    addedSkill.ApplyEffects(player, player);
                    List<BuffInfo> skills = [];
                    foreach (BuffInfo info in player.getEffectList().getEffects())
                    {
                        if (info.getSkill().Id == addedSkill.Id)
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