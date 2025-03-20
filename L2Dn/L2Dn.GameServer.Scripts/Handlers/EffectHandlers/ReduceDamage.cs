using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class ReduceDamage: AbstractEffect
{
    private readonly double _amount;
    private readonly StatModifierType _mode;

    public ReduceDamage(EffectParameterSet parameters)
    {
        _amount = parameters.GetDouble(XmlSkillEffectParameterType.Amount);
        _mode = parameters.GetEnum(XmlSkillEffectParameterType.Mode, StatModifierType.DIFF);
    }

    private void onDamageReceivedEvent(OnCreatureDamageReceived ev)
    {
        // DOT effects are not taken into account.
        if (ev.isDamageOverTime())
            return;

        double newDamage;
        if (_mode == StatModifierType.PER)
        {
            newDamage = ev.getDamage() - ev.getDamage() * (_amount / 100);
        }
        else // DIFF
        {
            Creature? attacker = ev.getAttacker();
            double ignoreReduceDamage = attacker?.getStat().getAddValue(Stat.IGNORE_REDUCE_DAMAGE) ?? 0.0;
            newDamage = ev.getDamage() - Math.Max(_amount - ignoreReduceDamage, 0.0);
        }

        ev.OverrideDamage = true;
        ev.OverridenDamage = newDamage;
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        effected.Events.Unsubscribe<OnCreatureDamageReceived>(onDamageReceivedEvent);
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.Events.Subscribe<OnCreatureDamageReceived>(this, onDamageReceivedEvent);
    }

    public override int GetHashCode() => HashCode.Combine(_amount, _mode);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._amount, x._mode));
}