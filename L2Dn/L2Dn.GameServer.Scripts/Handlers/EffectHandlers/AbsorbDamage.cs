using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("AbsorbDamage")]
public sealed class AbsorbDamage: AbstractEffect
{
    private static readonly Map<int, double> _diffDamageHolder = new();
    private static readonly Map<int, double> _perDamageHolder = new();

    private readonly double _damage;
    private readonly StatModifierType _mode;

    public AbsorbDamage(EffectParameterSet parameters)
    {
        _damage = parameters.GetDouble(XmlSkillEffectParameterType.Damage, 0);
        _mode = parameters.GetEnum(XmlSkillEffectParameterType.Mode, StatModifierType.DIFF);
    }

    private static void OnDamageReceivedDiffEvent(OnCreatureDamageReceived ev, Creature effected, Skill skill)
    {
        // DOT effects are not taken into account.
        if (ev.isDamageOverTime())
            return;

        int objectId = ev.getTarget().ObjectId;

        double damageLeft = _diffDamageHolder.GetValueOrDefault(objectId);
        double newDamageLeft = Math.Max(damageLeft - ev.getDamage(), 0);
        double newDamage = Math.Max(ev.getDamage() - damageLeft, 0);

        if (newDamageLeft > 0)
            _diffDamageHolder.put(objectId, newDamageLeft);
        else
            effected.stopSkillEffects(skill);

        ev.OverrideDamage = true;
        ev.OverridenDamage = newDamage;
    }

    private static void OnDamageReceivedPerEvent(OnCreatureDamageReceived ev)
    {
        // DOT effects are not taken into account.
        if (ev.isDamageOverTime())
            return;

        int objectId = ev.getTarget().ObjectId;

        double damagePercent = _perDamageHolder.GetValueOrDefault(objectId);
        double currentDamage = ev.getDamage();
        double newDamage = currentDamage - currentDamage / 100 * damagePercent;

        ev.OverrideDamage = true;
        ev.OverridenDamage = newDamage;
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        if (_mode == StatModifierType.DIFF)
        {
            effected.Events.UnsubscribeAll<OnCreatureDamageReceived>(this);
            _diffDamageHolder.remove(effected.ObjectId);
        }
        else
        {
            effected.Events.Unsubscribe<OnCreatureDamageReceived>(OnDamageReceivedPerEvent);
            _perDamageHolder.remove(effected.ObjectId);
        }
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (_mode == StatModifierType.DIFF)
        {
            _diffDamageHolder.put(effected.ObjectId, _damage);
            effected.Events.Subscribe<OnCreatureDamageReceived>(this,
                ev => OnDamageReceivedDiffEvent(ev, effected, skill));
        }
        else
        {
            _perDamageHolder.put(effected.ObjectId, _damage);
            effected.Events.Subscribe<OnCreatureDamageReceived>(this, OnDamageReceivedPerEvent);
        }
    }

    public override int GetHashCode() => HashCode.Combine(_damage, _mode);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._damage, x._mode));
}