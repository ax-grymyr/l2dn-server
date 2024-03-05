namespace L2Dn.GameServer.Enums;

public enum RankingCategory
{
    SERVER,
    RACE,
    CLASS,
    CLAN,
    FRIEND
}

public static class RankingCategoryUtil
{
    public static RankingScope getScopeByGroup(this RankingCategory category, int id) =>
        category switch
        {
            RankingCategory.SERVER => id == 0 ? RankingScope.TOP_150 : RankingScope.SELF,
            RankingCategory.RACE => id == 0 ? RankingScope.TOP_100 : RankingScope.SELF,
            RankingCategory.CLASS => id == 0 ? RankingScope.TOP_100 : RankingScope.SELF,
            RankingCategory.CLAN => RankingScope.ALL,
            RankingCategory.FRIEND => RankingScope.ALL,
            _ => RankingScope.ALL
        };
}