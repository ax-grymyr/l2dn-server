using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.NewSkillEnchant;

public readonly struct ExSkillEnchantChargePacket: IOutgoingPacket
{
    private readonly int _skillId;
    private readonly int _skillresult;
	
    public ExSkillEnchantChargePacket(int skillId, int skillresult)
    {
        _skillId = skillId;
        _skillresult = skillresult;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SKILL_ENCHANT_CHARGE);
        
        writer.WriteInt32(_skillId);
        writer.WriteInt32(_skillresult);
    }
}