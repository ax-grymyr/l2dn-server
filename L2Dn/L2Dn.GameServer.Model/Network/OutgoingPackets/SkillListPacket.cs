using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.StaticData;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct SkillListPacket: IOutgoingPacket
{
    private readonly List<SkillDto> _skills;
    private readonly int _lastLearnedSkillId = 0;

    public SkillListPacket(int lastLearnedSkillId)
    {
        _skills = new List<SkillDto>();
        _lastLearnedSkillId = lastLearnedSkillId;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SKILL_LIST);

        _skills.Sort((a, b) => a.Toggle.CompareTo(b.Toggle));
        writer.WriteInt32(_skills.Count);

        // Toggle skills
        foreach (SkillDto temp in _skills)
        {
            writer.WriteInt32(temp.Passive);
            writer.WriteInt16((short)temp.Level);
            writer.WriteInt16((short)temp.SubLevel);
            writer.WriteInt32(temp.Id);
            writer.WriteInt32(temp.ReuseDelayGroup); // GOD ReuseDelayShareGroupID
            writer.WriteByte(temp.Disabled); // iSkillDisabled
            writer.WriteByte(temp.Enchanted); // CanEnchant
        }

        writer.WriteInt32(_lastLearnedSkillId);
    }

    public void addSkill(int id, int reuseDelayGroup, int level, int subLevel, bool passive, bool disabled, bool enchanted)
    {
        bool toggle = SkillData.Instance.GetSkill(id, level, subLevel)?.IsToggle ?? false;
        _skills.Add(new SkillDto(id, reuseDelayGroup, level, subLevel, passive, disabled, enchanted, toggle));
    }

    private record SkillDto(
        int Id,
        int ReuseDelayGroup,
        int Level,
        int SubLevel,
        bool Passive,
        bool Disabled,
        bool Enchanted,
        bool Toggle);
}