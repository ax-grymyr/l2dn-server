using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Scripting;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Handlers;

/**
 * @author BiggBoss, UnAfraid
 */
public class EffectHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(EffectHandler));
	private readonly Map<String, Func<StatSet, AbstractEffect>> _effectHandlerFactories = new();

	private EffectHandler()
	{
		//_logger.Info(GetType().Name + ": Loaded " + size() + " effect handlers.");
	}
	
	public void registerHandler(String name, Func<StatSet, AbstractEffect> handlerFactory)
	{
		_effectHandlerFactories.put(name, handlerFactory);
	}
	
	public Func<StatSet, AbstractEffect> getHandlerFactory(String name)
	{
		return _effectHandlerFactories.get(name);
	}
	
	public int size()
	{
		return _effectHandlerFactories.size();
	}
	
	public void executeScript()
	{
		try
		{
			ScriptEngineManager.getInstance().executeScript(ScriptEngineManager.EFFECT_MASTER_HANDLER_FILE);
		}
		catch (Exception e)
		{
			throw new Exception("Problems while running EffectMasterHandler", e);
		}
	}
	
	private static class SingletonHolder
	{
		public static readonly EffectHandler INSTANCE = new();
	}
	
	public static EffectHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
}