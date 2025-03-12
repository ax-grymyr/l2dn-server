namespace L2Dn.GameServer.Model.Holders;

public sealed class AttachSkillHolder(int skillId, int skillLevel, int requiredSkillId, int requiredSkillLevel)
    : SkillHolder(skillId, skillLevel)
{
    public int getRequiredSkillId() => requiredSkillId;

    public int getRequiredSkillLevel() => requiredSkillLevel;

    public static AttachSkillHolder FromStatSet(StatSet set) =>
        new(set.getInt("skillId"), set.getInt("skillLevel", 1), set.getInt("requiredSkillId"),
            set.getInt("requiredSkillLevel", 1));
}