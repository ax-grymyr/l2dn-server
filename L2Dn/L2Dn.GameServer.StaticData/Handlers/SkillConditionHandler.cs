namespace L2Dn.GameServer.Handlers;

/**
 * @author NosBit
 */
public sealed class SkillConditionHandler
{
    private readonly Dictionary<string, Func<SkillConditionParameterSet, ISkillConditionBase>>
        _skillConditionHandlerFactories = new();

	private SkillConditionHandler()
	{
	}

	public void registerHandler(string name, Func<SkillConditionParameterSet, ISkillConditionBase> handlerFactory)
	{
		_skillConditionHandlerFactories.Add(name, handlerFactory);
	}

	public Func<SkillConditionParameterSet, ISkillConditionBase>? getHandlerFactory(string name)
	{
		return _skillConditionHandlerFactories.GetValueOrDefault(name);
	}

	public int size()
	{
		return _skillConditionHandlerFactories.Count;
	}

	private static class SingletonHolder
	{
		public static readonly SkillConditionHandler INSTANCE = new();
	}

	public static SkillConditionHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
}