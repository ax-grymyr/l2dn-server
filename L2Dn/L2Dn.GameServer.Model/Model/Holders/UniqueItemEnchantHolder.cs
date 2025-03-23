using L2Dn.GameServer.Model.Interfaces;

namespace L2Dn.GameServer.Model.Holders;

public sealed record UniqueItemEnchantHolder(int Id, int ObjectId, long Count = 1, int EnchantLevel = 0)
    : ItemEnchantHolder(Id, Count, EnchantLevel), IUniqueId
{
    public UniqueItemEnchantHolder(ItemEnchantHolder itemHolder, int objectId): this(itemHolder.Id,
        objectId, itemHolder.Count, itemHolder.EnchantLevel)
    {
    }

    public override string ToString() =>
        $"[{nameof(UniqueItemEnchantHolder)}] ID: {Id}, object ID: {ObjectId}, count: {Count}, enchant level: {EnchantLevel}";
}