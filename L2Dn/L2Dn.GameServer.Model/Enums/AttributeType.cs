using System.Collections.Immutable;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Enums;

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