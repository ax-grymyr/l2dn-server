using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author JoeAlisson
 */
public class OnElementalSpiritLearn: IBaseEvent
{
	private readonly Player _player;
	
	public OnElementalSpiritLearn(Player player)
	{
		_player = player;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public EventType getType()
	{
		return EventType.ON_ELEMENTAL_SPIRIT_LEARN;
	}
}