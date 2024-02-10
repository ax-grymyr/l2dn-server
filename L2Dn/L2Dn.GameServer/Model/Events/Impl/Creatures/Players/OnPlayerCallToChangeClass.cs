using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author Sdw
 */
public class OnPlayerCallToChangeClass: IBaseEvent
{
	private readonly Player _player;
	
	public OnPlayerCallToChangeClass(Player player)
	{
		_player = player;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_CALL_TO_CHANGE_CLASS;
	}
}