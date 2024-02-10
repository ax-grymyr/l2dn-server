using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerPvPKill: IBaseEvent
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
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_PVP_KILL;
	}
}