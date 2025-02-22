using System.Collections.Immutable;
using L2Dn.GameServer.Enums;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Items.Types;

public static class CrystalTypeUtil
{
    private sealed record CrystalTypeInfo(
        CrystalType Level, ItemGrade ItemGrade, int CrystalId, int CrystalEnchantBonusArmor,
        int CrystalEnchantBonusWeapon);

    private static readonly ImmutableArray<CrystalTypeInfo> _crystalInfos;

    static CrystalTypeUtil()
    {
        CrystalTypeInfo none = new(CrystalType.NONE, ItemGrade.NONE, 0, 0, 0);
        CrystalTypeInfo d = new(CrystalType.D, ItemGrade.D, 1458, 11, 90);
        CrystalTypeInfo c = new(CrystalType.C, ItemGrade.C, 1459, 6, 45);
        CrystalTypeInfo b = new(CrystalType.B, ItemGrade.B, 1460, 11, 67);
        CrystalTypeInfo a = new(CrystalType.A, ItemGrade.A, 1461, 20, 145);
        CrystalTypeInfo s = new(CrystalType.S, ItemGrade.S, 1462, 25, 250);
        CrystalTypeInfo s80 = new(CrystalType.S80, ItemGrade.S, 1462, 25, 250);
        CrystalTypeInfo s84 = new(CrystalType.S84, ItemGrade.S, 1462, 25, 250);
        CrystalTypeInfo r = new(CrystalType.R, ItemGrade.R, 17371, 30, 500);
        CrystalTypeInfo r95 = new(CrystalType.R95, ItemGrade.R, 17371, 30, 500);
        CrystalTypeInfo r99 = new(CrystalType.R99, ItemGrade.R, 17371, 30, 500);
        CrystalTypeInfo @event = new(CrystalType.EVENT, ItemGrade.NONE, 0, 0, 0);

        _crystalInfos = [none, d, c, b, a, s, s80, s84, r, r95, r99, @event];
    }

    public static CrystalType getLevel(this CrystalType crystalType) => _crystalInfos[(int)crystalType].Level;
    public static int getCrystalId(this CrystalType crystalType) => _crystalInfos[(int)crystalType].CrystalId;
    public static ItemGrade GetItemGrade(this CrystalType crystalType) => _crystalInfos[(int)crystalType].ItemGrade;

    public static int getCrystalEnchantBonusArmor(this CrystalType crystalType) =>
        _crystalInfos[(int)crystalType].CrystalEnchantBonusArmor;

    public static int getCrystalEnchantBonusWeapon(this CrystalType crystalType) =>
        _crystalInfos[(int)crystalType].CrystalEnchantBonusWeapon;
}