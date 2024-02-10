using L2Dn.GameServer.Utilities;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;

/**
 * @author UnAfraid
 */
public class FlyMoveStartTask: Runnable
{
	private readonly Player _player;
	private readonly ZoneType _zone;

	public FlyMoveStartTask(ZoneType zone, Player player)
	{
		Objects.requireNonNull(zone);
		Objects.requireNonNull(player);
		_player = player;
		_zone = zone;
	}

	public override void run()
	{
		if (!_zone.isCharacterInZone(_player))
		{
			return;
		}

		if (!_player.hasRequest<SayuneRequest>())
		{
			_player.sendPacket(ExNotifyFlyMoveStart.STATIC_PACKET);
			ThreadPool.schedule(this, 1000);
		}
	}
}