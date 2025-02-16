using System.Collections.Immutable;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Ensoul;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Items;

/**
 * This class contains Item<br>
 * Use to sort Item of :
 * <ul>
 * <li>Armor</li>
 * <li>EtcItem</li>
 * <li>Weapon</li>
 * </ul>
 * @version $Revision: 1.7.2.2.2.5 $ $Date: 2005/04/06 18:25:18 $
 */
public class WarehouseItem
{
	private readonly ItemTemplate _item;
	private readonly int _object;
	private readonly long _count;
	private readonly int _owner;
	private readonly int _locationSlot;
	private readonly int _enchant;
	private readonly CrystalType _grade;
	private readonly VariationInstance _augmentation;
	private readonly int _customType1;
	private readonly int _customType2;
	private readonly int? _mana;
	
	private AttributeType _elemAtkType = AttributeType.NONE;
	private int _elemAtkPower = 0;
	
	private readonly int[] _elemDefAttr =
	{
		0,
		0,
		0,
		0,
		0,
		0
	};
	
	private readonly ImmutableArray<int> _enchantOptions;
	private readonly ICollection<EnsoulOption> _soulCrystalOptions;
	private readonly ICollection<EnsoulOption> _soulCrystalSpecialOptions;
	
	private readonly TimeSpan? _time;
	private readonly bool _isBlessed;
	
	public WarehouseItem(Item item)
	{
		ArgumentNullException.ThrowIfNull(item);
		_item = item.getTemplate();
		_object = item.ObjectId;
		_count = item.getCount();
		_owner = item.getOwnerId();
		_locationSlot = item.getLocationSlot();
		_enchant = item.getEnchantLevel();
		_customType1 = item.getCustomType1();
		_customType2 = item.getCustomType2();
		_grade = item.getTemplate().getCrystalType();
		_augmentation = item.getAugmentation();
		_mana = item.getMana();
		_time = item.isTimeLimitedItem() ? item.getRemainingTime() : null;
		_elemAtkType = item.getAttackAttributeType();
		_elemAtkPower = item.getAttackAttributePower();
		foreach (AttributeType type in EnumUtil.GetValues<AttributeType>())
		{
			if (type != AttributeType.NONE)
				_elemDefAttr[(int)type] = item.getDefenceAttribute(type);
		}
		_enchantOptions = item.getEnchantOptions();
		_soulCrystalOptions = item.getSpecialAbilities();
		_soulCrystalSpecialOptions = item.getAdditionalSpecialAbilities();
		_isBlessed = item.isBlessed();
	}
	
	/**
	 * @return the item.
	 */
	public ItemTemplate getItem()
	{
		return _item;
	}
	
	/**
	 * @return the unique objectId.
	 */
	public int getObjectId()
	{
		return _object;
	}
	
	/**
	 * @return the owner.
	 */
	public int getOwnerId()
	{
		return _owner;
	}
	
	/**
	 * @return the location slot.
	 */
	public int getLocationSlot()
	{
		return _locationSlot;
	}
	
	/**
	 * @return the count.
	 */
	public long getCount()
	{
		return _count;
	}
	
	/**
	 * @return the first type.
	 */
	public int getType1()
	{
		return _item.getType1();
	}
	
	/**
	 * @return the second type.
	 */
	public int getType2()
	{
		return _item.getType2();
	}
	
	/**
	 * @return the second type.
	 */
	public ItemType getItemType()
	{
		return _item.getItemType();
	}
	
	/**
	 * @return the ItemId.
	 */
	public int getItemId()
	{
		return _item.getId();
	}
	
	/**
	 * @return the part of body used with this item.
	 */
	public long getBodyPart()
	{
		return _item.getBodyPart();
	}
	
	/**
	 * @return the enchant level.
	 */
	public int getEnchantLevel()
	{
		return _enchant;
	}
	
	/**
	 * @return the item grade
	 */
	public CrystalType getItemGrade()
	{
		return _grade;
	}
	
	/**
	 * @return {@code true} if the item is a weapon, {@code false} otherwise.
	 */
	public bool isWeapon()
	{
		return (_item is Weapon);
	}
	
	/**
	 * @return {@code true} if the item is an armor, {@code false} otherwise.
	 */
	public bool isArmor()
	{
		return (_item is Armor);
	}
	
	/**
	 * @return {@code true} if the item is an etc item, {@code false} otherwise.
	 */
	public bool isEtcItem()
	{
		return (_item is EtcItem);
	}
	
	/**
	 * @return the name of the item
	 */
	public string getItemName()
	{
		return _item.getName();
	}
	
	/**
	 * @return the augmentation If.
	 */
	public VariationInstance getAugmentation()
	{
		return _augmentation;
	}
	
	/**
	 * @return the name of the item
	 */
	public string getName()
	{
		return _item.getName();
	}
	
	public int getCustomType1()
	{
		return _customType1;
	}
	
	public int getCustomType2()
	{
		return _customType2;
	}
	
	public int? getMana()
	{
		return _mana;
	}
	
	public AttributeType getAttackElementType()
	{
		return _elemAtkType;
	}
	
	public int getAttackElementPower()
	{
		return _elemAtkPower;
	}
	
	public int getElementDefAttr(byte i)
	{
		return _elemDefAttr[i];
	}
	
	public ImmutableArray<int> getEnchantOptions()
	{
		return _enchantOptions;
	}
	
	public ICollection<EnsoulOption> getSoulCrystalOptions()
	{
		return _soulCrystalOptions;
	}
	
	public ICollection<EnsoulOption> getSoulCrystalSpecialOptions()
	{
		return _soulCrystalSpecialOptions;
	}
	
	public TimeSpan? getTime()
	{
		return _time;
	}
	
	public bool isBlessed()
	{
		return _isBlessed;
	}
	
	/**
	 * @return the name of the item
	 */
	public override string ToString()
	{
		return _item.ToString();
	}
}