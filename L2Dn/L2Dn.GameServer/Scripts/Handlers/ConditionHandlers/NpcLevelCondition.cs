using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Conditions;

namespace L2Dn.GameServer.Scripts.Handlers.ConditionHandlers;

/**
 * @author Sdw
 */
public class NpcLevelCondition: ICondition
{
    private readonly int _minLevel;
    private readonly int _maxLevel;

    public NpcLevelCondition(StatSet @params)
    {
        _minLevel = @params.getInt("minLevel");
        _maxLevel = @params.getInt("maxLevel");
    }

    public bool test(Creature creature, WorldObject obj)
    {
        return obj.isNpc() && (((Creature)obj).getLevel() >= _minLevel) &&
               (((Creature)obj).getLevel() < _maxLevel);
    }
}