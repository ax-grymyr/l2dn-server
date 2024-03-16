using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author UnAfraid
 */
public class OnTrapAction: IBaseEvent
{
	private readonly Trap _trap;
	private readonly Creature _trigger;
	private readonly TrapAction _action;
	
	public OnTrapAction(Trap trap, Creature trigger, TrapAction action)
	{
		_trap = trap;
		_trigger = trigger;
		_action = action;
	}
	
	public Trap getTrap()
	{
		return _trap;
	}
	
	public Creature getTrigger()
	{
		return _trigger;
	}
	
	public TrapAction getAction()
	{
		return _action;
	}
	
	public EventType getType()
	{
		return EventType.ON_TRAP_ACTION;
	}
}