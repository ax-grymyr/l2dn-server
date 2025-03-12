using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Items.Types;

public readonly record struct ItemTypeMask(int MaskValue)
{
    private static readonly int _weaponCount = EnumUtil.GetValues<WeaponType>().Length;

    public static readonly ItemTypeMask Zero = default;

    public ItemTypeMask(WeaponType weaponType): this(1 << (int)weaponType)
    {
    }

    public ItemTypeMask(ArmorType armorType): this(1 << (_weaponCount + (int)armorType))
    {
    }

    public static implicit operator ItemTypeMask(WeaponType weaponType) => new(weaponType);
    public static implicit operator ItemTypeMask(ArmorType armorType) => new(armorType);

    public static ItemTypeMask operator ~(ItemTypeMask left) => new(~left.MaskValue);

    public static ItemTypeMask operator |(ItemTypeMask left, ItemTypeMask right) =>
        new(left.MaskValue | right.MaskValue);

    public static ItemTypeMask operator &(ItemTypeMask left, ItemTypeMask right) =>
        new(left.MaskValue & right.MaskValue);
}