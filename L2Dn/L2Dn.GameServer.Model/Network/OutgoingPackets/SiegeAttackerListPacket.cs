using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct SiegeAttackerListPacket: IOutgoingPacket
{
    private readonly Castle _castle;

    public SiegeAttackerListPacket(Castle castle)
    {
        _castle = castle;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.CASTLE_SIEGE_ATTACKER_LIST);

        writer.WriteInt32(_castle.getResidenceId());
        writer.WriteInt32(0); // 0
        writer.WriteInt32(1); // 1
        writer.WriteInt32(0); // 0
        int size = _castle.getSiege().getAttackerClans().Count;
        if (size > 0)
        {
            writer.WriteInt32(size);
            writer.WriteInt32(size);
            foreach (SiegeClan siegeclan in _castle.getSiege().getAttackerClans())
            {
                Clan? clan = ClanTable.getInstance().getClan(siegeclan.getClanId());
                if (clan == null)
                    continue;

                writer.WriteInt32(clan.Id);
                writer.WriteString(clan.getName());
                writer.WriteString(clan.getLeaderName());
                writer.WriteInt32(clan.getCrestId() ?? 0);
                writer.WriteInt32(0); // signed time (seconds) (not stored by L2J)
                writer.WriteInt32(clan.getAllyId() ?? 0);
                writer.WriteString(clan.getAllyName());
                writer.WriteString(""); // AllyLeaderName
                writer.WriteInt32(clan.getAllyCrestId() ?? 0);
            }
        }
        else
        {
            writer.WriteInt32(0);
            writer.WriteInt32(0);
        }
    }
}