using L2Dn.Geometry;

namespace L2Dn.GameServer.Model;

public class VehiclePathPoint
{
    private readonly LocationHeading _location;
    private readonly int _moveSpeed;
    private readonly int _rotationSpeed;

    public VehiclePathPoint(LocationHeading loc)
    {
        _location = loc;
    }

    public VehiclePathPoint(int x, int y, int z)
    {
        _location = new LocationHeading(x, y, z, 0);
        _moveSpeed = 350;
        _rotationSpeed = 4000;
    }

    public VehiclePathPoint(int x, int y, int z, int moveSpeed, int rotationSpeed)
    {
        _location = new LocationHeading(x, y, z, 0);
        _moveSpeed = moveSpeed;
        _rotationSpeed = rotationSpeed;
    }

    public LocationHeading Location => _location;

    public int getMoveSpeed()
    {
        return _moveSpeed;
    }

    public int getRotationSpeed()
    {
        return _rotationSpeed;
    }
}
