using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Synergy effect implementation.
/// </summary>
public sealed class Synergy: AbstractEffect
{
    private readonly FrozenSet<AbnormalType> _requiredSlots;
    private readonly FrozenSet<AbnormalType> _optionalSlots;
    private readonly int _partyBuffSkillId;
    private readonly int _skillLevelScaleTo;
    private readonly int _minSlot;

    public Synergy(StatSet @params)
    {
        string requiredSlots = @params.getString("requiredSlots", string.Empty);
        _requiredSlots = ParseUtil.ParseEnumSet<AbnormalType>(requiredSlots);

        string optionalSlots = @params.getString("optionalSlots", string.Empty);
        _optionalSlots = ParseUtil.ParseEnumSet<AbnormalType>(optionalSlots);

        _partyBuffSkillId = @params.getInt("partyBuffSkillId");
        _skillLevelScaleTo = @params.getInt("skillLevelScaleTo", 1);
        _minSlot = @params.getInt("minSlot", 2);
        Ticks = @params.getInt("ticks");
    }

    public override bool onActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effector.isDead())
            return false;

        foreach (AbnormalType required in _requiredSlots)
        {
            if (!effector.hasAbnormalType(required))
                return skill.IsToggle;
        }

        int abnormalCount = 0;
        foreach (AbnormalType abnormalType in _optionalSlots)
        {
            if (effector.hasAbnormalType(abnormalType))
                abnormalCount++;
        }

        if (abnormalCount >= _minSlot)
        {
            SkillHolder partyBuff = new SkillHolder(_partyBuffSkillId, Math.Min(abnormalCount - 1, _skillLevelScaleTo));
            Skill partyBuffSkill = partyBuff.getSkill();
            if (partyBuffSkill != null)
            {
                WorldObject? target = partyBuffSkill.GetTarget(effector, effected, false, false, false);
                if (target != null && target.isCreature())
                {
                    BuffInfo? abnormalBuffInfo = effector.getEffectList().
                        getFirstBuffInfoByAbnormalType(partyBuffSkill.AbnormalType);

                    if (abnormalBuffInfo != null && abnormalBuffInfo.getSkill().AbnormalLevel != abnormalCount - 1)
                    {
                        effector.getEffectList().stopSkillEffects(SkillFinishType.REMOVED, _partyBuffSkillId);
                    }
                    else
                    {
                        SkillCaster.triggerCast(effector, (Creature)target, partyBuffSkill);
                    }
                }
            }
            else
            {
                Logger.Warn("Skill not found effect called from " + skill);
            }
        }
        else
        {
            effector.getEffectList().stopSkillEffects(SkillFinishType.REMOVED, _partyBuffSkillId);
        }

        return skill.IsToggle;
    }

    public override int GetHashCode() =>
        HashCode.Combine(_requiredSlots.GetSetHashCode(), _optionalSlots.GetSetHashCode(), _partyBuffSkillId,
            _skillLevelScaleTo, _minSlot);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj,
            static x => (x._requiredSlots.GetSetComparable(), x._optionalSlots.GetSetComparable(), x._partyBuffSkillId,
                x._skillLevelScaleTo, x._minSlot));
}