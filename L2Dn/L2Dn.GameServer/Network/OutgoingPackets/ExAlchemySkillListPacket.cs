using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExAlchemySkillListPacket: IOutgoingPacket
{
    private readonly List<Skill> _skills;
	
    public ExAlchemySkillListPacket(Player player)
    {
        _skills = new List<Skill>();
        foreach (Skill s in player.getAllSkills())
        {
            if (SkillTreeData.getInstance().isAlchemySkill(s.getId(), s.getLevel()))
            {
                _skills.Add(s);
            }
        }
        
        _skills.Add(SkillData.getInstance().getSkill((int)CommonSkill.ALCHEMY_CUBE, 1));
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ALCHEMY_SKILL_LIST);
        
        writer.WriteInt32(_skills.Count);
        foreach (Skill skill in _skills)
        {
            writer.WriteInt32(skill.getId());
            writer.WriteInt32(skill.getLevel());
            writer.WriteInt64(0); // Always 0 on Naia, SP I guess?
            writer.WriteByte(skill.getId() != (int)CommonSkill.ALCHEMY_CUBE); // This is type in flash, visible or not
        }
    }
}