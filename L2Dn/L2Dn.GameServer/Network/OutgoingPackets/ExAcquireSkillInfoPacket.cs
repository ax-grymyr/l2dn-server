using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExAcquireSkillInfoPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly int _id;
    private readonly int _level;
    private readonly int _dualClassLevel;
    private readonly long _spCost;
    private readonly int _minLevel;
    private readonly List<List<ItemHolder>> _itemReq;
    private readonly List<Skill> _skillRem;
	
    /**
     * Special constructor for Alternate Skill Learning system.<br>
     * Sets a custom amount of SP.
     * @param player
     * @param skillLearn the skill learn.
     */
    public ExAcquireSkillInfoPacket(Player player, SkillLearn skillLearn)
    {
        _player = player;
        _id = skillLearn.getSkillId();
        _level = skillLearn.getSkillLevel();
        _dualClassLevel = skillLearn.getDualClassLevel();
        _spCost = skillLearn.getLevelUpSp();
        _minLevel = skillLearn.getGetLevel();
        _itemReq = skillLearn.getRequiredItems();
        _skillRem = new List<Skill>();
        foreach (int id in skillLearn.getRemoveSkills())
        {
            Skill removeSkill = player.getKnownSkill(id);
            if (removeSkill != null)
            {
                _skillRem.Add(removeSkill);
            }
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ACQUIRE_SKILL_INFO);
        
        writer.WriteInt32(_player.getReplacementSkill(_id));
        writer.WriteInt32(_level);
        writer.WriteInt64(_spCost);
        writer.WriteInt16((short)_minLevel);
        writer.WriteInt16((short)_dualClassLevel);
        writer.WriteInt32(_itemReq.Count);
        foreach (List<ItemHolder> holder in _itemReq)
        {
            writer.WriteInt32(holder[0].getId());
            writer.WriteInt64(holder[0].getCount());
        }
        writer.WriteInt32(_skillRem.Count);
        foreach (Skill skill in _skillRem)
        {
            writer.WriteInt32(skill.getId());
            writer.WriteInt32(skill.getLevel());
        }
    }
}