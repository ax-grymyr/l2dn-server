using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author JoeAlisson
 */
public class OnPlayerElementalSpiritLearn: EventBase
{
	private readonly Player _player;
	
	public OnPlayerElementalSpiritLearn(Player player)
	{
		_player = player;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
}