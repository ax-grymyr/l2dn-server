using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct AcquireSkillListPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly List<SkillLearn> _learnable;

    public AcquireSkillListPacket(Player player)
    {
        _player = player;
        _learnable = [];
        if (!player.isSubclassLocked()) // Changing class.
        {
            _learnable.AddRange(SkillTreeData.getInstance().getAvailableSkills(player, player.getClassId(), false, false));
            _learnable.AddRange(SkillTreeData.getInstance().getNextAvailableSkills(player, player.getClassId(), false, false));
        }
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.ACQUIRE_SKILL_LIST);
        if (_player == null)
        {
            writer.WriteInt16(0);
            return;
        }

        writer.WriteInt16((short)_learnable.Count);
        foreach (SkillLearn skill in _learnable)
        {
            int skillId = _player.getReplacementSkill(skill.getSkillId());
            writer.WriteInt32(skillId);

            if (ServerConfig.Instance.GameServerParams.ServerType == GameServerType.Classic)
                writer.WriteInt16((short)skill.getSkillLevel()); // Classic 16-bit integer
            else
                writer.WriteInt32(skill.getSkillLevel()); // 414 both Main and Essence 32-bit integer

            writer.WriteInt64(skill.getLevelUpSp());
            writer.WriteByte((byte)skill.getGetLevel());
            writer.WriteByte(0); // Skill dual class level.

            writer.WriteByte(_player.getKnownSkill(skillId) == null);

            List<List<ItemHolder>> requiredItems = skill.getRequiredItems();
            writer.WriteByte((byte)requiredItems.Count);
            foreach (List<ItemHolder> item in requiredItems)
            {
                writer.WriteInt32(item[0].Id);
                writer.WriteInt64(item[0].getCount());
            }

            List<Skill> removeSkills = new();
            foreach (int id in skill.getRemoveSkills())
            {
                Skill? removeSkill = _player.getKnownSkill(id);
                if (removeSkill != null)
                    removeSkills.Add(removeSkill);
            }

            writer.WriteByte((byte)removeSkills.Count);
            foreach (Skill removed in removeSkills)
            {
                writer.WriteInt32(removed.Id);

                if (ServerConfig.Instance.GameServerParams.ServerType == GameServerType.Classic)
                    writer.WriteInt16((short)removed.Level); // Classic 16-bit integer
                else
                    writer.WriteInt32(removed.Level); // 414 both Main and Essence 32-bit integer
            }
        }
    }
}