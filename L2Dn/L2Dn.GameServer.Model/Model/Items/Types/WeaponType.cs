using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Model.Items.Types;

/**
 * Weapon Type enumerated.
 * @author mkizub
 */
public enum WeaponType
{
	NONE,
	
	SWORD,
	BLUNT,
	DAGGER,
	POLE,
	DUALFIST,
	BOW,
	ETC,
	DUAL,
	FIST, // 0 items with that type
	FISHINGROD,
	RAPIER,
	CROSSBOW,
	ANCIENTSWORD,
	FLAG, // 0 items with that type
	DUALDAGGER,
	OWNTHING, // 0 items with that type
	TWOHANDCROSSBOW,
	DUALBLUNT,
	PISTOLS
}

public static class WeaponTypeUtil
{
	public static TraitType GetTraitType(this WeaponType weaponType) =>
		weaponType switch
		{
			WeaponType.NONE => TraitType.NONE,
			WeaponType.SWORD => TraitType.SWORD,
			WeaponType.BLUNT => TraitType.BLUNT,
			WeaponType.DAGGER => TraitType.DAGGER,
			WeaponType.POLE => TraitType.POLE,
			WeaponType.DUALFIST => TraitType.DUALFIST,
			WeaponType.BOW => TraitType.BOW,
			WeaponType.ETC => TraitType.ETC,
			WeaponType.DUAL => TraitType.DUAL,
			WeaponType.FIST => TraitType.FIST, // 0 items with that type
			WeaponType.FISHINGROD => TraitType.NONE,
			WeaponType.RAPIER => TraitType.RAPIER,
			WeaponType.CROSSBOW => TraitType.CROSSBOW,
			WeaponType.ANCIENTSWORD => TraitType.ANCIENTSWORD,
			WeaponType.FLAG => TraitType.NONE, // 0 items with that type
			WeaponType.DUALDAGGER => TraitType.DUALDAGGER,
			WeaponType.OWNTHING => TraitType.NONE, // 0 items with that type
			WeaponType.TWOHANDCROSSBOW => TraitType.TWOHANDCROSSBOW,
			WeaponType.DUALBLUNT => TraitType.DUALBLUNT,
			WeaponType.PISTOLS => TraitType.PISTOLS,
			_ => throw new ArgumentOutOfRangeException(nameof(weaponType))
		};

	public static bool isRanged(this WeaponType weaponType) =>
		weaponType switch
		{
			WeaponType.BOW or WeaponType.CROSSBOW or WeaponType.TWOHANDCROSSBOW or WeaponType.PISTOLS => true,
			_ => false
		};

	public static bool isCrossbow(this WeaponType weaponType) =>
		weaponType switch
		{
			WeaponType.CROSSBOW or WeaponType.TWOHANDCROSSBOW => true,
			_ => false
		};

	public static bool isPistols(this WeaponType weaponType) =>
		weaponType switch
		{
			WeaponType.PISTOLS => true,
			_ => false
		};

	public static bool isDual(this WeaponType weaponType) =>
		weaponType switch
		{
			WeaponType.DUALFIST or WeaponType.DUAL or WeaponType.DUALDAGGER or WeaponType.DUALBLUNT => true,
			_ => false
		};
}