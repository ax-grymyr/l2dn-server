using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExAcquireSkillResultPacket(int skillId, int skillLevel, bool success, SystemMessageId message)
    : IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ACQUIRE_SKILL_RESULT);

        writer.WriteInt32(skillId);
        writer.WriteInt32(skillLevel);
        writer.WriteByte(!success);
        writer.WriteInt32((int)message);
    }
}