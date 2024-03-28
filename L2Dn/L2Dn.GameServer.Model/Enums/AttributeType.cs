using System.Collections.Immutable;

namespace L2Dn.GameServer.Enums;

/**
 * An enum representing all attribute types.
 * Value corresponds client id.
 * @author NosBit
 */
public enum AttributeType
{
    NONE = -2,
    
    FIRE = 0,
    WATER = 1,
    WIND = 2,
    EARTH = 3,
    HOLY = 4,
    DARK = 5
}

public static class AttributeTypeUtil
{
    public static readonly ImmutableArray<AttributeType> AttributeTypes =
    [
        AttributeType.FIRE,
        AttributeType.WATER,
        AttributeType.WIND,
        AttributeType.EARTH,
        AttributeType.HOLY,
        AttributeType.DARK
    ];

    public static AttributeType getOpposite(this AttributeType attributeType)
    {
        return AttributeTypes[(int)attributeType % 2 == 0 ? (int)attributeType + 1 : (int)attributeType - 1];
    }
}