using System.Collections.Frozen;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerHasPet.
 */
public sealed class ConditionPlayerHasPet(FrozenSet<int> itemIds): Condition
{
    private readonly FrozenSet<int> _controlItemIds =
        itemIds.Count == 1 && itemIds.Contains(0) ? FrozenSet<int>.Empty : itemIds;

    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Summon? pet = effector.getActingPlayer()?.getPet();
        if (pet is null)
            return false;

        if (_controlItemIds.Count == 0)
            return true;

        Item? controlItem = ((Pet)pet).getControlItem();
        return controlItem != null && _controlItemIds.Contains(controlItem.Id);
    }
}