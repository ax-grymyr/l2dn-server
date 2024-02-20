using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct AcquireSkillListPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly List<SkillLearn> _learnable;
	
    public AcquireSkillListPacket(Player player)
    {
        if (!player.isSubclassLocked()) // Changing class.
        {
            _player = player;
            _learnable = new List<SkillLearn>();
            _learnable.AddRange(SkillTreeData.getInstance().getAvailableSkills(player, player.getClassId(), false, false));
            _learnable.AddRange(SkillTreeData.getInstance().getNextAvailableSkills(player, player.getClassId(), false, false));
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        if (_player == null)
        {
            return;
        }
		
        writer.WritePacketCode(OutgoingPacketCodes.ACQUIRE_SKILL_LIST);
        
        writer.WriteInt16((short)_learnable.Count);
        foreach (SkillLearn skill in _learnable)
        {
            int skillId = _player.getReplacementSkill(skill.getSkillId());
            writer.WriteInt32(skillId);
			
            writer.WriteInt32(skill.getSkillLevel()); // 414 both Main and Essence writer.WriteInt32.
            writer.WriteInt64(skill.getLevelUpSp());
            writer.WriteByte((byte)skill.getGetLevel());
            writer.WriteByte(0); // Skill dual class level.
			
            writer.WriteByte(_player.getKnownSkill(skillId) == null);
			
            writer.WriteByte((byte)skill.getRequiredItems().Count);
            foreach (List<ItemHolder> item in skill.getRequiredItems())
            {
                writer.WriteInt32(item[0].getId());
                writer.WriteInt64(item[0].getCount());
            }
			
            List<Skill> removeSkills = new();
            foreach (int id in skill.getRemoveSkills())
            {
                Skill removeSkill = _player.getKnownSkill(id);
                if (removeSkill != null)
                    removeSkills.Add(removeSkill);
            }
			
            writer.WriteByte((byte)removeSkills.Count);
            foreach (Skill removed in removeSkills)
            {
                writer.WriteInt32(removed.getId());
                writer.WriteInt32(removed.getLevel()); // 414 both Main and Essence writer.WriteInt32.
            }
        }
    }
}