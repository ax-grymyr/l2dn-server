using L2Dn.GameServer.Model.Interfaces;

namespace L2Dn.GameServer.Model;

public class SayuneEntry: ILocational
{
    private bool _isSelector = false;
    private readonly int _id;
    private int _x;
    private int _y;
    private int _z;
    private List<SayuneEntry> _innerEntries = new();

    public SayuneEntry(int id)
    {
        _id = id;
    }

    public SayuneEntry(bool isSelector, int id, int x, int y, int z)
    {
        _isSelector = isSelector;
        _id = id;
        _x = x;
        _y = y;
        _z = z;
    }

    public int getId()
    {
        return _id;
    }

    public int getX()
    {
        return _x;
    }

    public int getY()
    {
        return _y;
    }

    public int getZ()
    {
        return _z;
    }

    public int getHeading()
    {
        return 0;
    }

    public ILocational getLocation()
    {
        return new Location(_x, _y, _z);
    }

    public bool isSelector()
    {
        return _isSelector;
    }

    public List<SayuneEntry> getInnerEntries()
    {
        return _innerEntries;
    }

    public SayuneEntry addInnerEntry(SayuneEntry innerEntry)
    {
        _innerEntries.Add(innerEntry);
        return innerEntry;
    }
}
