namespace L2Dn.GameServer.Model;

public class VehiclePathPoint: Location
{
    private readonly int _moveSpeed;
    private readonly int _rotationSpeed;

    public VehiclePathPoint(Location loc): base(loc.getX(), loc.getY(), loc.getZ())
    {
    }

    public VehiclePathPoint(int x, int y, int z): base(x, y, z)
    {
        _moveSpeed = 350;
        _rotationSpeed = 4000;
    }

    public VehiclePathPoint(int x, int y, int z, int moveSpeed, int rotationSpeed): base(x, y, z)
    {
        _moveSpeed = moveSpeed;
        _rotationSpeed = rotationSpeed;
    }

    public int getMoveSpeed()
    {
        return _moveSpeed;
    }

    public int getRotationSpeed()
    {
        return _rotationSpeed;
    }
}
