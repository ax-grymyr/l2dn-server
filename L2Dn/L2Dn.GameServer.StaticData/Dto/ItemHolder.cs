namespace L2Dn.GameServer.Dto;

/// <summary>
/// A simple DTO for items; contains item ID and count.
/// </summary>
public class ItemHolder(int id, long count): IIdentifiable
{
    /**
     * @return the ID of the item contained in this object
     */
    public int Id => id;

    /**
     * @return the count of items contained in this object
     */
    public long getCount() => count;

    public override bool Equals(object? obj) =>
        ReferenceEquals(obj, this) || obj is ItemHolder other && id == other.Id && count == other.getCount();

    public override int GetHashCode() => HashCode.Combine(id, count);

    public override string ToString() => $"[{GetType().Name}] ID: {id}, count: {count}";
}