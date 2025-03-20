namespace L2Dn.GameServer.Handlers;

public sealed class EffectHandler
{
    private readonly Dictionary<string, Func<EffectParameterSet, IAbstractEffect>>
        _effectHandlerFactories = new();

	private EffectHandler()
	{
		//_logger.Info(GetType().Name + ": Loaded " + size() + " effect handlers.");
	}

	public void registerHandler(string name, Func<EffectParameterSet, IAbstractEffect> handlerFactory)
	{
		_effectHandlerFactories.Add(name, handlerFactory);
	}

	public Func<EffectParameterSet, IAbstractEffect>? getHandlerFactory(string name)
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