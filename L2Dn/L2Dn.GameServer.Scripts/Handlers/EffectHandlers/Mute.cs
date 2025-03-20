using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Mute effect implementation.
/// </summary>
[AbstractEffectName("Mute")]
public sealed class Mute: AbstractEffect
{
    public override EffectFlags EffectFlags => EffectFlags.MUTED;

    public override EffectTypes EffectTypes => EffectTypes.MUTE;

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected == null || effected.isRaid())
            return;

        effected.abortCast();
        effected.getAI().notifyEvent(CtrlEvent.EVT_MUTED);
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}