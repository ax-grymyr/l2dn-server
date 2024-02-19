using L2Dn.GameServer.Model.Holders;

namespace L2Dn.GameServer.Model.Ensoul;

public class EnsoulOption: SkillHolder
{
    private readonly int _id;
    private readonly String _name;
    private readonly String _desc;

    public EnsoulOption(int id, String name, String desc, int skillId, int skillLevel): base(skillId, skillLevel)
    {
        _id = id;
        _name = name;
        _desc = desc;
    }

    public int getId()
    {
        return _id;
    }

    public String getName()
    {
        return _name;
    }

    public String getDesc()
    {
        return _desc;
    }

    public override String ToString()
    {
        return "Ensoul Id: " + _id + " Name: " + _name + " Desc: " + _desc;
    }
}