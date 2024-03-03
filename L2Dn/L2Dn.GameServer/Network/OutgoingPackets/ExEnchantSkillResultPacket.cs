using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExEnchantSkillResultPacket: IOutgoingPacket
{
    public static readonly ExEnchantSkillResultPacket STATIC_PACKET_TRUE = new(false);
    public static readonly ExEnchantSkillResultPacket STATIC_PACKET_FALSE = new(true);
	
    private readonly bool _enchanted;
	
    public ExEnchantSkillResultPacket(bool enchanted)
    {
        _enchanted = enchanted;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ENCHANT_SKILL_RESULT);
        
        writer.WriteInt32(_enchanted);
    }
}