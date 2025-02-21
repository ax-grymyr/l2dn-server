using L2Dn.GameServer.Model.Holders;

namespace L2Dn.GameServer.Model.Ensoul;

public class EnsoulOption(int id, string name, string desc, int skillId, int skillLevel)
    : SkillHolder(skillId, skillLevel)
{
    public int getId()
    {
        return id;
    }

    public string getName()
    {
        return name;
    }

    public string getDesc()
    {
        return desc;
    }

    public override string ToString()
    {
        return "Ensoul Id: " + id + " Name: " + name + " Desc: " + desc;
    }
}