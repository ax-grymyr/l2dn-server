using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExSetPledgeCrestLargePacket: IIncomingPacket<GameSession>
{
    private int _length;
    private byte[]? _data;

    public void ReadContent(PacketBitReader reader)
    {
        _length = reader.ReadInt32();
        if (_length < 0 || _length > 2176)
        {
            return;
        }
		
        _data = reader.ReadBytes(_length).ToArray();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (_data == null)
            return ValueTask.CompletedTask;
		
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        Clan clan = player.getClan();
        if (clan == null)
            return ValueTask.CompletedTask;
		
        if ((_length < 0) || (_length > 2176))
        {
            player.sendPacket(SystemMessageId.THE_SIZE_OF_THE_UPLOADED_SYMBOL_DOES_NOT_MEET_THE_STANDARD_REQUIREMENTS);
            return ValueTask.CompletedTask;
        }
		
        if (clan.getDissolvingExpiryTime() > DateTime.UtcNow)
        {
            player.sendPacket(SystemMessageId.AS_YOU_ARE_CURRENTLY_SCHEDULE_FOR_CLAN_DISSOLUTION_YOU_CANNOT_REGISTER_OR_DELETE_A_CLAN_CREST);
            return ValueTask.CompletedTask;
        }
		
        if (!player.hasClanPrivilege(ClanPrivilege.CL_REGISTER_CREST))
        {
            player.sendPacket(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
            return ValueTask.CompletedTask;
        }
		
        if (_length == 0)
        {
            if (clan.getCrestLargeId() != 0)
            {
                clan.changeLargeCrest(0);
                player.sendPacket(SystemMessageId.THE_CLAN_MARK_HAS_BEEN_DELETED);
            }
        }
        else
        {
            if (clan.getLevel() < 3)
            {
                player.sendPacket(SystemMessageId.A_CLAN_CREST_CAN_ONLY_BE_REGISTERED_WHEN_THE_CLAN_S_SKILL_LEVEL_IS_3_OR_ABOVE);
                return ValueTask.CompletedTask;
            }
			
            Crest crest = CrestTable.getInstance().createCrest(_data, CrestType.PLEDGE_LARGE);
            if (crest != null)
            {
                clan.changeLargeCrest(crest.getId());
                player.sendPacket(SystemMessageId.THE_CLAN_MARK_WAS_SUCCESSFULLY_REGISTERED_THE_SYMBOL_WILL_APPEAR_ON_THE_CLAN_FLAG_AND_THE_INSIGNIA_IS_ONLY_DISPLAYED_ON_ITEMS_PERTAINING_TO_A_CLAN_THAT_OWNS_A_CLAN_HALL_OR_CASTLE);
            }
        }

        return ValueTask.CompletedTask;
    }
}