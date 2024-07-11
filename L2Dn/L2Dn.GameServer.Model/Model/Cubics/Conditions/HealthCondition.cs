using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;

namespace L2Dn.GameServer.Model.Cubics.Conditions;

public class HealthCondition: ICubicCondition
{
    private readonly int _min;
    private readonly int _max;

    public HealthCondition(int min, int max)
    {
        _min = min;
        _max = max;
    }

    public bool test(Cubic cubic, Creature owner, WorldObject target)
    {
        if (target.isCreature() || target.isDoor())
        {
            double hpPer = (target.isDoor() ? (Door)target : (Creature)target).getCurrentHpPercent();
            return (hpPer > _min) && (hpPer < _max);
        }

        return false;
    }

    public override string ToString()
    {
        return GetType().Name + " min: " + _min + " max: " + _max;
    }
}
