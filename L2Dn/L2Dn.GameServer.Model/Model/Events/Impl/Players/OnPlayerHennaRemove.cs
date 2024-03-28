using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Henna;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerHennaRemove: EventBase
{
	private readonly Player _player;
	private readonly Henna _henna;
	
	public OnPlayerHennaRemove(Player player, Henna henna)
	{
		_player = player;
		_henna = henna;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public Henna getHenna()
	{
		return _henna;
	}
}