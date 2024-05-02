using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Shuttles;

public class ShuttleDataHolder
{
    private readonly int _id;
    private readonly LocationHeading _loc;
    private readonly List<int> _doors = new(2);
    private readonly List<ShuttleStop> _stops = new(2);
    private readonly List<VehiclePathPoint[]> _routes = new(2);

    public ShuttleDataHolder(int id, LocationHeading loc)
    {
        _id = id;
        _loc = loc;
    }

    public int getId()
    {
        return _id;
    }

    public LocationHeading Location => _loc;

    public void addDoor(int id)
    {
        _doors.Add(id);
    }

    public List<int> getDoors()
    {
        return _doors;
    }

    public void addStop(ShuttleStop stop)
    {
        _stops.Add(stop);
    }

    public List<ShuttleStop> getStops()
    {
        return _stops;
    }

    public void addRoute(VehiclePathPoint[] route)
    {
        _routes.Add(route);
    }

    public List<VehiclePathPoint[]> getRoutes()
    {
        return _routes;
    }
}