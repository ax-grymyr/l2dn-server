using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Condition which checks if you are within the given range of a summoned by you npc.
 * @author Nik
 */
public sealed class ConditionPlayerRangeFromSummonedNpc(Set<int> npcIds, int radius, bool value): Condition
{
    protected override bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item)
    {
        bool existNpc = false;
        if (!npcIds.isEmpty() && radius > 0)
        {
            foreach (Npc target in World.getInstance().getVisibleObjectsInRange<Npc>(effector, radius))
            {
                if (npcIds.Contains(target.getId()) && effector == target.getSummoner())
                {
                    existNpc = true;
                    break;
                }
            }
        }

        return existNpc == value;
    }
}