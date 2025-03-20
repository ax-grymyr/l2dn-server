namespace L2Dn.GameServer.Handlers;

public sealed class SkillConditionFactory
{
    private readonly Dictionary<string, Func<SkillConditionParameterSet, ISkillConditionBase>>
        _skillConditionHandlerFactories = new();

    private SkillConditionFactory()
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
        public static readonly SkillConditionFactory INSTANCE = new();
    }

    public static SkillConditionFactory getInstance()
    {
        return SingletonHolder.INSTANCE;
    }
}