using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Interfaces;

public interface ISkillsHolder
{
    Map<int, Skill> getSkills();

    Skill? addSkill(Skill skill);

    Skill? getKnownSkill(int skillId);

    int getSkillLevel(int skillId);
}