using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model.Interfaces;

namespace L2Dn.GameServer.Model.Holders;

/// <summary>
/// A DTO for items; contains item ID, object ID and count.
/// </summary>
public record UniqueItemHolder(int Id, int ObjectId, long Count = 1): ItemHolder(Id, Count), IUniqueId
{
    public override string ToString() =>
        $"[{nameof(UniqueItemHolder)}] ID: {Id}, object ID: {ObjectId}, count: {Count}";
}