namespace L2Dn.GameServer.Enums;

public enum SkillConditionPercentType
{
    MORE,
    LESS
}

public static class SkillConditionPercentTypeUtil
{
    public static bool test(this SkillConditionPercentType percentType, int x1, int x2) =>
        percentType switch
        {
            SkillConditionPercentType.LESS => x1 <= x2,
            SkillConditionPercentType.MORE => x1 >= x2,
            _ => false
        };
}