namespace L2Dn.GameServer.Dto;

/// <summary>
/// A simple DTO for items; contains item ID and count.
/// </summary>
public record ItemHolder(int Id, long Count)
{
    public override string ToString() => $"[{GetType().Name}] ID: {Id}, count: {Count}";
}