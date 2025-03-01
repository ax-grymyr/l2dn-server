using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.TaskManagers;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionGameTime.
 * @author mkizub
 */
public sealed class ConditionGameTime(bool night): Condition
{
    /**
     * Test impl.
     * @return true, if successful
     */
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        return GameTimeTaskManager.getInstance().isNight() == night;
    }
}