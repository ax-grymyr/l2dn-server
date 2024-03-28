using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.PledgeBonus;

public readonly struct ExPledgeBonusListPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_BONUS_LIST);
        
        writer.WriteByte(0); // 140

        foreach (ClanRewardBonus bonus in ClanRewardData.getInstance()
                     .getClanRewardBonuses(ClanRewardType.MEMBERS_ONLINE).OrderBy(r => r.getLevel()))
        {
            writer.WriteInt32(bonus.getSkillReward().getSkillId());
        }

        writer.WriteByte(0); // 140
        
        foreach (ClanRewardBonus bonus in ClanRewardData.getInstance()
                     .getClanRewardBonuses(ClanRewardType.HUNTING_MONSTERS).OrderBy(r => r.getLevel()))
        {
            writer.WriteInt32(bonus.getSkillReward().getSkillId());
        }
    }
}