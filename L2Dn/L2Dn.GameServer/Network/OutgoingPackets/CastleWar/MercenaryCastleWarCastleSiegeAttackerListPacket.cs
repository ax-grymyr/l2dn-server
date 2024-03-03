using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.CastleWar;

public readonly struct MercenaryCastleWarCastleSiegeAttackerListPacket: IOutgoingPacket
{
    private readonly int _castleId;

    public MercenaryCastleWarCastleSiegeAttackerListPacket(int castleId)
    {
        _castleId = castleId;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MERCENARY_CASTLEWAR_CASTLE_SIEGE_ATTACKER_LIST);

        writer.WriteInt32(_castleId);
        writer.WriteInt32(0);
        writer.WriteInt32(1);
        writer.WriteInt32(0);

        Castle castle = CastleManager.getInstance().getCastleById(_castleId);
        if (castle == null)
        {
            writer.WriteInt32(0);
            writer.WriteInt32(0);
        }
        else
        {
            ICollection<SiegeClan> attackers = castle.getSiege().getAttackerClans();
            writer.WriteInt32(attackers.Count);
            writer.WriteInt32(attackers.Count);
            foreach (SiegeClan siegeClan in attackers)
            {
                Clan clan = ClanTable.getInstance().getClan(siegeClan.getClanId());
                if (clan == null)
                {
                    continue;
                }

                writer.WriteInt32(clan.getId());
                writer.WriteString(clan.getName());
                writer.WriteString(clan.getLeaderName());
                writer.WriteInt32(clan.getCrestId() ?? 0);
                writer.WriteInt32(0); // time

                writer.WriteInt32(0); // 286
                writer.WriteInt32(0); // 286
                writer.WriteInt32(0); // 286
                writer.WriteInt32(0); // 286

                writer.WriteInt32(clan.getAllyId() ?? 0);
                writer.WriteString(clan.getAllyName() ?? string.Empty);
                writer.WriteString(""); // Ally Leader name
                writer.WriteInt32(clan.getAllyCrestId() ?? 0);
            }
        }
    }
}