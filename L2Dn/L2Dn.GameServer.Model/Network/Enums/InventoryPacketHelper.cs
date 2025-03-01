﻿using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.BuyList;
using L2Dn.GameServer.Model.Ensoul;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.Enums;

public class InventoryPacketHelper
{
    private readonly List<ItemInfo> _items = new();

    public InventoryPacketHelper()
    {
    }

    public List<ItemInfo> Items => _items;

    public void WriteItems(PacketBitWriter writer)
    {
        writer.WriteInt32(_items.Count); // 140
        foreach (ItemInfo item in _items)
        {
            writer.WriteInt16((short)item.getChange()); // Update type : 01-add, 02-modify, 03-remove // TODO: make enum
            WriteItem(writer, item);
        }
    }

    public static void WriteItem(PacketBitWriter writer, Item item)
    {
        WriteItem(writer, new ItemInfo(item));
    }

    public static void WriteItem(PacketBitWriter writer, Product product)
    {
        WriteItem(writer, new ItemInfo(product));
    }

    public static void WriteItem(PacketBitWriter writer, TradeItem item)
    {
        WriteItem(writer, new ItemInfo(item));
    }

    public static void WriteItem(PacketBitWriter writer, TradeItem item, long count)
    {
        WriteItem(writer, new ItemInfo(item), count);
    }

    public static void WriteItem(PacketBitWriter writer, ItemInfo item)
    {
        ItemListType mask = CalculateMask(item);
        writer.WriteInt16((short)mask);
        writer.WriteInt32(item.getObjectId()); // ObjectId
        writer.WriteInt32(item.getItem().getDisplayId()); // ItemId
        writer.WriteByte((byte)(item.getItem().isQuestItem() || item.getEquipped() == 1 ? 0xFF : item.getLocation())); // T1
        writer.WriteInt64(item.getCount()); // Quantity
        writer.WriteByte((byte)item.getItem().getType2()); // Item Type 2 : 00-weapon, 01-shield/armor, 02-ring/earring/necklace, 03-questitem, 04-adena, 05-item
        writer.WriteByte((byte)item.getCustomType1()); // Filler (always 0)
        writer.WriteInt16((short)item.getEquipped()); // Equipped : 00-No, 01-yes
        writer.WriteInt64(item.getItem().getBodyPart()); // Slot : 0006-lr.ear, 0008-neck, 0030-lr.finger, 0040-head, 0100-l.hand, 0200-gloves, 0400-chest, 0800-pants, 1000-feet, 4000-r.hand, 8000-r.hand
        writer.WriteInt16((short)item.getEnchantLevel()); // Enchant level (pet level shown in control item)
        writer.WriteInt32(item.getMana() ?? -1);
        writer.WriteByte(0); // 270 protocol
        writer.WriteInt32((int?)item.getTime()?.TotalSeconds ?? -9999);
        writer.WriteByte(item.isAvailable()); // GOD Item enabled = 1 disabled (red) = 0
        writer.WriteInt16(0); // 140 - locked

        if (mask.HasFlag(ItemListType.AUGMENT_BONUS))
            WriteItemAugment(writer, item);

        if (mask.HasFlag(ItemListType.ELEMENTAL_ATTRIBUTE))
            WriteItemElemental(writer, item);

        // 362 - Removed
        // if (containsMask(mask, ItemListType.ENCHANT_EFFECT))
        // {
        // writeItemEnchantEffect(item);
        // }
        if (mask.HasFlag(ItemListType.VISUAL_ID))
            writer.WriteInt32(item.getVisualId()); // Item remodel visual ID

        if (mask.HasFlag(ItemListType.SOUL_CRYSTAL))
            WriteItemEnsoulOptions(writer, item);

        // TODO:
        // if (containsMask(mask, ItemListType.REUSE_DELAY))
        // {
        // final Player owner = item.getOwner();
        // writer.WriteInt32(owner == null ? 0 : (int) (owner.getItemRemainingReuseTime(item.getObjectId()) / 1000));
        // }

        if (mask.HasFlag(ItemListType.BLESSED))
            writer.WriteByte(1);
    }

    public static void WriteItem(PacketBitWriter writer, ItemInfo item, long count)
    {
        ItemListType mask = CalculateMask(item);
        writer.WriteInt16((short)mask);
        writer.WriteInt32(item.getObjectId()); // ObjectId
        writer.WriteInt32(item.getItem().getDisplayId()); // ItemId
        writer.WriteByte((byte)(item.getItem().isQuestItem() || item.getEquipped() == 1 ? 0xFF : item.getLocation())); // T1
        writer.WriteInt64(count); // Quantity
        writer.WriteByte((byte)item.getItem().getType2()); // Item Type 2 : 00-weapon, 01-shield/armor, 02-ring/earring/necklace, 03-questitem, 04-adena, 05-item
        writer.WriteByte((byte)item.getCustomType1()); // Filler (always 0)
        writer.WriteInt16((short)item.getEquipped()); // Equipped : 00-No, 01-yes
        writer.WriteInt64(item.getItem().getBodyPart()); // Slot : 0006-lr.ear, 0008-neck, 0030-lr.finger, 0040-head, 0100-l.hand, 0200-gloves, 0400-chest, 0800-pants, 1000-feet, 4000-r.hand, 8000-r.hand
        writer.WriteInt16((short)item.getEnchantLevel()); // Enchant level (pet level shown in control item)
        writer.WriteInt32(item.getMana() ?? -1);
        writer.WriteByte(0); // 270 protocol
        writer.WriteInt32((int?)item.getTime()?.TotalSeconds ?? -9999);
        writer.WriteByte(item.isAvailable()); // GOD Item enabled = 1 disabled (red) = 0
        writer.WriteInt16(0); // 140 - locked
        if (mask.HasFlag(ItemListType.AUGMENT_BONUS))
        {
            WriteItemAugment(writer, item);
        }
        if (mask.HasFlag(ItemListType.ELEMENTAL_ATTRIBUTE))
        {
            WriteItemElemental(writer, item);
        }
        // 362 - Removed
        // if (containsMask(mask, ItemListType.ENCHANT_EFFECT))
        // {
        // writeItemEnchantEffect(item);
        // }
        if (mask.HasFlag(ItemListType.VISUAL_ID))
        {
            writer.WriteInt32(item.getVisualId()); // Item remodel visual ID
        }
        if (mask.HasFlag(ItemListType.SOUL_CRYSTAL))
        {
            WriteItemEnsoulOptions(writer, item);
        }
        // TODO:
        // if (containsMask(mask, ItemListType.REUSE_DELAY))
        // {
        // final Player owner = item.getOwner();
        // writer.WriteInt32(owner == null ? 0 : (int) (owner.getItemRemainingReuseTime(item.getObjectId()) / 1000));
        // }
        if (mask.HasFlag(ItemListType.BLESSED))
        {
            writer.WriteByte(1);
        }
    }

    public static void WriteInventoryBlock(PacketBitWriter writer, PlayerInventory inventory)
    {
        if (inventory.hasInventoryBlock() && inventory.getBlockItems() is {} blockItems)
        {
            writer.WriteInt16((short)blockItems.Count);
            writer.WriteByte((byte)inventory.getBlockMode());
            foreach (int id in blockItems)
            {
                writer.WriteInt32(id);
            }
        }
        else
        {
            writer.WriteInt16(0);
        }
    }

    private static void WriteItemAugment(PacketBitWriter writer, ItemInfo item)
    {
        VariationInstance? augmentation = item.getAugmentation();
        if (augmentation != null)
        {
            writer.WriteInt32(augmentation.getOption1Id());
            writer.WriteInt32(augmentation.getOption2Id());
        }
        else
        {
            writer.WriteInt32(0);
            writer.WriteInt32(0);
        }
    }

    private static void WriteItemElemental(PacketBitWriter writer, ItemInfo item)
    {
        if (item != null)
        {
            writer.WriteInt16((short)item.getAttackElementType());
            writer.WriteInt16((short)item.getAttackElementPower());
            writer.WriteInt16((short)item.getAttributeDefence(AttributeType.FIRE));
            writer.WriteInt16((short)item.getAttributeDefence(AttributeType.WATER));
            writer.WriteInt16((short)item.getAttributeDefence(AttributeType.WIND));
            writer.WriteInt16((short)item.getAttributeDefence(AttributeType.EARTH));
            writer.WriteInt16((short)item.getAttributeDefence(AttributeType.HOLY));
            writer.WriteInt16((short)item.getAttributeDefence(AttributeType.DARK));
        }
        else
        {
            writer.WriteInt16(0);
            writer.WriteInt16(0);
            writer.WriteInt16(0);
            writer.WriteInt16(0);
            writer.WriteInt16(0);
            writer.WriteInt16(0);
            writer.WriteInt16(0);
            writer.WriteInt16(0);
        }
    }

    private static void WriteItemEnchantEffect(PacketBitWriter writer, ItemInfo item)
    {
        // Enchant Effects
        foreach (int op in item.getEnchantOptions())
            writer.WriteInt32(op);
    }

    private static void WriteItemEnsoulOptions(PacketBitWriter writer, ItemInfo item)
    {
        if (item != null)
        {
            writer.WriteByte((byte)item.getSoulCrystalOptions().Count); // Size of regular soul crystal options.
            foreach (EnsoulOption option in item.getSoulCrystalOptions())
                writer.WriteInt32(option.getId()); // Regular Soul Crystal Ability ID.

            writer.WriteByte((byte)item.getSoulCrystalSpecialOptions().Count); // Size of special soul crystal options.
            foreach (EnsoulOption option in item.getSoulCrystalSpecialOptions())
                writer.WriteInt32(option.getId()); // Special Soul Crystal Ability ID.
        }
        else
        {
            writer.WriteByte(0); // Size of regular soul crystal options.
            writer.WriteByte(0); // Size of special soul crystal options.
        }
    }

    private static ItemListType CalculateMask(ItemInfo item)
    {
        ItemListType mask = 0;
        if (item.getAugmentation() != null)
        {
            mask |= ItemListType.AUGMENT_BONUS;
        }

        if (item.getAttackElementType() >= 0 || item.getAttributeDefence(AttributeType.FIRE) > 0 ||
            item.getAttributeDefence(AttributeType.WATER) > 0 || item.getAttributeDefence(AttributeType.WIND) > 0 ||
            item.getAttributeDefence(AttributeType.EARTH) > 0 || item.getAttributeDefence(AttributeType.HOLY) > 0 ||
            item.getAttributeDefence(AttributeType.DARK) > 0)
        {
            mask |= ItemListType.ELEMENTAL_ATTRIBUTE;
        }

        // 362 - Removed
        // if (item.getEnchantOptions() != null)
        // {
        // for (int id : item.getEnchantOptions())
        // {
        // if (id > 0)
        // {
        // mask |= ItemListType.ENCHANT_EFFECT.getMask();
        // break;
        // }
        // }
        // }

        if (item.getVisualId() > 0)
        {
            mask |= ItemListType.VISUAL_ID;
        }

        if ((item.getSoulCrystalOptions() != null && item.getSoulCrystalOptions().Count != 0) ||
            (item.getSoulCrystalSpecialOptions() != null && item.getSoulCrystalSpecialOptions().Count != 0))
        {
            mask |= ItemListType.SOUL_CRYSTAL;
        }

        // TODO:
        // if (item.getReuseDelay() > 0)
        // {
        // mask |= ItemListType.REUSE_DELAY.getMask();
        // }

        if (item.isBlessed())
        {
            mask |= ItemListType.BLESSED;
        }

        return mask;
    }

	public static int CalculatePacketSize(ItemInfo item)
	{
		ItemListType mask = CalculateMask(item);
		int size = 0;
		size += 2; // writeShort(mask);
		size += 4; // writer.WriteInt32(item.getObjectId()); // ObjectId
		size += 4; // writer.WriteInt32(item.getItem().getDisplayId()); // ItemId
		size += 1; // writeByte(item.getItem().isQuestItem() || (item.getEquipped() == 1) ? 0xFF : item.getLocation()); // T1
		size += 8; // writeLong(item.getCount()); // Quantity
		size += 1; // writeByte(item.getItem().getType2()); // Item Type 2 : 00-weapon, 01-shield/armor, 02-ring/earring/necklace, 03-questitem, 04-adena, 05-item
		size += 1; // writeByte(item.getCustomType1()); // Filler (always 0)
		size += 2; // writeShort(item.getEquipped()); // Equipped : 00-No, 01-yes
		size += 8; // writeLong(item.getItem().getBodyPart()); // Slot : 0006-lr.ear, 0008-neck, 0030-lr.finger, 0040-head, 0100-l.hand, 0200-gloves, 0400-chest, 0800-pants, 1000-feet, 4000-r.hand, 8000-r.hand
		size += 2; // writeShort(item.getEnchantLevel()); // Enchant level (pet level shown in control item)
		size += 4; // writer.WriteInt32(item.getMana());
		size += 1; // writeByte(0); // 270 protocol
		size += 4; // writer.WriteInt32(item.getTime());
		size += 1; // writeByte(item.isAvailable()); // GOD Item enabled = 1 disabled (red) = 0
		size += 2; // writeShort(0); // 140 - locked

		if (mask.HasFlag(ItemListType.AUGMENT_BONUS))
		{
			size += 8;
		}
		if (mask.HasFlag(ItemListType.ELEMENTAL_ATTRIBUTE))
		{
			size += 16;
		}
		// 362 - Removed
		// if (containsMask(mask, ItemListType.ENCHANT_EFFECT))
		// {
		// size += (item.getEnchantOptions().length * 4);
		// }
		if (mask.HasFlag(ItemListType.VISUAL_ID))
		{
			size += 4;
		}
		if (mask.HasFlag(ItemListType.SOUL_CRYSTAL))
		{
			size += 1;
			size += item.getSoulCrystalOptions().Count * 4;
			size += 1;
			size += item.getSoulCrystalSpecialOptions().Count * 4;
		}
		// TODO:
		// if (containsMask(mask, ItemListType.REUSE_DELAY))
		// {
		// size += 4;
		// }
		if (mask.HasFlag(ItemListType.BLESSED))
		{
			size += 1;
		}

		return size;
	}
}