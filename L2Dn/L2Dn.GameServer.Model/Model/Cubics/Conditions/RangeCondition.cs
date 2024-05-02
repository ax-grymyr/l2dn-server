using L2Dn.GameServer.Model.Actor;

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
        return owner.calculateDistance2D(target.getLocation().ToLocation2D()) <= _range;
    }
}