using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author Sdw
 */
public class OnPlayerChangeToAwakenedClass: IBaseEvent
{
	private readonly Player _player;
	
	public OnPlayerChangeToAwakenedClass(Player player)
	{
		_player = player;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_CHANGE_TO_AWAKENED_CLASS;
	}
}