using System.Collections.Frozen;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Data.Xml;

public static class PetSkillDataExtensions
{
    public static int GetAvailableLevel(this PetSkillData self, Summon pet, int skillId)
    {
        int level = 0;
        FrozenDictionary<long, SkillHolder> holders = self.GetSkills(pet.Id);
        foreach (SkillHolder skillHolder in holders.Values)
        {
            if (skillHolder.getSkillId() != skillId)
            {
                continue;
            }

            if (skillHolder.getSkillLevel() == 0)
            {
                if (pet.getLevel() < 70)
                {
                    level = pet.getLevel() / 10;
                    if (level <= 0)
                    {
                        level = 1;
                    }
                }
                else
                {
                    level = 7 + (pet.getLevel() - 70) / 5;
                }

                // formula usable for skill that have 10 or more skill levels
                int maxLevel = SkillData.Instance.GetMaxLevel(skillHolder.getSkillId());
                if (level > maxLevel)
                {
                    level = maxLevel;
                }

                break;
            }

            if (1 <= pet.getLevel() && skillHolder.getSkillLevel() > level)
            {
                level = skillHolder.getSkillLevel();
            }
        }

        return level;
    }

    public static List<int> GetAvailableSkills(this PetSkillData self, Summon pet)
    {
        List<int> skillIds = [];
        FrozenDictionary<long, SkillHolder> holders = self.GetSkills(pet.Id);
        foreach (SkillHolder skillHolder in holders.Values)
        {
            if (skillIds.Contains(skillHolder.getSkillId()))
                continue;

            skillIds.Add(skillHolder.getSkillId());
        }

        return skillIds;
    }

    public static List<Skill> GetKnownSkills(this PetSkillData self, Summon pet)
    {
        List<Skill> skills = [];
        FrozenDictionary<long, SkillHolder> holders = self.GetSkills(pet.Id);
        foreach (SkillHolder skillHolder in holders.Values)
        {
            Skill skill = skillHolder.getSkill();
            if (skills.Contains(skill))
                continue;

            skills.Add(skill);
        }

        return skills;
    }

    public static Skill? GetKnownSkill(this PetSkillData self, Summon pet, int skillId)
    {
        FrozenDictionary<long, SkillHolder> holders = self.GetSkills(pet.Id);
        foreach (SkillHolder skillHolder in holders.Values)
        {
            if (skillHolder.getSkillId() == skillId)
                return skillHolder.getSkill();
        }

        return null;
    }
}