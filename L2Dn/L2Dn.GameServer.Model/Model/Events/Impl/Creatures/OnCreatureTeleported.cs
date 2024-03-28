using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures;

/**
 * @author UnAfraid
 */
public class OnCreatureTeleported: EventBase
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
}