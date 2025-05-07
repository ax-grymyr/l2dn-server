using L2Dn.GameServer.Model.Actor;
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

        conditions.Add(condition);
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