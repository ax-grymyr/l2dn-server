using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Enums;

public enum ElementalType: byte
{
    NONE,
    FIRE,
    WATER,
    WIND,
    EARTH
}

public static class ElementalTypeUtil
{
    public static bool isSuperior(this ElementalType elementalType, ElementalType targetType)
    {
        return elementalType == superior(targetType);
    }

    public static bool isInferior(this ElementalType elementalType, ElementalType targetType)
    {
        return targetType == superior(elementalType);
    }

    public static ElementalType getSuperior(this ElementalType elementalType)
    {
        return superior(elementalType);
    }

    public static ElementalType superior(this ElementalType elementalType) =>
        elementalType switch
        {
            ElementalType.FIRE => ElementalType.WATER,
            ElementalType.WATER => ElementalType.WIND,
            ElementalType.WIND => ElementalType.EARTH,
            ElementalType.EARTH => ElementalType.FIRE,
            _ => ElementalType.NONE
        };

    public static Stat getAttackStat(this ElementalType elementalType) =>
        elementalType switch
        {
            ElementalType.EARTH => Stat.ELEMENTAL_SPIRIT_EARTH_ATTACK,
            ElementalType.WIND => Stat.ELEMENTAL_SPIRIT_WIND_ATTACK,
            ElementalType.FIRE => Stat.ELEMENTAL_SPIRIT_FIRE_ATTACK,
            ElementalType.WATER => Stat.ELEMENTAL_SPIRIT_WATER_ATTACK,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static Stat getDefenseStat(this ElementalType elementalType) =>
        elementalType switch
        {
            ElementalType.EARTH => Stat.ELEMENTAL_SPIRIT_EARTH_DEFENSE,
            ElementalType.WIND => Stat.ELEMENTAL_SPIRIT_WIND_DEFENSE,
            ElementalType.FIRE => Stat.ELEMENTAL_SPIRIT_FIRE_DEFENSE,
            ElementalType.WATER => Stat.ELEMENTAL_SPIRIT_WATER_DEFENSE,
            _ => throw new ArgumentOutOfRangeException()
        };
}