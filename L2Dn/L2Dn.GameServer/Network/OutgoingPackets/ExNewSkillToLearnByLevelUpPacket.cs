using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExNewSkillToLearnByLevelUpPacket: IOutgoingPacket
{
    public static readonly ExNewSkillToLearnByLevelUpPacket STATIC_PACKET = default;
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_NEW_SKILL_TO_LEARN_BY_LEVEL_UP);
    }
}