using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Ensoul;
using L2Dn.Packets;

namespace L2Dn.GameServer.Utilities;

public static class PacketUtil
{
    public static void WriteItemAugment(this PacketBitWriter writer, ItemInfo? item)
    {
        VariationInstance? augmentation = item?.getAugmentation(); 
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
    
    public static void WriteItemElemental(this PacketBitWriter writer, ItemInfo? item)
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
    
    public static void WriteItemEnsoulOptions(this PacketBitWriter writer, ItemInfo? item)
    {
        if (item != null)
        {
            writer.WriteByte((byte)item.getSoulCrystalOptions().size()); // Size of regular soul crystal options.
            foreach (EnsoulOption option in item.getSoulCrystalOptions())
            {
                writer.WriteInt32(option.getId()); // Regular Soul Crystal Ability ID.
            }

            writer.WriteByte((byte)item.getSoulCrystalSpecialOptions().size()); // Size of special soul crystal options.
            foreach (EnsoulOption option in item.getSoulCrystalSpecialOptions())
            {
                writer.WriteInt32(option.getId()); // Special Soul Crystal Ability ID.
            }
        }
        else
        {
            writer.WriteByte(0); // Size of regular soul crystal options.
            writer.WriteByte(0); // Size of special soul crystal options.
        }
    }
}