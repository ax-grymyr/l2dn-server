using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author Gabriel Costa Souza
 */
public class OnPlayerLoad: IBaseEvent
{
	private readonly Player _player;
	
	public OnPlayerLoad(Player player)
	{
		_player = player;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_LOAD;
	}
}