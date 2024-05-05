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

	public override bool Equals(object? obj)
	{
		if (obj is ItemEnchantHolder other)
		{
			return getId() == other.getId() && getCount() == other.getCount() &&
				enchantLevel == other.getEnchantLevel();
		}

		return false;
	}

	public override string ToString()
		=> $"[{GetType().Name}] ID: {getId()}, count: {getCount()}, enchant level: {enchantLevel}";
}