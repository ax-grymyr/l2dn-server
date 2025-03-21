using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Trigger Skill effect implementation.
/// </summary>
[HandlerStringKey("TriggerSkill")]
public sealed class TriggerSkill: AbstractEffect
{
    private readonly SkillHolder _skill;
    private readonly TargetType _targetType;
    private readonly bool _adjustLevel;

    public TriggerSkill(EffectParameterSet parameters)
    {
        _skill = new SkillHolder(parameters.GetInt32(XmlSkillEffectParameterType.SkillId),
            parameters.GetInt32(XmlSkillEffectParameterType.SkillLevel, 1));

        _targetType = parameters.GetEnum(XmlSkillEffectParameterType.TargetType, TargetType.TARGET);
        _adjustLevel = parameters.GetBoolean(XmlSkillEffectParameterType.AdjustLevel, false);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        if (effected == null || !effected.isCreature() || !effector.isPlayer() || player == null)
            return;

        Skill? triggerSkill = _adjustLevel
            ? SkillData.Instance.GetSkill(_skill.getSkillId(), skill.Level)
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
            Logger.Warn("Exception in ITargetTypeHandler.getTarget(): " + e);
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