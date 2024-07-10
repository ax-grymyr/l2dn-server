using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionLogicOr.
 * @author mkizub
 */
public class ConditionLogicOr: Condition
{
	public readonly List<Condition> conditions = new();
	
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
		
		conditions.Add(condition);
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