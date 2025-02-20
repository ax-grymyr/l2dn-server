﻿using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPledgeRecruitInfoPacket(Clan clan): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_RECRUIT_INFO);

        ICollection<Clan.SubPledge> subPledges = clan.getAllSubPledges();
        writer.WriteString(clan.getName());
        writer.WriteString(clan.getLeaderName());
        writer.WriteInt32(clan.getLevel());
        writer.WriteInt32(clan.getMembersCount());
        writer.WriteInt32(subPledges.Count);
        foreach (Clan.SubPledge subPledge in subPledges)
        {
            writer.WriteInt32(subPledge.getId());
            writer.WriteString(subPledge.getName());
        }
    }
}