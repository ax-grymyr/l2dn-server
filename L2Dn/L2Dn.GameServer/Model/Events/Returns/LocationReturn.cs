using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Interfaces;

namespace L2Dn.GameServer.Model.Events.Returns;

public class LocationReturn: TerminateReturn
{
    private readonly bool _overrideLocation;
    private int _x;
    private int _y;
    private int _z;
    private int _heading;
    private Instance _instance;

    public LocationReturn(bool terminate, bool overrideLocation): base(terminate, false, false)
    {
        _overrideLocation = overrideLocation;
    }

    public LocationReturn(bool terminate, bool overrideLocation, ILocational targetLocation, Instance instance): base(
        terminate, false, false)
    {
        _overrideLocation = overrideLocation;
        if (targetLocation != null)
        {
            setX(targetLocation.getX());
            setY(targetLocation.getY());
            setZ(targetLocation.getZ());
            setHeading(targetLocation.getHeading());
            setInstance(instance);
        }
    }

    public void setX(int x)
    {
        _x = x;
    }

    public void setY(int y)
    {
        _y = y;
    }

    public void setZ(int z)
    {
        _z = z;
    }

    public void setHeading(int heading)
    {
        _heading = heading;
    }

    public void setInstance(Instance instance)
    {
        _instance = instance;
    }

    public bool overrideLocation()
    {
        return _overrideLocation;
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
        return _heading;
    }

    public Instance getInstance()
    {
        return _instance;
    }
}