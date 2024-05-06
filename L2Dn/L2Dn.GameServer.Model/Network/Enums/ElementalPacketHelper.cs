using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.Enums;

public static class ElementalPacketHelper
{
    public static void WriteUpdate(PacketBitWriter writer, Player player, ElementalType type, bool update)
    {
        writer.WriteByte(update);
        writer.WriteByte((byte)type);
        if (update)
        {
            ElementalSpirit spirit = player.getElementalSpirit(type);
            if (spirit == null)
            {
                return;
            }

            writer.WriteByte((byte)type);
            WriteSpiritInfo(writer, spirit);
        }
    }
    
    public static void WriteSpiritInfo(PacketBitWriter writer, ElementalSpirit spirit)
    {
        writer.WriteByte(spirit.getStage());
        writer.WriteInt32(spirit.getNpcId());
        writer.WriteInt64(spirit.getExperience());
        writer.WriteInt64(spirit.getExperienceToNextLevel());
        writer.WriteInt64(spirit.getExperienceToNextLevel());
        writer.WriteInt32(spirit.getLevel());
        writer.WriteInt32(spirit.getMaxLevel());
        writer.WriteInt32(spirit.getAvailableCharacteristicsPoints());
        writer.WriteInt32(spirit.getAttackPoints());
        writer.WriteInt32(spirit.getDefensePoints());
        writer.WriteInt32(spirit.getCriticalRatePoints());
        writer.WriteInt32(spirit.getCriticalDamagePoints());
        writer.WriteInt32(spirit.getMaxCharacteristics());
        writer.WriteInt32(spirit.getMaxCharacteristics());
        writer.WriteInt32(spirit.getMaxCharacteristics());
        writer.WriteInt32(spirit.getMaxCharacteristics());
        writer.WriteByte(1); // unk
        for (int j = 0; j < 1; j++)
        {
            writer.WriteInt16(2);
            writer.WriteInt64(100);
        }
    }
}