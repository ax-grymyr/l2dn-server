using System.Collections.Immutable;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Dto;

/// <summary>
///
/// </summary>
/// <param name="Name">The list identification (name).</param>
/// <param name="Type">The type of the teleport list.</param>
/// <param name="Locations">The teleport locations registered in current holder.</param>
public sealed record TeleportHolder(string Name, TeleportType Type, ImmutableArray<TeleportLocation> Locations)
{
    /// <summary>
    /// Check if teleport list is for noblesse or not.
    /// </summary>
    public bool IsNoblesse => Type is TeleportType.NOBLES_ADENA or TeleportType.NOBLES_TOKEN;

    public bool IsNormalTeleport => Type is TeleportType.NORMAL or TeleportType.HUNTING;
}