using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.Enums;

public class SpiritPacketHelper
{
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