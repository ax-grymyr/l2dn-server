using System.Collections.Immutable;

namespace L2Dn.GameServer.Model.Events.Annotations;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class IdListAttribute(params int[] ids): Attribute
{
    public ImmutableArray<int> Ids { get; } = ids.ToImmutableArray();
}