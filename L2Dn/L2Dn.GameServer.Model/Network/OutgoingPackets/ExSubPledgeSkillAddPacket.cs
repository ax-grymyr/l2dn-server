using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExSubPledgeSkillAddPacket: IOutgoingPacket
{
    private readonly int _type;
    private readonly int _skillId;
    private readonly int _skillLevel;
	
    public ExSubPledgeSkillAddPacket(int type, int skillId, int skillLevel)
    {
        _type = type;
        _skillId = skillId;
        _skillLevel = skillLevel;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SUB_PLEDGET_SKILL_ADD);
        
        writer.WriteInt32(_type);
        writer.WriteInt32(_skillId);
        writer.WriteInt32(_skillLevel);
    }
}