using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Items.Enchant;

/**
 * @author UnAfraid
 */
public abstract class AbstractEnchantItem
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(AbstractEnchantItem));

	private static readonly Set<EtcItemType> ENCHANT_TYPES = new();

	static AbstractEnchantItem()
	{
		ENCHANT_TYPES.add(EtcItemType.ENCHT_ATTR_ANCIENT_CRYSTAL_ENCHANT_AM);
		ENCHANT_TYPES.add(EtcItemType.ENCHT_ATTR_ANCIENT_CRYSTAL_ENCHANT_WP);
		ENCHANT_TYPES.add(EtcItemType.BLESS_ENCHT_AM);
		ENCHANT_TYPES.add(EtcItemType.BLESS_ENCHT_AM_DOWN);
		ENCHANT_TYPES.add(EtcItemType.BLESS_ENCHT_WP);
		ENCHANT_TYPES.add(EtcItemType.ENCHT_AM);
		ENCHANT_TYPES.add(EtcItemType.ENCHT_WP);
		ENCHANT_TYPES.add(EtcItemType.GIANT_ENCHT_AM);
		ENCHANT_TYPES.add(EtcItemType.GIANT_ENCHT_WP);
		ENCHANT_TYPES.add(EtcItemType.ENCHT_ATTR_INC_PROP_ENCHT_AM);
		ENCHANT_TYPES.add(EtcItemType.ENCHT_ATTR_INC_PROP_ENCHT_WP);
		ENCHANT_TYPES.add(EtcItemType.GIANT_ENCHT_ATTR_INC_PROP_ENCHT_AM);
		ENCHANT_TYPES.add(EtcItemType.GIANT_ENCHT_ATTR_INC_PROP_ENCHT_WP);
		ENCHANT_TYPES.add(EtcItemType.BLESSED_ENCHT_ATTR_INC_PROP_ENCHT_AM);
		ENCHANT_TYPES.add(EtcItemType.BLESSED_ENCHT_ATTR_INC_PROP_ENCHT_WP);
		ENCHANT_TYPES.add(EtcItemType.BLESSED_GIANT_ENCHT_ATTR_INC_PROP_ENCHT_AM);
		ENCHANT_TYPES.add(EtcItemType.BLESSED_GIANT_ENCHT_ATTR_INC_PROP_ENCHT_WP);
		ENCHANT_TYPES.add(EtcItemType.CURSED_ENCHT_AM);
		ENCHANT_TYPES.add(EtcItemType.CURSED_ENCHT_WP);
	}

	private readonly int _id;
	private readonly CrystalType _grade;
	private readonly int _minEnchantLevel;
	private readonly int _maxEnchantLevel;
	private readonly int _safeEnchantLevel;
	private readonly int _randomEnchantMin;
	private readonly int _randomEnchantMax;
	private readonly double _bonusRate;
	private readonly bool _isBlessed;

	public AbstractEnchantItem(StatSet set)
	{
		_id = set.getInt("id");
		if (getItem() == null)
		{
			throw new InvalidOperationException();
		}
		
		if (!ENCHANT_TYPES.Contains(getItem().getItemType()))
		{
			throw new InvalidOperationException();
		}

		_grade = set.getEnum("targetGrade", CrystalType.NONE);
		_minEnchantLevel = set.getInt("minEnchant", 0);
		_maxEnchantLevel = set.getInt("maxEnchant", 127);
		_safeEnchantLevel = set.getInt("safeEnchant", 0);
		_randomEnchantMin = set.getInt("randomEnchantMin", 1);
		_randomEnchantMax = set.getInt("randomEnchantMax", _randomEnchantMin);
		_bonusRate = set.getDouble("bonusRate", 0);
		_isBlessed = set.getBoolean("isBlessed", false);
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
	 * @return {@link ItemTemplate} current item/scroll
	 */
	public ItemTemplate getItem()
	{
		return ItemData.getInstance().getTemplate(_id);
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
	public virtual bool isValid(Item itemToEnchant, EnchantSupportItem supportItem)
	{
		if (itemToEnchant == null)
		{
			return false;
		}
		else if (!itemToEnchant.isEnchantable() || (!(itemToEnchant.getTemplate().getEnchantLimit() == 0) &&
		                                            (itemToEnchant.getEnchantLevel() ==
		                                             itemToEnchant.getTemplate().getEnchantLimit())))
		{
			return false;
		}
		else if (!isValidItemType(itemToEnchant.getTemplate().getType2()))
		{
			return false;
		}
		else if (((_minEnchantLevel != 0) && (itemToEnchant.getEnchantLevel() < _minEnchantLevel)) ||
		         ((_maxEnchantLevel != 0) && (itemToEnchant.getEnchantLevel() >= _maxEnchantLevel)))
		{
			return false;
		}
		else if (_grade != itemToEnchant.getTemplate().getCrystalTypePlus())
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
		else if ((type2 == ItemTemplate.TYPE2_SHIELD_ARMOR) || (type2 == ItemTemplate.TYPE2_ACCESSORY))
		{
			return !isWeapon();
		}

		return false;
	}
}