using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.Model.Enums;
using L2Dn.Model.Xml;
using L2Dn.Utilities;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Mobius
 */
public sealed class ElementalAttributeData: DataReaderBase
{
	public const int FirstWeaponBonus = 20;
	public const int NextWeaponBonus = 5;
	public const int ArmorBonus = 6;

	private static readonly Logger _logger = LogManager.GetLogger(nameof(ElementalAttributeData));

	private static FrozenDictionary<int, ElementalItemHolder> _elementalItems =
		FrozenDictionary<int, ElementalItemHolder>.Empty;

	public static readonly ImmutableArray<int> WeaponValues =
	[
		0, // Level 1
		25, // Level 2
		75, // Level 3
		150, // Level 4
		175, // Level 5
		225, // Level 6
		300, // Level 7
		325, // Level 8
		375, // Level 9
		450, // Level 10
		475, // Level 11
		525, // Level 12
		600, // Level 13
		int.MaxValue,
		// TODO: Higher stones
	];

	public static readonly ImmutableArray<int> ArmorValues =
	[
		0, // Level 1
		12, // Level 2
		30, // Level 3
		60, // Level 4
		72, // Level 5
		90, // Level 6
		120, // Level 7
		132, // Level 8
		150, // Level 9
		180, // Level 10
		192, // Level 11
		210, // Level 12
		240, // Level 13
		int.MaxValue
		// TODO: Higher stones
	];

	private static readonly ImmutableArray<ImmutableArray<int>> _chanceTable =
	[
		[
			Config.AttributeSystem.S_WEAPON_STONE, Config.AttributeSystem.S_ARMOR_STONE, Config.AttributeSystem.S_WEAPON_CRYSTAL, Config.AttributeSystem.S_ARMOR_CRYSTAL,
			Config.AttributeSystem.S_WEAPON_STONE_SUPER, Config.AttributeSystem.S_ARMOR_STONE_SUPER, Config.AttributeSystem.S_WEAPON_CRYSTAL_SUPER,
			Config.AttributeSystem.S_ARMOR_CRYSTAL_SUPER, Config.AttributeSystem.S_WEAPON_JEWEL, Config.AttributeSystem.S_ARMOR_JEWEL
		],
		[
			Config.AttributeSystem.S80_WEAPON_STONE, Config.AttributeSystem.S80_ARMOR_STONE, Config.AttributeSystem.S80_WEAPON_CRYSTAL, Config.AttributeSystem.S80_ARMOR_CRYSTAL,
			Config.AttributeSystem.S80_WEAPON_STONE_SUPER, Config.AttributeSystem.S80_ARMOR_STONE_SUPER, Config.AttributeSystem.S80_WEAPON_CRYSTAL_SUPER,
			Config.AttributeSystem.S80_ARMOR_CRYSTAL_SUPER, Config.AttributeSystem.S80_WEAPON_JEWEL, Config.AttributeSystem.S80_ARMOR_JEWEL
		],
		[
			Config.AttributeSystem.S84_WEAPON_STONE, Config.AttributeSystem.S84_ARMOR_STONE, Config.AttributeSystem.S84_WEAPON_CRYSTAL, Config.AttributeSystem.S84_ARMOR_CRYSTAL,
			Config.AttributeSystem.S84_WEAPON_STONE_SUPER, Config.AttributeSystem.S84_ARMOR_STONE_SUPER, Config.AttributeSystem.S84_WEAPON_CRYSTAL_SUPER,
			Config.AttributeSystem.S84_ARMOR_CRYSTAL_SUPER, Config.AttributeSystem.S84_WEAPON_JEWEL, Config.AttributeSystem.S84_ARMOR_JEWEL
		],
		[
			Config.AttributeSystem.R_WEAPON_STONE, Config.AttributeSystem.R_ARMOR_STONE, Config.AttributeSystem.R_WEAPON_CRYSTAL, Config.AttributeSystem.R_ARMOR_CRYSTAL,
			Config.AttributeSystem.R_WEAPON_STONE_SUPER, Config.AttributeSystem.R_ARMOR_STONE_SUPER, Config.AttributeSystem.R_WEAPON_CRYSTAL_SUPER,
			Config.AttributeSystem.R_ARMOR_CRYSTAL_SUPER, Config.AttributeSystem.R_WEAPON_JEWEL, Config.AttributeSystem.R_ARMOR_JEWEL
		],
		[
			Config.AttributeSystem.R95_WEAPON_STONE, Config.AttributeSystem.R95_ARMOR_STONE, Config.AttributeSystem.R95_WEAPON_CRYSTAL, Config.AttributeSystem.R95_ARMOR_CRYSTAL,
			Config.AttributeSystem.R95_WEAPON_STONE_SUPER, Config.AttributeSystem.R95_ARMOR_STONE_SUPER, Config.AttributeSystem.R95_WEAPON_CRYSTAL_SUPER,
			Config.AttributeSystem.R95_ARMOR_CRYSTAL_SUPER, Config.AttributeSystem.R95_WEAPON_JEWEL, Config.AttributeSystem.R95_ARMOR_JEWEL
		],
		[
			Config.AttributeSystem.R99_WEAPON_STONE, Config.AttributeSystem.R99_ARMOR_STONE, Config.AttributeSystem.R99_WEAPON_CRYSTAL, Config.AttributeSystem.R99_ARMOR_CRYSTAL,
			Config.AttributeSystem.R99_WEAPON_STONE_SUPER, Config.AttributeSystem.R99_ARMOR_STONE_SUPER, Config.AttributeSystem.R99_WEAPON_CRYSTAL_SUPER,
			Config.AttributeSystem.R99_ARMOR_CRYSTAL_SUPER, Config.AttributeSystem.R99_WEAPON_JEWEL, Config.AttributeSystem.R99_ARMOR_JEWEL
		],
	];

	private ElementalAttributeData()
	{
		load();
	}

	public void load()
	{
		Dictionary<int, ElementalItemHolder> elementalItems = [];

		XmlElementalAttributeData document =
			LoadXmlDocument<XmlElementalAttributeData>(DataFileLocation.Data, "ElementalAttributeData.xml");

		foreach (XmlElementalAttributeItem xmlElementalAttributeItem in document.Items)
		{
			int id = xmlElementalAttributeItem.Id;
			if (ItemData.getInstance().getTemplate(id) == null)
			{
				_logger.Error(GetType().Name + ": Could not find item with id " + id + ".");
				return;
			}

			ElementalItemHolder holder = new(id, xmlElementalAttributeItem.Elemental, xmlElementalAttributeItem.Type,
				xmlElementalAttributeItem.Power);

			if (!elementalItems.TryAdd(id, holder))
				_logger.Error(GetType().Name + ": Duplicated data " + id + ".");
		}

		_elementalItems = elementalItems.ToFrozenDictionary();

		_logger.Info(GetType().Name + ": Loaded " + _elementalItems.Count + " elemental attribute items.");
	}

	public AttributeType getItemElement(int itemId)
	{
		return _elementalItems.GetValueOrDefault(itemId)?.getElement() ?? AttributeType.NONE;
	}

	public ElementalItemHolder? getItemElemental(int itemId)
	{
		return _elementalItems.GetValueOrDefault(itemId);
	}

	public int getMaxElementLevel(int itemId)
	{
		return _elementalItems.GetValueOrDefault(itemId)?.getType().GetMaxLevel() ?? -1;
	}

	public bool isSuccess(Item item, int stoneId)
	{
		int row = item.getTemplate().getCrystalType() switch
		{
			CrystalType.S => 0,
			CrystalType.S80 => 1,
			CrystalType.S84 => 2,
			CrystalType.R => 3,
			CrystalType.R95 => 4,
			CrystalType.R99 => 5,
			_ => -1,
		};

		int column = _elementalItems.GetValueOrDefault(stoneId)?.getType() switch
		{
			ElementalItemType.STONE => item.isWeapon() ? 0 : 1,
			ElementalItemType.CRYSTAL => item.isWeapon() ? 2 : 3,
			ElementalItemType.STONE_SUPER => item.isWeapon() ? 4 : 5,
			ElementalItemType.CRYSTAL_SUPER => item.isWeapon() ? 6 : 7,
			ElementalItemType.JEWEL => item.isWeapon() ? 8 : 9,
			_ => -1,
		};

		if (row >= 0 && column >= 0)
			return Rnd.get(100) < _chanceTable[row][column];

		return true;
	}

	public static ElementalAttributeData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly ElementalAttributeData INSTANCE = new();
	}
}