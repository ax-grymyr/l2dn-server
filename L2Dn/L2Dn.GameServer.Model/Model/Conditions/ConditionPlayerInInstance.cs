using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Sdw
 */
public sealed class ConditionPlayerInInstance(bool inInstance): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        if (effector.getActingPlayer() == null)
            return false;

        return effector.getInstanceId() == 0 ? !inInstance : inInstance;
    }
}