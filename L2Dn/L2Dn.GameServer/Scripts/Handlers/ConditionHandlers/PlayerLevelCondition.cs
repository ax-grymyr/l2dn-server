using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Conditions;

namespace L2Dn.GameServer.Scripts.Handlers.ConditionHandlers;

public class PlayerLevelCondition: ICondition
{
    private readonly int _minLevel;
    private readonly int _maxLevel;

    public PlayerLevelCondition(StatSet @params)
    {
        _minLevel = @params.getInt("minLevel");
        _maxLevel = @params.getInt("maxLevel");
    }

    public bool test(Creature creature, WorldObject obj)
    {
        return creature.isPlayer() && (creature.getLevel() >= _minLevel) && (creature.getLevel() < _maxLevel);
    }
}