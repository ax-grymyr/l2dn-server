using L2Dn.GameServer.Model.Events.Containers;

namespace L2Dn.GameServer.Model.Events;

public static class GlobalEvents
{
    /// <summary>
    /// Global events.
    /// </summary>
    public static GlobalEventContainer Global { get; } = new();

    /// <summary>
    /// Global npc events.
    /// </summary>
    public static NpcEventContainer Npcs { get; } = new();
	
    /// <summary>
    /// Global monster events.
    /// </summary>
    public static MonsterEventContainer Monsters { get; } = new();

    /// <summary>
    /// Global player events.
    /// </summary>
    public static PlayerEventContainer Players { get; } = new();
}