using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.StaticData;
using L2Dn.Model.Enums;

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
        if (CategoryData.Instance.IsInCategory(CategoryType.STRIDER, npcId))
        {
            return MountType.STRIDER;
        }
        if (CategoryData.Instance.IsInCategory(CategoryType.WYVERN_GROUP, npcId))
        {
            return MountType.WYVERN;
        }
        if (CategoryData.Instance.IsInCategory(CategoryType.WOLF_GROUP, npcId))
        {
            return MountType.WOLF;
        }

        return MountType.NONE;
    }
}