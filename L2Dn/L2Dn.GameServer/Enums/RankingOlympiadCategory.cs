namespace L2Dn.GameServer.Enums;

public enum RankingOlympiadCategory
{
    SERVER,
    CLASS
}

public static class RankingOlympiadCategoryUtil
{
    public static RankingOlympiadScope getScopeByGroup(this RankingOlympiadCategory category, int id) =>
        category switch
        {
            RankingOlympiadCategory.SERVER => id == 0 ? RankingOlympiadScope.TOP_100 : RankingOlympiadScope.SELF,
            RankingOlympiadCategory.CLASS => id == 0 ? RankingOlympiadScope.TOP_50 : RankingOlympiadScope.SELF,
            _ => RankingOlympiadScope.TOP_100
        };
}