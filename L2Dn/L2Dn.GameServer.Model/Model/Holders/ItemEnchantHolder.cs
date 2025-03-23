using L2Dn.GameServer.Dto;

namespace L2Dn.GameServer.Model.Holders;

public record ItemEnchantHolder(int Id, long Count, int EnchantLevel = 0): ItemHolder(Id, Count)
{
    public override string ToString() =>
        $"[{GetType().Name}] ID: {Id}, count: {Count}, enchant level: {EnchantLevel}";
}