using System.Collections.Frozen;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerServitorNpcId.
 */
public class ConditionPlayerServitorNpcId(FrozenSet<int> npcIds): Condition
{
    private readonly FrozenSet<int> _npcIds =
        npcIds.Count == 1 && npcIds.Contains(0) ? FrozenSet<int>.Empty : npcIds;

    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Player? actingPlayer = effector.getActingPlayer();
        if (actingPlayer is null || !actingPlayer.hasSummon())
            return false;

        if (_npcIds.Count == 0)
            return true;

        foreach (Summon summon in effector.getServitors().Values)
        {
            if (_npcIds.Contains(summon.Id))
                return true;
        }

        return false;
    }
}