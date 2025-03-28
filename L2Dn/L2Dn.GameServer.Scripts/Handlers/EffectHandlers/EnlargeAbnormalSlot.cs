using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Enlarge Abnormal Slot effect implementation.
/// </summary>
public sealed class EnlargeAbnormalSlot: AbstractEffect
{
    private readonly int _slots;

    public EnlargeAbnormalSlot(StatSet @params)
    {
        _slots = @params.getInt("slots", 0);
    }

    public override bool canStart(Creature effector, Creature effected, Skill skill)
    {
        return effector != null && effected != null && effected.isPlayer();
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.getStat().setMaxBuffCount(effected.getStat().getMaxBuffCount() + _slots);
    }

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        effected.getStat().setMaxBuffCount(Math.Max(0, effected.getStat().getMaxBuffCount() - _slots));
    }

    public override int GetHashCode() => _slots;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._slots);
}