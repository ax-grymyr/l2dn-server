using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.OutgoingPackets.Sayune;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;

/**
 * @author UnAfraid
 */
public class FlyMoveStartTask: Runnable
{
	private readonly Player _player;
	private readonly Zone _zone;

	public FlyMoveStartTask(Zone zone, Player player)
	{
		ArgumentNullException.ThrowIfNull(zone);
		ArgumentNullException.ThrowIfNull(player);
		_player = player;
		_zone = zone;
	}

	public void run()
	{
		if (!_zone.isCharacterInZone(_player))
		{
			return;
		}

		if (!_player.hasRequest<SayuneRequest>())
		{
			_player.sendPacket(default(ExNotifyFlyMoveStartPacket));
			ThreadPool.schedule(this, 1000);
		}
	}
}