using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures;

/**
 * @author UnAfraid
 */
public class OnCreatureTeleported: IBaseEvent
{
	private readonly Creature _creature;
	
	public OnCreatureTeleported(Creature creature)
	{
		_creature = creature;
	}
	
	public Creature getCreature()
	{
		return _creature;
	}
	
	public EventType getType()
	{
		return EventType.ON_CREATURE_TELEPORTED;
	}
}