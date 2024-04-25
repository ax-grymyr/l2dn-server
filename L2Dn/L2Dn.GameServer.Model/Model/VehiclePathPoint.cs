namespace L2Dn.GameServer.Model;

public class VehiclePathPoint
{
    private readonly Location _location;
    private readonly int _moveSpeed;
    private readonly int _rotationSpeed;

    public VehiclePathPoint(Location loc)
    {
        _location = loc;
    }

    public VehiclePathPoint(int x, int y, int z)
    {
        _location = new Location(x, y, z);
        _moveSpeed = 350;
        _rotationSpeed = 4000;
    }

    public VehiclePathPoint(int x, int y, int z, int moveSpeed, int rotationSpeed)
    {
        _location = new Location(x, y, z);
        _moveSpeed = moveSpeed;
        _rotationSpeed = rotationSpeed;
    }

    public Location Location => _location;

    public int getMoveSpeed()
    {
        return _moveSpeed;
    }

    public int getRotationSpeed()
    {
        return _rotationSpeed;
    }
}
