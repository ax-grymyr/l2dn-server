using L2Dn.GameServer.Model.Holders;

namespace L2Dn.GameServer.Model.Ensoul;

public class EnsoulOption: SkillHolder
{
    private readonly int _id;
    private readonly string _name;
    private readonly string _desc;

    public EnsoulOption(int id, string name, string desc, int skillId, int skillLevel): base(skillId, skillLevel)
    {
        _id = id;
        _name = name;
        _desc = desc;
    }

    public int getId()
    {
        return _id;
    }

    public string getName()
    {
        return _name;
    }

    public string getDesc()
    {
        return _desc;
    }

    public override string ToString()
    {
        return "Ensoul Id: " + _id + " Name: " + _name + " Desc: " + _desc;
    }
}