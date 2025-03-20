namespace L2Dn.GameServer.Model.Holders;

public sealed class AttachSkillHolder(int skillId, int skillLevel, int requiredSkillId, int requiredSkillLevel)
    : SkillHolder(skillId, skillLevel)
{
    public int getRequiredSkillId() => requiredSkillId;

    public int getRequiredSkillLevel() => requiredSkillLevel;
}