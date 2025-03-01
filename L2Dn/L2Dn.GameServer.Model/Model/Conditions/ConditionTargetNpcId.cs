using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetNpcId.
 */
public sealed class ConditionTargetNpcId(Set<int> npcIds): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        if (effected != null && (effected.isNpc() || effected.isDoor()))
            return npcIds.Contains(effected.getId());

        return false;
    }
}