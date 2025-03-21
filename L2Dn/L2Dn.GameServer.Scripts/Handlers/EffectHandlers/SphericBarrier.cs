using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Templates;
using L2Dn.Geometry;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("SphericBarrier")]
public sealed class SphericBarrier(EffectParameterSet parameters)
    : AbstractStatAddEffect(parameters, Stat.SPHERIC_BARRIER_RANGE)
{
    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.Events.Subscribe<OnCreatureDamageReceived>(this, OnDamageReceivedEvent);
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        effected.Events.Unsubscribe<OnCreatureDamageReceived>(OnDamageReceivedEvent);
    }

    private void OnDamageReceivedEvent(OnCreatureDamageReceived ev)
    {
        if (ev.getAttacker()?.Distance3D(ev.getTarget()) > Amount)
        {
            ev.OverrideDamage = true;
            ev.OverridenDamage = 0;
        }
    }
}