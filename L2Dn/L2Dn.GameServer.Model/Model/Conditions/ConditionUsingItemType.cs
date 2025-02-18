using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionUsingItemType.
 * @author mkizub
 */
public class ConditionUsingItemType(ItemTypeMask mask): Condition
{
    protected override bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item)
    {
        if (effector == null)
        {
            return false;
        }

        bool isArmor = (mask & ((ItemTypeMask)ArmorType.MAGIC | ArmorType.LIGHT | ArmorType.HEAVY)) !=
            ItemTypeMask.Zero;

        if (!effector.isPlayer())
        {
            return !isArmor && (mask & effector.getAttackType()) != ItemTypeMask.Zero;
        }

        Inventory inv = effector.getInventory();
        // If ConditionUsingItemType is one between Light, Heavy or Magic.
        if (isArmor)
        {
            // Get the itemMask of the weared chest (if exists).
            Item? chest = inv.getPaperdollItem(Inventory.PAPERDOLL_CHEST);
            if (chest == null)
            {
                return (ArmorType.NONE & mask) == ArmorType.NONE;
            }

            // If chest armor is different from the condition one return false.
            ItemTypeMask chestMask = chest.getTemplate().getItemMask();
            if ((mask & chestMask) == ItemTypeMask.Zero)
            {
                return false;
            }

            // So from here, chest armor matches conditions.
            // Return True if chest armor is a Full Armor.
            long chestBodyPart = chest.getTemplate().getBodyPart();
            if (chestBodyPart == ItemTemplate.SLOT_FULL_ARMOR)
            {
                return true;
            }

            // Check legs armor.
            Item? legs = inv.getPaperdollItem(Inventory.PAPERDOLL_LEGS);
            if (legs == null)
            {
                return (ArmorType.NONE & mask) == ArmorType.NONE;
            }

            // Return true if legs armor matches too.
            ItemTypeMask legMask = legs.getTemplate().getItemMask();
            return (mask & legMask) != ItemTypeMask.Zero;
        }

        return (mask & inv.getWearedMask()) != ItemTypeMask.Zero;
    }
}