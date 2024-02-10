namespace L2Dn.GameServer.Model;

public class TeleportBookmark: Location
{
    private readonly int _id;
    private int _icon;
    private String _name;
    private String _tag;

    public TeleportBookmark(int id, int x, int y, int z, int icon, String tag, String name): base(x, y, z)
    {
        _id = id;
        _icon = icon;
        _name = name;
        _tag = tag;
    }

    public String getName()
    {
        return _name;
    }

    public void setName(String name)
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

    public String getTag()
    {
        return _tag;
    }

    public void setTag(String tag)
    {
        _tag = tag;
    }
}
