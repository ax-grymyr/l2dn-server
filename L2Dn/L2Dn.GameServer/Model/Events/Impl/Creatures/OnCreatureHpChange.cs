using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures;

/**
 * @author UnAfraid
 */
public class OnCreatureHpChange: IBaseEvent
{
	private readonly Creature _creature;
	private readonly double _newHp;
	private readonly double _oldHp;
	
	public OnCreatureHpChange(Creature creature, double oldHp, double newHp)
	{
		_creature = creature;
		_oldHp = oldHp;
		_newHp = newHp;
	}
	
	public Creature getCreature()
	{
		return _creature;
	}
	
	public double getOldHp()
	{
		return _oldHp;
	}
	
	public double getNewHp()
	{
		return _newHp;
	}
	
	public EventType getType()
	{
		return EventType.ON_CREATURE_HP_CHANGE;
	}
}