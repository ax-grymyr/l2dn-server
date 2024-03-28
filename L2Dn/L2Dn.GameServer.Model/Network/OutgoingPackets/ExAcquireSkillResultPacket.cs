using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExAcquireSkillResultPacket: IOutgoingPacket
{
    private readonly int _skillId;
    private readonly int _skillLevel;
    private readonly bool _success;
    private readonly SystemMessageId _message;
	
    public ExAcquireSkillResultPacket(int skillId, int skillLevel, bool success, SystemMessageId message)
    {
        _skillId = skillId;
        _skillLevel = skillLevel;
        _success = success;
        _message = message;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ACQUIRE_SKILL_RESULT);
        
        writer.WriteInt32(_skillId);
        writer.WriteInt32(_skillLevel);
        writer.WriteByte(!_success);
        writer.WriteInt32((int)_message);
    }
}