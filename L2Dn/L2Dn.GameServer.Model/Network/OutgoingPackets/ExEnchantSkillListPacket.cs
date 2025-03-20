using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExEnchantSkillListPacket: IOutgoingPacket
{
    private readonly SkillEnchantType _type;
    private readonly List<Skill> _skills;

    public ExEnchantSkillListPacket(SkillEnchantType type)
    {
        _skills = new List<Skill>();
        _type = type;
    }

    public void addSkill(Skill skill)
    {
        _skills.Add(skill);
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ENCHANT_SKILL_LIST);

        writer.WriteInt32((int)_type);
        writer.WriteInt32(_skills.Count);
        foreach (Skill skill in _skills)
        {
            writer.WriteInt32(skill.Id);
            writer.WriteInt16((short)skill.Level);
            writer.WriteInt16((short)skill.SubLevel);
        }
    }
}