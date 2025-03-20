using System.Collections.Frozen;
using System.Reflection;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Handlers;

public sealed class AbstractEffectFactory
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(AbstractEffectFactory));

    private FrozenDictionary<string, Func<EffectParameterSet, IAbstractEffect>> _factories =
        FrozenDictionary<string, Func<EffectParameterSet, IAbstractEffect>>.Empty;

    private AbstractEffectFactory()
    {
    }

    public static AbstractEffectFactory Instance { get; } = new();

    public void Register(Assembly assembly)
    {
        _factories = FactoryHelper.CreateFactories<EffectParameterSet, IAbstractEffect>(assembly);
        _logger.Info($"{nameof(AbstractEffectFactory)}: Registered {_factories.Count} effect handlers.");
    }

    public IAbstractEffect? Create(string name, EffectParameterSet parameters) =>
        _factories.TryGetValue(name, out Func<EffectParameterSet, IAbstractEffect>? factory)
            ? factory(parameters)
            : null;
}