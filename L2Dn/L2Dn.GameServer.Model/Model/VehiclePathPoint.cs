using L2Dn.Geometry;

namespace L2Dn.GameServer.Model;

public class VehiclePathPoint
{
    private readonly Location3D _location;
    private readonly int _moveSpeed;
    private readonly int _rotationSpeed;

    public VehiclePathPoint(Location3D location)
    {
        _location = location;
        _moveSpeed = 350;
        _rotationSpeed = 4000;
    }

    public VehiclePathPoint(Location3D location, int moveSpeed, int rotationSpeed)
    {
        _location = location;
        _moveSpeed = moveSpeed;
        _rotationSpeed = rotationSpeed;
    }

    public Location3D Location => _location;

    public int getMoveSpeed()
    {
        return _moveSpeed;
    }

    public int getRotationSpeed()
    {
        return _rotationSpeed;
    }
}