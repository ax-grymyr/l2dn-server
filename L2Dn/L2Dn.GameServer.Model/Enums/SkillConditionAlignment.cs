using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Enums;

public enum SkillConditionAlignment
{
    LAWFUL,
    CHAOTIC
}

public static class SkillConditionAlignmentUtil
{
    public static bool test(this SkillConditionAlignment alignment, Player player) =>
        alignment switch
        {
            SkillConditionAlignment.LAWFUL => player.getReputation() >= 0,
            SkillConditionAlignment.CHAOTIC => player.getReputation() < 0,
            _ => false
        };
}