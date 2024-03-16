using System.Runtime.CompilerServices;
using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures;

/**
 * An instantly executed event when Creature attack miss Creature.
 * @author Zealar
 */
public class OnCreatureAttackAvoid: EventBase
{
	private Creature _attacker;
	private Creature _target;
	private bool _damageOverTime;
	
	public Creature getAttacker()
	{
		return _attacker;
	}

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
	
	public void setDamageOverTime(bool damageOverTime)
	{
		_damageOverTime = damageOverTime;
	}
}