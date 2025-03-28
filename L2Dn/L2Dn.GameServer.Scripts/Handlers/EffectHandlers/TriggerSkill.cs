using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Trigger Skill effect implementation.
/// </summary>
public sealed class TriggerSkill: AbstractEffect
{
    private readonly SkillHolder _skill;
    private readonly TargetType _targetType;
    private readonly bool _adjustLevel;

    public TriggerSkill(StatSet @params)
    {
        _skill = new SkillHolder(@params.getInt("skillId"), @params.getInt("skillLevel", 1));
        _targetType = @params.getEnum("targetType", TargetType.TARGET);
        _adjustLevel = @params.getBoolean("adjustLevel", false);
    }

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        if (effected == null || !effected.isCreature() || !effector.isPlayer() || player == null)
            return;

        Skill? triggerSkill = _adjustLevel
            ? SkillData.getInstance().getSkill(_skill.getSkillId(), skill.getLevel())
            : _skill.getSkill();

        if (triggerSkill == null)
            return;

        WorldObject? target = null;
        try
        {
            target = TargetHandler.getInstance().getHandler(_targetType)?.
                getTarget(effector, effected, triggerSkill, false, false, false);
        }
        catch (Exception e)
        {
            LOGGER.Warn("Exception in ITargetTypeHandler.getTarget(): " + e);
        }

        if (target == null || !target.isCreature())
            return;

        SkillUseHolder? queuedSkill = player.getQueuedSkill();
        if (queuedSkill != null)
        {
            ThreadPool.schedule(() =>
            {
                player.setQueuedSkill(queuedSkill.getSkill(), queuedSkill.getItem(), queuedSkill.isCtrlPressed(),
                    queuedSkill.isShiftPressed());
            }, 10);
        }

        player.setQueuedSkill(triggerSkill, null, false, false);
    }

    public override int GetHashCode() => HashCode.Combine(_skill, _targetType, _adjustLevel);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._skill, x._targetType, x._adjustLevel));
}