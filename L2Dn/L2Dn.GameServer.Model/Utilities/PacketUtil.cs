using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Ensoul;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Utilities;

public static class PacketUtil
{
	public static Location3D ReadLocation3D(this ref PacketBitReader reader)
	{
		int x = reader.ReadInt32();
		int y = reader.ReadInt32();
		int z = reader.ReadInt32();
		return new Location3D(x, y, z);
	}

	public static Location ReadLocation(this ref PacketBitReader reader)
	{
		int x = reader.ReadInt32();
		int y = reader.ReadInt32();
		int z = reader.ReadInt32();
		int heading = reader.ReadInt32();
		return new Location(x, y, z, heading);
	}

	public static void WriteLocation3D(this PacketBitWriter writer, Location3D location)
	{
		writer.WriteInt32(location.X);
		writer.WriteInt32(location.Y);
		writer.WriteInt32(location.Z);
	}

	public static void WriteLocation(this PacketBitWriter writer, Location location)
	{
		writer.WriteInt32(location.X);
		writer.WriteInt32(location.Y);
		writer.WriteInt32(location.Z);
		writer.WriteInt32(location.Heading);
	}

	public static void WriteSystemMessageParam(this PacketBitWriter writer, SystemMessageParam param)
	{
		switch (param.Type)
		{
			case SystemMessageParamType.TYPE_ELEMENT_NAME:
			case SystemMessageParamType.TYPE_BYTE:
			case SystemMessageParamType.TYPE_FACTION_NAME:
			case SystemMessageParamType.TYPE_ELEMENTAL_SPIRIT:
			{
				writer.WriteByte((byte)(int)param.Value);
				break;
			}
			case SystemMessageParamType.TYPE_CASTLE_NAME:
			case SystemMessageParamType.TYPE_SYSTEM_STRING:
			case SystemMessageParamType.TYPE_INSTANCE_NAME:
			case SystemMessageParamType.TYPE_CLASS_ID:
			{
				writer.WriteInt16((short)(int)param.Value);
				break;
			}
			case SystemMessageParamType.TYPE_ITEM_NAME:
			case SystemMessageParamType.TYPE_INT_NUMBER:
			case SystemMessageParamType.TYPE_NPC_NAME:
			case SystemMessageParamType.TYPE_DOOR_NAME:
			{
				writer.WriteInt32((int)param.Value);
				break;
			}
			case SystemMessageParamType.TYPE_LONG_NUMBER:
			{
				writer.WriteInt64((long)param.Value);
				break;
			}
			case SystemMessageParamType.TYPE_TEXT:
			case SystemMessageParamType.TYPE_PLAYER_NAME:
			{
				writer.WriteString((string)param.Value);
				break;
			}
			case SystemMessageParamType.TYPE_SKILL_NAME:
			{
				int[] array = (int[])param.Value;
				writer.WriteInt32(array[0]); // skill id
				writer.WriteInt16((short)array[1]); // skill level
				writer.WriteInt16((short)array[2]); // skill sub level
				break;
			}
			case SystemMessageParamType.TYPE_POPUP_ID:
			case SystemMessageParamType.TYPE_ZONE_NAME:
			{
				int[] array = (int[])param.Value;
				writer.WriteInt32(array[0]); // x
				writer.WriteInt32(array[1]); // y
				writer.WriteInt32(array[2]); // z
				break;
			}
			default:
				throw new InvalidOperationException($"Invalid parameter type of SystemMessageParam: {param.Type}");
		}
	}

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
            writer.WriteByte((byte)item.getSoulCrystalOptions().Count); // Size of regular soul crystal options.
            foreach (EnsoulOption option in item.getSoulCrystalOptions())
            {
                writer.WriteInt32(option.getId()); // Regular Soul Crystal Ability ID.
            }

            writer.WriteByte((byte)item.getSoulCrystalSpecialOptions().Count); // Size of special soul crystal options.
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