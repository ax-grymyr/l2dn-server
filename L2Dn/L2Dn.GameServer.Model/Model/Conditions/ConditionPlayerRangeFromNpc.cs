using System.Collections.Frozen;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Exist NPC condition.
 * @author UnAfraid, Zoey76
 */
public class ConditionPlayerRangeFromNpc(FrozenSet<int> npcIds, int radius, bool value): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        bool existNpc = false;
        if (npcIds.Count != 0 && radius > 0)
        {
            foreach (Npc target in World.getInstance().getVisibleObjectsInRange<Npc>(effector, radius))
            {
                if (npcIds.Contains(target.Id))
                {
                    existNpc = true;
                    break;
                }
            }
        }

        return existNpc == value;
    }
}