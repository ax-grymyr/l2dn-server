using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerMoveRequest: IBaseEvent
{
	private readonly Player _player;
	private readonly Location _location;
	
	public OnPlayerMoveRequest(Player player, Location loc)
	{
		_player = player;
		_location = loc;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public Location getLocation()
	{
		return _location;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_MOVE_REQUEST;
	}
}