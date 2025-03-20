﻿using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct PledgeShowInfoUpdatePacket(Clan clan): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PLEDGE_SHOW_INFO_UPDATE);

        // sending empty data so client will ask all the info in response ;)
        writer.WriteInt32(clan.Id);
        writer.WriteInt32(ServerConfig.Instance.GameServerParams.ServerId);
        writer.WriteInt32(clan.getCrestId() ?? 0);
        writer.WriteInt32(clan.getLevel()); // clan level
        writer.WriteInt32(clan.getCastleId() ?? 0);
        writer.WriteInt32(0); // castle state ?
        writer.WriteInt32(clan.getHideoutId());
        writer.WriteInt32(clan.getFortId() ?? 0);
        writer.WriteInt32(clan.getRank());
        writer.WriteInt32(clan.getReputationScore()); // clan reputation score
        writer.WriteInt32(0); // ?
        writer.WriteInt32(0); // ?
        writer.WriteInt32(clan.getAllyId() ?? 0);
        writer.WriteString(clan.getAllyName()); // c5
        writer.WriteInt32(clan.getAllyCrestId() ?? 0); // c5
        writer.WriteInt32(clan.isAtWar()); // c5
        writer.WriteInt32(0); // TODO: Find me!
        writer.WriteInt32(0); // TODO: Find me!
    }
}