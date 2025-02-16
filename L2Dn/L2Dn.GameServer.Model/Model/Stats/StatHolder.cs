using L2Dn.GameServer.Model.Actor;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Stats;

public sealed class StatHolder(Stat stat, double value, Func<Creature, StatHolder, bool>? condition = null)
{
    public Stat Stat => stat;
    public double Value => value;
    public bool VerifyCondition(Creature creature) => condition is null || condition(creature, this);
}