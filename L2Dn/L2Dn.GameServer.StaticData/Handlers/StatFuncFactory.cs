using System.Collections.Frozen;
using System.Reflection;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Stats.Functions;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Handlers;

public sealed class StatFuncFactory
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(StatFuncFactory));

    private FrozenDictionary<StatFuncType, Func<StatFuncParameters, IStatFuncBase>> _factories =
        FrozenDictionary<StatFuncType, Func<StatFuncParameters, IStatFuncBase>>.Empty;

    private StatFuncFactory()
    {
    }

    public static StatFuncFactory Instance { get; } = new();

    public void Register(Assembly assembly)
    {
        _factories = FactoryHelper.CreateFactories<StatFuncType, StatFuncParameters, IStatFuncBase>(assembly);
        _logger.Info($"{nameof(StatFuncFactory)}: Registered {_factories.Count} stat functions.");
    }

    public IStatFuncBase? Create(StatFuncParameters parameters) =>
        _factories.TryGetValue(parameters.FuncType, out Func<StatFuncParameters, IStatFuncBase>? factory)
            ? factory(parameters)
            : null;
}