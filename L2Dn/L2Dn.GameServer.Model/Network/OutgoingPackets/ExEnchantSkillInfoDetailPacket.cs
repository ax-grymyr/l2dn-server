using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExEnchantSkillInfoDetailPacket: IOutgoingPacket
{
    private readonly SkillEnchantType _type;
    private readonly int _skillId;
    private readonly int _skillLevel;
    private readonly int _skillSubLevel;
    private readonly EnchantSkillHolder _enchantSkillHolder;
	
    public ExEnchantSkillInfoDetailPacket(SkillEnchantType type, int skillId, int skillLevel, int skillSubLevel, Player player)
    {
        _type = type;
        _skillId = skillId;
        _skillLevel = skillLevel;
        _skillSubLevel = skillSubLevel;
        _enchantSkillHolder = EnchantSkillGroupsData.getInstance().getEnchantSkillHolder(skillSubLevel % 1000);
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ENCHANT_SKILL_INFO_DETAIL);
        
        writer.WriteInt32((int)_type);
        writer.WriteInt32(_skillId);
        writer.WriteInt16((short)_skillLevel);
        writer.WriteInt16((short)_skillSubLevel);
        if (_enchantSkillHolder != null)
        {
            writer.WriteInt64(_enchantSkillHolder.getSp(_type));
            writer.WriteInt32(_enchantSkillHolder.getChance(_type));
            Set<ItemHolder> holders = _enchantSkillHolder.getRequiredItems(_type);
            writer.WriteInt32(holders.size());
            foreach (ItemHolder holder in holders)
            {
                writer.WriteInt32(holder.getId());
                writer.WriteInt32((int)holder.getCount());
            }
        }
    }
}