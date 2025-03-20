using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionSlotItemId.
 * @author mkizub
 */
public class ConditionSlotItemId(int slot, int itemId, int enchantLevel): ConditionInventory(slot)
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        if (!effector.isPlayer())
            return false;

        Item? itemSlot = effector.getInventory()?.getPaperdollItem(Slot);
        if (itemSlot == null)
            return itemId == 0;

        return itemSlot.Id == itemId && itemSlot.getEnchantLevel() >= enchantLevel;
    }
}