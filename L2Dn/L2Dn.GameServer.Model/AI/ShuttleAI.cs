using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Network.OutgoingPackets.Shuttle;

namespace L2Dn.GameServer.AI;

public class ShuttleAI: VehicleAI
{
	public ShuttleAI(Shuttle shuttle): base(shuttle)
	{
	}

	protected override void moveTo(int x, int y, int z)
	{
		if (!_actor.isMovementDisabled())
		{
			_clientMoving = true;
			_actor.moveToLocation(x, y, z, 0);
			_actor.broadcastPacket(new ExShuttleMovePacket(getActor(), x, y, z));
		}
	}

	public override Shuttle getActor()
	{
		return (Shuttle)_actor;
	}
}