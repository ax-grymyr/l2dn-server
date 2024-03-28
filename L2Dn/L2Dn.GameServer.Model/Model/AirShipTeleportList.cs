namespace L2Dn.GameServer.Model;

/**
 * Holds a list of all AirShip teleports.
 * @author xban1x
 */
public class AirShipTeleportList
{
    private readonly int _location;
    private readonly int[] _fuel;
    private readonly VehiclePathPoint[][] _routes;

    public AirShipTeleportList(int loc, int[] f, VehiclePathPoint[][] r)
    {
        _location = loc;
        _fuel = f;
        _routes = r;
    }

    public int getLocation()
    {
        return _location;
    }

    public int[] getFuel()
    {
        return _fuel;
    }

    public VehiclePathPoint[][] getRoute()
    {
        return _routes;
    }
}
