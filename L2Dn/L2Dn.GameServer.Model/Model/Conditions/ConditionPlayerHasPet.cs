using System.Collections.Immutable;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerHasPet.
 */
public sealed class ConditionPlayerHasPet(List<int> itemIds): Condition
{
    private readonly ImmutableArray<int> _controlItemIds = itemIds is [0] ? default : itemIds.ToImmutableArray();

    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Summon? pet = effector.getActingPlayer()?.getPet();
        if (pet is null)
            return false;

        if (_controlItemIds.IsDefaultOrEmpty)
            return true;

        Item? controlItem = ((Pet)pet).getControlItem();
        return controlItem != null && _controlItemIds.Contains(controlItem.Id);
    }
}