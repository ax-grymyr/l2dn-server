using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Network.OutgoingPackets.Shuttle;
using L2Dn.Geometry;

namespace L2Dn.GameServer.AI;

public class ShuttleAI: VehicleAI
{
	public ShuttleAI(Shuttle shuttle): base(shuttle)
	{
	}

	public override void moveTo(Location3D location)
	{
		if (!_actor.isMovementDisabled())
		{
			_clientMoving = true;
			_actor.moveToLocation(location, 0);
			_actor.broadcastPacket(new ExShuttleMovePacket(getActor(), location));
		}
	}

	public override Shuttle getActor()
	{
		return (Shuttle)_actor;
	}
}