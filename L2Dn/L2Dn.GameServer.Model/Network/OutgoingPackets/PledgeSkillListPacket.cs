using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PledgeSkillListPacket: IOutgoingPacket
{
    private readonly ICollection<Skill> _skills;
    private readonly List<SubPledgeSkill> _subSkills;

    public record SubPledgeSkill(int SubType, int SkillId, int SkillLevel);
	
    public PledgeSkillListPacket(Clan clan)
    {
        _skills = clan.getAllSkills();
        _subSkills = clan.getAllSubSkills();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PLEDGE_SKILL_LIST);

        writer.WriteInt32(_skills.Count);
        writer.WriteInt32(_subSkills.Count); // Squad skill length
        foreach (Skill sk in _skills)
        {
            writer.WriteInt32(sk.getDisplayId());
            writer.WriteInt16((short)sk.getDisplayLevel());
            writer.WriteInt16(0); // Sub level
        }
        foreach (SubPledgeSkill sk in _subSkills)
        {
            writer.WriteInt32(sk.SubType); // Clan Sub-unit types
            writer.WriteInt32(sk.SkillId);
            writer.WriteInt16((short)sk.SkillLevel);
            writer.WriteInt16(0); // Sub level
        }
    }
}