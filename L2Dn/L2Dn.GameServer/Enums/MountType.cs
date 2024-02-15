using L2Dn.GameServer.Data.Xml;

namespace L2Dn.GameServer.Enums;

public enum MountType
{
    NONE,
    STRIDER,
    WYVERN,
    WOLF
}

public static class MountTypeUtil
{
    public static MountType findByNpcId(int npcId)
    {
        if (CategoryData.getInstance().isInCategory(CategoryType.STRIDER, npcId))
        {
            return MountType.STRIDER;
        }
        if (CategoryData.getInstance().isInCategory(CategoryType.WYVERN_GROUP, npcId))
        {
            return MountType.WYVERN;
        }
        if (CategoryData.getInstance().isInCategory(CategoryType.WOLF_GROUP, npcId))
        {
            return MountType.WOLF;
        }

        return MountType.NONE;
    }
}