namespace L2Dn.GameServer.Model.Items.Types;

public record struct ItemType(int Value)
{
    private static readonly int _weaponCount = Enum.GetValues<WeaponType>().Length;
    private static readonly int _armorCount = Enum.GetValues<ArmorType>().Length;
    private static readonly int _weaponAndArmorCount = _weaponCount + _armorCount;

    public static implicit operator ItemType(WeaponType weaponType) => new((int)weaponType);
    public static implicit operator ItemType(ArmorType armorType) => new(_weaponCount + (int)armorType);

    public static implicit operator ItemType(EtcItemType etcItemType) => new(_weaponAndArmorCount + (int)etcItemType);

    public ItemTypeMask GetMask() => Value >= _weaponAndArmorCount ? ItemTypeMask.Zero : new ItemTypeMask(1 << Value);

    public bool IsWeapon() => Value >= 0 && Value < _weaponCount;
    public bool IsArmor() => Value >= _weaponCount && Value < _weaponAndArmorCount;
    public bool IsEtcItem() => Value >= _weaponAndArmorCount;

    public bool isRanged() => Value switch
    {
        (int)WeaponType.BOW or (int)WeaponType.CROSSBOW or (int)WeaponType.TWOHANDCROSSBOW
            or (int)WeaponType.PISTOLS => true,
        _ => false
    };

    public bool isCrossbow() => Value switch
    {
        (int)WeaponType.CROSSBOW or (int)WeaponType.TWOHANDCROSSBOW => true,
        _ => false
    };

    public bool isPistols() => Value switch
    {
        (int)WeaponType.PISTOLS => true,
        _ => false
    };

    public bool isDual() => Value switch
    {
        (int)WeaponType.DUALFIST or (int)WeaponType.DUAL or (int)WeaponType.DUALDAGGER
            or (int)WeaponType.DUALBLUNT => true,
        _ => false
    };

    public WeaponType AsWeaponType()
    {
        if (Value < 0 || Value >= _weaponCount)
            throw new InvalidOperationException("Item type is not weapon");
        
        return (WeaponType)Value;
    }

    public ArmorType AsArmorType()
    {
        if (Value < _weaponCount || Value >= _weaponAndArmorCount)
            throw new InvalidOperationException("Item type is not armor");

        return (ArmorType)(Value - _weaponCount);
    }

    public EtcItemType AsEtcItemType()
    {
        if (Value < _weaponAndArmorCount)
            throw new InvalidOperationException("Item type is not Etc Item");

        return (EtcItemType)(Value - _weaponAndArmorCount);
    }
}