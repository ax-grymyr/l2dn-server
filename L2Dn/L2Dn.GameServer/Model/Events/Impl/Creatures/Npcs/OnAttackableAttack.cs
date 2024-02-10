using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Npcs;

/**
 * An instantly executed event when Attackable is attacked by Player.
 * @author UnAfraid
 */
public class OnAttackableAttack: IBaseEvent
{
	private readonly Player _attacker;
	private readonly Attackable _target;
	private readonly int _damage;
	private readonly Skill _skill;
	private readonly bool _isSummon;
	
	public OnAttackableAttack(Player attacker, Attackable target, int damage, Skill skill, bool isSummon)
	{
		_attacker = attacker;
		_target = target;
		_damage = damage;
		_skill = skill;
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
	
	public int getDamage()
	{
		return _damage;
	}
	
	public Skill getSkill()
	{
		return _skill;
	}
	
	public bool isSummon()
	{
		return _isSummon;
	}
	
	public EventType getType()
	{
		return EventType.ON_ATTACKABLE_ATTACK;
	}
}