using System.Collections.Frozen;
using System.Reflection;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Handlers;

public sealed class SkillConditionFactory
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(SkillConditionFactory));

    private FrozenDictionary<string, Func<SkillConditionParameterSet, ISkillConditionBase>> _factories =
        FrozenDictionary<string, Func<SkillConditionParameterSet, ISkillConditionBase>>.Empty;

    private SkillConditionFactory()
    {
    }

    public static SkillConditionFactory Instance { get; } = new();

    public void Register(Assembly assembly)
    {
        _factories = FactoryHelper.CreateFactories<string, SkillConditionParameterSet, ISkillConditionBase>(assembly);
        _logger.Info($"{nameof(SkillConditionFactory)}: Registered {_factories.Count} skill conditions.");
    }

    public ISkillConditionBase? Create(string name, SkillConditionParameterSet parameters) =>
        _factories.TryGetValue(name, out Func<SkillConditionParameterSet, ISkillConditionBase>? factory)
            ? factory(parameters)
            : null;
}