using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures;

/**
 * An instantly executed event when Creature is attacked by Creature.
 * @author UnAfraid
 */
public class OnCreatureDamageReceived: IBaseEvent
{
	private Creature _attacker;
	private Creature _target;
	private double _damage;
	private Skill _skill;
	private bool _crit;
	private bool _damageOverTime;
	private bool _reflect;
	
	public OnCreatureDamageReceived()
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
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void setTarget(Creature target)
	{
		_target = target;
	}
	
	public double getDamage()
	{
		return _damage;
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void setDamage(double damage)
	{
		_damage = damage;
	}
	
	public Skill getSkill()
	{
		return _skill;
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void setSkill(Skill skill)
	{
		_skill = skill;
	}
	
	public bool isCritical()
	{
		return _crit;
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void setCritical(bool crit)
	{
		_crit = crit;
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
	
	public bool isReflect()
	{
		return _reflect;
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void setReflect(bool reflect)
	{
		_reflect = reflect;
	}
	
	public EventType getType()
	{
		return EventType.ON_CREATURE_DAMAGE_RECEIVED;
	}
}