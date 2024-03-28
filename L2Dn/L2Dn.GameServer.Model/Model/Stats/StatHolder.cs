using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Stats;

/**
 * @author UnAfraid
 */
public class StatHolder
{
	private readonly Stat _stat;
	private readonly double _value;
	private readonly Func<Creature, StatHolder, bool> _condition;
	
	public StatHolder(Stat stat, double value, Func<Creature, StatHolder, bool> condition)
	{
		_stat = stat;
		_value = value;
		_condition = condition;
	}
	
	public StatHolder(Stat stat, double value): this(stat, value, null)
	{
	}
	
	public Stat getStat()
	{
		return _stat;
	}
	
	public Double getValue()
	{
		return _value;
	}
	
	public bool verifyCondition(Creature creature)
	{
		return (_condition == null) || _condition(creature, this);
	}
}