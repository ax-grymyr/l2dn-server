using L2Dn.GameServer.Model.Actor.Instances;

namespace L2Dn.GameServer.Model.Actor.Stats;

public class ControllableAirShipStat: VehicleStat
{
    public ControllableAirShipStat(ControllableAirShip activeChar): base(activeChar)
    {
    }

    public override ControllableAirShip getActiveChar()
    {
        return (ControllableAirShip)base.getActiveChar();
    }

    public override double getMoveSpeed()
    {
        if (getActiveChar().isInDock() || (getActiveChar().getFuel() > 0))
        {
            return base.getMoveSpeed();
        }

        return base.getMoveSpeed() * 0.05f;
    }
}