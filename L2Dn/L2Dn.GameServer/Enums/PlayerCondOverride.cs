namespace L2Dn.GameServer.Enums;

public enum PlayerCondOverride
{
    MAX_STATS_VALUE,
    ITEM_CONDITIONS,
    SKILL_CONDITIONS,
    ZONE_CONDITIONS,
    CASTLE_CONDITIONS,
    FORTRESS_CONDITIONS,
    CLANHALL_CONDITIONS,
    FLOOD_CONDITIONS,
    CHAT_CONDITIONS,
    INSTANCE_CONDITIONS,
    QUEST_CONDITIONS,
    DEATH_PENALTY,
    DESTROY_ALL_ITEMS,
    SEE_ALL_PLAYERS,
    TARGET_ALL,
    DROP_ALL_ITEMS
}

public static class PlayerCondOverrideUtil
{
    public static int getMask(this PlayerCondOverride value)
    {
        return 1 << (int)value;
    }
}

// public enum PlayerCondOverride
// {
//     MAX_STATS_VALUE(0, "Overrides maximum states conditions"),
//     ITEM_CONDITIONS(1, "Overrides item usage conditions"),
//     SKILL_CONDITIONS(2, "Overrides skill usage conditions"),
//     ZONE_CONDITIONS(3, "Overrides zone conditions"),
//     CASTLE_CONDITIONS(4, "Overrides castle conditions"),
//     FORTRESS_CONDITIONS(5, "Overrides fortress conditions"),
//     CLANHALL_CONDITIONS(6, "Overrides clan hall conditions"),
//     FLOOD_CONDITIONS(7, "Overrides floods conditions"),
//     CHAT_CONDITIONS(8, "Overrides chat conditions"),
//     INSTANCE_CONDITIONS(9, "Overrides instance conditions"),
//     QUEST_CONDITIONS(10, "Overrides quest conditions"),
//     DEATH_PENALTY(11, "Overrides death penalty conditions"),
//     DESTROY_ALL_ITEMS(12, "Overrides item destroy conditions"),
//     SEE_ALL_PLAYERS(13, "Overrides the conditions to see hidden players"),
//     TARGET_ALL(14, "Overrides target conditions"),
//     DROP_ALL_ITEMS(15, "Overrides item drop conditions");
// 	
//     private final int _mask;
//     private final String _descr;
// 	
//     PlayerCondOverride(int id, String descr)
//     {
//     _mask = 1 << id;
//     _descr = descr;
// }
// 	
// public int getMask()
// {
//     return _mask;
// }
// 	
// public String getDescription()
// {
//     return _descr;
// }
// 	
// public static PlayerCondOverride getCondOverride(int ordinal)
// {
//     try
//     {
//         return values()[ordinal];
//     }
//     catch (Exception e)
//     {
//         return null;
//     }
// }
// 	
// public static long getAllExceptionsMask()
// {
//     long result = 0;
//     for (PlayerCondOverride ex : values())
//     {
//         result |= ex.getMask();
//     }
//     return result;
// }
// }
