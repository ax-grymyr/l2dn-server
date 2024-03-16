using L2Dn.Events;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;

namespace L2Dn.GameServer.Model.Events.Impl.Traps;

/**
 * @author UnAfraid
 */
public class OnTrapAction: EventBase
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
}