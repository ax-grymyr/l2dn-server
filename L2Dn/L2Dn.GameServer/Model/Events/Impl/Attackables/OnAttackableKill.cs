using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Npcs;

/**
 * An instantly executed event when Attackable is killed by Player.
 * @author UnAfraid
 */
public class OnAttackableKill: IBaseEvent
{
	private readonly Player _attacker;
	private readonly Attackable _target;
	private readonly bool _isSummon;
	
	public OnAttackableKill(Player attacker, Attackable target, bool isSummon)
	{
		_attacker = attacker;
		_target = target;
		_isSummon = isSummon;
	}
	
	public Player getAttacker()
	{
		return _attacker;
	}
	
	public Attackable getTarget()
	{
		return _target;
	}
	
	public bool isSummon()
	{
		return _isSummon;
	}
	
	public EventType getType()
	{
		return EventType.ON_ATTACKABLE_KILL;
	}
}