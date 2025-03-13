using L2Dn.Extensions;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct SiegeInfoPacket: IOutgoingPacket
{
    private readonly Castle _castle;
    private readonly Player _player;

    public SiegeInfoPacket(Castle castle, Player player)
    {
        _castle = castle;
        _player = player;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.CASTLE_SIEGE_INFO);

        writer.WriteInt32(_castle.getResidenceId());
        int ownerId = _castle.getOwnerId();
        writer.WriteInt32(ownerId == _player.getClanId() && _player.isClanLeader());
        writer.WriteInt32(ownerId);
        if (ownerId > 0)
        {
            Clan? owner = ClanTable.getInstance().getClan(ownerId);
            writer.WriteString(owner?.getName() ?? string.Empty); // Clan Name
            writer.WriteString(owner?.getLeaderName() ?? string.Empty); // Clan Leader Name
            writer.WriteInt32(owner?.getAllyId() ?? 0); // Ally ID
            writer.WriteString(owner?.getAllyName() ?? string.Empty); // Ally Name
        }
        else
        {
            writer.WriteString(string.Empty); // Clan Name
            writer.WriteString(string.Empty); // Clan Leader Name
            writer.WriteInt32(0); // Ally ID
            writer.WriteString(string.Empty); // Ally Name
        }

        writer.WriteInt32(DateTime.UtcNow.getEpochSecond());
        if (!_castle.isTimeRegistrationOver() && _player.isClanLeader() && _player.getClanId() == _castle.getOwnerId())
        {
            DateTime cal = _castle.getSiegeDate();
            cal = new DateTime(cal.Year, cal.Month, cal.Day);
            writer.WriteInt32(0);
            writer.WriteInt32(Config.Feature.SIEGE_HOUR_LIST.Length);
            foreach (int hour in Config.Feature.SIEGE_HOUR_LIST)
            {
                cal = new DateTime(cal.Year, cal.Month, cal.Day, hour, 0, 0);
                writer.WriteInt32(cal.getEpochSecond());
            }
        }
        else
        {
            writer.WriteInt32(_castle.getSiegeDate().getEpochSecond());
            writer.WriteInt32(0);
        }
    }
}