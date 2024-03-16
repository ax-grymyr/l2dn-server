using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author Gabriel Costa Souza
 */
public class OnPlayerLoad: EventBase
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
}