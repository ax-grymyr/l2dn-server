using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures;

/**
 * An instantly executed event when Creature is attacked by Creature.
 * @author UnAfraid
 */
public class OnCreatureAttacked: IBaseEvent
{
	private Creature _attacker;
	private Creature _target;
	private Skill _skill;
	
	public OnCreatureAttacked()
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
	
	public Skill getSkill()
	{
		return _skill;
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void setSkill(Skill skill)
	{
		_skill = skill;
	}
	
	public EventType getType()
	{
		return EventType.ON_CREATURE_ATTACKED;
	}
}