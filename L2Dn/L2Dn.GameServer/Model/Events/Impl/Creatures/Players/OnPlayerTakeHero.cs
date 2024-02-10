using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author Mobius
 */
public class OnPlayerTakeHero: IBaseEvent
{
	private readonly Player _player;
	
	public OnPlayerTakeHero(Player player)
	{
		_player = player;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_TAKE_HERO;
	}
}