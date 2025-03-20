using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Mute effect implementation.
/// </summary>
public sealed class Mute: AbstractEffect
{
    public Mute(StatSet @params)
    {
    }

    public override long getEffectFlags() => EffectFlag.MUTED.getMask();

    public override EffectType getEffectType() => EffectType.MUTE;

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected == null || effected.isRaid())
            return;

        effected.abortCast();
        effected.getAI().notifyEvent(CtrlEvent.EVT_MUTED);
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}