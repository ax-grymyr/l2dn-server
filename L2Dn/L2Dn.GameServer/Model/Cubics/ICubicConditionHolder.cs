using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Cubics.Conditions;

namespace L2Dn.GameServer.Model.Cubics;

public interface ICubicConditionHolder
{
    bool validateConditions(Cubic cubic, Creature owner, WorldObject target);
    void addCondition(ICubicCondition condition);
}
