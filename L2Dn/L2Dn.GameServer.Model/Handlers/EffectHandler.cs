using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * @author BiggBoss, UnAfraid
 */
public class EffectHandler
{
	private readonly Map<string, Func<StatSet, AbstractEffect>> _effectHandlerFactories = new();

	private EffectHandler()
	{
		//_logger.Info(GetType().Name + ": Loaded " + size() + " effect handlers.");
	}
	
	public void registerHandler(string name, Func<StatSet, AbstractEffect> handlerFactory)
	{
		_effectHandlerFactories.put(name, handlerFactory);
	}
	
	public Func<StatSet, AbstractEffect>? getHandlerFactory(string name)
	{
		return _effectHandlerFactories.GetValueOrDefault(name);
	}
	
	public int size()
	{
		return _effectHandlerFactories.Count;
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