using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures;

/**
 * An instantly executed event when Creature attack miss Creature.
 * @author Zealar
 */
public class OnCreatureAttackAvoid: IBaseEvent
{
	private Creature _attacker;
	private Creature _target;
	private bool _damageOverTime;
	
	public OnCreatureAttackAvoid()
	{
	}
	
	public Creature getAttacker()
	{
		return _attacker;
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void setAttacker(Creature attacker)
	{
		_attacker = attacker;
	}
	
	public Creature getTarget()
	{
		return _target;
	}
	
	public void setTarget(Creature target)
	{
		_target = target;
	}
	
	public bool isDamageOverTime()
	{
		return _damageOverTime;
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void setDamageOverTime(bool damageOverTime)
	{
		_damageOverTime = damageOverTime;
	}
	
	public EventType getType()
	{
		return EventType.ON_CREATURE_ATTACK_AVOID;
	}
}