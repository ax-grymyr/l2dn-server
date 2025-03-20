using System.Collections.Immutable;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerServitorNpcId.
 */
public class ConditionPlayerServitorNpcId(List<int> npcIds): Condition
{
    private readonly ImmutableArray<int> _npcIds = npcIds is [0] ? default : npcIds.ToImmutableArray();

    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Player? actingPlayer = effector.getActingPlayer();
        if (actingPlayer is null || !actingPlayer.hasSummon())
            return false;

        if (_npcIds.IsDefaultOrEmpty)
            return true;

        foreach (Summon summon in effector.getServitors().Values)
        {
            if (_npcIds.Contains(summon.Id))
                return true;
        }

        return false;
    }
}