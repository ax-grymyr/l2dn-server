using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Geometry;

namespace L2Dn.GameServer.AI;

public class BoatAI: VehicleAI
{
    public BoatAI(Boat boat): base(boat)
    {
    }

    public override void moveTo(Location3D location)
    {
        if (!_actor.isMovementDisabled())
        {
            if (!_clientMoving)
            {
                _actor.broadcastPacket(new VehicleStartedPacket(getActor().getObjectId(), 1));
            }

            _clientMoving = true;
            _actor.moveToLocation(location, 0);
            _actor.broadcastPacket(new VehicleDeparturePacket(getActor()));
        }
    }

    public override void clientStopMoving(Location? loc)
    {
        if (_actor.isMoving())
        {
            _actor.stopMove(loc);
        }

        if (_clientMoving || (loc != null))
        {
            _clientMoving = false;
            _actor.broadcastPacket(new VehicleStartedPacket(getActor().getObjectId(), 0));
            _actor.broadcastPacket(new VehicleInfoPacket(getActor()));
        }
    }

    public override void describeStateToPlayer(Player player)
    {
        if (_clientMoving)
        {
            player.sendPacket(new VehicleDeparturePacket(getActor()));
        }
    }

    public override Boat getActor()
    {
        return (Boat)_actor;
    }
}