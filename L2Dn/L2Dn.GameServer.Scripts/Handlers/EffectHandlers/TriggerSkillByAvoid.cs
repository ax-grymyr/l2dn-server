using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Trigger Skill By Avoid effect implementation.
/// </summary>
public sealed class TriggerSkillByAvoid: AbstractEffect
{
    private readonly int _chance;
    private readonly SkillHolder _skill;
    private readonly TargetType _targetType;
    private readonly int _skillLevelScaleTo;

    public TriggerSkillByAvoid(StatSet @params)
    {
        _chance = @params.getInt("chance", 100);
        _skill = new SkillHolder(@params.getInt("skillId", 0), @params.getInt("skillLevel", 0));
        _targetType = @params.getEnum("targetType", TargetType.TARGET);
        _skillLevelScaleTo = @params.getInt("skillLevelScaleTo", 0);
    }

    private void onAvoidEvent(OnCreatureAttackAvoid @event)
    {
        if (@event.isDamageOverTime() || _chance == 0 || _skill.getSkillId() == 0 || _skill.getSkillLevel() == 0)
            return;

        ITargetTypeHandler? targetHandler = TargetHandler.getInstance().getHandler(_targetType);
        if (targetHandler == null)
        {
            Logger.Warn("Handler for target type: " + _targetType + " does not exist.");
            return;
        }

        if (_chance < 100 && Rnd.get(100) > _chance)
            return;

        WorldObject? target = null;
        try
        {
            target = TargetHandler.getInstance().getHandler(_targetType)?.getTarget(@event.getTarget(),
                @event.getAttacker(), _skill.getSkill(), false, false, false);
        }
        catch (Exception e)
        {
            Logger.Warn("Exception in ITargetTypeHandler.getTarget(): " + e);
        }

        if (target == null || !target.isCreature())
            return;

        Skill? triggerSkill;
        if (_skillLevelScaleTo <= 0)
        {
            triggerSkill = _skill.getSkill();
        }
        else
        {
            BuffInfo? buffInfo = ((Creature)target).getEffectList().getBuffInfoBySkillId(_skill.getSkillId());
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

        if (triggerSkill == null)
            return;

        SkillCaster.triggerCast(@event.getAttacker(), (Creature)target, triggerSkill);
    }

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        effected.Events.Unsubscribe<OnCreatureAttackAvoid>(onAvoidEvent);
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.Events.Subscribe<OnCreatureAttackAvoid>(this, onAvoidEvent);
    }

    public override int GetHashCode() => HashCode.Combine(_chance, _skill, _targetType, _skillLevelScaleTo);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._chance, x._skill, x._targetType, x._skillLevelScaleTo));
}