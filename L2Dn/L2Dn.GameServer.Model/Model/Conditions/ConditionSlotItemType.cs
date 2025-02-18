using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionSlotItemType.
 * @author mkizub
 */
public sealed class ConditionSlotItemType(int slot, ItemTypeMask mask): ConditionInventory(slot)
{
    protected override bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item)
    {
        if (effector == null || !effector.isPlayer())
            return false;

        Item? itemSlot = effector.getInventory().getPaperdollItem(Slot);
        if (itemSlot == null)
            return false;

        return (itemSlot.getTemplate().getItemMask() & mask) != ItemTypeMask.Zero;
    }
}