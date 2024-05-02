using L2Dn.GameServer.Model.Actor;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Cubics.Conditions;

public class RangeCondition: ICubicCondition
{
    private readonly int _range;

    public RangeCondition(int range)
    {
        _range = range;
    }

    public bool test(Cubic cubic, Creature owner, WorldObject target)
    {
        return owner.Distance2D(target) <= _range;
    }
}