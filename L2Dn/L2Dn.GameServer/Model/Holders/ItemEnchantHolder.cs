namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Index, Mobius
 */
public class ItemEnchantHolder: ItemHolder
{
	private readonly int _enchantLevel;

	public ItemEnchantHolder(StatSet set): base(set)
	{
		_enchantLevel = 0;
	}

	public ItemEnchantHolder(int id, long count): base(id, count)
	{
		_enchantLevel = 0;
	}

	public ItemEnchantHolder(int id, long count, int enchantLevel): base(id, count)
	{
		_enchantLevel = enchantLevel;
	}

	/**
	 * @return enchant level of items contained in this object
	 */
	public int getEnchantLevel()
	{
		return _enchantLevel;
	}

	public override bool Equals(Object? obj)
	{
		if (!(obj is ItemEnchantHolder objInstance))
		{
			return false;
		}
		else if (obj == this)
		{
			return true;
		}

		return (getId() == objInstance.getId()) &&
		       ((getCount() == objInstance.getCount()) && (_enchantLevel == objInstance.getEnchantLevel()));
	}

	public override String ToString()
	{
		return "[" + GetType().Name + "] ID: " + getId() + ", count: " + getCount() + ", enchant level: " +
		       _enchantLevel;
	}
}