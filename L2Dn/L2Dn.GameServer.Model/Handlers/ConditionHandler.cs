using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Conditions;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * @author Sdw
 */
public class ConditionHandler
{
    private readonly Map<string, Func<StatSet, IConditionBase>> _conditionHandlerFactories = new();

    private ConditionHandler()
    {
    }

    public void registerHandler(string name, Func<StatSet, IConditionBase> handlerFactory)
    {
        _conditionHandlerFactories.put(name, handlerFactory);
    }

    public Func<StatSet, IConditionBase>? getHandlerFactory(string name)
    {
        return _conditionHandlerFactories.get(name);
    }

    public int size()
    {
        return _conditionHandlerFactories.Count;
    }

    private static class SingletonHolder
    {
        public static readonly ConditionHandler INSTANCE = new ConditionHandler();
    }

    public static ConditionHandler getInstance()
    {
        return SingletonHolder.INSTANCE;
    }
}