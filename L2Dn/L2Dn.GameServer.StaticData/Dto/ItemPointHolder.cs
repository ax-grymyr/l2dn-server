namespace L2Dn.GameServer.Dto;

public sealed record ItemPointHolder(int Id, long Count, int Points): ItemHolder(Id, Count)
{
    public override string ToString() => $"[{nameof(ItemPointHolder)}] ID: {Id}, count: {Count}, points: {Points}";
}