using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionItemId.
 * @author mkizub
 */
public sealed class ConditionItemId(int itemId): Condition
{
    /**
     * Test impl.
     * @return true, if successful
     */
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        return item != null && item.Id == itemId;
    }
}