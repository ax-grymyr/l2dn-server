using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Conditions;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Stats.Functions;

/**
 * Returns the initial value divided the function value, if the condition are met.
 * @author Zoey76
 */
public class FuncDiv: AbstractFunction
{
	public FuncDiv(Stat stat, int order, Object owner, double value, Condition applayCond)
		: base(stat, order, owner, value, applayCond)
	{
	}

	public override double calc(Creature effector, Creature effected, Skill skill, double initVal)
	{
		if ((getApplayCond() == null) || getApplayCond().test(effector, effected, skill))
		{
			try
			{
				return initVal / getValue();
			}
			catch (Exception e)
			{
				LOG.Warn(nameof(FuncDiv) + ": Division by zero: " + getValue() + "!");
			}
		}

		return initVal;
	}
}