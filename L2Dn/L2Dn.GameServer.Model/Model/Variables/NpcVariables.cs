using System.Collections.Concurrent;

namespace L2Dn.GameServer.Model.Variables;

/**
 * NPC Variables implementation.
 * @author GKR
 */
public class NpcVariables(int npcId)
{
    private readonly ConcurrentDictionary<string, object> _values = new(StringComparer.OrdinalIgnoreCase);

    public int NpcId => npcId;

    public void Clear() => _values.Clear();
}