using System.Collections.Immutable;

namespace L2Dn.GameServer.Model.Commission;

public enum CommissionTreeType
{
    WEAPON,
 	ARMOR,
 	ACCESSORY,
 	SUPPLIES,
 	PET_GOODS,
 	MISC
}

public static class CommissionTreeTypeUtil
{
	private static readonly ImmutableArray<CommissionItemType> _weaponItemTypes =
	[
		CommissionItemType.ONE_HAND_SWORD, CommissionItemType.ONE_HAND_MAGIC_SWORD, CommissionItemType.DAGGER,
		CommissionItemType.RAPIER, CommissionItemType.TWO_HAND_SWORD, CommissionItemType.ANCIENT_SWORD,
		CommissionItemType.DUALSWORD, CommissionItemType.DUAL_DAGGER, CommissionItemType.BLUNT_WEAPON,
		CommissionItemType.ONE_HAND_MAGIC_BLUNT_WEAPON, CommissionItemType.TWO_HAND_BLUNT_WEAPON,
		CommissionItemType.TWO_HAND_MAGIC_BLUNT_WEAPON, CommissionItemType.DUAL_BLUNT_WEAPON, CommissionItemType.BOW,
		CommissionItemType.CROSSBOW, CommissionItemType.FIST_WEAPON, CommissionItemType.SPEAR,
		CommissionItemType.OTHER_WEAPON
	];
	
	private static readonly ImmutableArray<CommissionItemType> _armorItemTypes =
	[
		CommissionItemType.HELMET, CommissionItemType.ARMOR_TOP, CommissionItemType.ARMOR_PANTS,
		CommissionItemType.FULL_BODY, CommissionItemType.GLOVES, CommissionItemType.FEET, CommissionItemType.SHIELD,
		CommissionItemType.SIGIL, CommissionItemType.UNDERWEAR, CommissionItemType.CLOAK
	];

	private static readonly ImmutableArray<CommissionItemType> _accessoryItemTypes =
	[
		CommissionItemType.RING, CommissionItemType.EARRING, CommissionItemType.NECKLACE, CommissionItemType.BELT,
		CommissionItemType.BRACELET, CommissionItemType.AGATHION, CommissionItemType.HAIR_ACCESSORY,
		CommissionItemType.BROOCH_JEWEL, CommissionItemType.ARTIFACT
	];

	private static readonly ImmutableArray<CommissionItemType> _supplyItemTypes =
	[
		CommissionItemType.POTION, CommissionItemType.SCROLL_ENCHANT_WEAPON, CommissionItemType.SCROLL_ENCHANT_ARMOR,
		CommissionItemType.SCROLL_OTHER, CommissionItemType.SOULSHOT, CommissionItemType.SPIRITSHOT,
		CommissionItemType.OTHER_SUPPLIES
	];

	private static readonly ImmutableArray<CommissionItemType> _petGoodsItemTypes =
	[
		CommissionItemType.PET_EQUIPMENT, CommissionItemType.PET_SUPPLIES
	];

	private static readonly ImmutableArray<CommissionItemType> _miscItemTypes =
	[
		CommissionItemType.CRYSTAL, CommissionItemType.RECIPE, CommissionItemType.MAJOR_CRAFTING_INGREDIENTS,
		CommissionItemType.LIFE_STONE, CommissionItemType.SOUL_CRYSTAL, CommissionItemType.ATTRIBUTE_STONE,
		CommissionItemType.WEAPON_ENCHANT_STONE, CommissionItemType.ARMOR_ENCHANT_STONE, CommissionItemType.SPELLBOOK,
		CommissionItemType.GEMSTONE, CommissionItemType.POUCH, CommissionItemType.PIN,
		CommissionItemType.MAGIC_RUNE_CLIP, CommissionItemType.MAGIC_ORNAMENT, CommissionItemType.DYES,
		CommissionItemType.OTHER_ITEM
	];

	public static ImmutableArray<CommissionItemType> GetCommissionItemTypes(this CommissionTreeType treeType) =>
		treeType switch
		{
			CommissionTreeType.WEAPON => _weaponItemTypes,
			CommissionTreeType.ARMOR => _armorItemTypes,
			CommissionTreeType.ACCESSORY => _accessoryItemTypes,
			CommissionTreeType.SUPPLIES => _supplyItemTypes,
			CommissionTreeType.PET_GOODS => _petGoodsItemTypes,
			CommissionTreeType.MISC => _miscItemTypes,
			_ => ImmutableArray<CommissionItemType>.Empty
		};
}