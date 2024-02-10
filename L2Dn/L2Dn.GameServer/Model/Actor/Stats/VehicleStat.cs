namespace L2Dn.GameServer.Model.Actor.Stats;

public class VehicleStat: CreatureStat
{
    private float _moveSpeed = 0;
    private int _rotationSpeed = 0;

    public VehicleStat(Vehicle activeChar): base(activeChar)
    {
    }

    public override double getMoveSpeed()
    {
        return _moveSpeed;
    }

    public void setMoveSpeed(float speed)
    {
        _moveSpeed = speed;
    }

    public double getRotationSpeed()
    {
        return _rotationSpeed;
    }

    public void setRotationSpeed(int speed)
    {
        _rotationSpeed = speed;
    }
}