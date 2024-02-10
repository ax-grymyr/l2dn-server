using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Mobius
 */
public class ElementalAttributeData
{
	private static readonly Map<int, ElementalItemHolder> ELEMENTAL_ITEMS = new();
	
	public const int FIRST_WEAPON_BONUS = 20;
	public const int NEXT_WEAPON_BONUS = 5;
	public const int ARMOR_BONUS = 6;
	
	public static readonly int[] WEAPON_VALUES =
	{
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
		int.MaxValue
		// TODO: Higher stones
	};
	
	public static readonly int[] ARMOR_VALUES =
	{
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
	};
	
	/* @formatter:off */
	private static readonly int[][] CHANCE_TABLE =
	[
		[Config.S_WEAPON_STONE,		Config.S_ARMOR_STONE,		Config.S_WEAPON_CRYSTAL,	Config.S_ARMOR_CRYSTAL,		Config.S_WEAPON_STONE_SUPER,	Config.S_ARMOR_STONE_SUPER,		Config.S_WEAPON_CRYSTAL_SUPER,		Config.S_ARMOR_CRYSTAL_SUPER,		Config.S_WEAPON_JEWEL,		Config.S_ARMOR_JEWEL],
		[Config.S80_WEAPON_STONE,	Config.S80_ARMOR_STONE,		Config.S80_WEAPON_CRYSTAL,	Config.S80_ARMOR_CRYSTAL,	Config.S80_WEAPON_STONE_SUPER,	Config.S80_ARMOR_STONE_SUPER,	Config.S80_WEAPON_CRYSTAL_SUPER,	Config.S80_ARMOR_CRYSTAL_SUPER,		Config.S80_WEAPON_JEWEL,	Config.S80_ARMOR_JEWEL],
		[Config.S84_WEAPON_STONE,	Config.S84_ARMOR_STONE,		Config.S84_WEAPON_CRYSTAL,	Config.S84_ARMOR_CRYSTAL,	Config.S84_WEAPON_STONE_SUPER,	Config.S84_ARMOR_STONE_SUPER,	Config.S84_WEAPON_CRYSTAL_SUPER,	Config.S84_ARMOR_CRYSTAL_SUPER,		Config.S84_WEAPON_JEWEL,	Config.S84_ARMOR_JEWEL],
		[Config.R_WEAPON_STONE,		Config.R_ARMOR_STONE,		Config.R_WEAPON_CRYSTAL,	Config.R_ARMOR_CRYSTAL,		Config.R_WEAPON_STONE_SUPER,	Config.R_ARMOR_STONE_SUPER,		Config.R_WEAPON_CRYSTAL_SUPER,		Config.R_ARMOR_CRYSTAL_SUPER,		Config.R_WEAPON_JEWEL,		Config.R_ARMOR_JEWEL],
		[Config.R95_WEAPON_STONE,	Config.R95_ARMOR_STONE,		Config.R95_WEAPON_CRYSTAL,	Config.R95_ARMOR_CRYSTAL,	Config.R95_WEAPON_STONE_SUPER,	Config.R95_ARMOR_STONE_SUPER,	Config.R95_WEAPON_CRYSTAL_SUPER,	Config.R95_ARMOR_CRYSTAL_SUPER,		Config.R95_WEAPON_JEWEL,	Config.R95_ARMOR_JEWEL],
		[Config.R99_WEAPON_STONE,	Config.R99_ARMOR_STONE,		Config.R99_WEAPON_CRYSTAL,	Config.R99_ARMOR_CRYSTAL,	Config.R99_WEAPON_STONE_SUPER,	Config.R99_ARMOR_STONE_SUPER,	Config.R99_WEAPON_CRYSTAL_SUPER,	Config.R99_ARMOR_CRYSTAL_SUPER,		Config.R99_WEAPON_JEWEL,	Config.R99_ARMOR_JEWEL],
	];	
	/* @formatter:on */
	
	protected ElementalAttributeData()
	{
		load();
	}
	
	public void load()
	{
		ELEMENTAL_ITEMS.clear();
		parseDatapackFile("data/ElementalAttributeData.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + ELEMENTAL_ITEMS.size() + " elemental attribute items.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, "item", itemNode =>
		{
			StatSet set = new StatSet(parseAttributes(itemNode));
			
			int id = set.getInt("id");
			if (ItemData.getInstance().getTemplate(id) == null)
			{
				LOGGER.Info(GetType().Name + ": Could not find item with id " + id + ".");
				return;
			}
			
			ELEMENTAL_ITEMS.put(id, new ElementalItemHolder(id, set.getEnum("elemental", AttributeType.class), set.getEnum("type", ElementalItemType.class), set.getInt("power", 0)));
		}));
	}
	
	public AttributeType getItemElement(int itemId)
	{
		ElementalItemHolder item = ELEMENTAL_ITEMS.get(itemId);
		if (item != null)
		{
			return item.getElement();
		}
		return AttributeType.NONE;
	}
	
	public ElementalItemHolder getItemElemental(int itemId)
	{
		return ELEMENTAL_ITEMS.get(itemId);
	}
	
	public int getMaxElementLevel(int itemId)
	{
		ElementalItemHolder item = ELEMENTAL_ITEMS.get(itemId);
		if (item != null)
		{
			return item.getType().getMaxLevel();
		}
		return -1;
	}
	
	public bool isSuccess(Item item, int stoneId)
	{
		int row = -1;
		int column = -1;
		switch (item.getTemplate().getCrystalType())
		{
			case CrystalType.S:
			{
				row = 0;
				break;
			}
			case CrystalType.S80:
			{
				row = 1;
				break;
			}
			case CrystalType.S84:
			{
				row = 2;
				break;
			}
			case CrystalType.R:
			{
				row = 3;
				break;
			}
			case CrystalType.R95:
			{
				row = 4;
				break;
			}
			case CrystalType.R99:
			{
				row = 5;
				break;
			}
		}
		
		switch (ELEMENTAL_ITEMS.get(stoneId).getType())
		{
			case ElementalItemType.STONE:
			{
				column = item.isWeapon() ? 0 : 1;
				break;
			}
			case ElementalItemType.CRYSTAL:
			{
				column = item.isWeapon() ? 2 : 3;
				break;
			}
			case ElementalItemType.STONE_SUPER:
			{
				column = item.isWeapon() ? 4 : 5;
				break;
			}
			case ElementalItemType.CRYSTAL_SUPER:
			{
				column = item.isWeapon() ? 6 : 7;
				break;
			}
			case ElementalItemType.JEWEL:
			{
				column = item.isWeapon() ? 8 : 9;
				break;
			}
		}
		if ((row != -1) && (column != -1))
		{
			return Rnd.get(100) < CHANCE_TABLE[row][column];
		}
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