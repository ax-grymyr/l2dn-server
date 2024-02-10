using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures;

/**
 * @author JoeAlisson
 */
public class OnElementalSpiritUpgrade: IBaseEvent
{
	private readonly ElementalSpirit _spirit;
	private readonly Player _player;
	
	public OnElementalSpiritUpgrade(Player player, ElementalSpirit spirit)
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
	
	public EventType getType()
	{
		return EventType.ON_ELEMENTAL_SPIRIT_UPGRADE;
	}
}