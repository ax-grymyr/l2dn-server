﻿using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Geometry;

namespace L2Dn.GameServer.AI;

public class AirShipAI: VehicleAI
{
    public AirShipAI(AirShip airShip): base(airShip)
    {
    }

    public override void moveTo(Location3D location)
    {
        if (!_actor.isMovementDisabled())
        {
            _clientMoving = true;
            _actor.moveToLocation(location, 0);
            _actor.broadcastPacket(new ExMoveToLocationAirShipPacket(getActor()));
        }
    }

    public override void clientStopMoving(Location? loc)
    {
        if (_actor.isMoving())
        {
            _actor.stopMove(loc);
        }

        if (_clientMoving || loc != null)
        {
            _clientMoving = false;
            _actor.broadcastPacket(new ExStopMoveAirShipPacket(getActor()));
        }
    }

    public override void describeStateToPlayer(Player player)
    {
        if (_clientMoving)
        {
            player.sendPacket(new ExMoveToLocationAirShipPacket(getActor()));
        }
    }

    public override AirShip getActor()
    {
        return (AirShip)_actor;
    }
}