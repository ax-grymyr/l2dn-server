using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Shuttles;

public class ShuttleStop
{
    private readonly int _id;
    private bool _isOpen = true;
    private readonly List<Location3D> _dimensions = new();
    private DateTime _lastDoorStatusChanges = DateTime.UtcNow;

    public ShuttleStop(int id)
    {
        _id = id;
    }

    public int getId()
    {
        return _id;
    }

    public bool isDoorOpen()
    {
        return _isOpen;
    }

    public void addDimension(Location3D loc)
    {
        _dimensions.Add(loc);
    }

    public List<Location3D> getDimensions()
    {
        return _dimensions;
    }

    public void openDoor()
    {
        if (_isOpen)
        {
            return;
        }

        _isOpen = true;
        _lastDoorStatusChanges = DateTime.UtcNow;
    }

    public void closeDoor()
    {
        if (!_isOpen)
        {
            return;
        }

        _isOpen = false;
        _lastDoorStatusChanges = DateTime.UtcNow;
    }

    public bool hasDoorChanged()
    {
        return (DateTime.UtcNow - _lastDoorStatusChanges) <= TimeSpan.FromSeconds(1);
    }
}