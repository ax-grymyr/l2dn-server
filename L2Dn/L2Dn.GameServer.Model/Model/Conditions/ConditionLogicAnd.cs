using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionLogicAnd.
 * @author mkizub
 */
public class ConditionLogicAnd: Condition
{
    public readonly List<Condition> conditions = [];

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

    public override void setListener(ConditionListener? listener)
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

    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        foreach (Condition c in conditions)
        {
            if (!c.test(effector, effected, skill, item))
                return false;
        }

        return true;
    }
}