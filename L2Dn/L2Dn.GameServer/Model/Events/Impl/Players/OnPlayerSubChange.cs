using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author malyelfik
 */
public class OnPlayerSubChange: EventBase
{
	private readonly Player _player;
	
	public OnPlayerSubChange(Player player)
	{
		_player = player;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
}