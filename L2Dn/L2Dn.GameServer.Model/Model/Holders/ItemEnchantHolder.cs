using L2Dn.GameServer.Dto;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Index, Mobius
 */
public class ItemEnchantHolder(int id, long count, int enchantLevel = 0): ItemHolder(id, count)
{
    /**
     * @return enchant level of items contained in this object
     */
    public int getEnchantLevel() => enchantLevel;

    public override bool Equals(object? obj) =>
        obj == this || obj is ItemEnchantHolder other && getId() == other.getId() && getCount() == other.getCount() &&
        enchantLevel == other.getEnchantLevel();

    public override int GetHashCode() => HashCode.Combine(getId(), getCount(), getEnchantLevel());

    public override string ToString() =>
        $"[{GetType().Name}] ID: {getId()}, count: {getCount()}, enchant level: {enchantLevel}";
}