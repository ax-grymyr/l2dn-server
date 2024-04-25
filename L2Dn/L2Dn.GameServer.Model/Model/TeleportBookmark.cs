using L2Dn.Geometry;

namespace L2Dn.GameServer.Model;

public class TeleportBookmark
{
    private readonly int _id;
    private readonly Location3D _location;
    private int _icon;
    private string _name;
    private string _tag;

    public TeleportBookmark(int id, Location3D location, int icon, string tag, string name)
    {
        _id = id;
        _location = location;
        _icon = icon;
        _name = name;
        _tag = tag;
    }

    public Location3D Location => _location;

    public string getName()
    {
        return _name;
    }

    public void setName(string name)
    {
        _name = name;
    }

    public int getId()
    {
        return _id;
    }

    public int getIcon()
    {
        return _icon;
    }

    public void setIcon(int icon)
    {
        _icon = icon;
    }

    public string getTag()
    {
        return _tag;
    }

    public void setTag(string tag)
    {
        _tag = tag;
    }
}