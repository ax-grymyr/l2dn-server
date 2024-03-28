namespace L2Dn.GameServer.Enums;

public enum ElementalItemType
{
    STONE,
    STONE_SUPER,
    CRYSTAL,
    CRYSTAL_SUPER,
    JEWEL,
    ENERGY
}

public static class ElementalItemTypeUtil
{
    public static int GetMaxLevel(this ElementalItemType type) =>
        type switch
        {
            ElementalItemType.STONE => 3,
            ElementalItemType.STONE_SUPER => 3,
            ElementalItemType.CRYSTAL => 6,
            ElementalItemType.CRYSTAL_SUPER => 6,
            ElementalItemType.JEWEL => 9,
            ElementalItemType.ENERGY => 12,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
}