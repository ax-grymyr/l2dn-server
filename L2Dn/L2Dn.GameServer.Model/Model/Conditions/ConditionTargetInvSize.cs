using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetInvSize.
 * @author Zoey76
 */
public sealed class ConditionTargetInvSize(int size): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        if (effected is null)
            return false;

        Player? target = effected.getActingPlayer();
        if (effected.isPlayer() && target != null)
            return target.getInventory().getNonQuestSize() <= target.getInventoryLimit() - size;

        return false;
    }
}