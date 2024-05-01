using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Network.OutgoingPackets;
using NLog;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class Boat: Vehicle
{
    protected static readonly Logger LOGGER_BOAT = LogManager.GetLogger(nameof(Boat));

    public Boat(CreatureTemplate template): base(template)
    {
        setInstanceType(InstanceType.Boat);
        setAI(new BoatAI(this));
    }

    public override bool isBoat()
    {
        return true;
    }

    public override int getId()
    {
        return 0;
    }

    public override bool moveToNextRoutePoint()
    {
        bool result = base.moveToNextRoutePoint();
        if (result)
        {
            broadcastPacket(new VehicleDeparturePacket(this));
        }

        return result;
    }

    public override void oustPlayer(Player player)
    {
        base.oustPlayer(player);

        Location loc = getOustLoc();
        if (player.isOnline())
        {
            player.teleToLocation(loc.getX(), loc.getY(), loc.getZ());
        }
        else
        {
            player.setXYZInvisible(loc.ToLocation3D()); // disconnects handling
        }
    }

    public override void stopMove(Location loc)
    {
        base.stopMove(loc);

        broadcastPacket(new VehicleStartedPacket(getObjectId(), 0));
        broadcastPacket(new VehicleInfoPacket(this));
    }

    public override void sendInfo(Player player)
    {
        player.sendPacket(new VehicleInfoPacket(this));
    }
}
