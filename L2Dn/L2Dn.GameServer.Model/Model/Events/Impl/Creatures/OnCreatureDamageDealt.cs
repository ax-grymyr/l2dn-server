using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures;

/**
 * An instantly executed event when Creature is attacked by Creature.
 * @author UnAfraid
 */
public class OnCreatureDamageDealt: EventBase
{
	private Creature _attacker;
	private Creature _target;
	private double _damage;
	private Skill? _skill;
	private bool _crit;
	private bool _damageOverTime;
	private bool _reflect;

    public OnCreatureDamageDealt(Creature attacker, Creature target, Skill? skill, double damage, bool crit,
        bool damageOverTime, bool reflect)
    {
        _attacker = attacker;
        _target = target;
        _skill = skill;
        _damage = damage;
        _crit = crit;
        _damageOverTime = damageOverTime;
        _reflect = reflect;
    }

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

	public double getDamage()
	{
		return _damage;
	}

	public void setDamage(double damage)
	{
		_damage = damage;
	}

	public Skill? getSkill()
	{
		return _skill;
	}

	public void setSkill(Skill? skill)
	{
		_skill = skill;
	}

	public bool isCritical()
	{
		return _crit;
	}

	public void setCritical(bool crit)
	{
		_crit = crit;
	}

	public bool isDamageOverTime()
	{
		return _damageOverTime;
	}

	public void setDamageOverTime(bool damageOverTime)
	{
		_damageOverTime = damageOverTime;
	}

	public bool isReflect()
	{
		return _reflect;
	}

	public void setReflect(bool reflect)
	{
		_reflect = reflect;
	}
}