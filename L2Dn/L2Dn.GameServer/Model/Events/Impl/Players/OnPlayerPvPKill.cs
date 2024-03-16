using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerPvPKill: EventBase
{
	private readonly Player _player;
	private readonly Player _target;
	
	public OnPlayerPvPKill(Player player, Player target)
	{
		_player = player;
		_target = target;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public Player getTarget()
	{
		return _target;
	}
}