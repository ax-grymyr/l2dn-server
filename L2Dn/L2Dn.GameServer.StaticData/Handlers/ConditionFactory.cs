using System.Collections.Frozen;
using System.Reflection;
using L2Dn.GameServer.StaticData.Xml.Items;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

public sealed class ConditionFactory
{
    private IConditionFactory<XmlItemConditionList>? _factory;

    private ConditionFactory()
    {
    }

    public static ConditionFactory Instance { get; } = new();

    public void Register(IConditionFactory<XmlItemConditionList> factory)
    {
        _factory = factory;
    }

    public void Register(Assembly assembly)
    {
        FrozenDictionary<string, Func<IConditionFactory<XmlItemConditionList>>> factories =
            FactoryHelper.CreateFactories<string, IConditionFactory<XmlItemConditionList>>(assembly);

        Func<IConditionFactory<XmlItemConditionList>>? factory = factories.Values.FirstOrDefault();
        if (factory is not null)
            _factory = factory();
    }

    public IConditionBase Create(XmlItemConditionList xmlItemConditionList) =>
        (_factory ?? throw new InvalidOperationException("Factory is not registered")).Create(xmlItemConditionList);
}