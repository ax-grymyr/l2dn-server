using System.Runtime.CompilerServices;
using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures;

/**
 * An instantly executed event when Creature attack miss Creature.
 */
public class OnCreatureAttackAvoid(Creature attacker, Creature target, bool damageOverTime): EventBase
{
    private Creature _attacker = attacker;
    private Creature _target = target;
    private bool _damageOverTime = damageOverTime;

    public Creature getAttacker() => _attacker;

    public void setAttacker(Creature attacker)
    {
        _attacker = attacker;
    }

    public Creature getTarget() => _target;

    public void setTarget(Creature target)
    {
        _target = target;
    }

    public bool isDamageOverTime() => _damageOverTime;

    public void setDamageOverTime(bool damageOverTime)
    {
        _damageOverTime = damageOverTime;
    }
}