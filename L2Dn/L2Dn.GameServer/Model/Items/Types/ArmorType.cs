namespace L2Dn.GameServer.Model.Items.Types;

/**
 * Armor Type enumerated.
 */
public enum ArmorType
{
	NONE,
	LIGHT,
	HEAVY,
	MAGIC,
	SIGIL,
	
	// L2J CUSTOM
	SHIELD
}

public static class ArmorTypeUtils
{
	public static int GetMask(this ArmorType armorType)
	{
		return 1 << ((int)armorType + Enum.GetValues<WeaponType>().Length); // TODO: optimize
	}
}