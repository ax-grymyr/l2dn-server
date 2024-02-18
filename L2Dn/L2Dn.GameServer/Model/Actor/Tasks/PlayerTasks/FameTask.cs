using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;

/**
 * Task dedicated to reward player with fame while standing on siege zone.
 * @author UnAfraid
 */
public class FameTask: Runnable
{
	private readonly Player _player;
	private readonly int _value;

	public FameTask(Player player, int value)
	{
		_player = player;
		_value = value;
	}

	public void run()
	{
		if ((_player == null) || (_player.isDead() && !Config.FAME_FOR_DEAD_PLAYERS))
		{
			return;
		}

		if (((_player.getClient() == null) || _player.getClient().IsDetached) && !Config.OFFLINE_FAME)
		{
			return;
		}

		_player.setFame(_player.getFame() + _value);
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.PERSONAL_REPUTATION_S1);
		sm.Params.addInt(_value);
		_player.sendPacket(sm);
		_player.updateUserInfo();
	}
}