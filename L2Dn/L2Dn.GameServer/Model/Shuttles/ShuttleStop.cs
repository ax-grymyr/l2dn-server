namespace L2Dn.GameServer.Model.Shuttles;

public class ShuttleStop
{
    private readonly int _id;
    private bool _isOpen = true;
    private readonly List<Location> _dimensions = new(3);
    private long _lastDoorStatusChanges = System.currentTimeMillis();

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

    public void addDimension(Location loc)
    {
        _dimensions.Add(loc);
    }

    public List<Location> getDimensions()
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
        _lastDoorStatusChanges = System.currentTimeMillis();
    }

    public void closeDoor()
    {
        if (!_isOpen)
        {
            return;
        }

        _isOpen = false;
        _lastDoorStatusChanges = System.currentTimeMillis();
    }

    public bool hasDoorChanged()
    {
        return (System.currentTimeMillis() - _lastDoorStatusChanges) <= 1000;
    }
}