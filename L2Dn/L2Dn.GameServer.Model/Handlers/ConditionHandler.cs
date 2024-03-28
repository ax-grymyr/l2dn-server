using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Conditions;
using L2Dn.GameServer.Scripting;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * @author Sdw
 */
public class ConditionHandler
{
	private readonly Map<String, Func<StatSet, ICondition>> _conditionHandlerFactories = new();

	private ConditionHandler()
	{
	}
	
	public void registerHandler(String name, Func<StatSet, ICondition> handlerFactory)
	{
		_conditionHandlerFactories.put(name, handlerFactory);
	}
	
	public Func<StatSet, ICondition> getHandlerFactory(String name)
	{
		return _conditionHandlerFactories.get(name);
	}
	
	public int size()
	{
		return _conditionHandlerFactories.size();
	}
	
	public void executeScript()
	{
		try
		{
			ScriptEngineManager.getInstance().executeScript(ScriptEngineManager.CONDITION_HANDLER_FILE);
		}
		catch (Exception e)
		{
			throw new Exception("Problems while running ConditionMasterHandler", e);
		}
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