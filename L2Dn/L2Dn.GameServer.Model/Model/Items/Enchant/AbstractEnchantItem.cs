using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.Model.DataPack;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Items.Enchant;

/**
 * @author UnAfraid
 */
public abstract class AbstractEnchantItem
{
	private readonly int _id;
	private readonly CrystalType _grade;
	private readonly int _minEnchantLevel;
	private readonly int _maxEnchantLevel;
	private readonly int _safeEnchantLevel;
	private readonly int _randomEnchantMin;
	private readonly int _randomEnchantMax;
	private readonly double _bonusRate;
	private readonly bool _isBlessed;

	protected AbstractEnchantItem(XmlEnchantScroll enchantScroll)
	{
		_id = enchantScroll.Id;
		_grade = enchantScroll.TargetGrade;
		_minEnchantLevel = enchantScroll.MinEnchant;
		_maxEnchantLevel = enchantScroll.MaxEnchant;
		_safeEnchantLevel = enchantScroll.SafeEnchant;
		_randomEnchantMin = enchantScroll.RandomEnchantMin;
		_randomEnchantMax = Math.Max(enchantScroll.RandomEnchantMax, enchantScroll.RandomEnchantMin);
		_bonusRate = enchantScroll.BonusRate;
		_isBlessed = enchantScroll.IsBlessed;
	}

	/**
	 * @return id of current item
	 */
	public int getId()
	{
		return _id;
	}

	/**
	 * @return bonus chance that would be added
	 */
	public double getBonusRate()
	{
		return _bonusRate;
	}

	/**
	 * @return grade of the item/scroll.
	 */
	public CrystalType getGrade()
	{
		return _grade;
	}

	/**
	 * @return {@code true} if scroll is for weapon, {@code false} for armor
	 */
	public abstract bool isWeapon();

	/**
	 * @return the minimum enchant level that this scroll/item can be used with
	 */
	public int getMinEnchantLevel()
	{
		return _minEnchantLevel;
	}

	/**
	 * @return the maximum enchant level that this scroll/item can be used with
	 */
	public int getMaxEnchantLevel()
	{
		return _maxEnchantLevel;
	}

	/**
	 * @return the safe enchant level of this scroll/item
	 */
	public int getSafeEnchant()
	{
		return _safeEnchantLevel;
	}

	/**
	 * @return the minimum random enchant level of this scroll/item
	 */
	public int getRandomEnchantMin()
	{
		return _randomEnchantMin;
	}

	/**
	 * @return the maximum random enchant level of this scroll/item
	 */
	public int getRandomEnchantMax()
	{
		return _randomEnchantMax;
	}

	public bool isActionBlessed()
	{
		return _isBlessed;
	}

	/**
	 * @param itemToEnchant the item to be enchanted
	 * @param supportItem
	 * @return {@code true} if this support item can be used with the item to be enchanted, {@code false} otherwise
	 */
	public virtual bool isValid(Item itemToEnchant, EnchantSupportItem? supportItem)
	{
		if (itemToEnchant == null)
		{
			return false;
		}

		if (!itemToEnchant.isEnchantable() || (!(itemToEnchant.getTemplate().getEnchantLimit() == 0) &&
			    itemToEnchant.getEnchantLevel() ==
			    itemToEnchant.getTemplate().getEnchantLimit()))
		{
			return false;
		}

		if (!isValidItemType(itemToEnchant.getTemplate().getType2()))
		{
			return false;
		}

		if ((_minEnchantLevel != 0 && itemToEnchant.getEnchantLevel() < _minEnchantLevel) ||
		    (_maxEnchantLevel != 0 && itemToEnchant.getEnchantLevel() >= _maxEnchantLevel))
		{
			return false;
		}

		if (_grade != itemToEnchant.getTemplate().getCrystalTypePlus())
		{
			return false;
		}

		return true;
	}

	/**
	 * @param type2
	 * @return {@code true} if current type2 is valid to be enchanted, {@code false} otherwise
	 */
	private bool isValidItemType(int type2)
	{
		if (type2 == ItemTemplate.TYPE2_WEAPON)
		{
			return isWeapon();
		}

		if (type2 == ItemTemplate.TYPE2_SHIELD_ARMOR || type2 == ItemTemplate.TYPE2_ACCESSORY)
		{
			return !isWeapon();
		}

		return false;
	}
}