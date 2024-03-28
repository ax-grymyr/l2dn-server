using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExAcquirableSkillListByClassPacket: IOutgoingPacket
{
    private readonly ICollection<SkillLearn> _learnable;
    private readonly AcquireSkillType _type;
	
    public ExAcquirableSkillListByClassPacket(ICollection<SkillLearn> learnable, AcquireSkillType type)
    {
        _learnable = learnable;
        _type = type;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ACQUIRABLE_SKILL_LIST_BY_CLASS);
        writer.WriteInt16((short)_type);
        writer.WriteInt16((short)_learnable.Count);
        foreach (SkillLearn skill in _learnable)
        {
            writer.WriteInt32(skill.getSkillId());
            writer.WriteInt16((short)skill.getSkillLevel());
            writer.WriteInt16((short)skill.getSkillLevel());
            writer.WriteByte((byte)skill.getGetLevel());
            writer.WriteInt64(skill.getLevelUpSp());
            writer.WriteByte((byte)skill.getRequiredItems().Count);

            if (_type == AcquireSkillType.SUBPLEDGE)
                writer.WriteInt16(0);
        }
    }
}