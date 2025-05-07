using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionLogicNot.
 * @author mkizub
 */
public class ConditionLogicNot: Condition
{
    private readonly Condition _condition;

    /**
     * Instantiates a new condition logic not.
     * @param condition the condition
     */
    public ConditionLogicNot(Condition condition)
    {
        _condition = condition;
    }

    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        return !_condition.test(effector, effected, skill, item);
    }
}