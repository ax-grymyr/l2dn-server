using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author JoeAlisson
 */
public class OnPlayerElementalSpiritUpgrade: EventBase
{
	private readonly ElementalSpirit _spirit;
	private readonly Player _player;
	
	public OnPlayerElementalSpiritUpgrade(Player player, ElementalSpirit spirit)
	{
		_player = player;
		_spirit = spirit;
	}
	
	public ElementalSpirit getSpirit()
	{
		return _spirit;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
}