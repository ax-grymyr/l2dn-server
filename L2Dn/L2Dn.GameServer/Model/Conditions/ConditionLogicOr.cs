using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionLogicOr.
 * @author mkizub
 */
public class ConditionLogicOr: Condition
{
	private static Condition[] _emptyConditions = new Condition[0];
	public Condition[] conditions = _emptyConditions;
	
	/**
	 * Adds the.
	 * @param condition the condition
	 */
	public void add(Condition condition)
	{
		if (condition == null)
		{
			return;
		}
		if (getListener() != null)
		{
			condition.setListener(this);
		}
		int len = conditions.Length;
		Condition[] tmp = new Condition[len + 1];
		System.arraycopy(conditions, 0, tmp, 0, len);
		tmp[len] = condition;
		conditions = tmp;
	}
	
	public override void setListener(ConditionListener listener)
	{
		if (listener != null)
		{
			foreach (Condition c in conditions)
			{
				c.setListener(this);
			}
		}
		else
		{
			foreach (Condition c in conditions)
			{
				c.setListener(null);
			}
		}
		
		base.setListener(listener);
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		foreach (Condition c in conditions)
		{
			if (c.test(effector, effected, skill, item))
			{
				return true;
			}
		}
		return false;
	}
}
