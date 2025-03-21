using System.Collections.Immutable;
using System.Runtime.InteropServices;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Options;

public static class OptionExtensions
{
    public static bool HasEffects(this Option option) => !option.Effects.IsDefaultOrEmpty;

    public static IEnumerable<AbstractEffect> GetEffects(this Option option)
    {
        ImmutableArray<IAbstractEffect> abstractEffects = option.Effects;
        IAbstractEffect[]? array = ImmutableCollectionsMarshal.AsArray(abstractEffects);
        return array?.Cast<AbstractEffect>() ?? [];
    }

    public static void Apply(this Option option, Playable playable)
    {
        if (option.HasEffects())
        {
            Skill skill = null!; // TODO: WARNING! the skill is used in many AbstractEffects but here it is null
            BuffInfo info = new BuffInfo(playable, playable, skill, true, null, option);
            foreach (AbstractEffect effect in option.GetEffects())
            {
                if (effect.IsInstant)
                {
                    if (effect.CalcSuccess(info.getEffector(), info.getEffected(), info.getSkill()))
                    {
                        effect.Instant(info.getEffector(), info.getEffected(), info.getSkill(), info.getItem());
                    }
                }
                else
                {
                    effect.ContinuousInstant(info.getEffector(), info.getEffected(), info.getSkill(), info.getItem());
                    effect.Pump(playable, info.getSkill());
                    if (effect.CanStart(info.getEffector(), info.getEffected(), info.getSkill()))
                    {
                        info.addEffect(effect);
                    }
                }
            }

            if (info.getEffects().Count != 0)
            {
                playable.getEffectList().add(info);
            }
        }

        foreach (Skill skill in option.ActiveSkills)
            AddSkill(playable, skill);

        foreach (Skill skill in option.PassiveSkills)
            AddSkill(playable, skill);

        foreach (OptionSkillHolder holder in option.ActivationSkills)
            playable.addTriggerSkill(holder);

        playable.getStat().recalculateStats(true);

        Player? player = playable.getActingPlayer();
        if (playable.isPlayer() && player != null)
            player.sendSkillList();
    }

    public static void Remove(this Option option, Playable playable)
    {
        if (option.HasEffects())
        {
            foreach (BuffInfo info in playable.getEffectList().getOptions())
            {
                if (info.getOption() == option)
                {
                    playable.getEffectList().remove(info, SkillFinishType.NORMAL, true, true);
                }
            }
        }

        foreach (Skill skill in option.ActiveSkills)
            playable.removeSkill(skill, false);

        foreach (Skill skill in option.PassiveSkills)
            playable.removeSkill(skill, true);

        foreach (OptionSkillHolder holder in option.ActivationSkills)
            playable.removeTriggerSkill(holder);

        playable.getStat().recalculateStats(true);

        Player? player = playable.getActingPlayer();
        if (playable.isPlayer() && player != null)
            player.sendSkillList();
    }

    private static void AddSkill(Playable playable, Skill skill)
    {
        bool updateTimeStamp = false;
        playable.addSkill(skill);
        if (skill.IsActive)
        {
            TimeSpan remainingTime = playable.getSkillRemainingReuseTime(skill.ReuseHashCode);
            if (remainingTime > TimeSpan.Zero)
            {
                playable.addTimeStamp(skill, remainingTime);
                playable.disableSkill(skill, remainingTime);
            }

            updateTimeStamp = true;
        }

        Player? player = playable.getActingPlayer();
        if (updateTimeStamp && playable.isPlayer() && player != null)
        {
            playable.sendPacket(new SkillCoolTimePacket(player));
        }
    }
}