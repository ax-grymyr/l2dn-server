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
/// Root effect implementation.
/// </summary>
[AbstractEffectName("Root")]
public sealed class Root: AbstractEffect
{
    public override EffectFlags EffectFlags => EffectFlags.ROOTED;

    public override EffectTypes EffectTypes => EffectTypes.ROOT;

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        if (!effected.isPlayer())
            effected.getAI().notifyEvent(CtrlEvent.EVT_THINK);
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected == null || effected.isRaid())
            return;

        effected.stopMove(null);
        effected.getAI().notifyEvent(CtrlEvent.EVT_ROOTED);
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}