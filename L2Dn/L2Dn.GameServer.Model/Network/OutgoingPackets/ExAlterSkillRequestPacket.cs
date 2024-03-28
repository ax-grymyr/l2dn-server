using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExAlterSkillRequestPacket: IOutgoingPacket
{
    private readonly int _currentSkillId;
    private readonly int _nextSkillId;
    private readonly int _alterTime;
	
    public ExAlterSkillRequestPacket(int currentSkill, int nextSkill, int alterTime)
    {
        _currentSkillId = currentSkill;
        _nextSkillId = nextSkill;
        _alterTime = alterTime;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ALTER_SKILL_REQUEST);
        
        writer.WriteInt32(_nextSkillId);
        writer.WriteInt32(_currentSkillId);
        writer.WriteInt32(_alterTime);
    }
}