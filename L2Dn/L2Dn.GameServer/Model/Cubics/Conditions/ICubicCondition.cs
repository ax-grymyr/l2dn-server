using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Cubics.Conditions;

public interface ICubicCondition
{
    bool test(Cubic cubic, Creature owner, WorldObject target);
}
