using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures;

/**
 * @author UnAfraid
 */
public class OnCreatureSee: EventBase
{
	private readonly Creature _creature;
	private readonly Creature _seen;
	
	public OnCreatureSee(Creature creature, Creature seen)
	{
		_creature = creature;
		_seen = seen;
	}
	
	public Creature getCreature()
	{
		return _creature;
	}
	
	public Creature getSeen()
	{
		return _seen;
	}
}