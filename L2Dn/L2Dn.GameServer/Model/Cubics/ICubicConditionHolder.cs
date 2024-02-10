using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Cubics;

public interface ICubicConditionHolder
{
    bool validateConditions(Cubic cubic, Creature owner, WorldObject target);
    void addCondition(ICubicCondition condition);
}
