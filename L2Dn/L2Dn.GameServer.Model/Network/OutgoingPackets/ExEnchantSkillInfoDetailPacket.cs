using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExEnchantSkillInfoDetailPacket(
    SkillEnchantType type, int skillId, int skillLevel, int skillSubLevel)
    : IOutgoingPacket
{
    private readonly EnchantSkillHolder? _enchantSkillHolder =
        EnchantSkillGroupsData.getInstance().getEnchantSkillHolder(skillSubLevel % 1000);

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ENCHANT_SKILL_INFO_DETAIL);

        writer.WriteInt32((int)type);
        writer.WriteInt32(skillId);
        writer.WriteInt16((short)skillLevel);
        writer.WriteInt16((short)skillSubLevel);
        if (_enchantSkillHolder != null)
        {
            writer.WriteInt64(_enchantSkillHolder.getSp(type));
            writer.WriteInt32(_enchantSkillHolder.getChance(type));
            Set<ItemHolder> holders = _enchantSkillHolder.getRequiredItems(type);
            writer.WriteInt32(holders.size());
            foreach (ItemHolder holder in holders)
            {
                writer.WriteInt32(holder.Id);
                writer.WriteInt32((int)holder.Count);
            }
        }
    }
}