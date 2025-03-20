using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestSetPledgeCrestPacket: IIncomingPacket<GameSession>
{
    private int _length;
    private byte[] _data;

    public void ReadContent(PacketBitReader reader)
    {
        _length = reader.ReadInt32();
        if (_length > 0 && _length <= 384)
            _data = reader.ReadBytes(_length).ToArray();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_length < 0)
        {
            connection.Send(SystemMessageId.THE_SIZE_OF_THE_UPLOADED_SYMBOL_DOES_NOT_MEET_THE_STANDARD_REQUIREMENTS);
            return ValueTask.CompletedTask;
        }

        if (_length > 384)
        {
            connection.Send(SystemMessageId.THE_FILE_FORMAT_BMP_256_COLORS_24X12_PIXELS);
            return ValueTask.CompletedTask;
        }

        Clan? clan = player.getClan();
        if (clan == null)
            return ValueTask.CompletedTask;

        if (clan.getDissolvingExpiryTime() > DateTime.UtcNow)
        {
            connection.Send(SystemMessageId.AS_YOU_ARE_CURRENTLY_SCHEDULE_FOR_CLAN_DISSOLUTION_YOU_CANNOT_REGISTER_OR_DELETE_A_CLAN_CREST);
            return ValueTask.CompletedTask;
        }

        if (!player.hasClanPrivilege(ClanPrivilege.CL_REGISTER_CREST))
        {
            connection.Send(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
            return ValueTask.CompletedTask;
        }

        if (_length == 0)
        {
            if (clan.getCrestId() != 0)
            {
                clan.changeClanCrest(0);
                connection.Send(SystemMessageId.THE_CLAN_MARK_HAS_BEEN_DELETED);
            }
        }
        else
        {
            if (clan.getLevel() < 3)
            {
                connection.Send(SystemMessageId.A_CLAN_CREST_CAN_ONLY_BE_REGISTERED_WHEN_THE_CLAN_S_SKILL_LEVEL_IS_3_OR_ABOVE);
                return ValueTask.CompletedTask;
            }

            Crest? crest = CrestTable.getInstance().createCrest(_data, CrestType.PLEDGE);
            if (crest != null)
            {
                clan.changeClanCrest(crest.Id);
                connection.Send(SystemMessageId.THE_CREST_WAS_SUCCESSFULLY_REGISTERED);
            }
        }

        return ValueTask.CompletedTask;
    }
}