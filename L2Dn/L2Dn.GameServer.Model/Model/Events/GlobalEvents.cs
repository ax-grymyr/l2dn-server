using L2Dn.Events;

namespace L2Dn.GameServer.Model.Events;

public static class GlobalEvents
{
    /// <summary>
    /// Global events.
    /// </summary>
    public static EventContainer Global { get; } = new("Global");

    /// <summary>
    /// Global npc events.
    /// </summary>
    public static EventContainer Npcs { get; } = new("Global Npcs", Global);
	
    /// <summary>
    /// Global monster events.
    /// </summary>
    public static EventContainer Monsters { get; } = new("Global Monsters", Global);

    /// <summary>
    /// Global player events.
    /// </summary>
    public static EventContainer Players { get; } = new("Global players", Global);
}