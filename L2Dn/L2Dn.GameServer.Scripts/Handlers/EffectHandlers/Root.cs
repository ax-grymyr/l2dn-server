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
/// Root effect implementation.
/// </summary>
public sealed class Root: AbstractEffect
{
    public Root(StatSet @params)
    {
    }

    public override EffectFlags EffectFlags => EffectFlags.ROOTED;

    public override EffectTypes EffectType => EffectTypes.ROOT;

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        if (!effected.isPlayer())
            effected.getAI().notifyEvent(CtrlEvent.EVT_THINK);
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected == null || effected.isRaid())
            return;

        effected.stopMove(null);
        effected.getAI().notifyEvent(CtrlEvent.EVT_ROOTED);
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}