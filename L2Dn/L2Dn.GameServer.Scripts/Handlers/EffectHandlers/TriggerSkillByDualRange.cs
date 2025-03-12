using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Trigger Skill By Dual Range effect implementation.
/// </summary>
public sealed class TriggerSkillByDualRange: AbstractEffect
{
    private readonly SkillHolder _closeSkill;
    private readonly SkillHolder _rangeSkill;
    private readonly int _distance;
    private readonly bool _adjustLevel;

    public TriggerSkillByDualRange(StatSet @params)
    {
        // Just use closeSkill and rangeSkill parameters.
        _closeSkill = new SkillHolder(@params.getInt("closeSkill"), @params.getInt("closeSkillLevel", 1));
        _rangeSkill = new SkillHolder(@params.getInt("rangeSkill"), @params.getInt("rangeSkillLevel", 1));
        _distance = @params.getInt("distance", 120);
        _adjustLevel = @params.getBoolean("adjustLevel", true);
    }

    public override bool isInstant() => true;

    public override EffectType getEffectType() => EffectType.DUAL_RANGE;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        if (effected == null || !effector.isPlayer() || player == null)
            return;

        SkillHolder skillHolder = effector.Distance3D(effected) < _distance ? _closeSkill : _rangeSkill;
        Skill? triggerSkill = _adjustLevel
            ? SkillData.getInstance().getSkill(skillHolder.getSkillId(), skill.getLevel())
            : skillHolder.getSkill();

        if (triggerSkill == null)
            return;

        if (effected.isPlayable() && !effected.isAutoAttackable(effector))
            player.updatePvPStatus();

        player.useMagic(triggerSkill, null, true, triggerSkill.getCastRange() > 600);
    }

    public override int GetHashCode() => HashCode.Combine(_closeSkill, _rangeSkill, _distance, _adjustLevel);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._closeSkill, x._rangeSkill, x._distance, x._adjustLevel));
}