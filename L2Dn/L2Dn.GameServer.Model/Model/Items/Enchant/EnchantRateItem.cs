using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Items.Enchant;

/**
 * @author UnAfraid, Mobius
 */
public class EnchantRateItem
{
	private readonly string _name;
	private readonly Set<int> _items = new();
	private long _slot;
	private bool? _isMagicWeapon;

	public EnchantRateItem(string name)
	{
		_name = name;
	}

	/**
	 * @return name of enchant group.
	 */
	public string getName()
	{
		return _name;
	}

	/**
	 * Adds item id verification.
	 * @param id
	 */
	public void addItemId(int id)
	{
		_items.add(id);
	}

	/**
	 * Adds body slot verification.
	 * @param slot
	 */
	public void addSlot(long slot)
	{
		_slot |= slot;
	}

	/**
	 * Adds magic weapon verification.
	 * @param magicWeapon
	 */
	public void setMagicWeapon(bool magicWeapon)
	{
		_isMagicWeapon = magicWeapon;
	}

	/**
	 * @param item
	 * @return {@code true} if item can be used with this rate group, {@code false} otherwise.
	 */
	public bool validate(ItemTemplate item)
	{
		if (!_items.isEmpty() && !_items.Contains(item.getId()))
		{
			return false;
		}
		else if (_slot != 0 && (item.getBodyPart() & _slot) == 0)
		{
			return false;
		}
		else if (_isMagicWeapon != null && item.isMagicWeapon() != _isMagicWeapon.Value)
		{
			return false;
		}

		return true;
	}
}