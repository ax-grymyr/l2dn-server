using L2Dn.Geometry;

namespace L2Dn.GameServer.Model;

public class SayuneEntry
{
    private readonly List<SayuneEntry> _innerEntries = [];
    private readonly Location3D _location;
    private readonly int _id;
    private readonly bool _isSelector;

    public SayuneEntry(int id)
    {
        _id = id;
    }

    public SayuneEntry(bool isSelector, int id, Location3D location)
    {
        _isSelector = isSelector;
        _id = id;
        _location = location;
    }

    public int getId()
    {
        return _id;
    }

    public Location3D Location => _location;

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