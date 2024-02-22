using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Conditions;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Stats.Functions;

/**
 * Function template.
 * @author mkizub, Zoey76
 */
public class FuncTemplate
{
	private readonly Type _functionClass;
	private readonly Condition _attachCond;
	private readonly Condition _applayCond;
	private readonly Stat _stat;
	private readonly int _order;
	private readonly double _value;

	public FuncTemplate(Condition attachCond, Condition applayCond, String functionName, int order, Stat stat,
		double value)
	{
		StatFunction function = StatFunction.valueOf(functionName.toUpperCase());
		if (order >= 0)
		{
			_order = order;
		}
		else
		{
			_order = function.getOrder();
		}

		_attachCond = attachCond;
		_applayCond = applayCond;
		_stat = stat;
		_value = value;

		try
		{
			_functionClass = Class.forName("org.l2jmobius.gameserver.model.stats.functions.Func" + function.getName());
		}
		catch (ClassNotFoundException e)
		{
			throw new RuntimeException(e);
		}
	}

	public Type getFunctionClass()
	{
		return _functionClass;
	}

	/**
	 * Gets the function stat.
	 * @return the stat.
	 */
	public Stat getStat()
	{
		return _stat;
	}

	/**
	 * Gets the function priority order.
	 * @return the order
	 */
	public int getOrder()
	{
		return _order;
	}

	/**
	 * Gets the function value.
	 * @return the value
	 */
	public double getValue()
	{
		return _value;
	}

	public bool meetCondition(Creature effected, Skill skill)
	{
		if ((_attachCond != null) && !_attachCond.test(effected, effected, skill))
		{
			return false;
		}

		if ((_applayCond != null) && !_applayCond.test(effected, effected, skill))
		{
			return false;
		}

		return true;
	}
}