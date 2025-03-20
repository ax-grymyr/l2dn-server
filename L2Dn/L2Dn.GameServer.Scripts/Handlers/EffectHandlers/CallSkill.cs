using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Call Skill effect implementation.
/// </summary>
public sealed class CallSkill: AbstractEffect
{
    private readonly SkillHolder _skill;
    private readonly int _skillLevelScaleTo;
    private readonly int _chance;

    public CallSkill(StatSet @params)
    {
        _skill = new SkillHolder(@params.getInt("skillId"), @params.getInt("skillLevel", 1),
            @params.getInt("skillSubLevel", 0));

        _skillLevelScaleTo = @params.getInt("skillLevelScaleTo", 0);
        _chance = @params.getInt("chance", 100);
    }

    public override bool isInstant()
    {
        return true;
    }

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (_chance < 100 && Rnd.get(100) > _chance)
            return;

        Skill? triggerSkill;
        if (_skillLevelScaleTo <= 0)
        {
            // Mobius: Use 0 to trigger max effector learned skill level.
            if (_skill.getSkillLevel() == 0)
            {
                int knownLevel = effector.getSkillLevel(_skill.getSkillId());
                if (knownLevel > 0)
                {
                    triggerSkill = SkillData.Instance.
                        GetSkill(_skill.getSkillId(), knownLevel, _skill.getSkillSubLevel());
                }
                else
                {
                    LOGGER.Warn("Player " + effector + " called unknown skill " + _skill + " triggered by " + skill +
                        " CallSkill.");

                    return;
                }
            }
            else
            {
                triggerSkill = _skill.getSkill();
            }
        }
        else
        {
            BuffInfo? buffInfo = effected.getEffectList().getBuffInfoBySkillId(_skill.getSkillId());
            if (buffInfo != null)
            {
                triggerSkill = SkillData.Instance.GetSkill(_skill.getSkillId(),
                    Math.Min(_skillLevelScaleTo, buffInfo.getSkill().Level + 1));
            }
            else
            {
                triggerSkill = _skill.getSkill();
            }
        }

        if (triggerSkill != null)
        {
            // Prevent infinite loop.
            if (skill.Id == triggerSkill.Id && skill.Level == triggerSkill.Level)
            {
                return;
            }

            TimeSpan hitTime = triggerSkill.HitTime;
            if (hitTime > TimeSpan.Zero)
            {
                if (effector.isSkillDisabled(triggerSkill))
                {
                    return;
                }

                effector.broadcastPacket(new MagicSkillUsePacket(effector, effected, triggerSkill.DisplayId,
                    triggerSkill.Level, hitTime, TimeSpan.Zero));

                ThreadPool.schedule(() => SkillCaster.triggerCast(effector, effected, triggerSkill), hitTime);
            }
            else
            {
                SkillCaster.triggerCast(effector, effected, triggerSkill);
            }
        }
        else
        {
            LOGGER.Warn("Skill not found effect called from " + skill);
        }
    }

    public override int GetHashCode() => HashCode.Combine(_skill, _skillLevelScaleTo, _chance);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._skill, x._skillLevelScaleTo, x._chance));
}