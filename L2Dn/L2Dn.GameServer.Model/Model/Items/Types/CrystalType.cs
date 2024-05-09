using L2Dn.GameServer.Enums;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Items.Types;

public struct CrystalTypeInfo(
	CrystalType level,
	int crystalId,
	int crystalEnchantBonusArmor,
	int crystalEnchantBonusWeapon)
{
	public static readonly CrystalTypeInfo NONE = new(CrystalType.NONE, 0, 0, 0);
	public static readonly CrystalTypeInfo D = new(CrystalType.D, 1458, 11, 90);
	public static readonly CrystalTypeInfo C = new(CrystalType.C, 1459, 6, 45);
	public static readonly CrystalTypeInfo B = new(CrystalType.B, 1460, 11, 67);
	public static readonly CrystalTypeInfo A = new(CrystalType.A, 1461, 20, 145);
	public static readonly CrystalTypeInfo S = new(CrystalType.S, 1462, 25, 250);
	public static readonly CrystalTypeInfo S80 = new(CrystalType.S80, 1462, 25, 250);
	public static readonly CrystalTypeInfo S84 = new(CrystalType.S84, 1462, 25, 250);
	public static readonly CrystalTypeInfo R = new(CrystalType.R, 17371, 30, 500);
	public static readonly CrystalTypeInfo R95 = new(CrystalType.R95, 17371, 30, 500);
	public static readonly CrystalTypeInfo R99 = new(CrystalType.R99, 17371, 30, 500);
	public static readonly CrystalTypeInfo EVENT = new(CrystalType.EVENT, 0, 0, 0);

	public static readonly IReadOnlyList<CrystalTypeInfo> All = new[] { NONE, D, C, B, A, S, S80, S84, R, R95, R99, EVENT }; 
	
	public CrystalType getLevel() => level;
	public int getCrystalId() => crystalId;
	public int getCrystalEnchantBonusArmor() => crystalEnchantBonusArmor;
	public int getCrystalEnchantBonusWeapon() => crystalEnchantBonusWeapon;

	public static CrystalTypeInfo Get(CrystalType crystalType) => All[(int)crystalType];
}

public static class CrystalTypeUtil
{
	public static CrystalType getLevel(this CrystalType crystalType) => CrystalTypeInfo.Get(crystalType).getLevel();
    
	public static ItemGrade GetItemGrade(this CrystalType crystalType) =>
		crystalType switch
		{
			CrystalType.NONE => ItemGrade.NONE,
			CrystalType.D => ItemGrade.D,
			CrystalType.C => ItemGrade.C,
			CrystalType.B => ItemGrade.B,
			CrystalType.A => ItemGrade.A,
			CrystalType.S or CrystalType.S80 or CrystalType.S84 => ItemGrade.S,
			CrystalType.R or CrystalType.R95 or CrystalType.R99 => ItemGrade.R,
			_ => ItemGrade.NONE
		};
}