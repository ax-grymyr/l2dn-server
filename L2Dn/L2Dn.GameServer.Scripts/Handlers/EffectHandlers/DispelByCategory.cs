using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Dispel By Category effect implementation.
/// </summary>
public sealed class DispelByCategory: AbstractEffect
{
    private readonly DispelSlotType _slot;
    private readonly int _rate;
    private readonly int _max;

    public DispelByCategory(StatSet @params)
    {
        _slot = @params.getEnum("slot", DispelSlotType.BUFF);
        _rate = @params.getInt("rate", 0);
        _max = @params.getInt("max", 0);
    }

    public override EffectTypes EffectType => EffectTypes.DISPEL;

    public override bool IsInstant => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isDead())
            return;

        List<BuffInfo> canceled = Formulas.calcCancelStealEffects(effector, effected, skill, _slot, _rate, _max);
        foreach (BuffInfo can in canceled)
            effected.getEffectList().stopSkillEffects(SkillFinishType.REMOVED, can.getSkill());
    }

    public override int GetHashCode() => HashCode.Combine(_slot, _rate, _max);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._slot, x._rate, x._max));
}