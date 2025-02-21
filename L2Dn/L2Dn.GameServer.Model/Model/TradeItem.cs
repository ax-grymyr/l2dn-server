using System.Collections.Immutable;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Ensoul;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model;

public class TradeItem
{
	private int _objectId;
	private readonly ItemTemplate _item;
	private readonly int _location;
	private int _enchant;
	private readonly int _type1;
	private readonly int _type2;
	private long _count;
	private long _storeCount;
	private long _price;
	private AttributeType _elemAtkType;
	private int _elemAtkPower;
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
	private ICollection<EnsoulOption> _soulCrystalOptions;
	private ICollection<EnsoulOption> _soulCrystalSpecialOptions;
	private int _visualId;
	private int _augmentationOption1 = -1;
	private int _augmentationOption2 = -1;
	private bool _isBlessed;
	
	public TradeItem(Item item, long count, long price)
	{
		ArgumentNullException.ThrowIfNull(item);
		_objectId = item.ObjectId;
		_item = item.getTemplate();
		_location = item.getLocationSlot();
		_enchant = item.getEnchantLevel();
		_type1 = item.getCustomType1();
		_type2 = item.getCustomType2();
		_count = count;
		_price = price;
		_elemAtkType = item.getAttackAttributeType();
		_elemAtkPower = item.getAttackAttributePower();
		foreach (AttributeType type in AttributeTypeUtil.AttributeTypes)
		{
			_elemDefAttr[(int)type] = item.getDefenceAttribute(type);
		}
		_enchantOptions = item.getEnchantOptions();
		_soulCrystalOptions = item.getSpecialAbilities();
		_soulCrystalSpecialOptions = item.getAdditionalSpecialAbilities();
		_visualId = item.getVisualId();
		_isBlessed = item.isBlessed();
		if (item.getAugmentation() != null)
		{
			_augmentationOption1 = item.getAugmentation().getOption1Id();
			_augmentationOption1 = item.getAugmentation().getOption2Id();
		}
	}
	
	public TradeItem(ItemTemplate item, long count, long price)
	{
		ArgumentNullException.ThrowIfNull(item);
		_objectId = 0;
		_item = item;
		_location = 0;
		_enchant = 0;
		_type1 = 0;
		_type2 = 0;
		_count = count;
		_storeCount = count;
		_price = price;
		_elemAtkType = AttributeType.NONE;
		_elemAtkPower = 0;
		_enchantOptions = [];
		_soulCrystalOptions = new List<EnsoulOption>();
		_soulCrystalSpecialOptions = new List<EnsoulOption>();
	}
	
	public TradeItem(TradeItem item, long count, long price)
	{
		ArgumentNullException.ThrowIfNull(item);
		_objectId = item.getObjectId();
		_item = item.getItem();
		_location = item.getLocationSlot();
		_enchant = item.getEnchant();
		_type1 = item.getCustomType1();
		_type2 = item.getCustomType2();
		_count = count;
		_storeCount = count;
		_price = price;
		_elemAtkType = item.getAttackElementType();
		_elemAtkPower = item.getAttackElementPower();
		for (byte i = 0; i < 6; i++)
		{
			_elemDefAttr[i] = item.getElementDefAttr(i);
		}
		_enchantOptions = item.getEnchantOptions();
		_soulCrystalOptions = item.getSoulCrystalOptions();
		_soulCrystalSpecialOptions = item.getSoulCrystalSpecialOptions();
		_visualId = item.getVisualId();
		_isBlessed = item.isBlessed();
	}
	
	public void setObjectId(int objectId)
	{
		_objectId = objectId;
	}
	
	public int getObjectId()
	{
		return _objectId;
	}
	
	public ItemTemplate getItem()
	{
		return _item;
	}
	
	public int getLocationSlot()
	{
		return _location;
	}
	
	public void setEnchant(int enchant)
	{
		_enchant = enchant;
	}
	
	public int getEnchant()
	{
		return _enchant;
	}
	
	public int getCustomType1()
	{
		return _type1;
	}
	
	public int getCustomType2()
	{
		return _type2;
	}
	
	public void setCount(long count)
	{
		_count = count;
	}
	
	public long getCount()
	{
		return _count;
	}
	
	public long getStoreCount()
	{
		return _storeCount;
	}
	
	public void setPrice(long price)
	{
		_price = price;
	}
	
	public long getPrice()
	{
		return _price;
	}
	
	public void setAttackElementType(AttributeType attackElement)
	{
		_elemAtkType = attackElement;
	}
	
	public AttributeType getAttackElementType()
	{
		return _elemAtkType;
	}
	
	public void setAttackElementPower(int attackElementPower)
	{
		_elemAtkPower = attackElementPower;
	}
	
	public int getAttackElementPower()
	{
		return _elemAtkPower;
	}
	
	public void setElementDefAttr(AttributeType element, int value)
	{
		_elemDefAttr[(int)element] = value;
	}
	
	public int getElementDefAttr(byte i)
	{
		return _elemDefAttr[i];
	}
	
	public ImmutableArray<int> getEnchantOptions()
	{
		return _enchantOptions;
	}
	
	public void setSoulCrystalOptions(ICollection<EnsoulOption> soulCrystalOptions)
	{
		_soulCrystalOptions = soulCrystalOptions;
	}
	
	public ICollection<EnsoulOption> getSoulCrystalOptions()
	{
		return _soulCrystalOptions == null ? new List<EnsoulOption>() : _soulCrystalOptions;
	}
	
	public void setSoulCrystalSpecialOptions(ICollection<EnsoulOption> soulCrystalSpecialOptions)
	{
		_soulCrystalSpecialOptions = soulCrystalSpecialOptions;
	}
	
	public ICollection<EnsoulOption> getSoulCrystalSpecialOptions()
	{
		return _soulCrystalSpecialOptions == null ? new List<EnsoulOption>() : _soulCrystalSpecialOptions;
	}
	
	public void setAugmentation(int option1, int option2)
	{
		_augmentationOption1 = option1;
		_augmentationOption2 = option2;
	}
	
	public int getAugmentationOption1()
	{
		return _augmentationOption1;
	}
	
	public int getAugmentationOption2()
	{
		return _augmentationOption2;
	}
	
	public void setVisualId(int visualItemId)
	{
		_visualId = visualItemId;
	}
	
	public int getVisualId()
	{
		return _visualId;
	}
	
	public bool isBlessed()
	{
		return _isBlessed;
	}
}
