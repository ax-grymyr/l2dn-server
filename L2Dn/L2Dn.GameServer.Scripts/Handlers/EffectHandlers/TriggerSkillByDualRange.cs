using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Trigger Skill By Dual Range effect implementation.
/// </summary>
[HandlerName("TriggerSkillByDualRange")]
public sealed class TriggerSkillByDualRange: AbstractEffect
{
    private readonly SkillHolder _closeSkill;
    private readonly SkillHolder _rangeSkill;
    private readonly int _distance;
    private readonly bool _adjustLevel;

    public TriggerSkillByDualRange(EffectParameterSet parameters)
    {
        // Just use closeSkill and rangeSkill parameters.
        _closeSkill = new SkillHolder(parameters.GetInt32(XmlSkillEffectParameterType.CloseSkill),
            parameters.GetInt32(XmlSkillEffectParameterType.CloseSkillLevel, 1));

        _rangeSkill = new SkillHolder(parameters.GetInt32(XmlSkillEffectParameterType.RangeSkill),
            parameters.GetInt32(XmlSkillEffectParameterType.RangeSkillLevel, 1));

        _distance = parameters.GetInt32(XmlSkillEffectParameterType.Distance, 120);
        _adjustLevel = parameters.GetBoolean(XmlSkillEffectParameterType.AdjustLevel, true);
    }

    public override bool IsInstant => true;

    public override EffectTypes EffectTypes => EffectTypes.DUAL_RANGE;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        if (effected == null || !effector.isPlayer() || player == null)
            return;

        SkillHolder skillHolder = effector.Distance3D(effected) < _distance ? _closeSkill : _rangeSkill;
        Skill? triggerSkill = _adjustLevel
            ? SkillData.Instance.GetSkill(skillHolder.getSkillId(), skill.Level)
            : skillHolder.getSkill();

        if (triggerSkill == null)
            return;

        if (effected.isPlayable() && !effected.isAutoAttackable(effector))
            player.updatePvPStatus();

        player.useMagic(triggerSkill, null, true, triggerSkill.CastRange > 600);
    }

    public override int GetHashCode() => HashCode.Combine(_closeSkill, _rangeSkill, _distance, _adjustLevel);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._closeSkill, x._rangeSkill, x._distance, x._adjustLevel));
}