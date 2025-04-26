using System.Collections.Frozen;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Condition which checks if you are within the given range of a summoned by you npc.
 * @author Nik
 */
public sealed class ConditionPlayerRangeFromSummonedNpc(FrozenSet<int> npcIds, int radius, bool value): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        bool existNpc = false;
        if (npcIds.Count != 0 && radius > 0)
        {
            foreach (Npc target in World.getInstance().getVisibleObjectsInRange<Npc>(effector, radius))
            {
                if (npcIds.Contains(target.Id) && effector == target.getSummoner())
                {
                    existNpc = true;
                    break;
                }
            }
        }

        return existNpc == value;
    }
}