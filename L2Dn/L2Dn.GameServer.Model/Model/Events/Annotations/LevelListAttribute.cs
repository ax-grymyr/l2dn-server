using System.Collections.Immutable;

namespace L2Dn.GameServer.Model.Events.Annotations;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class LevelListAttribute(params int[] levels): Attribute
{
    public ImmutableArray<int> Levels { get; } = levels.ToImmutableArray();
}